using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Work02.Domain
{
    public class BankAccount : IBankAccount
    {
        public Guid Id { get; private set; } = Guid.NewGuid();
        public string Name { get; private set; }
        public AccountType AccountType { get; private set; }
        public string Currency { get; private set; }
        public decimal Balance { get; private set; }
        public DateTime LastUpdated { get; private set; }
        private readonly List<Transaction> _transactions = new();

        public BankAccount(string name, AccountType accountType, string currency, decimal initialBalance)
        {
            Name = name;
            AccountType = accountType;
            Currency = currency;
            Balance = initialBalance;
            LastUpdated = DateTime.UtcNow;
        }
        [JsonConstructor]
        public BankAccount(Guid id, string name, AccountType accountType, string currency,
            decimal balance, DateTime lastUpdated)
        {
            Name = name;
            AccountType = accountType;
            Currency = currency;
            Balance = balance;
            Id = id;
            LastUpdated = DateTime.Now;
        }

        public void Withdraw(decimal amount)
        {
            if (amount <= 0) throw new ArgumentException("Belopp måste vara större än 0.");
            if (amount > Balance) throw new InvalidOperationException("Otillräckligt saldo.");
            Balance -= amount;
            LastUpdated = DateTime.UtcNow;
            _transactions.Add(new Transaction
            {
                TimeStamp = DateTime.UtcNow,
                TransactionType = TransactionType.Withdraw,
                Amount = amount,
                BalanceAfter = Balance,
                FromAccountId = Id
            });
        }
        public void Deposit(decimal amount)
        {
            if (amount <= 0) throw new ArgumentException("Belopp måste vara större än 0.");
            Balance += amount;
            LastUpdated = DateTime.UtcNow;
            _transactions.Add(new Transaction
            {
                TimeStamp = DateTime.UtcNow,
                TransactionType = TransactionType.Deposit,
                Amount = amount,
                BalanceAfter = Balance,
                ToAccountId = Id
            });
        }

        public void TransferTo(IBankAccount toAccountInterface, decimal amount)
        {
            if (toAccountInterface is not BankAccount toAccount)
                throw new ArgumentException("Destination must be a BankAccount", nameof(toAccountInterface));

            if (amount <= 0) throw new ArgumentException("Amount must be > 0", nameof(amount));
            if (Balance < amount) throw new InvalidOperationException("Insufficient funds");

            // from
            Balance -= amount;
            LastUpdated = DateTime.UtcNow;
            _transactions.Add(new Transaction
            {
                TimeStamp = DateTime.UtcNow,
                TransactionType = TransactionType.TransferOut,
                Amount = amount,
                BalanceAfter = Balance,
                FromAccountId = Id,
                ToAccountId = toAccount.Id
            });

            // to
            toAccount.Balance += amount;
            toAccount.LastUpdated = DateTime.UtcNow;
            toAccount._transactions.Add(new Transaction
            {
                TimeStamp = DateTime.UtcNow,
                TransactionType = TransactionType.TransferIn,
                Amount = amount,
                BalanceAfter = toAccount.Balance,
                FromAccountId = Id,
                ToAccountId = toAccount.Id
            });
        }

        public IReadOnlyList<Transaction> GetTransactions() => _transactions.AsReadOnly();
    }
}