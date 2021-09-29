using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.InteropServices;
using Shops.Entities;
using Shops.Tools;

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

        public Shop LowestPrice(Product product, int count)
        {
            if (count <= 0)
                throw new ShopException("Invalid number of products");
            var listOfShopsWithThisProduct = new List<Shop>();
            _listOfShops.ForEach(shop =>
            {
                if (shop.Products.ContainsKey(product) && shop.Products[product].Count >= count)
                    listOfShopsWithThisProduct.Add(shop);
            });
            if (listOfShopsWithThisProduct.Count == 0)
                throw new ShopException("Can't find shops withs this product");
            decimal minPrice = listOfShopsWithThisProduct[0].Products.GetPrice(product, count);
            Shop shopWithMinPrice = listOfShopsWithThisProduct[0];
            listOfShopsWithThisProduct.ForEach(shop =>
            {
                if (shop.Products.GetPrice(product, count) < minPrice)
                {
                    minPrice = shop.Products.GetPrice(product, count);
                    shopWithMinPrice = shop;
                }
            });
            return shopWithMinPrice;
        }
    }
}