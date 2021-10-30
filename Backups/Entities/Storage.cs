using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.IO.Compression;
using System.Linq;
using Backups.Tools;

namespace Backups.Entities
{
    public class Storage
    {
        private List<JobObject> _jobObjects = new ();
        public Storage(string name, params JobObject[] jobObjects)
        {
            Name = name ?? throw new BackupException("Incorrect name");
            if (jobObjects is null || jobObjects.Length == 0)
                throw new BackupException("Incorrect objects");
            jobObjects.ToList().ForEach(_jobObjects.Add);
            JobObjects = new ReadOnlyCollection<JobObject>(_jobObjects);
        }

        public string Name { get; }
        public ReadOnlyCollection<JobObject> JobObjects { get; }
    }
}