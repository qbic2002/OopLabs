using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Backups.Entities;
using BackupsExtra.Tools;

namespace BackupsExtra.Entities
{
    public class RestorePointSerialize
    {
        public RestorePointSerialize(RestorePoint restorePoint)
        {
            if (restorePoint is null)
                throw new BackupsExtraException("Incorrect restore point");
            Number = restorePoint.Number;
            DateTime = restorePoint.DateTime;
            JobObjectSerializes = new List<JobObjectSerialize>();
            restorePoint.JobObjects.ToList().ForEach(jobObject => JobObjectSerializes.Add(new JobObjectSerialize(jobObject)));
        }

        public RestorePointSerialize()
        {
        }

        public int Number { get; set; }
        public DateTime DateTime { get; set; }
        public List<JobObjectSerialize> JobObjectSerializes { get; set; }

        public RestorePoint ToRestorePoint(IRepository repository, IAlgorithm storageAlgorithm)
        {
            var jobObjects = new List<JobObject>();
            JobObjectSerializes.ForEach(serialize => jobObjects.Add(serialize.ToObject()));
            return new RestorePoint(repository, Number, storageAlgorithm, DateTime, jobObjects.ToArray());
        }
    }
}