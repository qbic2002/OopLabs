using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Banks.Entities;
using Banks.Tools;

namespace Banks.Services
{
    public class CentralBank : ITransactionHandler
    {
        private List<Bank> _banks = new ();
        private Dictionary<Bank, int> _bankAndKey = new ();

        public CentralBank()
        {
        }

        public Bank AddBank(string name)
        {
            var random = new Random();
            int key = random.Next(int.MaxValue);
            var bank = new Bank(key, name, this);
            _banks.Add(bank);
            _bankAndKey.Add(bank, key);
            return bank;
        }

        public void HandleTransaction(ITransaction transaction)
        {
            if (transaction is null)
                throw new BanksException("Incorrect transaction");
            TransactionBuilder.BecomeHandler(this, transaction);
            if (transaction.TransactionType != TransactionType.Transfer)
            {
                Bank bank = _banks.Find(bank => bank.ContainsBankAccount(transaction.Sender));
                if (bank is null)
                {
                    TransactionBuilder.FailTransaction(transaction);
                    throw new BanksException("Incorrect transaction");
                }

                bank.HandleTransaction(transaction);
            }
            else
            {
                Bank senderBank = _banks.Find(bank => bank.ContainsBankAccount(transaction.Sender));
                Bank receiverBank = _banks.Find(bank => bank.ContainsBankAccount(transaction.Receiver));
                if (senderBank is null)
                {
                    TransactionBuilder.FailTransaction(transaction);
                    throw new BanksException("Incorrect sender");
                }

                if (receiverBank is null)
                {
                    TransactionBuilder.FailTransaction(transaction);
                    throw new BanksException("Incorrect receiver");
                }

                if (!senderBank.BankAccountAndCredits.ContainsKey(transaction.Sender))
                {
                    TransactionBuilder.FailTransaction(transaction);
                    throw new BanksException("Cannot find info about sender");
                }

                if (!receiverBank.BankAccountAndCredits.ContainsKey(transaction.Receiver))
                {
                    TransactionBuilder.FailTransaction(transaction);
                    throw new BanksException("Cannot find info about receiver");
                }

                decimal senderAccountCredits = transaction.Sender.Credits;
                decimal minimalCreditsOnSenderAccount = transaction.Sender.MinimalCredits;
                if (senderAccountCredits - transaction.Credits < minimalCreditsOnSenderAccount)
                {
                    TransactionBuilder.FailTransaction(transaction);
                    throw new BanksException("Too low credits on account");
                }

                senderBank.ChangeCredits(_bankAndKey[senderBank], transaction.Sender, transaction.Sender.Credits - transaction.Credits);
                receiverBank.ChangeCredits(_bankAndKey[receiverBank], transaction.Receiver, transaction.Receiver.Credits + transaction.Credits);
                TransactionBuilder.SuccessTransaction(transaction);
            }
        }

        public void CancelTransaction(ITransaction transaction)
        {
            if (transaction.TransactionType != TransactionType.Transfer)
            {
                Bank bank = _banks.Find(bank => bank.ContainsBankAccount(transaction.Sender));
                if (bank is null)
                {
                    TransactionBuilder.FailTransaction(transaction);
                    throw new BanksException("Incorrect transaction");
                }

                bank.CancelTransaction(transaction);
            }
            else
            {
                Bank senderBank = _banks.Find(bank => bank.ContainsBankAccount(transaction.Sender));
                Bank receiverBank = _banks.Find(bank => bank.ContainsBankAccount(transaction.Receiver));
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
    }
}