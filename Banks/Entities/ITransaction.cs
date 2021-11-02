namespace Banks.Entities
{
    public interface ITransaction
    {
        TransactionType TransactionType { get; }
        TransactionStatus Status { get; set; }
        void Cancel();
    }
}