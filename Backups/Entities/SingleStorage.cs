using System.Collections.Generic;
using Backups.Tools;

namespace Backups.Entities
{
    public class SingleStorage : IAlgorithm
    {
        public List<Storage> DoStrategy(params JobObject[] jobObjects)
        {
            if (jobObjects is null || jobObjects.Length == 0)
                throw new BackupException("Incorrect objects");
            var storages = new List<Storage> { new Storage("Storage", jobObjects) };
            return storages;
        }
    }
}