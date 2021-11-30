using System;
using System.Collections.Generic;
using System.Linq;
using Backups.Entities;
using BackupsExtra.Services;
using BackupsExtra.Tools;

namespace BackupsExtra.Entities
{
    public class CountDelete : IRemoveAlgorithm
    {
        public CountDelete(int maxRestorePoints)
        {
            if (maxRestorePoints <= 0)
                throw new BackupsExtraException("Incorrect number of max restore points");
            Param = maxRestorePoints;
        }

        public object Param { get; }
        public void RemoveRestorePoints(ExtraBackupJob extraBackupJob)
        {
            int range = GetRange(extraBackupJob);
            if (range > 0 && range < extraBackupJob.Backup.RestorePoints.Count)
            {
                extraBackupJob.RemoveRestorePointRangeWithMerge(0, range);
            }
        }

        public int GetRange(ExtraBackupJob extraBackupJob)
        {
            return Math.Max(extraBackupJob.Backup.RestorePoints.Count - (Param is int ? (int)Param : 0), 0);
        }

        public override string ToString()
        {
            return "CountDelete";
        }
    }
}