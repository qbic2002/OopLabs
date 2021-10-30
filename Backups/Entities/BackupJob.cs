using System;
using System.Collections.Generic;
using System.Linq;
using Backups.Tools;

namespace Backups.Entities
{
    public class BackupJob
    {
        private List<JobObject> _jobObjects;
        private List<RestorePoint> _restorePoints = new ();
        private Algorithms.StorageAlgorithm _storageAlgorithm;
        public BackupJob(IRepository repository, string name, Algorithms.StorageAlgorithm storageAlgorithm, params JobObject[] jobObjects)
        {
            Repository = repository ?? throw new BackupException("Incorrect repository");
            Name = name ?? throw new BackupException("Incorrect name");
            if (jobObjects is null || jobObjects.Length == 0)
                throw new BackupException("Incorrect objects");
            _jobObjects = jobObjects.ToList();
            _storageAlgorithm = storageAlgorithm ?? throw new BackupException("Incorrect algorithm");

            CreateRestorePoint();
        }

        public string Name { get; }
        public IRepository Repository { get; }

        public RestorePoint CreateRestorePoint()
        {
            var restorePoint = new RestorePoint(Repository, _restorePoints.Count + 1, _storageAlgorithm, _jobObjects.ToArray());
            _restorePoints.Add(restorePoint);
            return restorePoint;
        }

        public void AddJobObject(JobObject jobObject)
        {
            if (jobObject is null || _jobObjects.Contains(jobObject))
                throw new BackupException("Incorrect job object");
            _jobObjects.Add(jobObject);
            CreateRestorePoint();
        }

        public JobObject RemoveJobObject(JobObject jobObject)
        {
            if (jobObject is null || !_jobObjects.Contains(jobObject))
                throw new BackupException("Incorrect job object");
            _jobObjects.Remove(jobObject);
            CreateRestorePoint();
            return jobObject;
        }
    }
}