using System.Collections.Generic;
using System.Linq;
using Backups.Tools;

namespace Backups.Entities
{
    public class SplitStorage : IAlgorithm
    {
        public List<Storage> DoStrategy(params JobObject[] jobObjects)
        {
            if (jobObjects is null || jobObjects.Length == 0)
                throw new BackupException("Incorrect objects");
            var storages = new List<Storage>();
            jobObjects.ToList().ForEach(jobObject => storages.Add(new Storage(jobObject.NameWithoutExtension, jobObject)));
            return storages;
        }

        public override string ToString()
        {
            return "SplitStorage";
        }
    }
}