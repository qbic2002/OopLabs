using System;
using System.Transactions;
using Banks.Services;
using Banks.Tools;

namespace Banks.Entities
{
    public class DebitAccount : IBankAccount
    {
        public DebitAccount(Client client, decimal minimalCredits, BankAccountType bankAccountType)
        {
            Client = client ?? throw new BanksException("Incorrect client");
            MinimalCredits = minimalCredits;
            Id = Guid.NewGuid();
            BankAccountType = bankAccountType;
        }

        public Guid Id { get; }
        public decimal MinimalCredits { get; set; }
        public BankAccountType BankAccountType { get; }
        public decimal Credits => Client.Bank.BankAccountAndCredits[this];
        public Client Client { get; }

        public ITransaction WithdrawCredits(decimal credits)
        {
            if (credits <= 0)
                throw new BanksException("Incorrect credits");
            ITransaction withdrawTransaction = TransactionBuilder.CreateTransaction(TransactionType.Withdraw, credits, this);
            Client.Bank.HandleTransaction(withdrawTransaction);
            return withdrawTransaction;
        }

        public ITransaction PutCredits(decimal credits)
        {
            if (credits <= 0)
                throw new BanksException("Incorrect credits");
            ITransaction putTransaction = TransactionBuilder.CreateTransaction(TransactionType.Put, credits, this);
            Client.Bank.HandleTransaction(putTransaction);
            return putTransaction;
        }

        public ITransaction TransferCredits(decimal credits, IBankAccount receiver)
        {
            if (credits <= 0)
                throw new BanksException("Incorrect credits");
            ITransaction transferTransaction = TransactionBuilder.CreateTransaction(TransactionType.Transfer, credits, this, receiver);
            Client.Bank.HandleTransaction(transferTransaction);
            return transferTransaction;
        }

        public void ChargeInterest()
        {
            throw new System.NotImplementedException();
        }

        public void CancelTransaction(ITransaction transaction)
        {
            if (transaction is null)
                throw new BanksException("Incorrect transaction");

            transaction.Cancel();
        }
    }
}