using Banks.Tools;

namespace Banks.Entities
{
    public class PutTransaction : ITransaction
    {
        public PutTransaction(decimal credits, IBankAccount sender)
        {
            Sender = sender ?? throw new BanksException("Incorrect sender");
            Receiver = sender;
            if (credits <= 0)
                throw new BanksException("Invalid transaction");

            Credits = credits;
            Status = TransactionStatus.Pending;
        }

        public decimal Credits { get; }
        public TransactionStatus Status { get; set; }
        public IBankAccount Sender { get; }
        public IBankAccount Receiver { get; }
        public ITransactionHandler Handler { get; set; } = null;

        public TransactionType TransactionType => TransactionType.Put;

        public void Cancel()
        {
            Handler.CancelTransaction(this);
        }

        public override string ToString()
        {
            return new string($"Sender: {Sender.Id}; Receiver: {Receiver.Id}; Credits {Credits}; Type: {TransactionType}; Status: {Status}");
        }
    }
}