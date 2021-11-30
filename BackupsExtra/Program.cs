using System;
using System.Dynamic;
using System.IO;
using System.Linq;
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
            _backupExtraManager = new BackupExtraManager(_backupManager, new ConsoleLogger());
            _localRepository = _backupManager.AddLocalRepository("testJob");

            Create();
        }

        private static void Create()
        {
            JobObject jobObject1 = _backupExtraManager.AddJobObject(@"C:\Users\golov\GitLabs\tst\File1.txt");
            JobObject jobObject2 = _backupExtraManager.AddJobObject(@"C:\Users\golov\GitLabs\tst\File2.txt");
            ExtraBackupJob job = _backupExtraManager.AddBackupJob("testJob", _localRepository, new SplitStorage(), new CountDelete(3), jobObject1, jobObject2);

            _backupExtraManager.RemoveJobObjectFromBackupJob(jobObject1, job);
            _backupExtraManager.CreateRestorePoint(job);
            _backupExtraManager.CreateRestorePoint(job);
            _backupExtraManager.CreateRestorePoint(job);
            _backupExtraManager.CreateRestorePoint(job);
        }

        private static void Back()
        {
            ExtraBackupJob resJob = _backupExtraManager.AddBackupJob("testJob");
            _backupExtraManager.Restore(resJob.Backup.RestorePoints.FirstOrDefault());
        }
    }
}