﻿using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
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
        public Passport Passport { get; private set; } = null;
        public bool IsDoubtful => Address is null || Passport is null;
        public List<IBankAccount> BankAccounts => Bank.ClientsAndAccounts[this];
        public bool IsReceiveNotifications { get; private set; } = true;
        public ReadOnlyCollection<ITransaction> Transactions
        {
            get
            {
                var transactions = BankAccounts.SelectMany(bankAccount => bankAccount.Transactions).ToList();
                return new ReadOnlyCollection<ITransaction>(transactions);
            }
        }

        public ReadOnlyCollection<INotification> Notifications
        {
            get
            {
                var notifications = BankAccounts.SelectMany(bankAccount => bankAccount.Notifications).ToList();
                return new ReadOnlyCollection<INotification>(notifications);
            }
        }

        public void SetAddress(string address)
        {
            Address = address ?? throw new BanksException("Incorrect Address");
        }

        public void SetPassport(int passportNumber)
        {
            var passport = new Passport(passportNumber);
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