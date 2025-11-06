using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Work02.Interface
{
    public interface IAccountService
    {
        Task<IBankAccount> CreateAccount(string name, AccountType accountType, string currency, decimal initialBalance);
        Task<List<IBankAccount>> GetAccounts();
        Task DeleteAccount(string name);

        Task TransferAsync(Guid fromAccountId, Guid toAccountId, decimal amount);

        Task DepositAsync(Guid accountId, decimal amount);
        Task WithdrawAsync(Guid accountId, decimal amount);
    }
}
