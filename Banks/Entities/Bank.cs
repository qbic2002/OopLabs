using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Banks.Services;
using Banks.Tools;

namespace Banks.Entities
{
    public class Bank : ITransactionHandler
    {
        private Dictionary<IBankAccount, decimal> _bankAccountAndCredits = new ();
        private Dictionary<Client, List<IBankAccount>> _clientAndAccounts = new ();
        private List<Client> _clients = new ();

        public Bank(string name, CentralBank centralBank)
        {
            Name = name ?? throw new BanksException("Incorrect name");
            CentralBank = centralBank ?? throw new BanksException("Incorrect central bank");

            BankAccountAndCredits = new ReadOnlyDictionary<IBankAccount, decimal>(_bankAccountAndCredits);
        }

        public string Name { get; }
        public CentralBank CentralBank { get; }
        public ReadOnlyDictionary<IBankAccount, decimal> BankAccountAndCredits { get; }

        public Client AddClient(string firstName, string lastName)
        {
            var client = new Client(firstName, lastName);
            _clients.Add(client);
            return client;
        }

        public void HandleTransaction(ITransaction transaction)
        {
            if (transaction is null)
                throw new BanksException("Incorrect transaction");
            if (transaction.Status != TransactionStatus.Pending)
                throw new BanksException("Not pending transaction");

            switch (transaction.TransactionType)
            {
                case TransactionType.Withdraw:
                    WithdrawTransactionHandler(transaction as WithdrawTransaction);
                    break;
            }
        }

        public void FailTransaction(ITransaction transaction)
        {
            if (transaction is null)
                throw new BanksException("Incorrect transaction");

            transaction.Status = TransactionStatus.Fail;
        }

        public void SuccessTransaction(ITransaction transaction)
        {
            if (transaction is null)
                throw new BanksException("Incorrect transaction");

            transaction.Status = TransactionStatus.Success;
        }

        private void WithdrawTransactionHandler(WithdrawTransaction transaction)
        {
            if (transaction is null)
                throw new BanksException("Incorrect transaction");
            if (!_bankAccountAndCredits.ContainsKey(transaction.Sender))
            {
                FailTransaction(transaction);
                throw new BanksException("Cannot find info about sender");
            }

            decimal accountCredits = _bankAccountAndCredits[transaction.Sender];
            decimal minimalCreditsOnAccount = transaction.Sender.MinimalCredits;
            if (accountCredits - transaction.Credits < minimalCreditsOnAccount)
            {
                FailTransaction(transaction);
                throw new BanksException("Too low money on account");
            }

            _bankAccountAndCredits[transaction.Sender] = accountCredits - transaction.Credits;
            SuccessTransaction(transaction);
        }
    }
}