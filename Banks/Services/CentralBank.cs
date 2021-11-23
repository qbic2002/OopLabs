using System;
using System.Collections.Generic;
using Banks.Entities;
using Banks.Tools;

namespace Banks.Services
{
    public class CentralBank : ITransactionHandler
    {
        private Dictionary<Bank, int> _bankAndKey = new ();
        private ITransactionHandler _nextTransactionHandler = null;
        private Random _random;

        public CentralBank()
        {
            _random = new Random();
        }

        public TimeManager TimeManager { get; set; }
        public List<Bank> Banks { get; } = new List<Bank>();

        public Bank AddBank(string name)
        {
            if (name is null)
                throw new BanksException("Incorrect name");

            _random = new Random();
            int key = _random.Next(int.MaxValue);
            var bank = new Bank(key, this, name);
            var transactionManager = new TransactionManager(bank);
            bank.SetTransactionManager(transactionManager);
            Banks.Add(bank);
            _bankAndKey.Add(bank, key);
            return bank;
        }

        public IBankAccount FindBankAccountById(BankAccountId id)
        {
            Bank bankWithAccount = Banks.Find(bank => bank.ContainsBankAccount(id));
            if (bankWithAccount is null)
                throw new BanksException("Cannot find account");
            return bankWithAccount.BankAccounts.Find(bankAccount => bankAccount.Id.Equals(id));
        }

        public void AddInterest()
        {
            Banks.ForEach(bank => bank.AddInterest());
        }

        public void ChargeInterest()
        {
            Banks.ForEach(bank => bank.ChargeInterest());
        }

        public void AddOneDay()
        {
            Banks.ForEach(bank => bank.AddOneDay());
            ChargeInterest();
        }

        public void HandleTransaction(ITransaction transaction)
        {
            if (transaction is null)
                throw new BanksException("Incorrect transaction");
            TransactionManager.BecomeHandler(this, transaction);
            if (transaction.Sender == transaction.Receiver)
            {
                SetNextTransactionHandler(Banks.Find(bank => bank.ContainsBankAccount(transaction.Receiver)));
                _nextTransactionHandler.HandleTransaction(transaction);
            }

            if (transaction.Sender != transaction.Receiver)
            {
                Bank senderBank = Banks.Find(bank => bank.ContainsBankAccount(transaction.Sender));
                Bank receiverBank = Banks.Find(bank => bank.ContainsBankAccount(transaction.Receiver));
                if (senderBank is null)
                {
                    TransactionManager.FailTransaction(transaction);
                    throw new BanksException("Incorrect sender");
                }

                if (receiverBank is null)
                {
                    TransactionManager.FailTransaction(transaction);
                    throw new BanksException("Incorrect receiver");
                }

                if (!senderBank.BankAccountAndCredits.ContainsKey(transaction.Sender))
                {
                    TransactionManager.FailTransaction(transaction);
                    throw new BanksException("Cannot find info about sender");
                }

                if (!receiverBank.BankAccountAndCredits.ContainsKey(transaction.Receiver))
                {
                    TransactionManager.FailTransaction(transaction);
                    throw new BanksException("Cannot find info about receiver");
                }

                switch (transaction.TransactionType)
                {
                    case TransactionType.Transfer:
                        TransferTransactionHandler(transaction as TransferTransaction);
                        break;
                }
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

            if (transaction.TransactionType != TransactionType.Transfer)
            {
                Bank bank = Banks.Find(bank => bank.ContainsBankAccount(transaction.Sender));
                if (bank is null)
                {
                    throw new BanksException("Incorrect transaction");
                }

                bank.CancelTransaction(transaction);
            }
            else
            {
                Bank senderBank = Banks.Find(bank => bank.ContainsBankAccount(transaction.Sender));
                Bank receiverBank = Banks.Find(bank => bank.ContainsBankAccount(transaction.Receiver));
                if (senderBank is null)
                {
                    throw new BanksException("Incorrect sender");
                }

                if (receiverBank is null)
                {
                    throw new BanksException("Incorrect receiver");
                }

                if (!senderBank.BankAccountAndCredits.ContainsKey(transaction.Sender))
                {
                    throw new BanksException("Cannot find info about sender");
                }

                if (!receiverBank.BankAccountAndCredits.ContainsKey(transaction.Receiver))
                {
                    throw new BanksException("Cannot find info about receiver");
                }

                senderBank.ChangeCredits(_bankAndKey[senderBank], transaction.Sender, transaction.Sender.Credits + transaction.Credits);
                receiverBank.ChangeCredits(_bankAndKey[receiverBank], transaction.Receiver, transaction.Receiver.Credits - transaction.Credits);
                transaction.Status = TransactionStatus.Cancel;
            }
        }

        public void SetNextTransactionHandler(ITransactionHandler nextHandler)
        {
            _nextTransactionHandler = nextHandler ?? throw new BanksException("Incorrect handler to set");
        }

        private void TransferTransactionHandler(TransferTransaction transaction)
        {
            decimal senderAccountCredits = transaction.Sender.Credits;
            decimal minimalCreditsOnSenderAccount = transaction.Sender.MinimalCredits;
            if (senderAccountCredits - transaction.Credits < minimalCreditsOnSenderAccount)
            {
                TransactionManager.FailTransaction(transaction);
                throw new BanksException("Too low credits on account");
            }

            if (transaction.Sender.Client.IsDoubtful && transaction.Credits > transaction.Sender.Bank.DoubtfulLimit)
            {
                TransactionManager.FailTransaction(transaction);
                throw new BanksException("Over the doubtful limit");
            }

            transaction.Sender.Bank.ChangeCredits(_bankAndKey[transaction.Sender.Bank], transaction.Sender, transaction.Sender.Credits - transaction.Credits);
            transaction.Receiver.Bank.ChangeCredits(_bankAndKey[transaction.Receiver.Bank], transaction.Receiver, transaction.Receiver.Credits + transaction.Credits);
            TransactionManager.SuccessTransaction(transaction);
        }
    }
}