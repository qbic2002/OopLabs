using BackupsExtra.Tools;

namespace BackupsExtra.Entities
{
    public interface IRemovePredicate
    {
        object Param { get; }
        int GetRange(ExtraBackupJob extraBackupJob);
        string Type();
    }
}