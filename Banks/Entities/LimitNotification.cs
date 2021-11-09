namespace Banks.Entities
{
    public class LimitNotification : INotification
    {
        public LimitNotification(IBankAccount bankAccount, decimal oldLimit, decimal newLimit)
        {
            BankAccount = bankAccount;
            OldValue = oldLimit;
            NewValue = newLimit;
        }

        public IBankAccount BankAccount { get; }
        public decimal OldValue { get; }
        public decimal NewValue { get; }

        public override string ToString()
        {
            return $"Account: {BankAccount.Id.Id}; Old doubtful limit: {OldValue}; New doubtful limit: {NewValue}";
        }
    }
}