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
            MaxRestorePoints = maxRestorePoints;
        }

        public int MaxRestorePoints { get; }
        public void RemoveRestorePoints(BackupJob backupJob)
        {
            IExtraRepository extraRepository = ExtraRepositoryManager.AddExtraRepository(backupJob.Repository);
            List<RestorePoint> restorePoints = backupJob.RemoveRestorePointRange(0, backupJob.Backup.RestorePoints.Count - MaxRestorePoints);

            extraRepository.DeleteRestorePoints(restorePoints.ToArray());
        }
    }
}