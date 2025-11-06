using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Work02.Interface
{
    // Service contract for account operations used by UI components.
    // Keep business-facing methods async to avoid blocking the WebAssembly UI thread.
    public interface IAccountService
    {
        Task<IBankAccount> CreateAccount(string name, AccountType accountType, string currency, decimal initialBalance);
        Task<List<IBankAccount>> GetAccounts();
        Task DeleteAccount(string name);

        // Async transfer ensures persistence completes and avoids race conditions
        Task TransferAsync(Guid fromAccountId, Guid toAccountId, decimal amount);

        Task DepositAsync(Guid accountId, decimal amount);
        Task WithdrawAsync(Guid accountId, decimal amount);

        // add signature to the interface
        Task ApplyInterestAsync(decimal annualRatePercent);
    }
}
