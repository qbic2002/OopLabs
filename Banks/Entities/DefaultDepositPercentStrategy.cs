using Banks.Tools;

namespace Banks.Entities
{
    public class DefaultDepositPercentStrategy : IDepositPercentStrategy
    {
        public decimal Calculate(decimal startDeposit)
        {
            if (startDeposit < 0)
                throw new BanksException("Incorrect start deposit");
            switch (startDeposit)
            {
                case < 50000:
                    return 0.03M;
                case < 100000:
                    return 0.035M;
                case > 100000:
                    return 0.04M;
            }

            throw new BanksException("Cannot calculate percent");
        }
    }
}