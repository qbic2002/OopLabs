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

        public void DeleteRestorePoints(params RestorePoint[] restorePoints)
        {
            if (restorePoints is null)
                throw new BackupsExtraException("Incorrect restore point");
            restorePoints.ToList().ForEach(restorePoint =>
            {
                string restorePointName = string.Concat("RP", restorePoint.Number);
                string restorePointPath = System.IO.Path.Combine(_localRepository.Path, restorePointName);
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
            string restorePointPath = System.IO.Path.Combine(_localRepository.Path, restorePointName);
            if (!Directory.Exists(restorePointPath))
                throw new BackupException("Restore point does not exist");
            Directory.GetFiles(restorePointPath).ToList().ForEach(storagePath =>
            {
                using (var zip = new ZipArchive(File.Open(storagePath, FileMode.Open), ZipArchiveMode.Read))
                {
                    zip.ExtractToDirectory(Path.Combine(_localRepository.Path, "tmp"));
                    Directory.GetFiles(Path.Combine(_localRepository.Path, "tmp")).ToList().ForEach(fileToCopy =>
                    {
                        string restorePath = restorePoint.JobObjects.ToList().Find(jobObject => Path.GetFileName(fileToCopy) == jobObject.Name)?.Fullname;
                        File.Copy(fileToCopy, restorePath);
                    });
                    Directory.Delete(Path.Combine(_localRepository.Path, "tmp"), true);
                }
            });
        }
    }
}