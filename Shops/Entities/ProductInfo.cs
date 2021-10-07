using Shops.Tools;

namespace Shops.Entities
{
    public class ProductInfo
    {
        public ProductInfo(decimal price, int count)
        {
            if (price <= 0)
                throw new ShopException("Invalid price of products");
            if (count <= 0)
                throw new ShopException("Invalid number of products");
            Price = price;
            Count = count;
        }

        public decimal Price { get; private set; }
        public int Count { get; private set; }

        public void RemoveCount(int count)
        {
            if (count > Count)
                throw new ShopException("Not enough items");
            if (count <= 0)
                throw new ShopException("Incorrect number of products");
            Count -= count;
        }

        public void AddItems(int count)
        {
            if (count <= 0)
                throw new ShopException("Incorrect number of products");
            Count += count;
        }

        public void ChangePrice(decimal newPrice)
        {
            if (newPrice <= 0)
                throw new ShopException("Incorrect price");
            Price = newPrice;
        }
    }
}