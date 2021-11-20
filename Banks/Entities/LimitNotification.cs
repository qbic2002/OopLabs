namespace Banks.Entities
{
    public class LimitNotification : INotification
    {
        public LimitNotification(decimal oldLimit, decimal newLimit)
        {
            OldValue = oldLimit;
            NewValue = newLimit;
        }

        public IBankAccount BankAccount { get; set; }
        public decimal OldValue { get; }
        public decimal NewValue { get; }

        public override string ToString()
        {
            return $"Account: {BankAccount.Id}; Old doubtful limit: {OldValue}; New doubtful limit: {NewValue}";
        }
    }
}