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
        private List<IBankAccount> _bankAccounts = new ();
        private int _key;

        public Bank(int key, string name, CentralBank centralBank)
        {
            Name = name ?? throw new BanksException("Incorrect name");
            CentralBank = centralBank ?? throw new BanksException("Incorrect central bank");
            _key = key;

            BankAccountAndCredits = new ReadOnlyDictionary<IBankAccount, decimal>(_bankAccountAndCredits);
            ClientsAndAccounts = new ReadOnlyDictionary<Client, List<IBankAccount>>(_clientAndAccounts);
        }

        public string Name { get; }
        public CentralBank CentralBank { get; }
        public ReadOnlyDictionary<IBankAccount, decimal> BankAccountAndCredits { get; }
        public ReadOnlyDictionary<Client, List<IBankAccount>> ClientsAndAccounts { get; }

        public void ChangeCredits(int key, IBankAccount bankAccount, decimal newCredits)
        {
            if (key != _key)
                throw new BanksException("Incorrect key");
            _bankAccountAndCredits[bankAccount] = newCredits;
        }

        public Client AddClient(string firstName, string lastName)
        {
            var client = new Client(this, firstName, lastName);
            _clients.Add(client);
            _clientAndAccounts.Add(client, new List<IBankAccount>());
            return client;
        }

        public IBankAccount CreateBankAccount(Client client, BankAccountType bankAccountType)
        {
            if (client is null)
                throw new BanksException("Incorrect client");
            if (!ContainsClient(client))
                throw new BanksException("Client not in this bank");

            IBankAccount bankAccount = null;
            switch (bankAccountType)
            {
                case BankAccountType.Debit:
                    bankAccount = new DebitAccount(client, 0, BankAccountType.Debit);
                    _bankAccountAndCredits.Add(bankAccount, 0);
                    break;
            }

            if (!_clientAndAccounts.ContainsKey(client))
                _clientAndAccounts.Add(client, new List<IBankAccount>());

            _clientAndAccounts[client].Add(bankAccount);

            _bankAccounts.Add(bankAccount);
            return bankAccount;
        }

        public void HandleTransaction(ITransaction transaction)
        {
            if (transaction is null)
                throw new BanksException("Incorrect transaction");
            if (transaction.Status != TransactionStatus.Pending)
                throw new BanksException("Not pending transaction");
            TransactionBuilder.BecomeHandler(this, transaction);

            switch (transaction.TransactionType)
            {
                case TransactionType.Withdraw:
                    WithdrawTransactionHandler(transaction as WithdrawTransaction);
                    break;
                case TransactionType.Put:
                    PutTransactionHandler(transaction as PutTransaction);
                    break;
                case TransactionType.Transfer:
                    TransferTransactionHandler(transaction as TransferTransaction);
                    break;
            }
        }

        public void CancelTransaction(ITransaction transaction)
        {
            if (transaction is null)
                throw new BanksException("Incorrect transaction");
            if (transaction.Status == TransactionStatus.Pending)
                transaction.Status = TransactionStatus.Cancel;
            if (transaction.Status == TransactionStatus.Fail || transaction.Status == TransactionStatus.Cancel)
                throw new BanksException("Transaction cannot be cancelled");

            switch (transaction.TransactionType)
            {
                case TransactionType.Withdraw:
                    CancelWithdraw(transaction as WithdrawTransaction);
                    break;
                case TransactionType.Put:
                    CancelPut(transaction as PutTransaction);
                    break;
                case TransactionType.Transfer:
                    CancelTransfer(transaction as TransferTransaction);
                    break;
            }
        }

        public bool ContainsClient(Client client) => _clients.Contains(client);
        public bool ContainsBankAccount(IBankAccount bankAccount) => _bankAccounts.Contains(bankAccount);

        private void WithdrawTransactionHandler(WithdrawTransaction transaction)
        {
            if (transaction is null)
                throw new BanksException("Incorrect transaction");
            if (!_bankAccountAndCredits.ContainsKey(transaction.Sender))
            {
                TransactionBuilder.FailTransaction(transaction);
                throw new BanksException("Cannot find info about sender");
            }

            decimal accountCredits = transaction.Sender.Credits;
            decimal minimalCreditsOnAccount = transaction.Sender.MinimalCredits;
            if (accountCredits - transaction.Credits < minimalCreditsOnAccount)
            {
                TransactionBuilder.FailTransaction(transaction);
                throw new BanksException("Too low credits on account");
            }

            _bankAccountAndCredits[transaction.Sender] -= transaction.Credits;
            TransactionBuilder.SuccessTransaction(transaction);
        }

        private void PutTransactionHandler(PutTransaction transaction)
        {
            if (transaction is null)
                throw new BanksException("Incorrect transaction");
            if (!_bankAccountAndCredits.ContainsKey(transaction.Sender))
            {
                TransactionBuilder.FailTransaction(transaction);
                throw new BanksException("Cannot find info about sender");
            }

            _bankAccountAndCredits[transaction.Sender] += transaction.Credits;
            TransactionBuilder.SuccessTransaction(transaction);
        }

        private void TransferTransactionHandler(TransferTransaction transaction)
        {
            if (transaction is null)
                throw new BanksException("Incorrect transaction");
            if (!_bankAccountAndCredits.ContainsKey(transaction.Sender))
            {
                TransactionBuilder.FailTransaction(transaction);
                throw new BanksException("Cannot find info about sender");
            }

            if (!ContainsBankAccount(transaction.Receiver))
            {
                CentralBank.HandleTransaction(transaction);
            }
            else
            {
                decimal accountCredits = transaction.Sender.Credits;
                decimal minimalCreditsOnAccount = transaction.Sender.MinimalCredits;
                if (accountCredits - transaction.Credits < minimalCreditsOnAccount)
                {
                    TransactionBuilder.FailTransaction(transaction);
                    throw new BanksException("Too low credits on account");
                }

                if (!_bankAccountAndCredits.ContainsKey(transaction.Receiver))
                {
                    TransactionBuilder.FailTransaction(transaction);
                    throw new BanksException("Cannot find info about receiver");
                }

                _bankAccountAndCredits[transaction.Sender] -= transaction.Credits;
                _bankAccountAndCredits[transaction.Receiver] += transaction.Credits;
                TransactionBuilder.SuccessTransaction(transaction);
            }
        }

        private void CancelWithdraw(WithdrawTransaction transaction)
        {
            if (transaction is null)
                throw new BanksException("Incorrect transaction");
            if (!_bankAccountAndCredits.ContainsKey(transaction.Sender))
                throw new BanksException("Cannot find info about sender");

            _bankAccountAndCredits[transaction.Sender] += transaction.Credits;
            transaction.Status = TransactionStatus.Cancel;
        }

        private void CancelPut(PutTransaction transaction)
        {
            if (transaction is null)
                throw new BanksException("Incorrect transaction");
            if (!_bankAccountAndCredits.ContainsKey(transaction.Sender))
                throw new BanksException("Cannot find info about sender");

            decimal accountCredits = transaction.Sender.Credits;
            decimal minimalCreditsOnAccount = transaction.Sender.MinimalCredits;
            if (accountCredits - transaction.Credits < minimalCreditsOnAccount)
                throw new BanksException("Too low credits on account");

            _bankAccountAndCredits[transaction.Sender] -= transaction.Credits;
            transaction.Status = TransactionStatus.Cancel;
        }

        private void CancelTransfer(TransferTransaction transaction)
        {
            if (transaction is null)
                throw new BanksException("Incorrect transaction");
            if (!_bankAccountAndCredits.ContainsKey(transaction.Sender))
                throw new BanksException("Cannot find info about sender");
            if (!_bankAccountAndCredits.ContainsKey(transaction.Receiver))
                throw new BanksException("Cannot find info about receiver");

            _bankAccountAndCredits[transaction.Sender] += transaction.Credits;
            _bankAccountAndCredits[transaction.Receiver] -= transaction.Credits;
            transaction.Status = TransactionStatus.Cancel;
        }
    }
}