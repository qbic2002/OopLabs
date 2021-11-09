namespace Banks.Entities
{
    public class PercentNotification : INotification
    {
        public PercentNotification(IBankAccount bankAccount, decimal oldPercent, decimal newPercent)
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
            return $"Account: {BankAccount.Id.Id}; Old percent: {OldValue}; New percent: {NewValue}";
        }
    }
}