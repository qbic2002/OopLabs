using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Backups.Tools;

namespace Backups.Entities
{
    public class RestorePoint
    {
        private List<JobObject> _jobObjects = new ();
        private Algorithms.StorageAlgorithm _storageAlgorithm;
        private List<Storage> _storages = new ();
        private IRepository _repository;
        public RestorePoint(IRepository repository, int number, Algorithms.StorageAlgorithm storageAlgorithm, params JobObject[] jobObjects)
        {
            _repository = repository ?? throw new BackupException("Incorrect repository");
            if (number <= 0)
                throw new BackupException("Incorrect number");
            Number = number;
            _storageAlgorithm = storageAlgorithm ?? throw new BackupException("Incorrect algorithm");
            DateTime = DateTime.Now;
            if (jobObjects is null || jobObjects.Length == 0)
                throw new BackupException("Incorrect objects");
            jobObjects.ToList().ForEach(_jobObjects.Add);

            JobObjects = new ReadOnlyCollection<JobObject>(_jobObjects);
            _repository.AddRestorePoint(this);
            CreateStorage();
        }

        public DateTime DateTime { get; }
        public int Number { get; }
        public ReadOnlyCollection<JobObject> JobObjects { get; }

        private void CreateStorage()
        {
            List<Storage> storages = _storageAlgorithm(_jobObjects.ToArray());
            _storages.AddRange(storages);
            _repository.AddStorages(this, storages.ToArray());
        }
    }
}