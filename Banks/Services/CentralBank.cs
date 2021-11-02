using System.Collections.Generic;
using System.Threading;
using Banks.Entities;

namespace Banks.Services
{
    public class CentralBank : ITransactionHandler
    {
        private List<Bank> _banks = new ();

        public CentralBank()
        {
        }

        public Bank AddBank(string name)
        {
            var bank = new Bank(name, this);
            _banks.Add(bank);
            return bank;
        }

        public void HandleTransaction(ITransaction transaction)
        {
            throw new System.NotImplementedException();
        }

        private void FailTransaction(ITransaction transaction)
        {
            throw new System.NotImplementedException();
        }
    }
}