using System;
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
            for (int i = 0; i < 5; i++)
            {
                _shopManager.RegisterProduct(i.ToString());
            }

            for (int i = 0; i < 5; i++)
            {
                shop.AddProducts(_shopManager.ListOfProducts[i], (i + 1) * 2, i + 1);
            }

            shop.Products.Values.ToList().ForEach(productInfo => Console.WriteLine(productInfo.Price));
        }
    }
}