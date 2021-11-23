namespace Banks.Entities
{
    public interface INotification
    {
        public IBankAccount BankAccount { get; set; }
        public decimal OldValue { get; }
        public decimal NewValue { get; }
    }
}