﻿using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Backups.Entities;
using Backups.Tools;

namespace Backups.Services
{
    public class BackupManager
    {
        private List<BackupJob> _backupJobs = new ();
        private string _rootPath = @"C:\Users\golov\Backups";
        public BackupManager()
        {
        }

        public BackupJob AddBackupJob(string name, IRepository repository, Algorithms.StorageAlgorithm storageAlgorithm, params JobObject[] jobObjects)
        {
            if (name is null)
                throw new BackupException("Incorrect name of Job");
            if (storageAlgorithm is null)
                throw new BackupException("Incorrect algorithm");
            return new BackupJob(repository, name, storageAlgorithm, jobObjects);
        }

        public JobObject AddJobObject(string filename)
        {
            if (filename is null)
                throw new BackupException("empty filename");
            var jobObject = new JobObject(filename);
            return jobObject;
        }

        public IRepository AddLocalRepository(string nameOfJob)
        {
            if (nameOfJob is null)
                throw new BackupException("Incorrect name of job");
            return new LocalRepository(_rootPath, nameOfJob);
        }
    }
}