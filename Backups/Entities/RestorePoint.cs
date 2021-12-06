using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Backups.Tools;

namespace Backups.Entities
{
    public class RestorePoint : IComparable<RestorePoint>
    {
        private List<JobObject> _jobObjects = new ();
        private List<Storage> _storages = new ();
        public RestorePoint(IRepository repository, int number, IAlgorithm storageAlgorithm, DateTime dateTime, params JobObject[] jobObjects)
        {
            Repository = repository ?? throw new BackupException("Incorrect repository");
            if (number <= 0)
                throw new BackupException("Incorrect number");
            Number = number;
            StorageAlgorithm = storageAlgorithm ?? throw new BackupException("Incorrect algorithm");
            DateTime = dateTime;
            if (jobObjects is null || jobObjects.Length == 0)
                throw new BackupException("Incorrect objects");
            _jobObjects.AddRange(jobObjects);
            JobObjects = new ReadOnlyCollection<JobObject>(_jobObjects);
            Storages = new ReadOnlyCollection<Storage>(_storages);
        }

        public DateTime DateTime { get; }
        public int Number { get; }
        public ReadOnlyCollection<JobObject> JobObjects { get; }
        public ReadOnlyCollection<Storage> Storages { get; }
        public IRepository Repository { get; }
        public IAlgorithm StorageAlgorithm { get; }

        public void CreateStorage()
        {
            List<Storage> storages = StorageAlgorithm.DoStrategy(_jobObjects.ToArray());
            _storages.AddRange(storages);
            Repository.AddStorages(this, storages.ToArray());
        }

        public void AddRestorePointToRepository()
        {
            Repository.AddRestorePoint(this);
        }

        public int CompareTo(RestorePoint other)
        {
            if (ReferenceEquals(this, other)) return 0;
            if (ReferenceEquals(null, other)) return 1;
            return DateTime.CompareTo(other.DateTime);
        }

        public override string ToString()
        {
            string log = $"Number: {Number}; Date: {DateTime};";
            string jobObjects = string.Join<JobObject>(", ", JobObjects.ToArray());
            return string.Concat(log, "Job objects: ", jobObjects);
        }
    }
}