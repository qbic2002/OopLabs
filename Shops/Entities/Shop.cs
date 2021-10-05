using System.Collections.Generic;
using Shops.Tools;

namespace Shops.Entities
{
    public class Shop
    {
        private ProductList _products = new ProductList(new Dictionary<Product, ProductInfo>());
        public Shop(string name, string address, int id)
        {
            if (id < 0)
                throw new ShopException("Incorrect Id");
            Name = name ?? throw new ShopException("Incorrect name");
            Address = address ?? throw new ShopException("Incorrect address");
            Id = id;
            Products = new ReadOnlyProductList(_products.Products);
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
            if (newPrice <= 0)
                throw new ShopException("Incorrect new price");
            _products.ChangePrice(product, newPrice);
        }

        public void Buy(Person person, params ProductSet[] productSets)
        {
            if (productSets is null || productSets.Length == 0)
                throw new ShopException("Incorrect set of products");
            decimal totalPrice = _products.GetPrice(productSets);

            if (totalPrice > person.Money)
                throw new ShopException("Not enough money");
            _products.Buy(productSets);
            person.ReduceMoney(totalPrice);
        }

        public bool IsContains(params ProductSet[] productSets) => _products.IsContains(productSets);
        public decimal GetPrice(params ProductSet[] productSets) => _products.GetPrice(productSets);
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