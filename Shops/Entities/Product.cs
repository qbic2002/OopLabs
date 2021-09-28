using System;

namespace Shops.Entities
{
    public class Product
    {
        public Product(string name, int id)
        {
            Name = name ?? throw new Exception("Incorrect name");
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