using System;
using System.Collections.Generic;
using System.Linq;
using Shops.Entities;
using Shops.Services;

namespace Shops
{
    internal class Program
    {
        private static ShopManager _shopManager = new ShopManager();

        private static void Main()
        {
            Shop shop = _shopManager.AddShop("Shop", "Address");
            Product product = _shopManager.RegisterProduct("test");
            var p = new Person(1000);
        }
    }
}