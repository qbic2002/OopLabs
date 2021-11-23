using Banks.Tools;

namespace Banks.Services
{
    public class TimeManager
    {
        public TimeManager(CentralBank centralBank)
        {
            CentralBank = centralBank ?? throw new BanksException("Incorrect central bank");
            CentralBank.TimeManager = this;
        }

        public CentralBank CentralBank { get; }
        public int Day { get; private set; } = 0;

        public void SkipDay()
        {
            CentralBank.AddOneDay();
            ++Day;
            if (Day % 30 == 0)
                CentralBank.AddInterest();
        }

        public void SkipMonth()
        {
            for (int day = 0; day < 30; day++)
            {
                SkipDay();
            }
        }
    }
}