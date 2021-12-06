using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using Backups.Entities;
using Backups.Services;
using BackupsExtra.Entities;
using BackupsExtra.Tools;
using Newtonsoft.Json;

namespace BackupsExtra.Services
{
    public class BackupExtraManager
    {
        private BackupManager _backupManager;
        private ILogger _logger;

        public BackupExtraManager(BackupManager backupManager, ILogger logger)
        {
            _backupManager = backupManager ?? throw new BackupsExtraException("Incorrect backupManager");
            _logger = logger ?? throw new BackupsExtraException("Incorrect logger");
        }

        public ExtraBackupJob AddBackupJob(string name, IRepository repository, IAlgorithm storageAlgorithm, RemoveAlgorithm removeAlgorithm, params JobObject[] jobObjects)
        {
            if (name is null)
                throw new BackupsExtraException("Incorrect name of Job");
            if (storageAlgorithm is null)
                throw new BackupsExtraException("Incorrect algorithm");

            var backupJob = new BackupJob(new Backup(), repository, name, storageAlgorithm, DateTime.Now, 0, jobObjects);
            var extraBackupJob = new ExtraBackupJob(backupJob, _backupManager.RootPath);
            _logger.PrintLog("Create backup job " + extraBackupJob.ToString(), false);
            AddRemoveAlgorithmToJob(extraBackupJob, removeAlgorithm);
            return extraBackupJob;
        }

        public ExtraBackupJob AddBackupJob(string name)
        {
            if (name is null)
                throw new BackupsExtraException("Incorrect name of Job");
            string jsonPath = Path.Combine(_backupManager.RootPath, name + ".cfg");
            if (!File.Exists(jsonPath))
                throw new BackupsExtraException("Incorrect name");
            ExtraBackupJob extraBackupJob = JsonConvert.DeserializeObject<ExtraBackupJobSerializer>(File.ReadAllText(jsonPath)).ToExtraBackupJob(_backupManager.RootPath);
            _logger.PrintLog("Open from config: " + extraBackupJob.ToString(), false);
            return extraBackupJob;
        }

        public void AddRemoveAlgorithmToJob(ExtraBackupJob backupJob, RemoveAlgorithm removeAlgorithm)
        {
            if (backupJob is null)
                throw new BackupsExtraException("Incorrect backup job");
            if (removeAlgorithm is null)
                throw new BackupsExtraException("Incorrect remove algorithm");

            backupJob.RemoveAlgorithm = removeAlgorithm;
            removeAlgorithm.RemoveRestorePoints(backupJob);
            _logger.PrintLog("Add removal algorithm To " + backupJob.ToString(), false);
        }

        public JobObject AddJobObject(string filename) => _backupManager.AddJobObject(filename);
        public IRepository AddLocalRepository(string nameOfJob) => _backupManager.AddLocalRepository(nameOfJob);

        public void Restore(RestorePoint restorePoint)
        {
            if (restorePoint is null)
                throw new BackupsExtraException("incorrect restorePoint");

            ExtraRepositoryManager.AddExtraRepository(restorePoint.Repository).RestoreRestorePoint(restorePoint);
            _logger.PrintLog("Restored: " + restorePoint, false);
        }

        public void Restore(RestorePoint restorePoint, string destination)
        {
            if (restorePoint is null)
                throw new BackupsExtraException("incorrect restorePoint");
            if (destination is null)
                throw new BackupsExtraException("incorrect destination");

            ExtraRepositoryManager.AddExtraRepository(restorePoint.Repository).RestoreRestorePoint(restorePoint, destination);
            _logger.PrintLog("Restored: " + restorePoint + " To: " + destination, false);
        }

        public void AddJobObjectToBackupJob(JobObject jobObject, ExtraBackupJob extraBackupJob)
        {
            if (jobObject is null)
                throw new BackupsExtraException("Incorrect job object");
            if (extraBackupJob is null)
                throw new BackupsExtraException("Incorrect backup job");
            extraBackupJob.AddJobObject(jobObject, DateTime.Now);
            _logger.PrintLog("Add " + jobObject + " To " + extraBackupJob, false);
        }

        public void RemoveJobObjectFromBackupJob(JobObject jobObject, ExtraBackupJob extraBackupJob)
        {
            if (jobObject is null)
                throw new BackupsExtraException("Incorrect job object");
            if (extraBackupJob is null)
                throw new BackupsExtraException("Incorrect backup job");
            extraBackupJob.RemoveJobObject(jobObject, DateTime.Now);
            _logger.PrintLog("Remove " + jobObject + " From " + extraBackupJob, false);
        }

        public RestorePoint CreateRestorePoint(ExtraBackupJob extraBackupJob)
        {
            if (extraBackupJob is null)
                throw new BackupsExtraException("Incorrect backup job");
            RestorePoint restorePoint = extraBackupJob.CreateRestorePoint(DateTime.Now);
            _logger.PrintLog("Created restore point: " + restorePoint + " To " + extraBackupJob, false);
            return restorePoint;
        }
    }
}