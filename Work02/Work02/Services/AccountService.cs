using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Work02.Domain;

namespace Work02.Services
{
    public class AccountService : IAccountService
    {
        private const string StorageKey = "banken.accounts";
        private readonly List<BankAccount> _accounts = new();
        private readonly IStorageService _storageService;

        public AccountService(IStorageService storageService) => _storageService = storageService;

        private bool isLoaded;

        private async Task IsInitialized()
        {
            if (isLoaded) return;

            var fromStorage = await _storageService.GetItemAsync<List<BankAccount>>(StorageKey);

            _accounts.Clear();
            if (fromStorage is { Count: > 0 })
            {
                _accounts.AddRange(fromStorage
                    .GroupBy(a => a.Name, StringComparer.OrdinalIgnoreCase)
                    .Select(g => g.First())
                    .ToList());
            }

            isLoaded = true;
        }

        private Task SaveAsync() => _storageService.SetItemAsync(StorageKey, _accounts);

        public async Task DeleteAccount(string name)
        {
            await IsInitialized();
            var toRemove = _accounts.FirstOrDefault(a => a.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
            if (toRemove is not null)
            {
                _accounts.Remove(toRemove);
                await SaveAsync();
            }
        }

        public async Task<IBankAccount> CreateAccount(string name, AccountType accountType, string currency, decimal initialBalance)
        {
            await IsInitialized();
            var account = new BankAccount(name, accountType, currency, initialBalance);
            _accounts.Add(account);
            await SaveAsync();
            return account;
        }

        public async Task<List<IBankAccount>> GetAccounts()
        {
            await IsInitialized();
            return _accounts.Cast<IBankAccount>().ToList();
        }

        public async Task TransferAsync(Guid fromAccountId, Guid toAccountId, decimal amount)
        {
            var fromAccount = _accounts.OfType<BankAccount>().FirstOrDefault(x => x.Id == fromAccountId)
                ?? throw new KeyNotFoundException($"Account with ID {fromAccountId} not found");

            var toAccount = _accounts.OfType<BankAccount>().FirstOrDefault(x => x.Id == toAccountId)
                ?? throw new KeyNotFoundException($"Account with ID {toAccountId} not found");

            fromAccount.TransferTo(toAccount, amount);
            await SaveAsync();
        }

        public async Task DepositAsync(Guid accountId, decimal amount)
        {
            await IsInitialized();
            var account = _accounts.FirstOrDefault(a => a.Id == accountId);
            if (account == null) throw new Exception("Konto hittades inte.");
            account.Deposit(amount);
            await SaveAsync();
        }

        public async Task WithdrawAsync(Guid accountId, decimal amount)
        {
            await IsInitialized();
            var account = _accounts.FirstOrDefault(a => a.Id == accountId);
            if (account == null) throw new Exception("Konto hittades inte.");
            if (amount > account.Balance) throw new Exception("Otillräckligt saldo.");
            account.Withdraw(amount);
            await SaveAsync();
        }
    }
}

