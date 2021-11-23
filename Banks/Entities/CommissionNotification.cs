namespace Banks.Entities
{
    public class CommissionNotification : INotification
    {
        public CommissionNotification(decimal oldCommission, decimal newCommission)
        {
            OldValue = oldCommission;
            NewValue = newCommission;
        }

        public IBankAccount BankAccount { get; set; }
        public decimal OldValue { get; }
        public decimal NewValue { get; }

        public override string ToString()
        {
            return $"Account: {BankAccount.Id}; Old commission: {OldValue}; New commission: {NewValue}";
        }
    }
}