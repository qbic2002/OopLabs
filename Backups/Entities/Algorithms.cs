using System.Collections.Generic;
using System.Linq;
using Backups.Tools;

namespace Backups.Entities
{
    public static class Algorithms
    {
        static Algorithms()
        {
        }

        public delegate List<Storage> StorageAlgorithm(params JobObject[] jobObjects);
        public static StorageAlgorithm SplitStorage { get; } = SplitStorageMethod;
        public static StorageAlgorithm SingleStorage { get; } = SingleStorageMethod;

        private static List<Storage> SplitStorageMethod(params JobObject[] jobObjects)
        {
            if (jobObjects is null || jobObjects.Length == 0)
                throw new BackupException("Incorrect objects");
            var storages = new List<Storage>();
            jobObjects.ToList().ForEach(jobObject => storages.Add(new Storage(jobObject.NameWithoutExtension, jobObject)));
            return storages;
        }

        private static List<Storage> SingleStorageMethod(params JobObject[] jobObjects)
        {
            if (jobObjects is null || jobObjects.Length == 0)
                throw new BackupException("Incorrect objects");
            var storages = new List<Storage>();
            storages.Add(new Storage("Storage", jobObjects));
            return storages;
        }
    }
}