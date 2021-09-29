using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Shops.Tools;

namespace Shops.Entities
{
    public class Shop
    {
        private ProductList _products = new ProductList();
        public Shop(string name, string address, int id)
        {
            Name = name ?? throw new ShopException("Incorrect name");
            Address = address ?? throw new ShopException("Incorrect address");
            Id = id;
            Products = new ReadOnlyProductList(_products);
        }

        public string Name { get; }
        public string Address { get; }
        public int Id { get; }
        public ReadOnlyProductList Products { get; }

        public void AddProducts(Product product, decimal price, int count)
        {
            var productInfo = new ProductInfo(price, count);
            if (_products.ContainsKey(product))
                _products[product].AddItems(count);
            else
                _products.Add(product, productInfo);
        }

        public void ChangePrice(Product product, decimal newPrice)
        {
            if (product is null)
                throw new ShopException("Incorrect product");
            _products.ChangePrice(product, newPrice);
        }

        public void Buy(Person person, Product product, int count)
        {
            if (product is null)
                throw new ShopException("Incorrect product");
            if (person is null)
                throw new ShopException("Incorrect person");
            if (count <= 0)
                throw new ShopException("Incorrect number of products");
            if (!_products.ContainsKey(product))
                throw new ShopException("No product");

            decimal totalPrice = _products.GetPrice(product, count);

            if (totalPrice > person.Money)
                throw new ShopException("Not enough money");
            _products.Buy(product, count);
            person.ReduceMoney(totalPrice);
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