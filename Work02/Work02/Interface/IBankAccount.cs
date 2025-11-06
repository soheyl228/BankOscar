namespace Work02.Interface
{
    // Interface implemented by domain BankAccount.
    // UI and services reference this interface to avoid direct coupling to the concrete class.
    public interface IBankAccount
    {
        Guid Id { get; }    
        string Name { get; }
        AccountType AccountType { get; }
        string Currency { get; }
        decimal Balance { get; }
        DateTime LastUpdated { get; }

        void Withdraw(decimal amount);
        void Deposit(decimal amount);
        void TransferTo(IBankAccount toAccount, decimal amount);
    }
}