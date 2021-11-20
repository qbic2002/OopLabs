namespace Banks.Entities
{
    public class PercentNotification : INotification
    {
        public PercentNotification(decimal oldPercent, decimal newPercent)
        {
            OldValue = oldPercent;
            NewValue = newPercent;
        }

        public IBankAccount BankAccount { get; set; }
        public decimal OldValue { get; }
        public decimal NewValue { get; }

        public override string ToString()
        {
            return $"Account: {BankAccount.Id}; Old percent: {OldValue}; New percent: {NewValue}";
        }
    }
}