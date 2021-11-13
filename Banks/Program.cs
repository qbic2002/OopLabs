using System;
using Banks.Services;
using Banks.UI;

namespace Banks
{
    internal static class Program
    {
        private static void Main()
        {
            var ui = new CentralBankUI(new CentralBank());
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
