namespace Banks.Entities
{
    public interface IBankAccount
    {
        Client Client { get; }
        BankAccountType BankAccountType { get; }
        decimal MinimalCredits { get; set; }
        decimal Credits { get; }
        ITransaction PutCredits(decimal credits);
        ITransaction WithdrawCredits(decimal credits);
        ITransaction TransferCredits(decimal credits, IBankAccount receiver);
        void ChargeInterest();
        void CancelTransaction(ITransaction transaction);
    }
}