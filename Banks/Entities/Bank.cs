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

        public Bank(int key, CentralBank centralBank, string name)
        {
            Name = name ?? throw new BanksException("Incorrect name");
            CentralBank = centralBank ?? throw new BanksException("Incorrect central bank");
            _key = key;

            BankAccountAndCredits = new ReadOnlyDictionary<IBankAccount, decimal>(_bankAccountAndCredits);
            ClientsAndAccounts = new ReadOnlyDictionary<Client, List<IBankAccount>>(_clientAndAccounts);
            DepositAccountAndTerm = new ReadOnlyDictionary<DepositAccount, int>(_depositAccountAndTerm);
        }

        public string Name { get; }
        public CentralBank CentralBank { get; }
        public decimal DebitPercent { get; private set; } = 0.02M;
        public decimal CreditCommission { get; private set; } = 0.02M;
        public IDepositPercent DepositPercentStrategy { get; private set; } = new DepositPercent();
        public decimal CreditLowLimit { get; private set; } = -100000M;
        public decimal DoubtfulLimit { get; private set; } = 5000;
        public ReadOnlyDictionary<IBankAccount, decimal> BankAccountAndCredits { get; }
        public ReadOnlyDictionary<Client, List<IBankAccount>> ClientsAndAccounts { get; }
        public List<IBankAccount> BankAccounts => _bankAccountAndCredits.Keys.ToList();
        public List<Client> Clients => _clientAndAccounts.Keys.ToList();
        public ReadOnlyDictionary<DepositAccount, int> DepositAccountAndTerm { get; }

        public void SetPercents(IDepositPercent depositPercentStrategy, decimal debitPercent = 0.2M, decimal creditCommission = 0.2M)
        {
            if (creditCommission < 0)
                throw new BanksException("Incorrect credit commission");
            if (debitPercent < 0)
                throw new BanksException("Incorrect debit percent");

            if (CreditCommission != creditCommission)
            {
                BankAccounts.ForEach(bankAccount =>
                {
                    if (bankAccount is CreditAccount creditAccount)
                    {
                        bankAccount.Percent = CreditCommission;
                        if (bankAccount.Client.IsReceiveNotifications)
                            creditAccount.HandleNotification(new PercentNotification(creditAccount, CreditCommission, creditCommission));
                    }
                });
            }

            if (DebitPercent != debitPercent)
            {
                BankAccounts.ForEach(bankAccount =>
                {
                    if (bankAccount is DebitAccount debitAccount)
                    {
                        bankAccount.Percent = debitPercent;
                        if (bankAccount.Client.IsReceiveNotifications)
                            debitAccount.HandleNotification(new PercentNotification(debitAccount, DebitPercent, debitPercent));
                    }
                });
            }

            DepositPercentStrategy = depositPercentStrategy ?? throw new BanksException("Incorrect strategy");
            CreditCommission = creditCommission;
            DebitPercent = debitPercent;
        }

        public void SetDoubtfulLimit(decimal doubtfulLimit)
        {
            if (doubtfulLimit < 0)
                throw new BanksException("Incorrect doubtful limit");

            if (doubtfulLimit != DoubtfulLimit)
               BankAccounts.Where(bankAccount => bankAccount.Client.IsDoubtful).ToList().ForEach(bankAccount => bankAccount.HandleNotification(new LimitNotification(bankAccount, DoubtfulLimit, doubtfulLimit)));
            DoubtfulLimit = doubtfulLimit;
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

        public Client AddClient(string firstName, string lastName) =>
            CentralBank.BankMethods.AddClient(firstName, lastName, this, _clientAndAccounts);

        public void AddInterest() => CentralBank.BankMethods.AddInterest(BankAccounts, _bankAccountAndCredits);
        public void ChargeInterest() => CentralBank.BankMethods.ChargeInterest(BankAccounts);
        public void AddOneDay() => CentralBank.BankMethods.AddOneDay(BankAccounts);

        public IBankAccount CreateBankAccount(Client client, BankAccountType bankAccountType, decimal startMoney = 0, int term = 365) =>
            CentralBank.BankMethods.CreateBankAccount(client, bankAccountType, DebitPercent, CreditLowLimit, CreditCommission, _bankAccountAndCredits, _depositAccountAndTerm, _clientAndAccounts, Clients, DepositPercentStrategy, startMoney, term);

        public bool IsTermExpired(DepositAccount depositAccount) =>
            CentralBank.BankMethods.IsTermExpired(depositAccount, _depositAccountAndTerm);

        public void HandleTransaction(ITransaction transaction) =>
            CentralBank.BankMethods.HandleTransaction(transaction, this, _bankAccountAndCredits);

        public void CancelTransaction(ITransaction transaction) =>
            CentralBank.BankMethods.CancelTransaction(transaction, _bankAccountAndCredits);
        public bool ContainsClient(Client client) => CentralBank.BankMethods.ContainsClient(client, Clients);
        public bool ContainsBankAccount(IBankAccount bankAccount) => CentralBank.BankMethods.ContainsBankAccount(bankAccount, BankAccounts);
        public bool ContainsBankAccount(BankAccountId id) => CentralBank.BankMethods.ContainsBankAccount(id, BankAccounts);
        public override string ToString()
        {
            return new string($"{Name}");
        }

        private decimal CalculateDepositPercent(decimal startDeposit) =>
            CentralBank.BankMethods.CalculateDepositPercent(startDeposit, DepositPercentStrategy);
    }
}