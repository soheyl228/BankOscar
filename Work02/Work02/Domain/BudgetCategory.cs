using System;

namespace Work02.Domain
{
    public class BudgetCategory
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Name { get; set; } = string.Empty;
    }
}