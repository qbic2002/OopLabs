using Banks.Tools;

namespace Banks.Entities
{
    public class TransferTransaction : ITransaction
    {
        public TransferTransaction(decimal credits, IBankAccount sender, IBankAccount receiver)
        {
            Sender = sender ?? throw new BanksException("Incorrect sender");
            Receiver = receiver ?? throw new BanksException("Incorrect receiver");
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

        public TransactionType TransactionType => TransactionType.Transfer;

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