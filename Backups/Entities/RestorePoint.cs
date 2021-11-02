using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Backups.Tools;

namespace Backups.Entities
{
    public class RestorePoint
    {
        private List<JobObject> _jobObjects = new ();
        private IAlgorithm _storageAlgorithm;
        private List<Storage> _storages = new ();
        private IRepository _repository;
        public RestorePoint(IRepository repository, int number, IAlgorithm storageAlgorithm, params JobObject[] jobObjects)
        {
            _repository = repository ?? throw new BackupException("Incorrect repository");
            if (number <= 0)
                throw new BackupException("Incorrect number");
            Number = number;
            _storageAlgorithm = storageAlgorithm ?? throw new BackupException("Incorrect algorithm");
            DateTime = DateTime.Now;
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
    }
}