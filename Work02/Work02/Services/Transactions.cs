using System;
using Work02.Services;

namespace Work02.Domain
{
    public class Transaction
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public DateTime TimeStamp { get; set; } = DateTime.UtcNow;
        public decimal Amount { get; set; }
        public decimal BalanceAfter { get; set; }
        public Guid? FromAccountId { get; set; }
        public Guid? ToAccountId { get; set; }
        public TransactionType TransactionType { get; set; }
    }

    public enum TransactionType
    {
        Deposit,
        Withdraw,
        TransferIn,
        TransferOut
    }
}