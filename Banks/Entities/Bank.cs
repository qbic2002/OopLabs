using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Banks.Services;
using Banks.Tools;

namespace Banks.Entities
{
    public class Bank : ITransactionHandler
    {
        private Dictionary<IBankAccount, decimal> _bankAccountAndCredits = new ();
        private Dictionary<Client, List<IBankAccount>> _clientAndAccounts = new ();
        private Dictionary<DepositAccount, int> _depositAccountAndTerm = new ();
        private int _key;
        private ITransactionHandler _nextTransactionHandler = null;

        public Bank(int key, CentralBank centralBank, string name)
        {
            Name = name ?? throw new BanksException("Incorrect name");
            CentralBank = centralBank ?? throw new BanksException("Incorrect central bank");
            _key = key;

            BankAccountAndCredits = new ReadOnlyDictionary<IBankAccount, decimal>(_bankAccountAndCredits);
            ClientsAndAccounts = new ReadOnlyDictionary<Client, List<IBankAccount>>(_clientAndAccounts);
            DepositAccountAndTerm = new ReadOnlyDictionary<DepositAccount, int>(_depositAccountAndTerm);

            SetNextTransactionHandler(centralBank);
        }

        public string Name { get; }
        public CentralBank CentralBank { get; }
        public decimal DebitPercent { get; private set; } = 0.02M;
        public decimal CreditCommission { get; private set; } = 100M;
        public DepositPercent DepositPercent { get; private set; } = DepositPercent.GetDefault();
        public decimal CreditLowLimit { get; private set; } = -100000M;
        public decimal DoubtfulLimit { get; private set; } = 5000;
        public ReadOnlyDictionary<IBankAccount, decimal> BankAccountAndCredits { get; }
        public ReadOnlyDictionary<Client, List<IBankAccount>> ClientsAndAccounts { get; }
        public List<IBankAccount> BankAccounts => _bankAccountAndCredits.Keys.ToList();
        public List<Client> Clients => _clientAndAccounts.Keys.ToList();
        public ReadOnlyDictionary<DepositAccount, int> DepositAccountAndTerm { get; }

        public void SetPercents(DepositPercent depositPercent, decimal debitPercent = 0.2M, decimal creditCommission = 0.2M)
        {
            SetCreditCommission(creditCommission);
            SetDebitPercent(debitPercent);
        }

        public void SetCreditCommission(decimal creditCommission)
        {
            if (creditCommission < 0)
                throw new BanksException("Incorrect credit commission");
            if (CreditCommission != creditCommission)
            {
                decimal oldCreditCommission = CreditCommission;
                CreditCommission = creditCommission;
                BankAccounts.Where(bankAccount => bankAccount is CreditAccount).ToList().ForEach(bankAccount => bankAccount.Percent = CreditCommission);
                SendNotification(new CommissionNotification(oldCreditCommission, creditCommission), BankAccounts.Where(bankAccount => bankAccount is CreditAccount).ToArray());
            }
        }

        public void SetDebitPercent(decimal debitPercent)
        {
            if (debitPercent < 0)
                throw new BanksException("Incorrect debit percent");
            if (DebitPercent != debitPercent)
            {
                decimal oldDebitPercent = DebitPercent;
                DebitPercent = debitPercent;
                BankAccounts.Where(bankAccount => bankAccount is DebitAccount).ToList().ForEach(bankAccount => bankAccount.Percent = DebitPercent);
                SendNotification(new PercentNotification(oldDebitPercent, debitPercent), BankAccounts.Where(bankAccount => bankAccount is DebitAccount).ToArray());
            }
        }

        public void SetDepositPercents(decimal defaultValue, params DepositPercentRange[] depositPercentRanges) =>
            SetDepositPercents(new DepositPercent(defaultValue, depositPercentRanges));

        public void SetDepositPercents(DepositPercent depositPercent)
        {
            DepositPercent = depositPercent ?? throw new BanksException("Incorrect deposit percents");
        }

        public void SetDoubtfulLimit(decimal doubtfulLimit)
        {
            if (doubtfulLimit < 0)
                throw new BanksException("Incorrect doubtful limit");
            decimal oldDoubtfulLimit = DoubtfulLimit;
            DoubtfulLimit = doubtfulLimit;
            if (doubtfulLimit != DoubtfulLimit)
                SendNotification(new LimitNotification(oldDoubtfulLimit, doubtfulLimit), BankAccounts.Where(bankAccount => bankAccount.Client.IsDoubtful).ToArray());
        }

        public void SetCreditLowLimit(decimal creditLowLimit)
        {
            if (creditLowLimit >= 0)
                throw new BanksException("Incorrect low limit");
            CreditLowLimit = creditLowLimit;
        }

        public void ChangeCredits(int key, IBankAccount bankAccount, decimal newCredits)
        {
            if (key != _key)
                throw new BanksException("Incorrect key");
            _bankAccountAndCredits[bankAccount] = newCredits;
        }

        public Client AddClient(string firstName, string lastName)
        {
            var client = new Client(this, firstName, lastName);
            _clientAndAccounts.Add(client, new List<IBankAccount>());
            return client;
        }

        public void AddInterest()
        {
            BankAccounts.ForEach(bankAccount => _bankAccountAndCredits[bankAccount] += bankAccount.AddInterest());
        }

        public void ChargeInterest()
        {
            BankAccounts.ForEach(bankAccount => bankAccount.ChargeInterest());
        }

        public void AddOneDay()
        {
            BankAccounts.ForEach(bankAccount => bankAccount.AddOneDay());
        }

        public IBankAccount CreateBankAccount(Client client, BankAccountType bankAccountType, decimal startMoney = 0, int term = 365)
        {
            if (client is null)
                throw new BanksException("Incorrect client");
            if (!ContainsClient(client))
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
                    bankAccount = new DebitAccount(client, id, 0, DebitPercent, BankAccountType.Debit);
                    _bankAccountAndCredits.Add(bankAccount, 0);
                    break;
                case BankAccountType.Deposit:
                    bankAccount = new DepositAccount(client, id, 0, CalculateDepositPercent(startMoney), BankAccountType.Deposit);
                    _bankAccountAndCredits.Add(bankAccount, startMoney);
                    _depositAccountAndTerm.Add(bankAccount as DepositAccount, term);
                    break;
                case BankAccountType.Credit:
                    bankAccount = new CreditAccount(client, id, CreditLowLimit, CreditCommission, BankAccountType.Credit);
                    _bankAccountAndCredits.Add(bankAccount, startMoney);
                    break;
            }

            _clientAndAccounts[client].Add(bankAccount);
            return bankAccount;
        }

        public bool IsTermExpired(DepositAccount depositAccount)
        {
            if (depositAccount is null)
                throw new BanksException("Incorrect deposit account");
            if (_depositAccountAndTerm is null)
                throw new BanksException("Cannot find info about terms");

            return depositAccount.DaysOpened > _depositAccountAndTerm[depositAccount];
        }

        public void HandleTransaction(ITransaction transaction)
        {
            if (transaction is null)
                throw new BanksException("Incorrect transaction");
            if (transaction.Status != TransactionStatus.Pending)
                throw new BanksException("Not pending transaction");
            Transactions.BecomeHandler(this, transaction);

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

        public bool ContainsClient(Client client) => Clients.Contains(client);
        public bool ContainsBankAccount(IBankAccount bankAccount) =>
            BankAccounts.Contains(bankAccount);
        public bool ContainsBankAccount(BankAccountId id) =>
            BankAccounts.Exists(bankAccount => bankAccount.Id.Equals(id));

        public void SetNextTransactionHandler(ITransactionHandler nextHandler)
        {
            _nextTransactionHandler = nextHandler ?? throw new BanksException("Incorrect handler to set");
        }

        public override string ToString()
        {
            return new string($"{Name}");
        }

        private decimal CalculateDepositPercent(decimal startDeposit) => DepositPercent.Calculate(startDeposit);
        private void SendNotification(INotification notification, params IBankAccount[] bankAccounts)
        {
            if (notification is null)
                throw new BanksException("Incorrect notification");
            bankAccounts.ToList().ForEach(bankAccount => bankAccount.HandleNotification(notification));
        }

        private void WithdrawTransactionHandler(WithdrawTransaction transaction)
        {
            if (transaction is null)
                throw new BanksException("Incorrect transaction");
            if (!_bankAccountAndCredits.ContainsKey(transaction.Sender))
            {
                Transactions.FailTransaction(transaction);
                throw new BanksException("Cannot find info about sender");
            }

            decimal accountCredits = transaction.Sender.Credits;
            decimal minimalCreditsOnAccount = transaction.Sender.MinimalCredits;
            if (accountCredits - transaction.Credits < minimalCreditsOnAccount)
            {
                Transactions.FailTransaction(transaction);
                throw new BanksException("Too low credits on account");
            }

            if (transaction.Sender.Client.IsDoubtful && transaction.Credits > DoubtfulLimit)
            {
                Transactions.FailTransaction(transaction);
                throw new BanksException("Over the doubtful limit");
            }

            _bankAccountAndCredits[transaction.Sender] -= transaction.Credits;
            Transactions.SuccessTransaction(transaction);
        }

        private void PutTransactionHandler(PutTransaction transaction)
        {
            if (transaction is null)
                throw new BanksException("Incorrect transaction");
            if (!_bankAccountAndCredits.ContainsKey(transaction.Sender))
            {
                Transactions.FailTransaction(transaction);
                throw new BanksException("Cannot find info about sender");
            }

            _bankAccountAndCredits[transaction.Sender] += transaction.Credits;
            Transactions.SuccessTransaction(transaction);
        }

        private void TransferTransactionHandler(TransferTransaction transaction)
        {
            if (transaction is null)
                throw new BanksException("Incorrect transaction");
            if (!_bankAccountAndCredits.ContainsKey(transaction.Sender))
            {
                Transactions.FailTransaction(transaction);
                throw new BanksException("Cannot find info about sender");
            }

            if (!ContainsBankAccount(transaction.Receiver))
            {
                _nextTransactionHandler.HandleTransaction(transaction);
            }
            else
            {
                decimal accountCredits = transaction.Sender.Credits;
                decimal minimalCreditsOnAccount = transaction.Sender.MinimalCredits;
                if (accountCredits - transaction.Credits < minimalCreditsOnAccount)
                {
                    Transactions.FailTransaction(transaction);
                    throw new BanksException("Too low credits on account");
                }

                if (transaction.Sender.Client.IsDoubtful && transaction.Credits > DoubtfulLimit)
                {
                    Transactions.FailTransaction(transaction);
                    throw new BanksException("Over the doubtful limit");
                }

                if (!_bankAccountAndCredits.ContainsKey(transaction.Receiver))
                {
                    Transactions.FailTransaction(transaction);
                    throw new BanksException("Cannot find info about receiver");
                }

                _bankAccountAndCredits[transaction.Sender] -= transaction.Credits;
                _bankAccountAndCredits[transaction.Receiver] += transaction.Credits;
                Transactions.SuccessTransaction(transaction);
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