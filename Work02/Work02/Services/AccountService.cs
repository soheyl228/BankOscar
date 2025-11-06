using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Work02.Domain;

namespace Work02.Services
{
    // AccountService: application-level service that manages BankAccount entities.
    // - Persists accounts via IStorageService (localStorage wrapper)
    // - Exposes async methods the UI can call (create, list, transfer, deposit, withdraw)
    public class AccountService : IAccountService
    {
        private const string StorageKey = "banken.accounts";
        private readonly List<BankAccount> _accounts = new();
        private readonly IStorageService _storageService;

        public AccountService(IStorageService storageService) => _storageService = storageService;

        // Tracks whether accounts have been loaded from persistent storage
        private bool isLoaded;

        // Lazy-load from storage on first access
        private async Task IsInitialized()
        {
            if (isLoaded) return;

            var fromStorage = await _storageService.GetItemAsync<List<BankAccount>>(StorageKey);

            _accounts.Clear();
            if (fromStorage is { Count: > 0 })
            {
                // Remove duplicates by name (case-insensitive) when loading
                _accounts.AddRange(fromStorage
                    .GroupBy(a => a.Name, StringComparer.OrdinalIgnoreCase)
                    .Select(g => g.First())
                    .ToList());
            }

            isLoaded = true;
        }

        // Persist in-memory accounts to storage
        private Task SaveAsync() => _storageService.SetItemAsync(StorageKey, _accounts);

        // Delete account by name (UI triggers this)
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

        // Create new account and persist
        public async Task<IBankAccount> CreateAccount(string name, AccountType accountType, string currency, decimal initialBalance)
        {
            await IsInitialized();
            var account = new BankAccount(name, accountType, currency, initialBalance);
            _accounts.Add(account);
            await SaveAsync();
            return account;
        }

        // Return list of accounts for UI (as IBankAccount to decouple UI from concrete type)
        public async Task<List<IBankAccount>> GetAccounts()
        {
            await IsInitialized();
            return _accounts.Cast<IBankAccount>().ToList();
        }

        // Transfer funds between two accounts (domain logic on BankAccount.TransferTo)
        public async Task TransferAsync(Guid fromAccountId, Guid toAccountId, decimal amount)
        {
            var fromAccount = _accounts.OfType<BankAccount>().FirstOrDefault(x => x.Id == fromAccountId)
                ?? throw new KeyNotFoundException($"Account with ID {fromAccountId} not found");

            var toAccount = _accounts.OfType<BankAccount>().FirstOrDefault(x => x.Id == toAccountId)
                ?? throw new KeyNotFoundException($"Account with ID {toAccountId} not found");

            fromAccount.TransferTo(toAccount, amount);
            await SaveAsync();
        }

        // Deposit and withdraw: thin wrappers that call domain methods and persist
        public async Task DepositAsync(Guid accountId, decimal amount)
        {
            await IsInitialized();
            var account = _accounts.FirstOrDefault(a => a.Id == accountId);
            if (account == null) throw new Exception("Account not found.");
            account.Deposit(amount);
            await SaveAsync();
        }

        public async Task WithdrawAsync(Guid accountId, decimal amount)
        {
            await IsInitialized();
            var account = _accounts.FirstOrDefault(a => a.Id == accountId);
            if (account == null) throw new Exception("Account not found.");
            if (amount > account.Balance) throw new Exception("Insufficient funds.");
            account.Withdraw(amount);
            await SaveAsync();
        }

        // ApplyInterestAsync: apply simple interest to savings accounts
        public async Task ApplyInterestAsync(decimal annualRatePercent)
        {
            await IsInitialized();

            // Apply simple interest once (rate percent e.g. 1.5 for 1.5%)
            var savings = _accounts.OfType<BankAccount>().Where(a => a.AccountType == AccountType.Savings).ToList();

            foreach (var acc in savings)
            {
                var interest = Math.Round(acc.Balance * (annualRatePercent / 100m), 2);
                if (interest <= 0) continue;
                acc.Deposit(interest); // Deposit records a transaction
            }

            await SaveAsync();
        }
    }
}

