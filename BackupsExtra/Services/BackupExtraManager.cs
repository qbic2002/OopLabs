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
        private Dictionary<BackupJob, IRemoveAlgorithm> _backupJobsAndRemoveAlgorithms = new ();
        private BackupManager _backupManager;

        public BackupExtraManager(BackupManager backupManager)
        {
            _backupManager = backupManager ?? throw new BackupsExtraException("Incorrect backupManager");
        }

        public BackupJob AddBackupJob(string name, IRepository repository, IAlgorithm storageAlgorithm, IRemoveAlgorithm removeAlgorithm, params JobObject[] jobObjects)
        {
            if (name is null)
                throw new BackupsExtraException("Incorrect name of Job");
            if (storageAlgorithm is null)
                throw new BackupsExtraException("Incorrect algorithm");

            var backupJob = new BackupJob(new Backup(), repository, name, storageAlgorithm, jobObjects);
            AddRemoveAlgorithmToJob(backupJob, removeAlgorithm);
            return backupJob;
        }

        public void AddRemoveAlgorithmToJob(BackupJob backupJob, IRemoveAlgorithm removeAlgorithm)
        {
            if (backupJob is null)
                throw new BackupsExtraException("Incorrect backup job");
            if (removeAlgorithm is null)
                throw new BackupsExtraException("Incorrect remove algorithm");

            if (!_backupJobsAndRemoveAlgorithms.ContainsKey(backupJob))
            {
                _backupJobsAndRemoveAlgorithms.Add(backupJob, removeAlgorithm);
            }
            else
            {
                _backupJobsAndRemoveAlgorithms[backupJob] = removeAlgorithm;
            }

            removeAlgorithm.RemoveRestorePoints(backupJob);
        }

        public JobObject AddJobObject(string filename) => _backupManager.AddJobObject(filename);
        public IRepository AddLocalRepository(string nameOfJob) => _backupManager.AddLocalRepository(nameOfJob);

        public void CreateRestorePoint(BackupJob backupJob, DateTime dateTime)
        {
            if (backupJob is null)
                throw new BackupsExtraException("Incorrect backup job");
            backupJob.CreateRestorePoint(dateTime);

            DoRemoveAlgorithm(backupJob);
        }

        public void DoRemoveAlgorithm(BackupJob backupJob)
        {
            if (!_backupJobsAndRemoveAlgorithms.ContainsKey(backupJob))
                throw new BackupsExtraException("Cannot find info about remove algorithm");
            _backupJobsAndRemoveAlgorithms[backupJob].RemoveRestorePoints(backupJob);
        }

        public void Restore(RestorePoint restorePoint)
        {
            if (restorePoint is null)
                throw new BackupsExtraException("incorrect restorePoint");

            ExtraRepositoryManager.AddExtraRepository(restorePoint.Repository).RestoreRestorePoint(restorePoint);
        }

        public void Restore(RestorePoint restorePoint, string restorePath)
        {
            ExtraRepositoryManager.AddExtraRepository(restorePoint.Repository).RestoreRestorePoint(restorePoint);
        }
    }
}