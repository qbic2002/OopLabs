using System.Collections.Generic;
using System.Linq;
using Backups.Entities;
using BackupsExtra.Tools;

namespace BackupsExtra.Services
{
    public static class RestorePointManager
    {
        public static RestorePoint Merge(params RestorePoint[] restorePoints)
        {
            if (restorePoints.Length <= 1)
                throw new BackupsExtraException("Incorrect restore points");
            var restorePointsList = restorePoints.ToList();
            restorePointsList.Sort();

            RestorePoint oldRestorePoint = restorePointsList[0];
            RestorePoint newerRestorePoint = restorePointsList[1];
            var oldJobObjects = oldRestorePoint.JobObjects.ToList();
            var newerJobObjects = newerRestorePoint.JobObjects.ToList();
            var newJobObjects = new List<JobObject>();

            oldJobObjects.ForEach(oldJobObject =>
            {
                if (newerJobObjects.Exists(newerJobObject => newerJobObject.Fullname == oldJobObject.Fullname))
                {
                    newerJobObjects.Add(newerJobObjects.Find(newerJobObject => newerJobObject.Fullname == oldJobObject.Fullname));
                }
                else
                {
                    newJobObjects.Add(oldJobObject);
                }
            });
            newerJobObjects.ForEach(newerJobObject =>
            {
                if (!newJobObjects.Exists(newJobObject => newerJobObject.Fullname == newJobObject.Fullname))
                    newJobObjects.Add(newerJobObject);
            });
            var newRestorePoint = new RestorePoint(newerRestorePoint.Repository, newerRestorePoint.Number, newerRestorePoint.StorageAlgorithm, newerRestorePoint.DateTime, newJobObjects.ToArray());

            if (restorePointsList.Count == 2)
            {
                return newRestorePoint;
            }

            restorePointsList.RemoveAt(0);
            restorePointsList.RemoveAt(0);
            restorePointsList.Add(newRestorePoint);
            return Merge(restorePointsList.ToArray());
        }
    }
}