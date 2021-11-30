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

        public ExtraBackupJob(string rootPath, string jobName)
        {
            _rootPath = rootPath ?? throw new BackupsExtraException("Incorrect root path");
            if (jobName is null)
                throw new BackupsExtraException("Incorrect job name");
            BackupJob = RestoreFromJson(Path.Combine(_rootPath, jobName + ".cfg"));
        }

        public BackupJob BackupJob { get; }
        public IRemoveAlgorithm RemoveAlgorithm { get; set; }
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

        public BackupJob RestoreFromJson(string jsonPath)
        {
            string jsonContent = File.ReadAllText(jsonPath);
            JsonTextReader reader = new JsonTextReader(new StringReader(jsonContent));

            string name = null;
            string repositoryType = null;
            IAlgorithm storageAlgorithm = null;
            IRemoveAlgorithm removeAlgorithm = null;
            string repositoryPath;
            IRepository repository = null;
            List<JobObject> jobObjects = new ();
            List<RestorePoint> restorePoints = new ();
            while (reader.Read())
            {
                if (reader.TokenType == JsonToken.PropertyName)
                {
                    if (reader.Value != null && reader.Value.Equals("Name"))
                    {
                        reader.Read();
                        name = reader.Value.ToString();
                    }

                    if (reader.Value.Equals("Repository type"))
                    {
                        reader.Read();
                        repositoryType = reader.Value.ToString();
                    }

                    if (reader.Value.Equals("Repository Path"))
                    {
                        reader.Read();
                        repositoryPath = reader.Value.ToString();
                        switch (repositoryType)
                        {
                            case "local":
                                repository = new LocalRepository(Directory.GetParent(repositoryPath).FullName, name);
                                break;
                        }
                    }

                    if (reader.Value.Equals("StorageAlgorithm"))
                    {
                        reader.Read();
                        string storageAlgorithmType = reader.Value.ToString();
                        switch (storageAlgorithmType)
                        {
                            case "SingleStorage":
                                storageAlgorithm = new SingleStorage();
                                break;
                            case "SplitStorage":
                                storageAlgorithm = new SplitStorage();
                                break;
                        }
                    }

                    if (reader.Value.Equals("removeAlgorithm"))
                    {
                        reader.Read();
                        string storageAlgorithmType = reader.Value.ToString();
                        reader.Read();
                        reader.Read();
                        string param = reader.Value.ToString();
                        switch (storageAlgorithmType)
                        {
                            case "CountDelete":
                                removeAlgorithm = new CountDelete(int.Parse(param));
                                break;
                            case "DateDelete":
                                removeAlgorithm = new DateDelete(DateTime.Parse(param));
                                break;
                            case "CombineDelete":
                                removeAlgorithm = new CombineDelete(new CombineParams(param));
                                break;
                        }
                    }

                    if (reader.Value.Equals("JobObjects"))
                    {
                        reader.Read();
                        reader.Read();
                        while (reader.TokenType != JsonToken.EndArray)
                        {
                            jobObjects.Add(new JobObject(reader.Value.ToString()));
                            reader.Read();
                        }
                    }

                    if (reader.Value != null && reader.Value.Equals("RestorePoints"))
                    {
                        reader.Read();
                        reader.Read();
                        while (reader.TokenType != JsonToken.EndArray)
                        {
                            int restorePointNumber = 0;
                            restorePointNumber = int.Parse(reader.Value.ToString());
                            reader.Read();
                            var dateTime = DateTime.Parse(reader.Value.ToString());
                            reader.Read();
                            reader.Read();
                            List<JobObject> restorePointJobObjects = new ();
                            while (reader.TokenType != JsonToken.EndArray)
                            {
                                restorePointJobObjects.Add(new JobObject(reader.Value.ToString()));
                                reader.Read();
                            }

                            reader.Read();

                            restorePoints.Add(new RestorePoint(repository, restorePointNumber, storageAlgorithm, dateTime, restorePointJobObjects.ToArray()));
                        }
                    }
                }
            }

            restorePoints.Sort();
            var backupJob = new BackupJob(new Backup(), repository, name, storageAlgorithm, restorePoints.FirstOrDefault().DateTime, restorePoints.FirstOrDefault().Number - 1, restorePoints.FirstOrDefault().JobObjects.ToArray());
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
            RemoveAlgorithm = removeAlgorithm;
            return backupJob;
        }

        public void ToJson()
        {
            var stringBuilder = new StringBuilder();
            var stringWriter = new StringWriter(stringBuilder);
            using (JsonWriter writer = new JsonTextWriter(stringWriter))
            {
                writer.Formatting = Formatting.Indented;

                writer.WriteStartObject();
                writer.WritePropertyName("Name");
                writer.WriteValue(BackupJob.Name);
                writer.WritePropertyName("Repository type");
                writer.WriteValue(ExtraRepositoryManager.AddExtraRepository(BackupJob.Repository).Type());
                writer.WritePropertyName("Repository Path");
                writer.WriteValue(ExtraRepositoryManager.AddExtraRepository(BackupJob.Repository).RepositoryPath);
                writer.WritePropertyName("StorageAlgorithm");
                writer.WriteValue(StorageAlgorithm.ToString());
                writer.WritePropertyName("removeAlgorithm");
                writer.WriteValue(RemoveAlgorithm.ToString());
                writer.WritePropertyName("RemoveAlgorithmParam");
                writer.WriteValue(RemoveAlgorithm.Param.ToString());
                writer.WritePropertyName("JobObjects");
                writer.WriteStartArray();
                BackupJob.JobObjects.ToList().ForEach(jobObject => writer.WriteValue(jobObject.Fullname));
                writer.WriteEndArray();

                writer.WritePropertyName("RestorePoints");
                writer.WriteStartArray();
                BackupJob.Backup.RestorePoints.ToList().ForEach(restorePoint =>
                {
                    writer.WriteValue(restorePoint.Number);
                    writer.WriteValue(restorePoint.DateTime);
                    writer.WriteStartArray();
                    restorePoint.JobObjects.ToList().ForEach(jobObject => writer.WriteValue(jobObject.Fullname));
                    writer.WriteEndArray();
                });
                writer.WriteEndArray();
                writer.WriteEndObject();
            }

            string jsonPath = Path.Combine(_rootPath, BackupJob.Name + ".cfg");
            File.WriteAllText(jsonPath, stringWriter.ToString());
        }
    }
}