using Shops.Tools;

namespace Shops.Entities
{
    public class Product
    {
        public Product(string name, int id)
        {
            if (id < 0)
                throw new ShopException("Incorrect Id");
            Name = name ?? throw new ShopException("Incorrect name");
            Id = id;
        }

        public int Id { get; }
        public string Name { get; }
        public override bool Equals(object obj)
        {
            return obj is Product product && Id.Equals(product.Id);
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
    }
}