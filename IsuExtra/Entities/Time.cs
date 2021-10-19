using System;
using Isu.Tools;
using IsuExtra.Tools;
using Microsoft.VisualBasic.CompilerServices;

namespace IsuExtra.Entities
{
    public class Time
    {
        private static int lengthOfString = 5;
        public Time(string timeString)
        {
            if (timeString is null)
                throw new IsuExtraException("Incorrect time");
            ParseTimeString(timeString);
            TimeString = timeString;
        }

        public Time(int hours, int minutes)
        {
            if (hours < 0 || hours > 23)
                throw new IsuExtraException("Incorrect hours");
            if (minutes < 0 || minutes > 59)
                throw new IsuExtraException("Incorrect minutes");

            Hours = hours;
            Minutes = minutes;
            TimeString = Hours.ToString("D2") + ':' + Minutes.ToString("D2");
        }

        public string TimeString { get; }
        public int Seconds { get; }
        public int Minutes { get; private set; }
        public int Hours { get; private set; }

        public static Time operator -(Time firstTime, Time secondTime)
        {
            if (firstTime < secondTime)
                return secondTime - firstTime;
            int resultHours = firstTime.Hours - secondTime.Hours;
            int resultMinutes;
            if (firstTime.Minutes < secondTime.Minutes)
            {
                resultHours -= 1;
                resultMinutes = 60 - (secondTime.Minutes - firstTime.Minutes);
            }
            else
            {
                resultMinutes = firstTime.Minutes - secondTime.Minutes;
            }

            return new Time(resultHours, resultMinutes);
        }

        public static bool operator <(Time firstTime, Time secondTime)
        {
            if (firstTime.Hours < secondTime.Hours)
            {
                return true;
            }

            if (firstTime.Hours == secondTime.Hours)
            {
                if (firstTime.Minutes < secondTime.Minutes)
                    return true;
            }

            return false;
        }

        public static bool operator >(Time firstTime, Time secondTime)
        {
            if (firstTime.Hours > secondTime.Hours)
            {
                return true;
            }

            if (firstTime.Hours == secondTime.Hours)
            {
                if (firstTime.Minutes > secondTime.Minutes)
                    return true;
            }

            return false;
        }

        public static bool operator ==(Time firstTime, Time secondTime)
        {
            return firstTime is not null && firstTime.Equals(secondTime);
        }

        public static bool operator !=(Time firstTime, Time secondTime)
        {
            return !(firstTime == secondTime);
        }

        public static bool operator <=(Time firstTime, Time secondTime)
        {
            return !(firstTime > secondTime);
        }

        public static bool operator >=(Time firstTime, Time secondTime)
        {
            return !(firstTime < secondTime);
        }

        public void ParseTimeString(string timeString)
        {
            if (timeString.Length != lengthOfString)
                throw new IsuExtraException("Incorrect format of time");
            if (timeString[2] != ':')
                throw new IsuExtraException("Incorrect format of time");

            string hoursString = timeString[0].ToString() + timeString[1].ToString();
            if (!int.TryParse(hoursString, out int hours))
                throw new IsuExtraException("Incorrect format of time");

            string minutesString = timeString[3].ToString() + timeString[4].ToString();
            if (!int.TryParse(minutesString, out int minutes))
                throw new IsuExtraException("Incorrect format of time");

            if (hours < 0 || hours > 23)
                throw new IsuExtraException("Incorrect hours");
            if (minutes < 0 || minutes > 59)
                throw new IsuExtraException("Incorrect minutes");

            Hours = hours;
            Minutes = minutes;
        }

        public override bool Equals(object obj)
        {
            return obj is Time time && (this.Hours == time.Hours && this.Minutes == time.Minutes);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Hours, Minutes);
        }
    }
}