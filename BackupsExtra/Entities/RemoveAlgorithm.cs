using System;
using System.Collections.Generic;
using System.Linq;
using BackupsExtra.Tools;

namespace BackupsExtra.Entities
{
    public class RemoveAlgorithm
    {
        public RemoveAlgorithm(bool allPredicates, params IRemovePredicate[] predicates)
        {
            AllPredicates = allPredicates;
            if (predicates is null || predicates.Length == 0)
                throw new BackupsExtraException("Incorrect predicates");
            RemovePredicates = predicates.ToList();
        }

        public List<IRemovePredicate> RemovePredicates { get; }
        public bool AllPredicates { get; }
        public void RemoveRestorePoints(ExtraBackupJob extraBackupJob)
        {
            var ranges = new List<int>();
            RemovePredicates.ForEach(predicate => ranges.Add(predicate.GetRange(extraBackupJob)));
            ranges.Sort();
            int range = 0;
            if (AllPredicates)
                range = ranges.FirstOrDefault();
            else
                range = ranges.LastOrDefault();
            if (range > 0 && range < extraBackupJob.Backup.RestorePoints.Count)
            {
                extraBackupJob.RemoveRestorePointRangeWithMerge(0, range);
            }
        }
    }
}