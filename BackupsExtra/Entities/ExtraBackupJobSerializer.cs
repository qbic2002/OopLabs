using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using Backups.Entities;
using BackupsExtra.Tools;

namespace BackupsExtra.Entities
{
    public class ExtraBackupJobSerializer
    {
        public ExtraBackupJobSerializer(ExtraBackupJob extraBackupJob)
        {
            if (extraBackupJob is null)
                throw new BackupsExtraException("Incorrect backup job");
            Name = extraBackupJob.Name;
            RepositoryType = extraBackupJob.ExtraRepository.Type();
            RepositoryPath = extraBackupJob.ExtraRepository.RepositoryPath;
            StorageAlgorithm = extraBackupJob.StorageAlgorithm.ToString();
            RemoveAlgorithmSerializer = new RemoveAlgorithmSerializer(extraBackupJob.RemoveAlgorithm);
            JobObjectSerializes = new List<JobObjectSerialize>();
            extraBackupJob.JobObjects.ToList().ForEach(jobObject => JobObjectSerializes.Add(new JobObjectSerialize(jobObject)));
            RestorePointSerializes = new List<RestorePointSerialize>();
            extraBackupJob.Backup.RestorePoints.ToList().ForEach(restorePoint => RestorePointSerializes.Add(new RestorePointSerialize(restorePoint)));
        }

        public ExtraBackupJobSerializer()
        {
        }

        public string Name { get; set; }
        public string RepositoryType { get; set; }
        public string RepositoryPath { get; set; }
        public string StorageAlgorithm { get; set; }
        public RemoveAlgorithmSerializer RemoveAlgorithmSerializer { get; set; }
        public List<JobObjectSerialize> JobObjectSerializes { get; set; }
        public List<RestorePointSerialize> RestorePointSerializes { get; set; }

        public ExtraBackupJob ToExtraBackupJob(string rootPath)
        {
            List<RestorePoint> restorePoints = GetRestorePoints();
            restorePoints.Sort();
            var backupJob = new BackupJob(new Backup(), GetRepository(), Name, GetStorageAlgorithm(), restorePoints.FirstOrDefault().DateTime, restorePoints.FirstOrDefault().Number - 1, restorePoints.FirstOrDefault().JobObjects.ToArray());
            restorePoints.RemoveAt(0);
            restorePoints.ForEach(restorePoint =>
            {
                if (restorePoint.JobObjects.Count == backupJob.JobObjects.Count)
                    backupJob.CreateRestorePoint(restorePoint.DateTime);
                if (restorePoint.JobObjects.Count < backupJob.JobObjects.Count)
                {
                    JobObject jobObjectToRemove = backupJob.JobObjects.ToList().Find(jobObject => !restorePoint.JobObjects.Contains(jobObject));
                    backupJob.RemoveJobObject(new JobObject(jobObjectToRemove.Fullname), restorePoint.DateTime);
                }

                if (restorePoint.JobObjects.Count > backupJob.JobObjects.Count)
                {
                    JobObject jobObjectToAdd = restorePoint.JobObjects.ToList().Find(jobObject => !backupJob.JobObjects.Contains(jobObject));
                    backupJob.AddJobObject(new JobObject(jobObjectToAdd.Fullname), restorePoint.DateTime);
                }
            });
            return new ExtraBackupJob(rootPath, backupJob, GetRemoveAlgorithm());
        }

        private IRepository GetRepository()
        {
            IRepository repository = null;
            switch (RepositoryType)
            {
                case "local":
                    repository = new LocalRepository(Directory.GetParent(RepositoryPath).FullName, Name);
                    break;
            }

            return repository;
        }

        private IAlgorithm GetStorageAlgorithm()
        {
            IAlgorithm storageAlgorithm = null;
            switch (StorageAlgorithm)
            {
                case "SingleStorage":
                    storageAlgorithm = new SingleStorage();
                    break;
                case "SplitStorage":
                    storageAlgorithm = new SplitStorage();
                    break;
            }

            return storageAlgorithm;
        }

        private RemoveAlgorithm GetRemoveAlgorithm()
        {
            return RemoveAlgorithmSerializer.ToRemoveAlgorithm();
        }

        private List<RestorePoint> GetRestorePoints()
        {
            var restorePoints = new List<RestorePoint>();
            RestorePointSerializes.ForEach(serialize => restorePoints.Add(serialize.ToRestorePoint(GetRepository(), GetStorageAlgorithm())));
            return restorePoints;
        }
    }
}