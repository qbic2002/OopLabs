using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Backups.Tools;

namespace Backups.Entities
{
    public class BackupJob
    {
        private List<JobObject> _jobObjects;
        private int _restorePointCount = 0;
        public BackupJob(Backup backup, IRepository repository, string name, IAlgorithm storageAlgorithm, DateTime dateTime, int restorePointCount, params JobObject[] jobObjects)
        {
            Backup = backup ?? throw new BackupException("Incorrect backup");
            Repository = repository ?? throw new BackupException("Incorrect repository");
            Name = name ?? throw new BackupException("Incorrect name");
            if (jobObjects is null || jobObjects.Length == 0)
                throw new BackupException("Incorrect objects");
            _jobObjects = jobObjects.ToList();
            this.StorageAlgorithm = storageAlgorithm ?? throw new BackupException("Incorrect algorithm");
            if (restorePointCount < 0)
                throw new BackupException("Incorrect restore point count");
            _restorePointCount = restorePointCount;

            JobObjects = new ReadOnlyCollection<JobObject>(_jobObjects);
            CreateRestorePoint(dateTime);
        }

        public string Name { get; }
        public IRepository Repository { get; }
        public ReadOnlyCollection<JobObject> JobObjects { get; }
        public Backup Backup { get; }
        public IAlgorithm StorageAlgorithm { get; }

        public RestorePoint CreateRestorePoint(DateTime dateTime)
        {
            var restorePoint = new RestorePoint(Repository, ++_restorePointCount, StorageAlgorithm, dateTime, _jobObjects.ToArray());
            Backup.AddRestorePoint(restorePoint);
            restorePoint.AddRestorePointToRepository();
            restorePoint.CreateStorage();
            return restorePoint;
        }

        public void AddJobObject(JobObject jobObject, DateTime dateTime)
        {
            if (jobObject is null || _jobObjects.Contains(jobObject))
                throw new BackupException("Incorrect job object");
            _jobObjects.Add(jobObject);
            CreateRestorePoint(dateTime);
        }

        public JobObject RemoveJobObject(JobObject jobObject, DateTime dateTime)
        {
            if (jobObject is null || !_jobObjects.Contains(jobObject))
                throw new BackupException("Incorrect job object");
            _jobObjects.Remove(jobObject);
            CreateRestorePoint(dateTime);
            return jobObject;
        }

        public List<RestorePoint> RemoveRestorePointRange(int index, int range) =>
            Backup.RemoveRestorePointRange(index, range);

        public RestorePoint RemoveRestorePoint(int index) => Backup.RemoveRestorePoint(index);
    }
}