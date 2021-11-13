using Banks.Tools;

namespace Banks.Entities
{
    public class DepositPercent : IDepositPercent
    {
        public DepositPercent(decimal firstPercent = 0.03M, decimal secondPercent = 0.035M, decimal thirdPercent = 0.04M)
        {
            if (firstPercent <= 0 || secondPercent <= 0 || thirdPercent <= 0)
                throw new BanksException("Incorrect percents");
            FirstPercent = firstPercent;
            SecondPercent = secondPercent;
            ThirdPercent = thirdPercent;
        }

        public decimal FirstPercent { get; }
        public decimal SecondPercent { get; }
        public decimal ThirdPercent { get; }

        public decimal Calculate(decimal startDeposit)
        {
            if (startDeposit < 0)
                throw new BanksException("Incorrect start deposit");
            switch (startDeposit)
            {
                case < 50000:
                    return FirstPercent;
                case < 100000:
                    return SecondPercent;
                case > 100000:
                    return ThirdPercent;
            }

            throw new BanksException("Cannot calculate percent");
        }

        public override string ToString()
        {
            return new string($"less than 50000: {FirstPercent}; less than 100000 {SecondPercent}; more than 100000 {ThirdPercent}");
        }
    }
}