using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using Backups.Entities;
using BackupsExtra.Services;
using BackupsExtra.Tools;
using Newtonsoft.Json;

namespace BackupsExtra.Entities
{
    public class ExtraBackupJob
    {
        private string _rootPath;

        public ExtraBackupJob(BackupJob backupJob, string rootPath)
        {
            BackupJob = backupJob ?? throw new BackupsExtraException("Incorrect backup job");
            _rootPath = rootPath ?? throw new BackupsExtraException("Incorrect root path");
        }

        public ExtraBackupJob(string rootPath, BackupJob backupJob, RemoveAlgorithm removeAlgorithm)
        {
            _rootPath = rootPath ?? throw new BackupsExtraException("Incorrect root path");
            BackupJob = backupJob ?? throw new BackupsExtraException("Incorrect backup job");
            RemoveAlgorithm = removeAlgorithm ?? throw new BackupsExtraException("Incorrect remove algorithm");
        }

        public BackupJob BackupJob { get; }
        public RemoveAlgorithm RemoveAlgorithm { get; set; }
        public string Name => BackupJob.Name;
        public IRepository Repository => BackupJob.Repository;
        public ReadOnlyCollection<JobObject> JobObjects => BackupJob.JobObjects;
        public Backup Backup => BackupJob.Backup;
        public IAlgorithm StorageAlgorithm => BackupJob.StorageAlgorithm;
        public IExtraRepository ExtraRepository => ExtraRepositoryManager.AddExtraRepository(Repository);

        public RestorePoint CreateRestorePoint(DateTime dateTime)
        {
            RestorePoint restorePoint = BackupJob.CreateRestorePoint(dateTime);
            RemoveAlgorithm.RemoveRestorePoints(this);
            ToJson();
            return restorePoint;
        }

        public void AddJobObject(JobObject jobObject, DateTime dateTime)
        {
            BackupJob.AddJobObject(jobObject, dateTime);
            RemoveAlgorithm.RemoveRestorePoints(this);
            ToJson();
        }

        public JobObject RemoveJobObject(JobObject jobObject, DateTime dateTime)
        {
            if (jobObject is null)
                throw new BackupsExtraException("Incorrect job object");
            BackupJob.RemoveJobObject(jobObject, dateTime);
            RemoveAlgorithm.RemoveRestorePoints(this);
            ToJson();
            return jobObject;
        }

        public List<RestorePoint> RemoveRestorePointRange(int index, int range) =>
            BackupJob.RemoveRestorePointRange(index, range);

        public RestorePoint RemoveRestorePoint(int index) => BackupJob.RemoveRestorePoint(index);
        public List<RestorePoint> RemoveRestorePointRangeWithMerge(int index, int range)
        {
            RestorePoint restorePoint = RestorePointManager.Merge(Backup.RestorePoints.ToList().GetRange(0, range + 1).ToArray());
            restorePoint.CreateStorage();
            RestorePoint oldRestorePoint = Backup.RestorePoints[range];
            List<RestorePoint> restorePoints = BackupJob.RemoveRestorePointRange(index, range + 1);
            Backup.AddRestorePoint(restorePoint);
            ExtraRepository.DeleteRestorePoints(restorePoints.GetRange(0, range).ToArray());
            ExtraRepository.UpdateRestorePoint(oldRestorePoint, restorePoint);
            return restorePoints.GetRange(0, range);
        }

        public void RemoveRestorePointWithMerge(int index) => RemoveRestorePointRange(index, 1);

        public void ToJson()
        {
            string jsonPath = Path.Combine(_rootPath, BackupJob.Name + ".cfg");
            File.WriteAllText(jsonPath, JsonConvert.SerializeObject(new ExtraBackupJobSerializer(this), Formatting.Indented));
        }

        public override string ToString()
        {
            string log = $"Name: {Name};";
            return log;
        }
    }
}