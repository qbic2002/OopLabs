using System;
using BackupsExtra.Tools;

namespace BackupsExtra.Entities
{
    public class CountPredicate : IRemovePredicate
    {
        public CountPredicate(int param)
        {
            if (param <= 0)
                throw new BackupsExtraException("Incorrect max number of restore points");
            Param = param;
        }

        public object Param { get; }

        public int GetRange(ExtraBackupJob extraBackupJob)
        {
            int maxRestorePoints = Param is int i ? i : 0;
            return Math.Max(extraBackupJob.Backup.RestorePoints.Count - maxRestorePoints, 0);
        }

        public string Type()
        {
            return "Count";
        }
    }
}