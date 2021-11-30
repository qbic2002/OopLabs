using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using Backups.Entities;
using Backups.Services;
using BackupsExtra.Entities;
using BackupsExtra.Tools;

namespace BackupsExtra.Services
{
    public class BackupExtraManager
    {
        private BackupManager _backupManager;

        public BackupExtraManager(BackupManager backupManager)
        {
            _backupManager = backupManager ?? throw new BackupsExtraException("Incorrect backupManager");
        }

        public ExtraBackupJob AddBackupJob(string name, IRepository repository, IAlgorithm storageAlgorithm, IRemoveAlgorithm removeAlgorithm, params JobObject[] jobObjects)
        {
            if (name is null)
                throw new BackupsExtraException("Incorrect name of Job");
            if (storageAlgorithm is null)
                throw new BackupsExtraException("Incorrect algorithm");

            var backupJob = new BackupJob(new Backup(), repository, name, storageAlgorithm, DateTime.Now, 0, jobObjects);
            var extraBackupJob = new ExtraBackupJob(backupJob, _backupManager.RootPath);
            AddRemoveAlgorithmToJob(extraBackupJob, removeAlgorithm);
            return extraBackupJob;
        }

        public ExtraBackupJob AddBackupJob(string name)
        {
            if (name is null)
                throw new BackupsExtraException("Incorrect name of Job");

            var extraBackupJob = new ExtraBackupJob(_backupManager.RootPath, name);
            return extraBackupJob;
        }

        public void AddRemoveAlgorithmToJob(ExtraBackupJob backupJob, IRemoveAlgorithm removeAlgorithm)
        {
            if (backupJob is null)
                throw new BackupsExtraException("Incorrect backup job");
            if (removeAlgorithm is null)
                throw new BackupsExtraException("Incorrect remove algorithm");

            backupJob.RemoveAlgorithm = removeAlgorithm;
            removeAlgorithm.RemoveRestorePoints(backupJob);
        }

        public JobObject AddJobObject(string filename) => _backupManager.AddJobObject(filename);
        public IRepository AddLocalRepository(string nameOfJob) => _backupManager.AddLocalRepository(nameOfJob);

        public void Restore(RestorePoint restorePoint)
        {
            if (restorePoint is null)
                throw new BackupsExtraException("incorrect restorePoint");

            ExtraRepositoryManager.AddExtraRepository(restorePoint.Repository).RestoreRestorePoint(restorePoint);
        }

        public void Restore(RestorePoint restorePoint, string destination)
        {
            if (restorePoint is null)
                throw new BackupsExtraException("incorrect restorePoint");
            if (destination is null)
                throw new BackupsExtraException("incorrect destination");

            ExtraRepositoryManager.AddExtraRepository(restorePoint.Repository).RestoreRestorePoint(restorePoint, destination);
        }
    }
}