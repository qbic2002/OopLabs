using System.Threading;
using Backups.Entities;
using Backups.Services;
using BackupsExtra.Entities;
using BackupsExtra.Services;

namespace BackupsExtra
{
    internal class Program
    {
        private static BackupManager _backupManager;
        private static BackupExtraManager _backupExtraManager;
        private static IRepository _localRepository;
        private static void Main()
        {
            _backupManager = new BackupManager(@"C:\Users\golov\GitLabs\tst");
            _backupExtraManager = new BackupExtraManager(_backupManager);
            _localRepository = _backupManager.AddLocalRepository("testJob");

            JobObject jobObject1 = _backupExtraManager.AddJobObject(@"C:\Users\golov\GitLabs\tst\File1.txt");
            JobObject jobObject2 = _backupExtraManager.AddJobObject(@"C:\Users\golov\GitLabs\tst\File2.txt");
            BackupJob job = _backupExtraManager.AddBackupJob("testJob", _localRepository, new SingleStorage(), new CountDelete(5), jobObject1, jobObject2);
            Thread.Sleep(5000);

            ExtraRepositoryManager.AddExtraRepository(_localRepository).RestoreRestorePoint(job.Backup.RestorePoints[0]);
        }
    }
}
