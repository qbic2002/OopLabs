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
    }
}