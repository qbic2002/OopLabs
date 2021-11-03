using System;
using System.Collections.Generic;
using Banks.Services;
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
        public int Passport { get; private set; }
        public List<IBankAccount> BankAccounts => Bank.ClientsAndAccounts[this];

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
            if (passport < 0)
                throw new BanksException("Incorrect passport");
            Passport = passport;
        }

        public IBankAccount CreateBankAccount(BankAccountType bankAccountType) => Bank.CreateBankAccount(this, bankAccountType);
    }
}