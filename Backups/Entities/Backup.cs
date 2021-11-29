using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Backups.Entities
{
    public class Backup
    {
        private List<RestorePoint> _restorePoints = new ();

        public Backup()
        {
            RestorePoints = new ReadOnlyCollection<RestorePoint>(_restorePoints);
        }

        public ReadOnlyCollection<RestorePoint> RestorePoints { get; }
        public void AddRestorePoint(RestorePoint restorePoint) => _restorePoints.Add(restorePoint);

        public RestorePoint RemoveRestorePoint(int index)
        {
            _restorePoints.Sort();
            RestorePoint restorePoint = _restorePoints[index];
            _restorePoints.RemoveAt(index);
            return restorePoint;
        }

        public List<RestorePoint> RemoveRestorePointRange(int index, int range)
        {
            var restorePoints = new List<RestorePoint>();
            for (int i = 0; i < range; i++)
            {
                restorePoints.Add(RemoveRestorePoint(index));
            }

            return restorePoints;
        }
    }
}