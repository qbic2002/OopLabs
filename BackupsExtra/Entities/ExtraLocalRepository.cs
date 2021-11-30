using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using Backups.Entities;
using Backups.Tools;
using BackupsExtra.Tools;

namespace BackupsExtra.Entities
{
    public class ExtraLocalRepository : IExtraRepository
    {
        private LocalRepository _localRepository;
        public ExtraLocalRepository(LocalRepository localRepository)
        {
            _localRepository = localRepository ?? throw new BackupsExtraException("Incorrect local repository");
        }

        public string RepositoryPath => _localRepository.Path;
        public void DeleteRestorePoints(params RestorePoint[] restorePoints)
        {
            if (restorePoints is null)
                throw new BackupsExtraException("Incorrect restore point");
            restorePoints.ToList().ForEach(restorePoint =>
            {
                string restorePointName = string.Concat("RP", restorePoint.Number);
                string restorePointPath = Path.Combine(RepositoryPath, restorePointName);
                if (!Directory.Exists(restorePointPath))
                    throw new BackupException("Restore point already deleted");
                Directory.Delete(restorePointPath, true);
            });
        }

        public void RestoreRestorePoint(RestorePoint restorePoint)
        {
            if (restorePoint is null)
                throw new BackupsExtraException("Incorrect restore point");

            string restorePointName = string.Concat("RP", restorePoint.Number);
            string restorePointPath = System.IO.Path.Combine(RepositoryPath, restorePointName);
            if (!Directory.Exists(restorePointPath))
                throw new BackupException("Restore point does not exist");
            Directory.GetFiles(restorePointPath).ToList().ForEach(storagePath =>
            {
                using (var zip = new ZipArchive(File.Open(storagePath, FileMode.Open), ZipArchiveMode.Read))
                {
                    zip.Entries.ToList().ForEach(entry =>
                    {
                        string restorePath = restorePoint.JobObjects.ToList().Find(jobObject => entry.Name == jobObject.Name)?.Fullname;
                        entry.ExtractToFile(restorePath);
                    });
                }
            });
        }

        public void RestoreRestorePoint(RestorePoint restorePoint, string destination)
        {
            if (restorePoint is null)
                throw new BackupsExtraException("Incorrect restore point");
            if (destination is null)
                throw new BackupsExtraException("Incorrect destination");

            string restorePointName = string.Concat("RP", restorePoint.Number);
            string restorePointPath = System.IO.Path.Combine(RepositoryPath, restorePointName);
            if (!Directory.Exists(restorePointPath))
                throw new BackupException("Restore point does not exist");
            Directory.GetFiles(restorePointPath).ToList().ForEach(storagePath =>
            {
                using (var zip = new ZipArchive(File.Open(storagePath, FileMode.Open), ZipArchiveMode.Read))
                {
                    if (!Directory.Exists(destination))
                        throw new BackupsExtraException("Incorrect destination");
                    zip.ExtractToDirectory(Path.Combine(_localRepository.Path, destination), true);
                }
            });
        }

        public void UpdateRestorePoint(RestorePoint oldRestorePoint, RestorePoint newRestorePoint)
        {
            if (oldRestorePoint is null || newRestorePoint is null)
                throw new BackupException("Incorrect restore Point");
            string restorePointName = string.Concat("RP", oldRestorePoint.Number);
            string restorePointPath = System.IO.Path.Combine(RepositoryPath, restorePointName);
            newRestorePoint.Storages.ToList().ForEach(storage =>
            {
                string storageName = System.IO.Path.ChangeExtension(storage.Name, ".zip");
                string storagePath = System.IO.Path.Combine(restorePointPath, storageName);
                if (File.Exists(storagePath))
                    File.Delete(storagePath);
                var zipArc = new ZipArchive(File.Open(storagePath, FileMode.Create), ZipArchiveMode.Create);
                zipArc.Dispose();
                storage.JobObjects.ToList().ForEach(jobObject =>
                {
                    using (var zip = new ZipArchive(File.Open(storagePath, FileMode.Open), ZipArchiveMode.Update))
                    {
                        zip.CreateEntryFromFile(jobObject.Fullname, jobObject.Name);
                        zip.Dispose();
                    }
                });
            });
        }

        public string Type()
        {
            return "local";
        }
    }
}