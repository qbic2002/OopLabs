using System;
using Shops.Tools;

namespace Shops.Entities
{
    public class Person
    {
        public Person(decimal money)
        {
            if (money <= 0)
                throw new ShopException("Incorrect money");
            Money = money;
        }

        public decimal Money { get; private set; }

        public void ReduceMoney(decimal totalPrice)
        {
            if (totalPrice > Money)
                throw new ShopException("Not enough money");
            Money -= totalPrice;
        }
    }
}