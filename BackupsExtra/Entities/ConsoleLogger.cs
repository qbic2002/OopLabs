using System;

namespace BackupsExtra.Entities
{
    public class ConsoleLogger : ILogger
    {
        public void PrintLog(string log, bool printDateTime)
        {
            if (printDateTime)
                Console.Write(DateTime.Now + ": ");
            Console.WriteLine(log);
        }
    }
}