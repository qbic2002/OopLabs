using Backups.Entities;
using BackupsExtra.Entities;
using BackupsExtra.Tools;

namespace BackupsExtra.Services
{
    public static class ExtraRepositoryManager
    {
        public static IExtraRepository AddExtraRepository(IRepository repository)
        {
            if (repository is null)
                throw new BackupsExtraException("Incorrect repository");
            switch (repository)
            {
                case LocalRepository localRepository:
                    return new ExtraLocalRepository(localRepository);
                default:
                    throw new BackupsExtraException("Incorrect repository");
            }
        }
    }
}