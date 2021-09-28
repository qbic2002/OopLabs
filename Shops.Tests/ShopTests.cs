using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Shops.Entities;
using Shops.Services;

namespace Shops.Tests
{
    [TestFixture]
    public class ShopTests
    {
        private ShopManager _shopManager;

        [SetUp]
        public void SetUp()
        {
            _shopManager = new ShopManager();
        }

        [Test]
        public void AddShops_ListOfShopContainsThem()
        {
            var expectedList = new List<Shop>();
            for (int i = 0; i < 5; i++)
            {
                expectedList.Add(_shopManager.AddShop(i.ToString(), "Address"));
            }

            expectedList.ForEach(shop => Assert.Contains(shop, _shopManager.ListOfShops));
            Assert.AreEqual(expectedList.Count, _shopManager.ListOfShops.Count);
        }

        [Test]
        public void RegisterProducts_ListOfProductsContainsThem()
        {
            var expectedList = new List<Product>();
            for (int i = 0; i < 5; i++)
            {
                expectedList.Add(_shopManager.RegisterProduct(i.ToString()));
            }

            expectedList.ForEach(shop => Assert.Contains(shop, _shopManager.ListOfProducts));
            Assert.AreEqual(expectedList.Count, _shopManager.ListOfProducts.Count);
        }

        [Test]
        public void AddNewProductsToShop_ProductsContainsThem()
        {
            Shop shop = _shopManager.AddShop("Shop", "Address");
            for (int i = 0; i < 5; i++)
            {
                _shopManager.RegisterProduct(i.ToString());
            }

            for (int i = 0; i < 5; i++)
            {
                shop.AddProducts(_shopManager.ListOfProducts[i], (i + 1) * 2, i + 1);
            }
            
            _shopManager.ListOfProducts.ToList().ForEach(product =>
            {
                Assert.Contains(product, shop.Products.Keys);
                Assert.AreEqual((product.Id + 1) * 2, shop.Products[product].Price);
                Assert.AreEqual(product.Id + 1, shop.Products[product].Count);
            });
        }
        
        [TestCase(5, 7)]
        [TestCase(1,1)]
        [TestCase(10,6)]
        public void AddExistingProduct_NumberOfProductGrows(int firstNumber, int extraNumber)
        {
            Shop shop = _shopManager.AddShop("Shop", "Address");
            Product product = _shopManager.RegisterProduct("1");
            shop.AddProducts(product, 100, firstNumber);
            shop.AddProducts(product, 100, extraNumber);
            Assert.AreEqual(firstNumber + extraNumber, shop.Products[product].Count);
        }
    }
}