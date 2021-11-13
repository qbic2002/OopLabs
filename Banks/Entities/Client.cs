using System.Collections.Generic;
using System.Collections.ObjectModel;
using Banks.Tools;

namespace Banks.Entities
{
    public class Client
    {
        public Client(Bank bank, string firstName, string lastName)
        {
            FirstName = firstName ?? throw new BanksException("Incorrect First Name");
            LastName = lastName ?? throw new BanksException("Incorrect Last Name");
            Bank = bank ?? throw new BanksException("Incorrect Bank");
        }

        public Client(Bank bank, string firstName, string lastName, string address)
            : this(bank, firstName, lastName)
        {
            SetAddress(address);
        }

        public Client(Bank bank, string firstName, string lastName, int passport)
            : this(bank, firstName, lastName)
        {
            SetPassport(passport);
        }

        public Client(Bank bank, string firstName, string lastName, string address, int passport)
            : this(bank, firstName, lastName)
        {
            SetAddress(address);
            SetPassport(passport);
        }

        public Bank Bank { get; }
        public string FirstName { get; }
        public string LastName { get; }
        public string Address { get; private set; }
        public int Passport { get; private set; } = -1;
        public bool IsDoubtful => Address is null || Passport == -1;
        public List<IBankAccount> BankAccounts => Bank.ClientsAndAccounts[this];
        public bool IsReceiveNotifications { get; private set; } = true;
        public ReadOnlyCollection<ITransaction> Transactions
        {
            get
            {
                var transactions = new List<ITransaction>();
                BankAccounts.ForEach(bankAccount => transactions.AddRange(bankAccount.Transactions));
                return new ReadOnlyCollection<ITransaction>(transactions);
            }
        }

        public ReadOnlyCollection<INotification> Notifications
        {
            get
            {
                var notifications = new List<INotification>();
                BankAccounts.ForEach(bankAccount => notifications.AddRange(bankAccount.Notifications));
                return new ReadOnlyCollection<INotification>(notifications);
            }
        }

        public void SetAddress(string address)
        {
            if (Address is not null)
                throw new BanksException("Address already set");
            Address = address ?? throw new BanksException("Incorrect Address");
        }

        public void SetPassport(int passport)
        {
            if (Passport > 0)
                throw new BanksException("Passport already set");
            if (passport <= 0)
                throw new BanksException("Incorrect passport");
            Passport = passport;
        }

        public void ReceiveNotification(bool isReceive)
        {
            IsReceiveNotifications = isReceive;
        }

        public IBankAccount CreateBankAccount(BankAccountType bankAccountType, decimal startMoney = 0, int term = 365) => Bank.CreateBankAccount(this, bankAccountType, startMoney, term);
        public override string ToString()
        {
            return new string($"{FirstName} {LastName}");
        }
    }
}