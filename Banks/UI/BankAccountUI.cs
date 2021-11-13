﻿using System;
using System.Linq;
using Banks.Entities;
using Banks.Tools;

namespace Banks.UI
{
    public class BankAccountUI
    {
        private IBankAccount _bankAccount;

        public BankAccountUI(IBankAccount bankAccount)
        {
            _bankAccount = bankAccount ?? throw new BanksException("Incorrect bank account");
        }

        public void WriteDescription()
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine($"{_bankAccount.BankAccountType} ACCOUNT {_bankAccount.Id} MENU. Credits: {_bankAccount.Credits:0.00}");
                Console.WriteLine("1 - Put credits");
                Console.WriteLine("2 - Withdraw credits");
                Console.WriteLine("3 - Transfer credits");
                Console.WriteLine("4 - Show transactions");
                Console.WriteLine("5 - Show notifications");
                Console.WriteLine("6 - Show information");
                if (WaitForAction())
                    break;
            }
        }

        private bool WaitForAction()
        {
            int choose = Convert.ToInt32(Console.ReadLine());
            switch (choose)
            {
                case 1:
                    PutCredits();
                    break;
                case 2:
                    WithdrawCredits();
                    break;
                case 3:
                    TransferCredits();
                    break;
                case 4:
                    ShowTransaction();
                    break;
                case 5:
                    ShowNotifications();
                    break;
                case 6:
                    ShowInformation();
                    break;
                case 0:
                    return true;
                default:
                    break;
            }

            return false;
        }

        private void PutCredits()
        {
            Console.Clear();
            Console.WriteLine("Enter credits to put");
            decimal putCredits = Convert.ToDecimal(Console.ReadLine());
            try
            {
                _bankAccount.PutCredits(putCredits);
            }
            catch (BanksException e)
            {
                Console.WriteLine(e.Message);
                Console.ReadLine();
            }
        }

        private void WithdrawCredits()
        {
            Console.Clear();
            Console.WriteLine("Enter credits to withdraw");
            decimal withdrawCredits = Convert.ToDecimal(Console.ReadLine());
            try
            {
                _bankAccount.WithdrawCredits(withdrawCredits);
            }
            catch (BanksException e)
            {
                Console.WriteLine(e.Message);
                Console.ReadLine();
            }
        }

        private void TransferCredits()
        {
            Console.Clear();
            Console.WriteLine("Enter credits to transfer");
            decimal transferCredits = Convert.ToDecimal(Console.ReadLine());
            Console.WriteLine("Enter receiver");
            var receiver = new BankAccountId(Convert.ToInt32(Console.ReadLine()));
            try
            {
                _bankAccount.TransferCredits(transferCredits, receiver);
            }
            catch (BanksException e)
            {
                Console.WriteLine(e.Message);
                Console.ReadLine();
            }
        }

        private void ShowTransaction()
        {
            Console.Clear();
            int index = 1;
            _bankAccount.Transactions.ToList().ForEach(transaction => Console.WriteLine($"{index++}: {transaction}"));
            CancelTransaction();
        }

        private void CancelTransaction()
        {
            int transactionIndex = Convert.ToInt32(Console.ReadLine());
            if (transactionIndex == 0)
                return;
            if (transactionIndex > _bankAccount.Transactions.Count)
                CancelTransaction();
            try
            {
                _bankAccount.Transactions[transactionIndex - 1].Cancel();
            }
            catch (BanksException e)
            {
                Console.WriteLine(e.Message);
                Console.ReadLine();
            }
        }

        private void ShowNotifications()
        {
            Console.Clear();
            int index = 1;
            _bankAccount.Notifications.ToList().ForEach(notification => Console.WriteLine($"{index++}: {notification}"));
            Console.ReadLine();
        }

        private void ShowInformation()
        {
            Console.Clear();
            Console.WriteLine($"Type: {_bankAccount.BankAccountType}");
            Console.WriteLine($"Id: {_bankAccount.Id}");
            Console.WriteLine($"Client: {_bankAccount.Client}");
            Console.WriteLine($"Credits {_bankAccount.Credits:0.00}");
            Console.WriteLine($"Minimal credits {_bankAccount.MinimalCredits}");
            Console.WriteLine($"{(_bankAccount is CreditAccount ? new string("Commission") : new string("Percent"))} {_bankAccount.Percent}");
            Console.WriteLine($"Days opened: {_bankAccount.DaysOpened}");
            if (_bankAccount is DepositAccount depositAccount)
            {
                Console.WriteLine($"Term: {depositAccount.Term}");
            }

            Console.ReadLine();
        }
    }
}