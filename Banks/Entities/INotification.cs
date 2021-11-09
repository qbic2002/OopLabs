namespace Banks.Entities
{
    public interface INotification
    {
        public IBankAccount BankAccount { get; }
        public decimal OldValue { get; }
        public decimal NewValue { get; }
        string ToString();
    }
}