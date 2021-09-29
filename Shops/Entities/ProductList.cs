using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using Shops.Tools;

namespace Shops.Entities
{
    public class ProductList : Dictionary<Product, ProductInfo>
    {
        public void Buy(Product product, int count)
        {
            if (!ContainsKey(product))
                throw new ShopException("No product");
            if (this[product].Count < count)
                throw new ShopException("Not enough items");
            if (this[product].Count == count)
            {
                Remove(product);
                return;
            }

            if (this[product].Count > count)
            {
                this[product].RemoveCount(count);
            }
        }

        public decimal GetPrice(Product product, int count = 1)
        {
            if (!ContainsKey(product))
                throw new ShopException("No product");
            if (this[product].Count < count)
                throw new ShopException("Not enough items");
            return this[product].Price * count;
        }

        public void ChangePrice(Product product, decimal newPrice)
        {
            if (newPrice <= 0)
                throw new ShopException("Incorrect price");
            if (!ContainsKey(product))
                throw new ShopException("No product");
            this[product].ChangePrice(newPrice);
        }
    }
}