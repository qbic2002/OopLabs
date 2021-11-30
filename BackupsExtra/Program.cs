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
            _backupExtraManager = new BackupExtraManager(_backupManager);
            _localRepository = _backupManager.AddLocalRepository("testJob");

            Create();
        }

        private static void Create()
        {
            JobObject jobObject1 = _backupExtraManager.AddJobObject(@"C:\Users\golov\GitLabs\tst\File1.txt");
            JobObject jobObject2 = _backupExtraManager.AddJobObject(@"C:\Users\golov\GitLabs\tst\File2.txt");
            ExtraBackupJob job = _backupExtraManager.AddBackupJob("testJob", _localRepository, new SplitStorage(), new CombineDelete(3, DateTime.Now, false), jobObject1, jobObject2);

            job.RemoveJobObject(jobObject1, DateTime.Now);
            job.CreateRestorePoint(DateTime.Now);
            job.CreateRestorePoint(DateTime.Now);
            job.CreateRestorePoint(DateTime.Now);
            ExtraBackupJob resJob = _backupExtraManager.AddBackupJob("testJob");
            resJob.CreateRestorePoint(DateTime.Now);
            resJob.CreateRestorePoint(DateTime.Now);
            resJob.CreateRestorePoint(DateTime.Now);
            resJob.CreateRestorePoint(DateTime.Now);
            job.RemoveAlgorithm.RemoveRestorePoints(job);
        }

        private static void Back()
        {
            ExtraBackupJob resJob = _backupExtraManager.AddBackupJob("testJob");
            ExtraRepositoryManager.AddExtraRepository(_localRepository).RestoreRestorePoint(resJob.Backup.RestorePoints.FirstOrDefault());
            resJob.AddJobObject(new JobObject(@"C:\Users\golov\GitLabs\tst\File1.txt"), DateTime.Now);
        }
    }
}