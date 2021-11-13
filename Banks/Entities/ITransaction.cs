namespace Banks.Entities
{
    public interface ITransaction
    {
        decimal Credits { get; }
        IBankAccount Sender { get; }
        IBankAccount Receiver { get; }
        ITransactionHandler Handler { get; set; }
        TransactionType TransactionType { get; }
        TransactionStatus Status { get; set; }
        void Cancel();
        string ToString();
    }
}