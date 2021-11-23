using System.Collections.ObjectModel;

namespace Banks.Entities
{
    public interface IBankAccount
    {
        ReadOnlyCollection<INotification> Notifications { get; }
        ReadOnlyCollection<ITransaction> Transactions { get; }
        Bank Bank { get; }
        decimal Percent { get; set; }
        int DaysOpened { get; }
        Client Client { get; }
        BankAccountType BankAccountType { get; }
        decimal MinimalCredits { get; set; }
        decimal Credits { get; }
        BankAccountId Id { get; }
        ITransaction PutCredits(decimal credits);
        ITransaction WithdrawCredits(decimal credits);
        ITransaction TransferCredits(decimal credits, BankAccountId receiverId);
        void ChargeInterest();
        void CancelTransaction(ITransaction transaction);
        decimal AddInterest();
        void HandleNotification(INotification notification);
        void AddOneDay();
    }
}