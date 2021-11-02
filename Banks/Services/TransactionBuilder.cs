using System.Linq.Expressions;
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
            }

            return null;
        }
    }
}