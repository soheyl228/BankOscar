using System;
using Work02.Services;

namespace Work02.Domain
{
    // Lightweight transaction model used by BankAccount to record changes
    public class Transaction
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        // Timestamp for ordering and display (UTC stored, converted to local in UI)
        public DateTime TimeStamp { get; set; } = DateTime.UtcNow;

        public decimal Amount { get; set; }
        public decimal BalanceAfter { get; set; }

        // Optional references to accounts involved in the transaction
        public Guid? FromAccountId { get; set; }
        public Guid? ToAccountId { get; set; }

        public TransactionType TransactionType { get; set; }
    }

    public enum TransactionType
    {
        Deposit,
        Withdraw,
        TransferIn,
        TransferOut,
        Interest
    }
}