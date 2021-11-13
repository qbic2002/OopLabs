using Banks.Entities;
using Banks.Tools;

namespace Banks.Services
{
    public static class TransactionBuilder
    {
        public static ITransaction CreateTransaction(TransactionType transactionType, decimal credits, IBankAccount sender, IBankAccount receiver = null)
        {
            if (credits <= 0)
                throw new BanksException("Incorrect amount of credits");
            if (sender is null)
                throw new BanksException("Incorrect sender");
            switch (transactionType)
            {
                case TransactionType.Withdraw:
                    return new WithdrawTransaction(credits, sender);
                case TransactionType.Put:
                    return new PutTransaction(credits, sender);
                case TransactionType.Transfer:
                    return new TransferTransaction(credits, sender, receiver);
            }

            return null;
        }

        public static void FailTransaction(ITransaction transaction)
        {
            if (transaction is null)
                throw new BanksException("Incorrect transaction");

            transaction.Status = TransactionStatus.Fail;
        }

        public static void SuccessTransaction(ITransaction transaction)
        {
            if (transaction is null)
                throw new BanksException("Incorrect transaction");

            transaction.Status = TransactionStatus.Success;
        }

        public static void BecomeHandler(ITransactionHandler handler, ITransaction transaction)
        {
            if (transaction is null)
                throw new BanksException("Incorrect transaction");
            if (handler is null)
                throw new BanksException("Incorrect handler");

            transaction.Handler = handler;
        }
    }
}