using System.Collections.Generic;
using Banks.Entities;
using Banks.Tools;

namespace Banks.Services
{
    public class TransactionManager
    {
        private Bank _bank;
        public TransactionManager(Bank bank)
        {
            _bank = bank ?? throw new BanksException("Incorrect bank");
        }

        public Dictionary<IBankAccount, List<ITransaction>> BankAccountAndTransactions { get; } = new ();

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

        public ITransaction CreateTransaction(TransactionType transactionType, decimal credits, IBankAccount sender, IBankAccount receiver = null)
        {
            if (credits <= 0)
                throw new BanksException("Incorrect amount of credits");
            if (sender is null)
                throw new BanksException("Incorrect sender");
            ITransaction transaction = null;
            switch (transactionType)
            {
                case TransactionType.Withdraw:
                    transaction = new WithdrawTransaction(credits, sender);
                    break;
                case TransactionType.Put:
                    transaction = new PutTransaction(credits, sender);
                    break;
                case TransactionType.Transfer:
                    transaction = new TransferTransaction(credits, sender, receiver);
                    break;
            }

            if (!BankAccountAndTransactions.ContainsKey(sender))
                BankAccountAndTransactions.Add(sender, new List<ITransaction>());
            BankAccountAndTransactions[sender].Add(transaction);
            _bank.HandleTransaction(transaction);
            return null;
        }
    }
}