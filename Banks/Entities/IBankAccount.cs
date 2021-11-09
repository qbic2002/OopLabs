using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Banks.Entities
{
    public interface IBankAccount
    {
        ReadOnlyCollection<INotification> Notifications { get; }
        ReadOnlyCollection<ITransaction> Transactions { get; }
        bool IsDoubtful { get; }
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
        void AddTransactionToHistory(ITransaction transaction);
        decimal AddInterest();
        void HandleNotification(INotification notification);
        void AddOneDay();
    }
}