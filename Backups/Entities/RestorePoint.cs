using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Backups.Tools;
using Microsoft.VisualBasic.CompilerServices;

namespace Backups.Entities
{
    public class RestorePoint : IComparable<RestorePoint>
    {
        private List<JobObject> _jobObjects = new ();
        private IAlgorithm _storageAlgorithm;
        private List<Storage> _storages = new ();
        private IRepository _repository;
        public RestorePoint(IRepository repository, int number, IAlgorithm storageAlgorithm, DateTime dateTime, params JobObject[] jobObjects)
        {
            _repository = repository ?? throw new BackupException("Incorrect repository");
            if (number <= 0)
                throw new BackupException("Incorrect number");
            Number = number;
            _storageAlgorithm = storageAlgorithm ?? throw new BackupException("Incorrect algorithm");
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

        public void CreateStorage()
        {
            List<Storage> storages = _storageAlgorithm.DoStrategy(_jobObjects.ToArray());
            _storages.AddRange(storages);
            _repository.AddStorages(this, storages.ToArray());
        }

        public void AddRestorePointToRepository()
        {
            _repository.AddRestorePoint(this);
        }

        public int CompareTo(RestorePoint other)
        {
            if (ReferenceEquals(this, other)) return 0;
            if (ReferenceEquals(null, other)) return 1;
            return DateTime.CompareTo(other.DateTime);
        }
    }
}