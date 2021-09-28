using System;

namespace Shops.Entities
{
    public class ProductInfo
    {
        public ProductInfo(decimal price, int count)
        {
            if (price <= 0)
                throw new Exception("Invalid price of products");
            Price = price;
            if (count <= 0)
                throw new Exception("Invalid number of products");
            Count = count;
        }

        public decimal Price { get; }
        public int Count { get; private set; }

        public void RemoveCount(int count)
        {
            if (count > Count)
                throw new Exception("Not enough items");
            Count -= count;
        }

        public void AddItems(int count)
        {
            Count += count;
        }
    }
}