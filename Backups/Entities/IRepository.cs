namespace Backups.Entities
{
    public interface IRepository
    {
        void CreateRepository();
        void AddRestorePoint(RestorePoint restorePoint);
        void AddStorages(RestorePoint restorePoint, params Storage[] storages);
    }
}