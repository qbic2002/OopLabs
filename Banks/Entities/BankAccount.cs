using System.Collections.Generic;
using System.Collections.ObjectModel;
using Banks.Services;
using Banks.Tools;

namespace Banks.Entities
{
    public abstract class BankAccount : IBankAccount
    {
        private decimal _extraMoney;
        private List<INotification> _notifications = new ();
        private List<ITransaction> _transactions = new ();

        protected BankAccount(Client client, BankAccountId id, decimal minimalCredits, decimal percent, BankAccountType bankAccountType)
        {
            Client = client ?? throw new BanksException("Incorrect client");
            MinimalCredits = minimalCredits;
            BankAccountType = bankAccountType;
            if (percent < 0)
                throw new BanksException("Incorrect percent");
            Percent = percent;
            Id = id ?? throw new BanksException("Incorrect Id");
            Bank = Client.Bank ?? throw new BanksException("Incorrect bank");
        }

        public ReadOnlyCollection<INotification> Notifications => new ReadOnlyCollection<INotification>(_notifications);
        public ReadOnlyCollection<ITransaction> Transactions => new ReadOnlyCollection<ITransaction>(_transactions);
        public Bank Bank { get; }
        public decimal Percent { get; set; }
        public int DaysOpened { get; private set; }
        public Client Client { get; }
        public BankAccountType BankAccountType { get; }
        public decimal MinimalCredits { get; set; }
        public decimal Credits => Bank.BankAccountAndCredits[this];
        public BankAccountId Id { get; }
        public ITransaction PutCredits(decimal credits)
        {
            if (credits <= 0)
                throw new BanksException("Incorrect credits");
            ITransaction putTransaction = Services.Transactions.CreateTransaction(TransactionType.Put, credits, this);
            AddTransactionToHistory(putTransaction);
            Client.Bank.HandleTransaction(putTransaction);
            return putTransaction;
        }

        public ITransaction WithdrawCredits(decimal credits)
        {
            if (credits <= 0)
                throw new BanksException("Incorrect credits");
            ITransaction withdrawTransaction = Services.Transactions.CreateTransaction(TransactionType.Withdraw, credits, this);
            AddTransactionToHistory(withdrawTransaction);
            Client.Bank.HandleTransaction(withdrawTransaction);
            return withdrawTransaction;
        }

        public ITransaction TransferCredits(decimal credits, BankAccountId receiverId) =>
            TransferCredits(credits, Client.Bank.CentralBank.FindBankAccountById(receiverId));

        public abstract void ChargeInterest();

        public void CancelTransaction(ITransaction transaction)
        {
            if (transaction is null)
                throw new BanksException("Incorrect transaction");

            transaction.Cancel();
        }

        public void AddTransactionToHistory(ITransaction transaction)
        {
            if (transaction is null)
                throw new BanksException("Incorrect transaction");

            _transactions.Add(transaction);
        }

        public decimal AddInterest()
        {
            decimal extraMoney = _extraMoney;
            _extraMoney = 0;
            return extraMoney;
        }

        public void HandleNotification(INotification notification)
        {
            notification.BankAccount = this;
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
            ITransaction transferTransaction = Services.Transactions.CreateTransaction(TransactionType.Transfer, credits, this, receiver);
            AddTransactionToHistory(transferTransaction);
            Client.Bank.HandleTransaction(transferTransaction);
            return transferTransaction;
        }
    }
}