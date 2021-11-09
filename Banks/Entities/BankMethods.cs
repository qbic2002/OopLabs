using System;
using System.Collections.Generic;
using System.IO;
using Banks.Services;
using Banks.Tools;

namespace Banks.Entities
{
    public class BankMethods
    {
        private CentralBank _centralBank;

        public BankMethods(CentralBank centralBank)
        {
            _centralBank = centralBank ?? throw new BanksException("Incorrect central bank");
        }

        public Client AddClient(string firstName, string lastName, Bank bank, Dictionary<Client, List<IBankAccount>> clientAndAccounts)
        {
            var client = new Client(bank, firstName, lastName);
            clientAndAccounts.Add(client, new List<IBankAccount>());
            return client;
        }

        public bool ContainsClient(Client client, List<Client> clients) => clients.Contains(client);
        public void AddInterest(List<IBankAccount> bankAccounts, Dictionary<IBankAccount, decimal> bankAccountAndCredits)
        {
            bankAccounts.ForEach(bankAccount => bankAccountAndCredits[bankAccount] += bankAccount.AddInterest());
        }

        public void ChargeInterest(List<IBankAccount> bankAccounts)
        {
            bankAccounts.ForEach(bankAccount => bankAccount.ChargeInterest());
        }

        public bool ContainsBankAccount(IBankAccount bankAccount, List<IBankAccount> bankAccounts) =>
            bankAccounts.Contains(bankAccount);
        public bool ContainsBankAccount(BankAccountId id, List<IBankAccount> bankAccounts) =>
            bankAccounts.Exists(bankAccount => bankAccount.Id.Equals(id));

        public decimal CalculateDepositPercent(decimal startDeposit, IDepositPercentStrategy depositPercentStrategy) =>
            depositPercentStrategy.Calculate(startDeposit);

        public IBankAccount CreateBankAccount(Client client, BankAccountType bankAccountType, decimal debitPercent, decimal creditLowLimit, decimal creditCommission, Dictionary<IBankAccount, decimal> bankAccountAndCredits, Dictionary<DepositAccount, int> depositAccountAndTerm, Dictionary<Client, List<IBankAccount>> clientAndAccounts, List<Client> clients, IDepositPercentStrategy depositPercentStrategy = null, decimal startMoney = 0, int term = 0)
        {
            if (client is null)
                throw new BanksException("Incorrect client");
            if (!ContainsClient(client, clients))
                throw new BanksException("Client not in this bank");
            if (term < 0)
                throw new BanksException("Incorrect term");
            if (startMoney < 0)
                throw new BanksException("Incorrect start money");

            IBankAccount bankAccount = null;
            var id = new BankAccountId(new Random().Next(0, 1000000000));
            switch (bankAccountType)
            {
                case BankAccountType.Debit:
                    bankAccount = new DebitAccount(client, id, 0, debitPercent, BankAccountType.Debit);
                    bankAccountAndCredits.Add(bankAccount, 0);
                    break;
                case BankAccountType.Deposit:
                    bankAccount = new DepositAccount(client, id, 0, CalculateDepositPercent(startMoney, depositPercentStrategy), BankAccountType.Deposit);
                    bankAccountAndCredits.Add(bankAccount, startMoney);
                    depositAccountAndTerm.Add(bankAccount as DepositAccount, term);
                    break;
                case BankAccountType.Credit:
                    bankAccount = new CreditAccount(client, id, creditLowLimit, creditCommission, BankAccountType.Credit);
                    bankAccountAndCredits.Add(bankAccount, startMoney);
                    break;
            }

            clientAndAccounts[client].Add(bankAccount);
            return bankAccount;
        }

        public bool IsTermExpired(DepositAccount depositAccount, Dictionary<DepositAccount, int> depositAccountAndTerm)
        {
            if (depositAccount is null)
                throw new BanksException("Incorrect deposit account");
            if (depositAccountAndTerm is null)
                throw new BanksException("Cannot find info about terms");

            return depositAccount.DaysOpened > depositAccountAndTerm[depositAccount];
        }

        public void HandleTransaction(ITransaction transaction, Bank bank, Dictionary<IBankAccount, decimal> bankAccountAndCredits)
        {
            if (transaction is null)
                throw new BanksException("Incorrect transaction");
            if (transaction.Status != TransactionStatus.Pending)
                throw new BanksException("Not pending transaction");
            TransactionBuilder.BecomeHandler(bank, transaction);

            switch (transaction.TransactionType)
            {
                case TransactionType.Withdraw:
                    WithdrawTransactionHandler(transaction as WithdrawTransaction, bankAccountAndCredits, bank.DoubtfulLimit);
                    break;
                case TransactionType.Put:
                    PutTransactionHandler(transaction as PutTransaction, bankAccountAndCredits);
                    break;
                case TransactionType.Transfer:
                    TransferTransactionHandler(transaction as TransferTransaction, bank.DoubtfulLimit, bankAccountAndCredits, bank.BankAccounts);
                    break;
            }
        }

        public void CancelTransaction(ITransaction transaction, Dictionary<IBankAccount, decimal> bankAccountAndCredits)
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
                    CancelWithdraw(transaction as WithdrawTransaction, bankAccountAndCredits);
                    break;
                case TransactionType.Put:
                    CancelPut(transaction as PutTransaction, bankAccountAndCredits);
                    break;
                case TransactionType.Transfer:
                    CancelTransfer(transaction as TransferTransaction, bankAccountAndCredits);
                    break;
            }
        }

        private void WithdrawTransactionHandler(WithdrawTransaction transaction, Dictionary<IBankAccount, decimal> bankAccountAndCredits, decimal doubtfulLimit)
        {
            if (transaction is null)
                throw new BanksException("Incorrect transaction");
            if (!bankAccountAndCredits.ContainsKey(transaction.Sender))
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

            if (transaction.Sender.IsDoubtful && transaction.Credits > doubtfulLimit)
            {
                TransactionBuilder.FailTransaction(transaction);
                throw new BanksException("Over the doubtful limit");
            }

            bankAccountAndCredits[transaction.Sender] -= transaction.Credits;
            TransactionBuilder.SuccessTransaction(transaction);
        }

        private void PutTransactionHandler(PutTransaction transaction, Dictionary<IBankAccount, decimal> bankAccountAndCredits)
        {
            if (transaction is null)
                throw new BanksException("Incorrect transaction");
            if (!bankAccountAndCredits.ContainsKey(transaction.Sender))
            {
                TransactionBuilder.FailTransaction(transaction);
                throw new BanksException("Cannot find info about sender");
            }

            bankAccountAndCredits[transaction.Sender] += transaction.Credits;
            TransactionBuilder.SuccessTransaction(transaction);
        }

        private void TransferTransactionHandler(TransferTransaction transaction, decimal doubtfulLimit, Dictionary<IBankAccount, decimal> bankAccountAndCredits, List<IBankAccount> bankAccounts)
        {
            if (transaction is null)
                throw new BanksException("Incorrect transaction");
            if (!bankAccountAndCredits.ContainsKey(transaction.Sender))
            {
                TransactionBuilder.FailTransaction(transaction);
                throw new BanksException("Cannot find info about sender");
            }

            if (!ContainsBankAccount(transaction.Receiver, bankAccounts))
            {
                _centralBank.HandleTransaction(transaction);
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

                if (transaction.Sender.IsDoubtful && transaction.Credits > doubtfulLimit)
                {
                    TransactionBuilder.FailTransaction(transaction);
                    throw new BanksException("Over the doubtful limit");
                }

                if (!bankAccountAndCredits.ContainsKey(transaction.Receiver))
                {
                    TransactionBuilder.FailTransaction(transaction);
                    throw new BanksException("Cannot find info about receiver");
                }

                bankAccountAndCredits[transaction.Sender] -= transaction.Credits;
                bankAccountAndCredits[transaction.Receiver] += transaction.Credits;
                TransactionBuilder.SuccessTransaction(transaction);
            }
        }

        private void CancelWithdraw(WithdrawTransaction transaction, Dictionary<IBankAccount, decimal> bankAccountAndCredits)
        {
            if (transaction is null)
                throw new BanksException("Incorrect transaction");
            if (!bankAccountAndCredits.ContainsKey(transaction.Sender))
                throw new BanksException("Cannot find info about sender");

            bankAccountAndCredits[transaction.Sender] += transaction.Credits;
            transaction.Status = TransactionStatus.Cancel;
        }

        private void CancelPut(PutTransaction transaction, Dictionary<IBankAccount, decimal> bankAccountAndCredits)
        {
            if (transaction is null)
                throw new BanksException("Incorrect transaction");
            if (!bankAccountAndCredits.ContainsKey(transaction.Sender))
                throw new BanksException("Cannot find info about sender");

            decimal accountCredits = transaction.Sender.Credits;
            decimal minimalCreditsOnAccount = transaction.Sender.MinimalCredits;
            if (accountCredits - transaction.Credits < minimalCreditsOnAccount)
                throw new BanksException("Too low credits on account");

            bankAccountAndCredits[transaction.Sender] -= transaction.Credits;
            transaction.Status = TransactionStatus.Cancel;
        }

        private void CancelTransfer(TransferTransaction transaction, Dictionary<IBankAccount, decimal> bankAccountAndCredits)
        {
            if (transaction is null)
                throw new BanksException("Incorrect transaction");
            if (!bankAccountAndCredits.ContainsKey(transaction.Sender))
                throw new BanksException("Cannot find info about sender");
            if (!bankAccountAndCredits.ContainsKey(transaction.Receiver))
                throw new BanksException("Cannot find info about receiver");

            bankAccountAndCredits[transaction.Sender] += transaction.Credits;
            bankAccountAndCredits[transaction.Receiver] -= transaction.Credits;
            transaction.Status = TransactionStatus.Cancel;
        }
    }
}