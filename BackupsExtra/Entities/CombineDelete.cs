using System;
using BackupsExtra.Tools;

namespace BackupsExtra.Entities
{
    public class CombineDelete : IRemoveAlgorithm
    {
        public CombineDelete(int maxRestorePoints, DateTime dateTime, bool allAlgorithms)
        {
            if (maxRestorePoints <= 0)
                throw new BackupsExtraException("Incorrect number of max restore points");
            Param = new CombineParams(maxRestorePoints, dateTime, allAlgorithms);
        }

        public CombineDelete(CombineParams param)
        {
            Param = param ?? throw new BackupsExtraException("Incorrect params");
        }

        public object Param { get; }
        public void RemoveRestorePoints(ExtraBackupJob extraBackupJob)
        {
            int range = GetRange(extraBackupJob);
            if (range > 0 && range < extraBackupJob.Backup.RestorePoints.Count)
            {
                extraBackupJob.RemoveRestorePointRangeWithMerge(0, range);
            }
        }

        public int GetRange(ExtraBackupJob extraBackupJob)
        {
            var combineParams = Param as CombineParams;
            if (combineParams.AllAlgorithms)
            {
                return Math.Min(new CountDelete(combineParams.MaxRestorePoints).GetRange(extraBackupJob), new DateDelete(combineParams.DateTime).GetRange(extraBackupJob));
            }

            return Math.Max(new CountDelete(combineParams.MaxRestorePoints).GetRange(extraBackupJob), new DateDelete(combineParams.DateTime).GetRange(extraBackupJob));
        }

        public override string ToString()
        {
            return "CombineDelete";
        }
    }
}