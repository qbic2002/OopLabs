namespace BackupsExtra.Entities
{
    public interface ILogger
    {
        void PrintLog(string log, bool printDateTime);
    }
}