using System.Collections.Generic;
using System.Collections.ObjectModel;
using Banks.Services;
using Banks.Tools;

namespace Banks.Entities
{
    public class DebitAccount : IBankAccount
    {
        private decimal _extraMoney;
        private List<ITransaction> _transactions = new ();
        private List<INotification> _notifications = new ();
        public DebitAccount(Client client, BankAccountId id, decimal minimalCredits, decimal percent, BankAccountType bankAccountType)
        {
            Client = client ?? throw new BanksException("Incorrect client");
            MinimalCredits = minimalCredits;
            BankAccountType = bankAccountType;
            if (percent < 0)
                throw new BanksException("Incorrect percent");
            Percent = percent;
            Id = id ?? throw new BanksException("Incorrect Id");

            Transactions = new ReadOnlyCollection<ITransaction>(_transactions);
            Notifications = new ReadOnlyCollection<INotification>(_notifications);
        }

        public BankAccountId Id { get; }
        public ReadOnlyCollection<INotification> Notifications { get; }
        public decimal MinimalCredits { get; set; }
        public BankAccountType BankAccountType { get; }
        public decimal Credits => Client.Bank.BankAccountAndCredits[this];
        public Client Client { get; }
        public int DaysOpened { get; private set; }
        public decimal Percent { get; set; }
        public bool IsDoubtful => Client.IsDoubtful;
        public ReadOnlyCollection<ITransaction> Transactions { get; }

        public override string ToString()
        {
            return new string($"ID: {Id}; Type: {BankAccountType}; Credits: {Credits}");
        }

        public void AddTransactionToHistory(ITransaction transaction)
        {
            if (transaction is null)
                throw new BanksException("Incorrect transaction");

            _transactions.Add(transaction);
        }

        public ITransaction WithdrawCredits(decimal credits)
        {
            if (credits <= 0)
                throw new BanksException("Incorrect credits");
            ITransaction withdrawTransaction = TransactionBuilder.CreateTransaction(TransactionType.Withdraw, credits, this);
            AddTransactionToHistory(withdrawTransaction);
            Client.Bank.HandleTransaction(withdrawTransaction);
            return withdrawTransaction;
        }

        public ITransaction PutCredits(decimal credits)
        {
            if (credits <= 0)
                throw new BanksException("Incorrect credits");
            ITransaction putTransaction = TransactionBuilder.CreateTransaction(TransactionType.Put, credits, this);
            AddTransactionToHistory(putTransaction);
            Client.Bank.HandleTransaction(putTransaction);
            return putTransaction;
        }

        public ITransaction TransferCredits(decimal credits, BankAccountId receiverId) =>
            TransferCredits(credits, Client.Bank.CentralBank.FindBankAccountById(receiverId));

        public void ChargeInterest()
        {
            _extraMoney += Credits * Percent / 365;
        }

        public decimal AddInterest()
        {
            decimal extraMoney = _extraMoney;
            _extraMoney = 0;
            return extraMoney;
        }

        public void CancelTransaction(ITransaction transaction)
        {
            if (transaction is null)
                throw new BanksException("Incorrect transaction");

            transaction.Cancel();
        }

        public void HandleNotification(INotification notification)
        {
            if (Client.IsReceiveNotifications)
                _notifications.Add(notification);
        }

        public void AddOneDay()
        {
            DaysOpened += 1;
        }

        private ITransaction TransferCredits(decimal credits, IBankAccount receiver)
        {
            if (credits <= 0)
                throw new BanksException("Incorrect credits");
            ITransaction transferTransaction = TransactionBuilder.CreateTransaction(TransactionType.Transfer, credits, this, receiver);
            AddTransactionToHistory(transferTransaction);
            Client.Bank.HandleTransaction(transferTransaction);
            return transferTransaction;
        }
    }
}