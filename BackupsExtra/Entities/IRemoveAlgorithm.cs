using System.Collections.Generic;
using Backups.Entities;

namespace BackupsExtra.Entities
{
    public interface IRemoveAlgorithm
    {
        void RemoveRestorePoints(BackupJob backupJob);
    }
}