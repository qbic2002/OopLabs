using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Shops.Entities;

namespace Shops.Services
{
    public class ShopManager
    {
        private int _shopId;
        private int _productId;
        private List<Shop> _listOfShops = new List<Shop>();
        private List<Product> _listOfProducts = new List<Product>();
        public ShopManager()
        {
            _shopId = 0;
            _productId = 0;
            ListOfShops = new ReadOnlyCollection<Shop>(_listOfShops);
            ListOfProducts = new ReadOnlyCollection<Product>(_listOfProducts);
        }

        public ReadOnlyCollection<Shop> ListOfShops { get; }
        public ReadOnlyCollection<Product> ListOfProducts { get; }
        public Shop AddShop(string name, string address)
        {
            var newShop = new Shop(name, address, _shopId);
            _listOfShops.Add(newShop);
            _shopId++;
            return newShop;
        }

        public Product RegisterProduct(string name)
        {
            var newProduct = new Product(name, _productId);
            _listOfProducts.Add(newProduct);
            _productId++;
            return newProduct;
        }
    }
}