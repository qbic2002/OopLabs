using System.Collections.Generic;
using System.Collections.ObjectModel;
using Banks.Services;
using Banks.Tools;

namespace Banks.Entities
{
    public class DebitAccount : BankAccount
    {
        private decimal _extraMoney;
        private List<ITransaction> _transactions = new ();
        private List<INotification> _notifications = new ();

        public DebitAccount(Client client, BankAccountId id, decimal minimalCredits, decimal percent, BankAccountType bankAccountType)
            : base(client, id, minimalCredits, percent, bankAccountType)
        {
        }

        public override string ToString()
        {
            return new string($"ID: {Id}; Type: {BankAccountType}; Credits: {Credits:0.00}");
        }

        public override void ChargeInterest()
        {
            _extraMoney += Credits * Percent / 365;
        }
    }
}