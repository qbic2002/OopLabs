using System;
using Banks.Tools;

namespace Banks.Entities
{
    public class DepositPercentRange
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DepositPercentRange"/> class.
        /// Set Range [min; max).
        /// </summary>
        /// <param name="value">Value.</param>
        /// <param name="min">Min border.</param>
        /// <param name="max">Max border.</param>
        public DepositPercentRange(decimal value, decimal min = 0, decimal max = decimal.MaxValue)
        {
            if (value < 0)
                throw new BanksException("Incorrect value");
            if (min < 0)
                throw new BanksException("Incorrect minimum");
            if (max < min)
                throw new BanksException("incorrect maximum");
            Value = value;
            Min = min;
            Max = max;
        }

        public decimal Min { get; }
        public decimal Max { get; }
        public decimal Value { get; }
        public bool InRange(decimal rangeElement) => Min <= rangeElement && rangeElement < Max;

        public decimal GetValue()
        {
            return Value;
        }

        public override string ToString()
        {
            return new string($"{Min} <= start deposit < {(Max == decimal.MaxValue ? "inf" : Max)}: {Value}");
        }
    }
}