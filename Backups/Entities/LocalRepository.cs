using System.IO;
using System.IO.Compression;
using System.Linq;
using Backups.Tools;

namespace Backups.Entities
{
    public class LocalRepository : IRepository
    {
        public LocalRepository(string pathToJobs, string nameOfJob)
        {
            if (pathToJobs is null)
                throw new BackupException("Incorrect path to Jobs");
            if (!Directory.Exists(pathToJobs))
                throw new BackupException("Incorrect path to Jobs");
            if (nameOfJob is null)
                throw new BackupException("Incorrect name of Job");

            Path = string.Concat(pathToJobs, @"\", nameOfJob);
            CreateRepository();
        }

        public string Path { get; }

        public void CreateRepository()
        {
            if (Path is null)
                throw new BackupException("Incorrect path");
            if (Directory.Exists(Path))
                throw new BackupException("Repository already exists");
            Directory.CreateDirectory(Path);
        }

        public void AddRestorePoint(RestorePoint restorePoint)
        {
            if (restorePoint is null)
                throw new BackupException("Incorrect restore Point");
            string restorePointPath = string.Concat(Path, @"\RP", restorePoint.Number);
            if (Directory.Exists(restorePointPath))
                throw new BackupException("Restore point already created");
            Directory.CreateDirectory(restorePointPath);
        }

        public void AddStorages(RestorePoint restorePoint, params Storage[] storages)
        {
            if (restorePoint is null)
                throw new BackupException("Incorrect restore Point");
            string restorePointPath = string.Concat(Path, @"\RP", restorePoint.Number);
            storages.ToList().ForEach(storage =>
            {
                string storagePath = string.Concat(restorePointPath, @"\", storage.Name, ".zip");
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
    }
}