using Banks.Services;
using Banks.Tools;

namespace Banks.UI.Entities
{
    public class CentralBankUI : ConsoleUI
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
                Clear();
                WriteString("CENTRAL BANK MENU");
                WriteString("1 - Create new Bank");
                WriteString("2 - Charge interest");
                WriteString("3 - Add Interest");
                WriteString("4 - Show Banks");
                WriteString("5 - Go to time manager");
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
                    AddBank();
                    break;
                case 2:
                    try
                    {
                        _centralBank.ChargeInterest();
                    }
                    catch (BanksException e)
                    {
                        WriteString(e.Message);
                        WaitEnter();
                    }

                    break;
                case 3:
                    try
                    {
                        _centralBank.AddInterest();
                    }
                    catch (BanksException e)
                    {
                        WriteString(e.Message);
                        WaitEnter();
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
            Clear();
            WriteString("Enter Name");
            string bankName = ReadString();
            try
            {
                _centralBank.AddBank(bankName);
            }
            catch (BanksException e)
            {
                WriteString(e.Message);
                WaitEnter();
            }
        }

        private void ShowBanks()
        {
            Clear();
            int index = 1;
            _centralBank.Banks.ForEach(bank => WriteString(index++ + ": " + bank));
            ChooseBank();
        }

        private void ChooseBank()
        {
            int bankIndex = ReadInt();
            if (bankIndex == 0)
                return;
            if (bankIndex > _centralBank.Banks.Count)
                ChooseBank();
            var bankUI = new BankUI(_centralBank.Banks[bankIndex - 1]);
            bankUI.WriteDescription();
        }
    }
}