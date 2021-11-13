using System;
using Banks.Services;
using Banks.Tools;

namespace Banks.UI
{
    public class CentralBankUI
    {
        private CentralBank _centralBank;
        public CentralBankUI(CentralBank centralBank)
        {
            _centralBank = centralBank ?? throw new BanksException("Incorrect central bank");
        }

        public void WriteDescription()
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("CENTRAL BANK MENU");
                Console.WriteLine("1 - Create new Bank");
                Console.WriteLine("2 - Charge interest");
                Console.WriteLine("3 - Add Interest");
                Console.WriteLine("4 - Show Banks");
                Console.WriteLine("5 - Go to time manager");
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
                    AddBank();
                    break;
                case 2:
                    try
                    {
                        _centralBank.ChargeInterest();
                    }
                    catch (BanksException e)
                    {
                        Console.WriteLine(e.Message);
                        Console.ReadLine();
                    }

                    break;
                case 3:
                    try
                    {
                        _centralBank.AddInterest();
                    }
                    catch (BanksException e)
                    {
                        Console.WriteLine(e.Message);
                        Console.ReadLine();
                    }

                    break;
                case 4:
                    ShowBanks();
                    break;
                case 5:
                    var timeManagerUI = new TimeManagerUI(_centralBank.TimeManager);
                    timeManagerUI.WriteDescription();
                    break;
                case 0:
                    return true;
                default:
                    break;
            }

            return false;
        }

        private void AddBank()
        {
            Console.Clear();
            Console.WriteLine("Enter Name");
            string bankName = Console.ReadLine();
            try
            {
                _centralBank.AddBank(bankName);
            }
            catch (BanksException e)
            {
                Console.WriteLine(e.Message);
                Console.ReadLine();
            }
        }

        private void ShowBanks()
        {
            Console.Clear();
            int index = 1;
            _centralBank.Banks.ForEach(bank => Console.WriteLine(index++ + ": " + bank));
            ChooseBank();
        }

        private void ChooseBank()
        {
            int bankIndex = Convert.ToInt32(Console.ReadLine());
            if (bankIndex == 0)
                return;
            if (bankIndex > _centralBank.Banks.Count)
                ChooseBank();
            var bankUI = new BankUI(_centralBank.Banks[bankIndex - 1]);
            bankUI.WriteDescription();
        }
    }
}