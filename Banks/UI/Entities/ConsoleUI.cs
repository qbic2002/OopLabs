using System;
using Banks.UI.Tools;

namespace Banks.UI
{
    public abstract class ConsoleUI : IReadInt, IReadDecimal, IWriteString, IClear, IWaitEnter, IReadString
    {
        public int ReadInt()
        {
            int result = default;
            try
            {
                result = Convert.ToInt32(Console.ReadLine());
            }
            catch (Exception)
            {
                throw new UIException("Incorrect input");
            }

            return result;
        }

        public decimal ReadDecimal()
        {
            decimal result = default;
            try
            {
                result = Convert.ToDecimal(Console.ReadLine());
            }
            catch (Exception)
            {
                throw new UIException("Incorrect input");
            }

            return result;
        }

        public void WriteString(string stringWrite)
        {
            Console.WriteLine(stringWrite);
        }

        public void Clear()
        {
            Console.Clear();
        }

        public void WaitEnter()
        {
            Console.ReadLine();
        }

        public string ReadString()
        {
            return Console.ReadLine();
        }
    }
}