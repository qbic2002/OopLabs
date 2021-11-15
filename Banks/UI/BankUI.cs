using System;
using Banks.Entities;
using Banks.Tools;

namespace Banks.UI
{
    public class BankUI
    {
        private Bank _bank;

        public BankUI(Bank bank)
        {
            _bank = bank ?? throw new BanksException("Incorrect bank");
        }

        public void WriteDescription()
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine($"BANK {_bank.Name} MENU");
                Console.WriteLine("1 - Add new client");
                Console.WriteLine("2 - Change percents");
                Console.WriteLine("3 - Set credit low limit");
                Console.WriteLine("4 - Set doubtful limit");
                Console.WriteLine("5 - show clients");
                Console.WriteLine("6 - Show info");
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
                    AddClient();
                    break;
                case 2:
                    SetPercents();
                    break;
                case 3:
                    SetCreditLowLimit();
                    break;
                case 4:
                    SetDoubtfulLimit();
                    break;
                case 5:
                    ShowClients();
                    break;
                case 6:
                    ShowInfo();
                    break;
                case 0:
                    return true;
                default:
                    break;
            }

            return false;
        }

        private void AddClient()
        {
            Console.Clear();
            Console.WriteLine("Enter First Name");
            string firstName = Console.ReadLine();
            Console.WriteLine("Enter Last Name");
            string lastName = Console.ReadLine();
            Client client = _bank.AddClient(firstName, lastName);

            var clientUI = new ClientUI(client);
            Console.WriteLine("Would you like to set your passport?\n1 - Yes\n2 - No");
            int choosePassport = Convert.ToInt32(Console.ReadLine());
            switch (choosePassport)
            {
                case 1:
                    clientUI.SetPassport();
                    break;
                case 2:
                    break;
            }

            Console.WriteLine("Would you like to set your address?\n1 - Yes\n2 - No");
            int chooseAddress = Convert.ToInt32(Console.ReadLine());
            switch (chooseAddress)
            {
                case 1:
                    clientUI.SetAddress();
                    break;
                case 2:
                    break;
            }

            clientUI.WriteDescription();
        }

        private void SetPercents()
        {
            Console.Clear();
            Console.WriteLine($"Enter deposit percents");
            decimal firstPercent = Convert.ToDecimal(Console.ReadLine());
            decimal secondPercent = Convert.ToDecimal(Console.ReadLine());
            decimal thirdPercent = Convert.ToDecimal(Console.ReadLine());
            Console.WriteLine("Enter debit percent and credit percent");
            decimal debitPercent = Convert.ToDecimal(Console.ReadLine());
            decimal creditCommission = Convert.ToDecimal(Console.ReadLine());
            try
            {
                _bank.SetPercents(new DepositPercent(firstPercent, secondPercent, thirdPercent), debitPercent, creditCommission);
            }
            catch (BanksException e)
            {
                Console.WriteLine(e.Message);
                Console.ReadLine();
            }
        }

        private void ShowInfo()
        {
            Console.Clear();
            Console.WriteLine($"Name: {_bank.Name}");
            Console.WriteLine($"Credit commission: {_bank.CreditCommission}");
            Console.WriteLine($"Debit percent: {_bank.DebitPercent}");
            Console.WriteLine($"Deposit percents: {_bank.DepositPercentStrategy.ToString()}");
            Console.WriteLine($"Doubtful limit {_bank.DoubtfulLimit}");
            Console.WriteLine($"Credit low limit: {_bank.CreditLowLimit}");
            Console.ReadLine();
        }

        private void SetCreditLowLimit()
        {
            Console.Clear();
            Console.WriteLine("Enter credit low limit");
            int creditLowLimit = Convert.ToInt32(Console.ReadLine());
            try
            {
                _bank.SetCreditLowLimit(creditLowLimit);
            }
            catch (BanksException e)
            {
                Console.WriteLine(e.Message);
                Console.ReadLine();
            }
        }

        private void SetDoubtfulLimit()
        {
            Console.Clear();
            Console.WriteLine("Enter doubtful limit");
            int doubtfulLimit = Convert.ToInt32(Console.ReadLine());
            try
            {
                _bank.SetDoubtfulLimit(doubtfulLimit);
            }
            catch (BanksException e)
            {
                Console.WriteLine(e.Message);
                Console.ReadLine();
            }
        }

        private void ShowClients()
        {
            Console.Clear();
            int index = 1;
            _bank.Clients.ForEach(client => Console.WriteLine($"{index++}: {client}"));
            ChooseClient();
        }

        private void ChooseClient()
        {
            int bankIndex = Convert.ToInt32(Console.ReadLine());
            if (bankIndex == 0)
                return;
            if (bankIndex > _bank.Clients.Count)
                ChooseClient();
            var clientUI = new ClientUI(_bank.Clients[bankIndex - 1]);
            clientUI.WriteDescription();
        }
    }
}