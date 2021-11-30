using System;
using System.Collections.Generic;
using System.Linq;
using Backups.Entities;
using BackupsExtra.Services;
using BackupsExtra.Tools;

namespace BackupsExtra.Entities
{
    public class DateDelete : IRemoveAlgorithm
    {
        public DateDelete(DateTime dateTime)
        {
            Param = dateTime;
        }

        public object Param { get; }
        public void RemoveRestorePoints(ExtraBackupJob extraBackupJob)
        {
            if (extraBackupJob.Backup.RestorePoints.ToList().All(restorePoint => restorePoint.DateTime < (Param is DateTime ? (DateTime)Param : default)))
                throw new BackupsExtraException("Cannot remove all restore points");

            int count = GetRange(extraBackupJob);

            if (count > 0)
            {
                extraBackupJob.RemoveRestorePointRangeWithMerge(0, count);
            }
        }

        public int GetRange(ExtraBackupJob extraBackupJob)
        {
            int count = 0;
            extraBackupJob.Backup.RestorePoints.ToList().ForEach(restorePoint =>
            {
                if (restorePoint.DateTime < (Param is DateTime ? (DateTime)Param : default))
                    ++count;
            });
            return count;
        }

        public override string ToString()
        {
            return "DateDelete";
        }
    }
}