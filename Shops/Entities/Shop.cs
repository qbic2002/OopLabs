using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Shops.Entities
{
    public class Shop
    {
        private Dictionary<Product, ProductInfo> _products = new Dictionary<Product, ProductInfo>();
        public Shop(string name, string address, int id)
        {
            Name = name ?? throw new Exception("Incorrect name");
            Address = address ?? throw new Exception("Incorrect address");
            Id = id;
            Products = new ReadOnlyDictionary<Product, ProductInfo>(_products);
        }

        public string Name { get; }
        public string Address { get; }
        public int Id { get; }
        public ReadOnlyDictionary<Product, ProductInfo> Products { get; }

        public void AddProducts(Product product, decimal price, int count)
        {
            var productInfo = new ProductInfo(price, count);
            if (_products.ContainsKey(product))
                _products[product].AddItems(count);
            else
                _products.Add(product, productInfo);
        }

        public override bool Equals(object obj)
        {
            return obj is Shop shop && Id.Equals(shop.Id);
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
    }
}