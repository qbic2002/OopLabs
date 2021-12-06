using System;
using BackupsExtra.Tools;

namespace BackupsExtra.Entities
{
    public class CombineParams
    {
        public CombineParams(int maxRestorePoints, DateTime dateTime, bool allAlgorithms)
        {
            if (maxRestorePoints <= 0)
                throw new BackupsExtraException("Incorrect number of restore points");
            MaxRestorePoints = maxRestorePoints;
            DateTime = dateTime;
            AllAlgorithms = allAlgorithms;
        }

        public CombineParams(string stringParams)
            : this(GetMaxNumber(stringParams), GetDateTime(stringParams), GetBool(stringParams))
        {
        }

        public int MaxRestorePoints { get; }
        public DateTime DateTime { get; }
        public bool AllAlgorithms { get; }

        public static int GetMaxNumber(string stringParams)
        {
            return int.Parse(stringParams.Substring(0, stringParams.IndexOf(',')));
        }

        public static DateTime GetDateTime(string stringParams)
        {
            string stringDate = stringParams.Substring(stringParams.IndexOf(',') + 1);
            return System.DateTime.Parse(stringDate.Substring(0, stringDate.IndexOf(',')));
        }

        public static bool GetBool(string stringParams)
        {
            string stringBool = stringParams.Substring(stringParams.IndexOf(',') + 1);
            return bool.Parse(stringBool.Substring(stringBool.IndexOf(',') + 1));
        }

        public override string ToString()
        {
            return $"{MaxRestorePoints},{DateTime.ToString()},{AllAlgorithms.ToString()}";
        }
    }
}