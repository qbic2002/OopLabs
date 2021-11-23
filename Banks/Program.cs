using System;
using Banks.Services;
using Banks.UI.Entities;

namespace Banks
{
    internal static class Program
    {
        private static void Main()
        {
            var centralBank = new CentralBank();
            var timeManager = new TimeManager(centralBank);
            var ui = new CentralBankUI(centralBank);
            while (true)
            {
                try
                {
                    ui.WriteDescription();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }
        }
    }
}
