using System;
using Backups.Entities;

namespace BackupsExtra.Entities
{
    public interface IExtraRepository
    {
        void DeleteRestorePoints(params RestorePoint[] restorePoints);
        void RestoreRestorePoint(RestorePoint restorePoint);
    }
}