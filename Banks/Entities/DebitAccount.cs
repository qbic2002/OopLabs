using System.Collections.Generic;

namespace Banks.Entities
{
    public class DebitAccount : BankAccount
    {
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
            ExtraMoney += Credits * Percent / 365;
        }
    }
}