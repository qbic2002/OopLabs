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
            Path = System.IO.Path.Combine(pathToJobs, nameOfJob);
            CreateRepository();
        }

        public string Path { get; }

        public void CreateRepository()
        {
            if (Path is null)
                throw new BackupException("Incorrect path");
            /*if (Directory.Exists(Path))
                throw new BackupException("Repository already exists");*/
            Directory.CreateDirectory(Path);
        }

        public void AddRestorePoint(RestorePoint restorePoint)
        {
            if (restorePoint is null)
                throw new BackupException("Incorrect restore Point");

            string restorePointName = string.Concat("RP", restorePoint.Number);
            string restorePointPath = System.IO.Path.Combine(Path, restorePointName);
            if (!Directory.Exists(restorePointPath))
                Directory.CreateDirectory(restorePointPath);
        }

        public void AddStorages(RestorePoint restorePoint, params Storage[] storages)
        {
            if (restorePoint is null)
                throw new BackupException("Incorrect restore Point");
            string restorePointName = string.Concat("RP", restorePoint.Number);
            string restorePointPath = System.IO.Path.Combine(Path, restorePointName);
            storages.ToList().ForEach(storage =>
            {
                string storageName = System.IO.Path.ChangeExtension(storage.Name, ".zip");
                string storagePath = System.IO.Path.Combine(restorePointPath, storageName);
                if (!File.Exists(storagePath))
                {
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
                }
            });
        }
    }
}