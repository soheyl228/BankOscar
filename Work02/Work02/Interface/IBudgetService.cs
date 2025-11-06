using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Work02.Domain;

namespace Work02.Interface
{
    public interface IBudgetService
    {
        Task InitializeAsync();
        Task<List<BudgetCategory>> GetCategoriesAsync();
        Task AddCategoryAsync(BudgetCategory category);
        Task<List<Expense>> GetExpensesAsync();
        Task AddExpenseAsync(Expense expense);
        Task<decimal> GetTotalByCategoryAsync(Guid categoryId);
    }
}
