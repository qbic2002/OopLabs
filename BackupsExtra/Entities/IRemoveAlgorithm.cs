using System;
using System.Collections.Generic;
using Backups.Entities;

namespace BackupsExtra.Entities
{
    public interface IRemoveAlgorithm
    {
        object Param { get; }
        void RemoveRestorePoints(ExtraBackupJob extraBackupJob);
        int GetRange(ExtraBackupJob extraBackupJob);
    }
}