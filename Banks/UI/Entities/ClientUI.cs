using System;
using System.Linq;
using Banks.Entities;
using Banks.Tools;

namespace Banks.UI
{
    public class ClientUI : ConsoleUI
    {
        private Client _client;

        public ClientUI(Client client)
        {
            _client = client ?? throw new BanksException("Incorrect client");
        }

        public void WriteDescription()
        {
            while (true)
            {
                Clear();
                Console.WriteLine($"CLIENT {_client.FirstName} {_client.LastName} MENU");
                Console.WriteLine("1 - Set passport");
                Console.WriteLine("2 - Set address");
                Console.WriteLine("3 - Show bank accounts");
                Console.WriteLine("4 - Show notifications");
                Console.WriteLine("5 - Show transactions");
                Console.WriteLine("6 - Show info");
                Console.WriteLine("7 - Create debit account");
                Console.WriteLine("8 - Create credit account");
                Console.WriteLine("9 - Create deposit account");
                Console.WriteLine("10 - Receive notifications");
                if (WaitForAction())
                    break;
            }
        }

        public void SetPassport()
        {
            Clear();
            Console.WriteLine("Enter passport");
            int passport = ReadInt();
            try
            {
                _client.SetPassport(passport);
            }
            catch (BanksException e)
            {
                Console.WriteLine(e.Message);
                WaitEnter();
            }
        }

        public void SetAddress()
        {
            Clear();
            Console.WriteLine("Enter address");
            string address = ReadString();
            try
            {
                _client.SetAddress(address);
            }
            catch (BanksException e)
            {
                Console.WriteLine(e.Message);
                WaitEnter();
            }
        }

        private bool WaitForAction()
        {
            int choose = ReadInt();
            switch (choose)
            {
                case 1:
                    SetPassport();
                    break;
                case 2:
                    SetAddress();
                    break;
                case 3:
                    ShowBanksAccounts();
                    break;
                case 4:
                    ShowNotifications();
                    break;
                case 5:
                    ShowTransactions();
                    break;
                case 6:
                    ShowInfo();
                    break;
                case 7:
                    CreateDebitAccount();
                    break;
                case 8:
                    CreateCreditAccount();
                    break;
                case 9:
                    CreateDepositAccount();
                    break;
                case 10:
                    IsReceiveNotifications();
                    break;
                case 0:
                    return true;
                default:
                    break;
            }

            return false;
        }

        private void ShowBanksAccounts()
        {
            Clear();
            int index = 1;
            _client.BankAccounts.ForEach(bankAccount => Console.WriteLine($"{index++}: {bankAccount}"));
            ChooseBankAccount();
        }

        private void ChooseBankAccount()
        {
            int bankAccountIndex = ReadInt();
            if (bankAccountIndex == 0)
                return;
            if (bankAccountIndex > _client.BankAccounts.Count)
                ChooseBankAccount();

            var bankAccountUI = new BankAccountUI(_client.BankAccounts[bankAccountIndex - 1]);
            bankAccountUI.WriteDescription();
        }

        private void ShowNotifications()
        {
            Clear();
            int index = 1;
            _client.Notifications.ToList().ForEach(notification => Console.WriteLine($"{index++}: {notification}"));
            WaitEnter();
        }

        private void ShowTransactions()
        {
            Clear();
            int index = 1;
            _client.Transactions.ToList().ForEach(transaction => Console.WriteLine($"{index++}: {transaction}"));
            CancelTransaction();
        }

        private void CancelTransaction()
        {
            int transactionIndex = ReadInt();
            if (transactionIndex == 0)
                return;
            if (transactionIndex > _client.Transactions.Count)
                CancelTransaction();
            try
            {
                _client.Transactions[transactionIndex - 1].Cancel();
            }
            catch (BanksException e)
            {
                Console.WriteLine(e.Message);
                WaitEnter();
            }
        }

        private void ShowInfo()
        {
            Clear();
            Console.WriteLine($"First Name: {_client.FirstName}");
            Console.WriteLine($"Last Name: {_client.LastName}");
            Console.WriteLine($"Address: {_client.Address}");
            Console.WriteLine($"Passport: {_client.Passport}");
            Console.WriteLine($"Doubtful: {_client.IsDoubtful}");
            Console.WriteLine($"Bank: {_client.Bank}");
            WaitEnter();
        }

        private void CreateDebitAccount()
        {
            try
            {
                _client.CreateBankAccount(BankAccountType.Debit);
            }
            catch (BanksException e)
            {
                Console.WriteLine(e.Message);
                WaitEnter();
            }
        }

        private void CreateCreditAccount()
        {
            try
            {
                _client.CreateBankAccount(BankAccountType.Credit);
            }
            catch (BanksException e)
            {
                Console.WriteLine(e.Message);
                WaitEnter();
            }
        }

        private void CreateDepositAccount()
        {
            Clear();
            Console.WriteLine("Enter start deposit");
            decimal startDeposit = ReadDecimal();
            Clear();
            Console.WriteLine("Enter term");
            int term = ReadInt();
            try
            {
                _client.CreateBankAccount(BankAccountType.Deposit, startDeposit, term);
            }
            catch (BanksException e)
            {
                Console.WriteLine(e.Message);
                WaitEnter();
            }
        }

        private void IsReceiveNotifications()
        {
            Clear();
            Console.WriteLine("Would you like to receive notifications?\n1 - Yes\n2 - No");
            int choose = ReadInt();
            switch (choose)
            {
                case 1:
                    try
                    {
                        _client.ReceiveNotification(true);
                    }
                    catch (BanksException e)
                    {
                        Console.WriteLine(e.Message);
                        WaitEnter();
                    }

                    break;
                case 2:
                    try
                    {
                        _client.ReceiveNotification(false);
                    }
                    catch (BanksException e)
                    {
                        Console.WriteLine(e.Message);
                        WaitEnter();
                    }

                    break;
            }
        }
    }
}