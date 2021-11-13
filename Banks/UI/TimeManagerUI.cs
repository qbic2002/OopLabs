using System;
using Banks.Entities;
using Banks.Services;
using Banks.Tools;

namespace Banks.UI
{
    public class TimeManagerUI
    {
        private TimeManager _timeManager;

        public TimeManagerUI(TimeManager timeManager)
        {
            _timeManager = timeManager ?? throw new BanksException("Incorrect time manager");
        }

        public void WriteDescription()
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("TIME MANAGER MENU");
                Console.WriteLine("1 - Skip 1 day");
                Console.WriteLine("2 - Skip 1 month");
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
                    try
                    {
                        _timeManager.SkipDay();
                    }
                    catch (BanksException e)
                    {
                        Console.WriteLine(e.Message);
                        Console.ReadLine();
                    }

                    break;
                case 2:
                    try
                    {
                        _timeManager.SkipMonth();
                    }
                    catch (BanksException e)
                    {
                        Console.WriteLine(e.Message);
                        Console.ReadLine();
                    }

                    break;
                case 0:
                    return true;
                default:
                    break;
            }

            return false;
        }
    }
}