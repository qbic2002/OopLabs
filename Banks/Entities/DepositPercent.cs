using System.Collections.Generic;
using System.Linq;
using Banks.Tools;

namespace Banks.Entities
{
    public class DepositPercent
    {
        private List<DepositPercentRange> _depositPercentRanges = new ();
        private decimal _defaultValue;
        public DepositPercent(decimal defaultValue)
        {
            if (defaultValue < 0)
                throw new BanksException("Incorrect default value");
            _defaultValue = defaultValue;
        }

        public DepositPercent(decimal defaultValue, params DepositPercentRange[] depositPercentRanges)
        {
            if (defaultValue < 0)
                throw new BanksException("Incorrect default value");
            _defaultValue = defaultValue;
            AddRanges(depositPercentRanges);
        }

        public static DepositPercent GetDefault()
        {
            var ranges = new List<DepositPercentRange>()
            {
                new DepositPercentRange(0.03M, 0, 50000),
                new DepositPercentRange(0.035M, 50000, 100000),
                new DepositPercentRange(0.04M, 100000),
            };
            var defaultDepositPercent = new DepositPercent(0.01M, ranges.ToArray());
            return defaultDepositPercent;
        }

        public void AddRange(DepositPercentRange depositPercentRange)
        {
            if (depositPercentRange is null)
                throw new BanksException("Incorrect range");
            _depositPercentRanges.ForEach(range =>
            {
                if (range.InRange(depositPercentRange.Min) || range.InRange(depositPercentRange.Max))
                {
                    throw new BanksException("Range intersects");
                }
            });
            _depositPercentRanges.Add(depositPercentRange);
        }

        public void AddRanges(params DepositPercentRange[] depositPercentRanges)
        {
            if (depositPercentRanges is null || depositPercentRanges.Length == 0)
                throw new BanksException("Incorrect deposit percent ranges");
            depositPercentRanges.ToList().ForEach(AddRange);
        }

        public void AddRange(decimal value, decimal min = 0, decimal max = decimal.MaxValue) =>
            AddRange(new DepositPercentRange(value, min, max));

        public decimal Calculate(decimal startDeposit)
        {
            if (startDeposit < 0)
                throw new BanksException("Incorrect start deposit");
            decimal percent = _defaultValue;
            _depositPercentRanges.ForEach(range =>
            {
                if (range.InRange(startDeposit))
                    percent = range.GetValue();
            });
            return percent;
        }

        public override string ToString()
        {
            return string.Concat("Default percent: " + _defaultValue + "; ", string.Join<DepositPercentRange>("; ", _depositPercentRanges.OrderBy(range => range.Min).ToArray()));
        }
    }
}