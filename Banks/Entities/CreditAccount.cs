using System.Collections.Generic;
using System.Collections.ObjectModel;
using Banks.Services;
using Banks.Tools;

namespace Banks.Entities
{
    public class CreditAccount : BankAccount
    {
        private decimal _extraMoney = 0;
        private List<ITransaction> _transactions = new ();
        private List<INotification> _notifications = new ();

        public CreditAccount(Client client, BankAccountId id, decimal minimalCredits, decimal commissionPercent, BankAccountType bankAccountType)
            : base(client, id, minimalCredits, commissionPercent, bankAccountType)
        {
        }

        public override string ToString()
        {
            return new string($"ID: {Id}; Type: {BankAccountType}; Credits: {Credits:0.00}");
        }

        public override void ChargeInterest()
        {
            if (Credits < 0)
            {
                _extraMoney -= Percent;
            }
        }
    }
}