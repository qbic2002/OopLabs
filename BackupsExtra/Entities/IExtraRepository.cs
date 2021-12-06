using System;
using Backups.Entities;

namespace BackupsExtra.Entities
{
    public interface IExtraRepository
    {
        string RepositoryPath { get; }
        void DeleteRestorePoints(params RestorePoint[] restorePoints);
        void RestoreRestorePoint(RestorePoint restorePoint);
        void RestoreRestorePoint(RestorePoint restorePoint, string destination);
        void UpdateRestorePoint(RestorePoint oldRestorePoint, RestorePoint newRestorePoint);
        string Type();
    }
}