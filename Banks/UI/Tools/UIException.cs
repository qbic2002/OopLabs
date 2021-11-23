using System;

namespace Banks.UI.Tools
{
    public class UIException : Exception
    {
        public UIException()
            : base()
        {
        }

        public UIException(string message)
            : base(message)
        {
        }

        public UIException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}