using System;
using System.Linq;
using BackupsExtra.Tools;

namespace BackupsExtra.Entities
{
    public class DatePredicate : IRemovePredicate
    {
        public DatePredicate(DateTime dateTime)
        {
            Param = dateTime;
        }

        public object Param { get; }

        public int GetRange(ExtraBackupJob extraBackupJob)
        {
            DateTime dateTime = Param is DateTime time ? time : default;
            int count = 0;
            extraBackupJob.Backup.RestorePoints.ToList().ForEach(restorePoint =>
            {
                if (restorePoint.DateTime < dateTime)
                    ++count;
            });
            if (count == extraBackupJob.Backup.RestorePoints.Count)
                throw new BackupsExtraException("Cannot remove all RestorePoints");
            return count;
        }

        public string Type()
        {
            return "Date";
        }
    }
}