using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Shops.Tools;

namespace Shops.Entities
{
    public class ReadOnlyProductList : ReadOnlyDictionary<Product, ProductInfo>
    {
        public ReadOnlyProductList(IDictionary<Product, ProductInfo> dictionary)
            : base(dictionary)
        {
        }

        public decimal GetPrice(Product product, int count = 1)
        {
            if (!ContainsKey(product))
                throw new ShopException("No product");
            if (this[product].Count < count)
                throw new ShopException("Not enough items");
            return this[product].Price * count;
        }
    }
}