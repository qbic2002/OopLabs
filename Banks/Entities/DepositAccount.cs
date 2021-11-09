﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Banks.Services;
using Banks.Tools;

namespace Banks.Entities
{
    public class DepositAccount : IBankAccount
    {
        private decimal _extraMoney = 0;
        private List<ITransaction> _transactions = new ();
        private List<INotification> _notifications = new ();

        public DepositAccount(Client client, BankAccountId id, decimal minimalCredits, decimal percent, BankAccountType bankAccountType)
        {
            Client = client ?? throw new BanksException("Incorrect client");
            Id = id ?? throw new BanksException("Incorrect id");
            MinimalCredits = minimalCredits;
            BankAccountType = bankAccountType;
            if (percent < 0)
                throw new BanksException("Incorrect percent");
            Percent = percent;

            Transactions = new ReadOnlyCollection<ITransaction>(_transactions);
            Notifications = new ReadOnlyCollection<INotification>(_notifications);
        }

        public ReadOnlyCollection<ITransaction> Transactions { get; }
        public ReadOnlyCollection<INotification> Notifications { get; }
        public BankAccountId Id { get; }
        public bool IsDoubtful => Client.IsDoubtful;
        public decimal Percent { get; set; }
        public int DaysOpened { get; private set; }
        public Client Client { get; }
        public BankAccountType BankAccountType { get; }
        public decimal MinimalCredits { get; set; }
        public decimal Credits => Client.Bank.BankAccountAndCredits[this];

        public ITransaction WithdrawCredits(decimal credits)
        {
            if (!Client.Bank.IsTermExpired(this))
                throw new BanksException("Term have not expired yet");
            if (credits <= 0)
                throw new BanksException("Incorrect credits");
            ITransaction withdrawTransaction = TransactionBuilder.CreateTransaction(TransactionType.Withdraw, credits, this);
            AddTransactionToHistory(withdrawTransaction);
            Client.Bank.HandleTransaction(withdrawTransaction);
            return withdrawTransaction;
        }

        public ITransaction PutCredits(decimal credits)
        {
            if (credits <= 0)
                throw new BanksException("Incorrect credits");
            ITransaction putTransaction = TransactionBuilder.CreateTransaction(TransactionType.Put, credits, this);
            AddTransactionToHistory(putTransaction);
            Client.Bank.HandleTransaction(putTransaction);
            return putTransaction;
        }

        public ITransaction TransferCredits(decimal credits, BankAccountId receiverId) =>
            TransferCredits(credits, Client.Bank.CentralBank.FindBankAccountById(receiverId));

        public void ChargeInterest()
        {
            _extraMoney += Credits * Percent / 365;
        }

        public void CancelTransaction(ITransaction transaction)
        {
            if (transaction is null)
                throw new BanksException("Incorrect transaction");

            transaction.Cancel();
        }

        public decimal AddInterest()
        {
            decimal extraMoney = _extraMoney;
            _extraMoney = 0;
            return extraMoney;
        }

        public void AddTransactionToHistory(ITransaction transaction)
        {
            if (transaction is null)
                throw new BanksException("Incorrect transaction");

            _transactions.Add(transaction);
        }

        public void HandleNotification(INotification notification)
        {
            if (Client.IsReceiveNotifications)
                _notifications.Add(notification);
        }

        public void AddOneDay()
        {
            DaysOpened += 1;
        }

        private ITransaction TransferCredits(decimal credits, IBankAccount receiver)
        {
            if (!Client.Bank.IsTermExpired(this))
                throw new BanksException("Term have not expired yet");
            if (credits <= 0)
                throw new BanksException("Incorrect credits");
            ITransaction transferTransaction = TransactionBuilder.CreateTransaction(TransactionType.Transfer, credits, this, receiver);
            AddTransactionToHistory(transferTransaction);
            Client.Bank.HandleTransaction(transferTransaction);
            return transferTransaction;
        }
    }
}