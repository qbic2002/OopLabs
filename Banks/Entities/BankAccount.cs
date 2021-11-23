using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Banks.Services;
using Banks.Tools;

namespace Banks.Entities
{
    public abstract class BankAccount : IBankAccount
    {
        private decimal _extraMoney;
        private List<INotification> _notifications = new ();

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
        public ReadOnlyCollection<ITransaction> Transactions => Bank.TransactionManager.BankAccountAndTransactions[this].AsReadOnly();
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
            ITransaction putTransaction = Bank.TransactionManager.CreateTransaction(TransactionType.Put, credits, this);
            return putTransaction;
        }

        public ITransaction WithdrawCredits(decimal credits)
        {
            if (credits <= 0)
                throw new BanksException("Incorrect credits");
            ITransaction withdrawTransaction = Bank.TransactionManager.CreateTransaction(TransactionType.Withdraw, credits, this);
            return withdrawTransaction;
        }

        public ITransaction TransferCredits(decimal credits, BankAccountId receiverId) =>
            TransferCredits(credits, Bank.CentralBank.FindBankAccountById(receiverId));

        public abstract void ChargeInterest();

        public void CancelTransaction(ITransaction transaction)
        {
            if (transaction is null)
                throw new BanksException("Incorrect transaction");

            transaction.Cancel();
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
            ITransaction transferTransaction = Bank.TransactionManager.CreateTransaction(TransactionType.Transfer, credits, this, receiver);
            return transferTransaction;
        }
    }
}