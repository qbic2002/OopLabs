using System;
using System.Collections.Generic;
using Banks.Entities;
using Banks.Tools;

namespace Banks.UI
{
    public class BankUI : ConsoleUI
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
                Clear();
                WriteString($"BANK {_bank.Name} MENU");
                WriteString("1 - Add new client");
                WriteString("2 - Set credit commission");
                WriteString("3 - Set debit percent");
                WriteString("4 - Set deposit percents");
                WriteString("5 - Set credit low limit");
                WriteString("6 - Set doubtful limit");
                WriteString("7 - show clients");
                WriteString("8 - Show info");
                if (WaitForAction())
                    break;
            }
        }

        private bool WaitForAction()
        {
            int choose = ReadInt();
            switch (choose)
            {
                case 1:
                    AddClient();
                    break;
                case 2:
                    SetCreditCommission();
                    break;
                case 3:
                    SetDebitPercent();
                    break;
                case 4:
                    SetDepositPercents();
                    break;
                case 5:
                    SetCreditLowLimit();
                    break;
                case 6:
                    SetDoubtfulLimit();
                    break;
                case 7:
                    ShowClients();
                    break;
                case 8:
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
            Clear();
            WriteString("Enter First Name");
            string firstName = ReadString();
            WriteString("Enter Last Name");
            string lastName = ReadString();
            Client client = _bank.AddClient(firstName, lastName);

            var clientUI = new ClientUI(client);
            WriteString("Would you like to set your passport?\n1 - Yes\n2 - No");
            int choosePassport = ReadInt();
            switch (choosePassport)
            {
                case 1:
                    clientUI.SetPassport();
                    break;
                case 2:
                    break;
            }

            WriteString("Would you like to set your address?\n1 - Yes\n2 - No");
            int chooseAddress = ReadInt();
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

        private void SetCreditCommission()
        {
            Clear();
            WriteString($"Enter credit commission (now: {_bank.CreditCommission})");
            decimal creditCommission = ReadDecimal();
            try
            {
                _bank.SetCreditCommission(creditCommission);
            }
            catch (BanksException e)
            {
                WriteString(e.Message);
                WaitEnter();
            }
        }

        private void SetDebitPercent()
        {
            Clear();
            WriteString($"Enter debit percent (now: {_bank.DebitPercent})");
            decimal debitPercent = ReadDecimal();
            try
            {
                _bank.SetDebitPercent(debitPercent);
            }
            catch (BanksException e)
            {
                WriteString(e.Message);
                WaitEnter();
            }
        }

        private void SetDepositPercents()
        {
            WriteString("Enter default percent");
            decimal defaultPercent = ReadDecimal();
            DepositPercent depositPercent;
            try
            {
                depositPercent = new DepositPercent(defaultPercent);
            }
            catch (BanksException e)
            {
                WriteString(e.Message);
                WaitEnter();
                return;
            }

            WriteString("Enter number of ranges");
            int numberOfRanges = ReadInt();
            if (numberOfRanges <= 0)
            {
                WriteString("Number of ranges must be positive");
                return;
            }

            int ranges = 0;
            while (ranges < numberOfRanges)
            {
                Clear();
                WriteString("Enter minimum border");
                decimal min = ReadDecimal();
                WriteString("Enter max border (-1 if there are no max border)");
                decimal max = ReadDecimal();
                if (max == -1)
                    max = decimal.MaxValue;
                WriteString("Enter percent");
                decimal percent = ReadDecimal();
                try
                {
                    var range = new DepositPercentRange(percent, min, max);
                    depositPercent.AddRange(range);
                    ranges++;
                }
                catch (BanksException e)
                {
                    WriteString(e.Message);
                    WaitEnter();
                }
            }

            try
            {
                _bank.SetDepositPercents(depositPercent);
            }
            catch (BanksException e)
            {
                WriteString(e.Message);
                WaitEnter();
            }
        }

        private void ShowInfo()
        {
            Clear();
            WriteString($"Name: {_bank.Name}");
            WriteString($"Credit commission: {_bank.CreditCommission}");
            WriteString($"Debit percent: {_bank.DebitPercent}");
            WriteString($"Deposit percents: {_bank.DepositPercent.ToString()}");
            WriteString($"Doubtful limit {_bank.DoubtfulLimit}");
            WriteString($"Credit low limit: {_bank.CreditLowLimit}");
            WaitEnter();
        }

        private void SetCreditLowLimit()
        {
            Clear();
            WriteString($"Enter credit low limit ({_bank.CreditLowLimit})");
            decimal creditLowLimit = ReadDecimal();
            try
            {
                _bank.SetCreditLowLimit(creditLowLimit);
            }
            catch (BanksException e)
            {
                WriteString(e.Message);
                WaitEnter();
            }
        }

        private void SetDoubtfulLimit()
        {
            Clear();
            WriteString($"Enter doubtful limit ({_bank.DoubtfulLimit})");
            decimal doubtfulLimit = ReadDecimal();
            try
            {
                _bank.SetDoubtfulLimit(doubtfulLimit);
            }
            catch (BanksException e)
            {
                WriteString(e.Message);
                WaitEnter();
            }
        }

        private void ShowClients()
        {
            Clear();
            int index = 1;
            _bank.Clients.ForEach(client => WriteString($"{index++}: {client}"));
            ChooseClient();
        }

        private void ChooseClient()
        {
            int bankIndex = ReadInt();
            if (bankIndex == 0)
                return;
            if (bankIndex > _bank.Clients.Count)
                ChooseClient();
            var clientUI = new ClientUI(_bank.Clients[bankIndex - 1]);
            clientUI.WriteDescription();
        }
    }
}