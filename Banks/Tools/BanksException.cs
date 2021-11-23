using System;

namespace Banks.Tools
{
    public class BanksException : Exception
    {
        public BanksException()
            : base()
        {
        }

        public BanksException(string message)
            : base(message)
        {
        }

        public BanksException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}