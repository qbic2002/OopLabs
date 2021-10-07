using Shops.Tools;

namespace Shops.Entities
{
    public class ProductSet
    {
        public ProductSet(Product product, int count = 1)
        {
            Product = product ?? throw new ShopException("Incorrect product");
            if (count <= 0)
                throw new ShopException("Incorrect number of products");
            Count = count;
        }

        public Product Product { get; }
        public int Count { get; }
    }
}