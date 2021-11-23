using System.Collections.Generic;

namespace Banks.Entities
{
    public class DepositAccount : BankAccount
    {
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
            ExtraMoney += Credits * Percent / 365;
        }
    }
}