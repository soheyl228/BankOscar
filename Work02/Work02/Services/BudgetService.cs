using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Work02.Domain;
using Work02.Interface;

namespace Work02.Services
{
    public class BudgetService : IBudgetService
    {
        private const string StorageKey = "banken.budget";
        private readonly IStorageService _storage;
        private readonly List<BudgetCategory> _categories = new();
        private readonly List<Expense> _expenses = new();
        private bool _initialized;

        public BudgetService(IStorageService storage) => _storage = storage;

        public async Task InitializeAsync()
        {
            if (_initialized) return;
            var fromStorage = await _storage.GetItemAsync<BudgetStore?>(StorageKey);
            if (fromStorage != null)
            {
                _categories.Clear();
                _categories.AddRange(fromStorage.Categories ?? new List<BudgetCategory>());
                _expenses.Clear();
                _expenses.AddRange(fromStorage.Expenses ?? new List<Expense>());
            }
            else
            {
                // seed with common categories
                _categories.AddRange(new[]
                {
                    new BudgetCategory { Name = "Food / Mat" },
                    new BudgetCategory { Name = "Rent / Hyra" },
                    new BudgetCategory { Name = "Transport" }
                });
                await SaveAsync();
            }
            _initialized = true;
        }

        private Task SaveAsync() => _storage.SetItemAsync(StorageKey, new BudgetStore { Categories = _categories, Expenses = _expenses });

        public async Task<List<BudgetCategory>> GetCategoriesAsync()
        {
            await InitializeAsync();
            return _categories.ToList();
        }

        public async Task AddCategoryAsync(BudgetCategory category)
        {
            await InitializeAsync();
            _categories.Add(category);
            await SaveAsync();
        }

        public async Task<List<Expense>> GetExpensesAsync()
        {
            await InitializeAsync();
            return _expenses.ToList();
        }

        public async Task AddExpenseAsync(Expense expense)
        {
            await InitializeAsync();
            _expenses.Add(expense);
            await SaveAsync();
        }

        public async Task<decimal> GetTotalByCategoryAsync(Guid categoryId)
        {
            await InitializeAsync();
            return _expenses.Where(e => e.CategoryId == categoryId).Sum(e => e.Amount);
        }

        private class BudgetStore
        {
            public List<BudgetCategory>? Categories { get; set; }
            public List<Expense>? Expenses { get; set; }
        }
    }
}
