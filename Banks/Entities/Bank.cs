using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using Banks.Services;
using Banks.Tools;

namespace Banks.Entities
{
    public class Bank : ITransactionHandler
    {
        private Dictionary<IBankAccount, decimal> _bankAccountAndCredits = new ();
        private Dictionary<Client, List<IBankAccount>> _clientAndAccounts = new ();
        private Dictionary<DepositAccount, int> _depositAccountAndTerm = new ();
        private decimal _creditLowLimit = -100000M;
        private int _key;
        private decimal _debitPercent = 0.02M;
        private decimal _creditCommission = 0.02M;
        private IDepositPercentStrategy _depositPercentStrategy = new DefaultDepositPercentStrategy();

        public Bank(int key, CentralBank centralBank, string name)
        {
            Name = name ?? throw new BanksException("Incorrect name");
            CentralBank = centralBank ?? throw new BanksException("Incorrect central bank");
            _key = key;

            BankAccountAndCredits = new ReadOnlyDictionary<IBankAccount, decimal>(_bankAccountAndCredits);
            ClientsAndAccounts = new ReadOnlyDictionary<Client, List<IBankAccount>>(_clientAndAccounts);
        }

        public string Name { get; }
        public CentralBank CentralBank { get; }
        public decimal DoubtfulLimit { get; private set; } = 5000;
        public ReadOnlyDictionary<IBankAccount, decimal> BankAccountAndCredits { get; }
        public ReadOnlyDictionary<Client, List<IBankAccount>> ClientsAndAccounts { get; }
        public List<IBankAccount> BankAccounts => _bankAccountAndCredits.Keys.ToList();
        private List<Client> Clients => _clientAndAccounts.Keys.ToList();

        public void SetPercents(IDepositPercentStrategy depositPercentStrategy, decimal debitPercent = 0.2M, decimal creditCommission = 0.2M)
        {
            if (creditCommission < 0)
                throw new BanksException("Incorrect credit commission");
            if (debitPercent < 0)
                throw new BanksException("Incorrect debit percent");

            if (_creditCommission != creditCommission)
            {
                BankAccounts.ForEach(bankAccount =>
                {
                    if (bankAccount is CreditAccount creditAccount)
                    {
                        bankAccount.Percent = _creditCommission;
                        if (bankAccount.Client.IsReceiveNotifications)
                            creditAccount.HandleNotification(new PercentNotification(creditAccount, _creditCommission, creditCommission));
                    }
                });
            }

            if (_debitPercent != debitPercent)
            {
                BankAccounts.ForEach(bankAccount =>
                {
                    if (bankAccount is DebitAccount debitAccount)
                    {
                        bankAccount.Percent = debitPercent;
                        if (bankAccount.Client.IsReceiveNotifications)
                            debitAccount.HandleNotification(new PercentNotification(debitAccount, _debitPercent, debitPercent));
                    }
                });
            }

            _depositPercentStrategy = depositPercentStrategy ?? throw new BanksException("Incorrect strategy");
            _creditCommission = creditCommission;
            _debitPercent = debitPercent;
        }

        public void SetDoubtfulLimit(decimal doubtfulLimit)
        {
            if (doubtfulLimit < 0)
                throw new BanksException("Incorrect doubtful limit");

            if (doubtfulLimit != DoubtfulLimit)
                BankAccounts.ForEach(bankAccount => bankAccount.HandleNotification(new LimitNotification(bankAccount, DoubtfulLimit, doubtfulLimit)));
            DoubtfulLimit = doubtfulLimit;
        }

        public void SetCreditLowLimit(decimal creditLowLimit)
        {
            if (creditLowLimit >= 0)
                throw new BanksException("Incorrect low limit");
            _creditLowLimit = creditLowLimit;
        }

        public void ChangeCredits(int key, IBankAccount bankAccount, decimal newCredits)
        {
            if (key != _key)
                throw new BanksException("Incorrect key");
            _bankAccountAndCredits[bankAccount] = newCredits;
        }

        public Client AddClient(string firstName, string lastName) =>
            CentralBank.BankMethods.AddClient(firstName, lastName, this, _clientAndAccounts);

        public void AddInterest() => CentralBank.BankMethods.AddInterest(BankAccounts, _bankAccountAndCredits);
        public void ChargeInterest() => CentralBank.BankMethods.ChargeInterest(BankAccounts);

        public IBankAccount CreateBankAccount(Client client, BankAccountType bankAccountType, decimal startMoney = 0, int term = 365) =>
            CentralBank.BankMethods.CreateBankAccount(client, bankAccountType, _debitPercent, _creditLowLimit, _creditCommission, _bankAccountAndCredits, _depositAccountAndTerm, _clientAndAccounts, Clients, _depositPercentStrategy, startMoney, term);

        public bool IsTermExpired(DepositAccount depositAccount) =>
            CentralBank.BankMethods.IsTermExpired(depositAccount, _depositAccountAndTerm);

        public void HandleTransaction(ITransaction transaction) =>
            CentralBank.BankMethods.HandleTransaction(transaction, this, _bankAccountAndCredits);

        public void CancelTransaction(ITransaction transaction) =>
            CentralBank.BankMethods.CancelTransaction(transaction, _bankAccountAndCredits);
        public bool ContainsClient(Client client) => CentralBank.BankMethods.ContainsClient(client, Clients);
        public bool ContainsBankAccount(IBankAccount bankAccount) => CentralBank.BankMethods.ContainsBankAccount(bankAccount, BankAccounts);
        public bool ContainsBankAccount(BankAccountId id) => CentralBank.BankMethods.ContainsBankAccount(id, BankAccounts);

        private decimal CalculateDepositPercent(decimal startDeposit) =>
            CentralBank.BankMethods.CalculateDepositPercent(startDeposit, _depositPercentStrategy);
    }
}