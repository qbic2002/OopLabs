using System;
using System.Transactions;
using Banks.Services;
using Banks.Tools;

namespace Banks.Entities
{
    public class DebitAccount : IBankAccount
    {
        private Bank _bank;

        public DebitAccount(Bank bank, decimal minimalCredits)
        {
            _bank = bank ?? throw new BanksException("Incorrect bank");
            MinimalCredits = minimalCredits;
            Id = Guid.NewGuid();
        }

        public Guid Id { get; }
        public decimal MinimalCredits { get; set; }
        private decimal Credits => _bank.BankAccountAndCredits[this];

        public void WithdrawCredits(decimal credits)
        {
            if (credits <= 0)
                throw new BanksException("Incorrect credits");
            ITransaction withdraw = TransactionBuilder.CreateTransaction(TransactionType.Withdraw, credits, this);
        }

        public void PutCredits(decimal credits)
        {
            throw new System.NotImplementedException();
        }

        public void TransferCredits(decimal credits, IBankAccount receiver)
        {
            throw new System.NotImplementedException();
        }

        public void ChargeInterest()
        {
            throw new System.NotImplementedException();
        }
    }
}