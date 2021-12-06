using System;
using Backups.Entities;
using Backups.Tools;

namespace Backups.Services
{
    public class BackupManager
    {
        public BackupManager(string rootPath)
        {
            RootPath = rootPath ?? throw new BackupException("Incorrect root path");
        }

        public string RootPath { get; }

        public BackupJob AddBackupJob(string name, IRepository repository, IAlgorithm storageAlgorithm, params JobObject[] jobObjects)
        {
            if (name is null)
                throw new BackupException("Incorrect name of Job");
            if (storageAlgorithm is null)
                throw new BackupException("Incorrect algorithm");
            return new BackupJob(new Backup(), repository, name, storageAlgorithm, DateTime.Now, 0, jobObjects);
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
            return new LocalRepository(RootPath, nameOfJob);
        }
    }
}