﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Backups.Tools;

namespace Backups.Entities
{
    public class BackupJob
    {
        private List<JobObject> _jobObjects;
        private IAlgorithm _storageAlgorithm;
        private int _restorePointCount = 0;
        public BackupJob(Backup backup, IRepository repository, string name, IAlgorithm storageAlgorithm, params JobObject[] jobObjects)
        {
            Backup = backup ?? throw new BackupException("Incorrect backup");
            Repository = repository ?? throw new BackupException("Incorrect repository");
            Name = name ?? throw new BackupException("Incorrect name");
            if (jobObjects is null || jobObjects.Length == 0)
                throw new BackupException("Incorrect objects");
            _jobObjects = jobObjects.ToList();
            _storageAlgorithm = storageAlgorithm ?? throw new BackupException("Incorrect algorithm");

            JobObjects = new ReadOnlyCollection<JobObject>(_jobObjects);
            CreateRestorePoint(DateTime.Now);
        }

        public string Name { get; }
        public IRepository Repository { get; }
        public ReadOnlyCollection<JobObject> JobObjects { get; }
        public Backup Backup { get; }

        public RestorePoint CreateRestorePoint(DateTime dateTime)
        {
            var restorePoint = new RestorePoint(Repository, ++_restorePointCount, _storageAlgorithm, dateTime, _jobObjects.ToArray());
            Backup.AddRestorePoint(restorePoint);
            restorePoint.AddRestorePointToRepository();
            restorePoint.CreateStorage();
            return restorePoint;
        }

        public void AddJobObject(JobObject jobObject)
        {
            if (jobObject is null || _jobObjects.Contains(jobObject))
                throw new BackupException("Incorrect job object");
            _jobObjects.Add(jobObject);
            CreateRestorePoint(DateTime.Now);
        }

        public JobObject RemoveJobObject(JobObject jobObject)
        {
            if (jobObject is null || !_jobObjects.Contains(jobObject))
                throw new BackupException("Incorrect job object");
            _jobObjects.Remove(jobObject);
            CreateRestorePoint(DateTime.Now);
            return jobObject;
        }

        public void RemoveRestorePointRange(int index, int range)
        {
            for (int i = 0; i < range; i++)
                RemoveRestorePoint(index);
        }

        public void RemoveRestorePoint(int index)
        {
            RestorePoint restorePoint = Backup.RemoveRestorePoint(index);
            Repository.DeleteRestorePoint(restorePoint);
        }
    }
}