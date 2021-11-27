using System;
using System.Collections.Generic;
using System.Linq;
using Backups.Entities;
using BackupsExtra.Tools;

namespace BackupsExtra.Entities
{
    public class DateDelete : IRemoveAlgorithm
    {
        public DateDelete(DateTime dateTime)
        {
            DateTime = dateTime;
        }

        public DateTime DateTime { get; }
        public void RemoveRestorePoints(BackupJob backupJob)
        {
            if (backupJob.Backup.RestorePoints.ToList().All(restorePoint => restorePoint.DateTime < DateTime))
                throw new BackupsExtraException("Cannot remove all restore points");
            var restorePoints = backupJob.Backup.RestorePoints.ToList();
            restorePoints.Sort();
            for (int i = 0; i < restorePoints.Count; i++)
            {
                if (restorePoints[i].DateTime < DateTime)
                    backupJob.RemoveRestorePoint(i);
            }
        }
    }
}