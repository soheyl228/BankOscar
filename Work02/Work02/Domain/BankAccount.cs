using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Work02.Domain
{
    // Domain entity representing a bank account.
    // Encapsulates balance changes and records transactions in a private list.
    public class BankAccount : IBankAccount
    {
        // Public properties used throughout the app and shown in UI
        public Guid Id { get; private set; } = Guid.NewGuid();
        public string Name { get; private set; }
        public AccountType AccountType { get; private set; }
        public string Currency { get; private set; }
        public decimal Balance { get; private set; }
        public DateTime LastUpdated { get; private set; }

        // Internal transaction list — use GetTransactions() to expose a read-only view
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
            // Constructor used when deserializing from storage
            Name = name;
            AccountType = accountType;
            Currency = currency;
            Balance = balance;
            Id = id;
            LastUpdated = lastUpdated; // preserve persisted timestamp
        }

        // Domain operations: validate and record transactions

        public void Withdraw(decimal amount)
        {
            if (amount <= 0) throw new ArgumentException("Amount must be greater than 0.");
            if (amount > Balance) throw new InvalidOperationException("Insufficient funds.");
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
            if (amount <= 0) throw new ArgumentException("Amount must be greater than 0.");
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

        // Transfer records both outgoing and incoming transactions on the respective accounts
        public void TransferTo(IBankAccount toAccountInterface, decimal amount)
        {
            if (toAccountInterface is not BankAccount toAccount)
                throw new ArgumentException("Destination must be a BankAccount", nameof(toAccountInterface));

            if (amount <= 0) throw new ArgumentException("Amount must be > 0", nameof(amount));
            if (Balance < amount) throw new InvalidOperationException("Insufficient funds");

            // From account entry
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

            // To account entry
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

        // Expose a read-only view of transactions for UI or reporting
        public IReadOnlyList<Transaction> GetTransactions() => _transactions.AsReadOnly();
    }
}