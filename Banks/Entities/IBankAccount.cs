namespace Banks.Entities
{
    public interface IBankAccount
    {
        decimal MinimalCredits { get; set; }
        void PutCredits(decimal credits);
        void WithdrawCredits(decimal credits);
        void TransferCredits(decimal credits, IBankAccount receiver);
        void ChargeInterest();
    }
}