using Banks.Tools;

namespace Banks.Entities
{
    public class WithdrawTransaction : ITransaction
    {
        public WithdrawTransaction(decimal credits, IBankAccount sender)
        {
            Sender = sender ?? throw new BanksException("Incorrect sender");
            if (credits <= 0)
                throw new BanksException("Invalid transaction");

            Credits = credits;
            Status = TransactionStatus.Pending;
        }

        public decimal Credits { get; }
        public TransactionStatus Status { get; set; }
        public IBankAccount Sender { get; }

        public TransactionType TransactionType => TransactionType.Withdraw;

        public void Cancel()
        {
            throw new System.NotImplementedException();
        }
    }
}