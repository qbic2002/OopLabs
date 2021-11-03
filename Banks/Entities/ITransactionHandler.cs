namespace Banks.Entities
{
    public interface ITransactionHandler
    {
        void HandleTransaction(ITransaction transaction);
        void CancelTransaction(ITransaction transaction);
    }
}