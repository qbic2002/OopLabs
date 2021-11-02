using System.Collections.Generic;
using Banks.Services;
using Banks.Tools;

namespace Banks.Entities
{
    public class Client
    {
        private List<IBankAccount> _bankAccounts = new ();
        public Client(string firstName, string lastName)
        {
            FirstName = firstName ?? throw new BanksException("Incorrect First Name");
            LastName = lastName ?? throw new BanksException("Incorrect Last Name");
        }

        public Client(string firstName, string lastName, string address)
            : this(firstName, lastName)
        {
            SetAddress(address);
        }

        public Client(string firstName, string lastName, int passport)
            : this(firstName, lastName)
        {
            SetPassport(passport);
        }

        public Client(string firstName, string lastName, string address, int passport)
            : this(firstName, lastName)
        {
            SetAddress(address);
            SetPassport(passport);
        }

        public string FirstName { get; }
        public string LastName { get; }
        public string Address { get; private set; }
        public int Passport { get; private set; }

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
    }
}