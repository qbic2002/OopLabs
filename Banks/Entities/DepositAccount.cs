using System.Collections.Generic;
using System.Collections.ObjectModel;
using Banks.Services;
using Banks.Tools;

namespace Banks.Entities
{
    public class DepositAccount : BankAccount
    {
        private decimal _extraMoney = 0;
        private List<ITransaction> _transactions = new ();
        private List<INotification> _notifications = new ();

        public DepositAccount(Client client, BankAccountId id, decimal minimalCredits, decimal percent, BankAccountType bankAccountType)
            : base(client, id, minimalCredits, percent, bankAccountType)
        {
        }

        public int Term => Bank.DepositAccountAndTerm[this];
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