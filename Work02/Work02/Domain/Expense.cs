using System;

namespace Work02.Domain
{
    public class Expense
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid AccountId { get; set; }
        public Guid CategoryId { get; set; }
        public DateTime TimeStamp { get; set; } = DateTime.UtcNow;
        public decimal Amount { get; set; }
        public string? Note { get; set; }
    }
}