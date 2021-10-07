using IsuExtra.Tools;

namespace IsuExtra.Entities
{
    public class TimeFormat
    {
        private static int lengthOfString = 5;
        public TimeFormat(string timeString)
        {
            if (timeString is null)
                throw new IsuExtraException("Incorrect time");
            
        }

        public string TimeString { get; }
        public int Seconds { get; }
        public int Minutes { get; }
        public int Hours { get; }

        public void ParseTimeString(string timeString)
        {
            if (timeString.Length != lengthOfString)
                throw new IsuExtraException("Incorrect format of time");

            string hoursString = timeString[0].ToString() + timeString[1].ToString();
            if (!int.TryParse(hoursString, out int hourse))
                throw new IsuExtraException("Incorrect format of time");

            string minutesString = timeString[0].ToString() + timeString[1].ToString();
            if (!int.TryParse(hoursString, out int hourse))
                throw new IsuExtraException("Incorrect format of time");
        }
    }
}