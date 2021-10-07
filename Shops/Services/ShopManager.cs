using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
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
            if (name is null)
                throw new ShopException("Incorrect name of shop");
            if (address is null)
                throw new ShopException("Incorrect address");
            var newShop = new Shop(name, address, _shopId);
            _listOfShops.Add(newShop);
            _shopId++;
            return newShop;
        }

        public Product RegisterProduct(string name)
        {
            if (name is null)
                throw new ShopException("Incorrect name of product");
            var newProduct = new Product(name, _productId);
            _listOfProducts.Add(newProduct);
            _productId++;
            return newProduct;
        }

        public Shop LowestPrice(params ProductSet[] productSets)
        {
            if (productSets is null || productSets.Length == 0)
                throw new ShopException("Incorrect set of products");
            var listOfShopsWithThisProduct = _listOfShops.Where(shop => shop.IsContains(productSets)).ToList();
            if (listOfShopsWithThisProduct.Count == 0 || listOfShopsWithThisProduct is null)
                throw new ShopException("Can't find shops with this products");
            Shop shopWithMinPrice = listOfShopsWithThisProduct.OrderBy(shop => shop.GetPrice(productSets)).FirstOrDefault();
            return shopWithMinPrice;
        }
    }
}