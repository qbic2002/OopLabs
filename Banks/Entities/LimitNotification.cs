namespace Banks.Entities
{
    public class LimitNotification
    {
        public LimitNotification(IBankAccount bankAccount, decimal oldPercent, decimal newPercent)
        {
            BankAccount = bankAccount;
            OldValue = oldPercent;
            NewValue = newPercent;
        }

        public IBankAccount BankAccount { get; }
        public decimal OldValue { get; }
        public decimal NewValue { get; }

        public override string ToString()
        {
            return $"Account: {BankAccount.Id}; Old doubtful limit: {OldValue}; New doubtful limit: {NewValue}";
        }
    }
}