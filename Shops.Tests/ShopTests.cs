using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Shops.Entities;
using Shops.Services;
using Shops.Tools;

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
        public void AddNewProductsToShop_ShopContainsThem()
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

        [TestCase(-1)]
        [TestCase(0)]
        [TestCase(-0.5)]
        [TestCase(-100)]
        public void AddProductWithNegativePrice_ThrowException(decimal price)
        {
            Shop shop = _shopManager.AddShop("Shop", "Address");
            Product product = _shopManager.RegisterProduct("1");
            
            Assert.Catch<ShopException>(() =>
            {
                shop.AddProducts(product, price, 1);
            });
        }

        [TestCase(-1)]
        [TestCase(0)]
        [TestCase(-100)]
        public void AddNegativeNumberOfProducts_ThrowException(int count)
        {
            Shop shop = _shopManager.AddShop("Shop", "Address");
            Product product = _shopManager.RegisterProduct("1");
            
            Assert.Catch<ShopException>(() =>
            {
                shop.AddProducts(product, 10, count);
            });
        }
        
        [TestCase(10,20)]
        [TestCase(10,10)]
        [TestCase(20, 10)]
        public void ChangePrice_PriceChanged(decimal firstPrice, decimal newPrice)
        {
            Shop shop = _shopManager.AddShop("Shop", "Address");
            Product product = _shopManager.RegisterProduct("test");
            shop.AddProducts(product, firstPrice,1);
            Assert.AreEqual(firstPrice, shop.Products.GetPrice(product));
            shop.ChangePrice(product, newPrice);
            Assert.AreEqual(newPrice, shop.Products.GetPrice(product));
        }

        [TestCase(1000,10,5,3)]
        [TestCase(1000,20,7,7)]
        public void PersonBuyProducts_MoneyReducedAndProductsRemoved(decimal firstMoney, decimal productPrice, int productsCount, int productsToBuyCount)
        {
            Shop shop = _shopManager.AddShop("Shop", "Address");
            Product product = _shopManager.RegisterProduct("test");
            var person = new Person(firstMoney);

            shop.AddProducts(product, productPrice, productsCount);
            shop.Buy(person, product, productsToBuyCount);

            Assert.AreEqual(firstMoney - (productPrice * productsToBuyCount), person.Money);
            if (productsCount == productsToBuyCount)
                if (shop.Products.ContainsKey(product))
                    Assert.Fail();
            if (productsCount > productsToBuyCount)
                Assert.AreEqual(productsCount - productsToBuyCount, shop.Products[product].Count);
        }

        [TestCase(100,100.5,1)]
        [TestCase(100, 30, 4)]
        public void PersonBuyProduct_PersonDoNotHasEnoughMoney_ThrowException(decimal money, decimal price, int count)
        {
            Shop shop = _shopManager.AddShop("Shop", "Address");
            Product product = _shopManager.RegisterProduct("test");
            var person = new Person(money);

            shop.AddProducts(product, price, count);
            Assert.Catch<ShopException>(() =>
            {
                shop.Buy(person, product, count);
            });
        }
        
        [TestCase(100,1,5,10)]
        [TestCase(100,5,5,6)]
        public void PersonBuyProduct_ShopDoNotHasEnoughProducts_ThrowException(decimal money, decimal price, int countProducts, int countProductsToBuy)
        {
            Shop shop = _shopManager.AddShop("Shop", "Address");
            Product product = _shopManager.RegisterProduct("test");
            var person = new Person(money);

            shop.AddProducts(product, price, countProducts);
            Assert.Catch<ShopException>(() =>
            {
                shop.Buy(person, product, countProductsToBuy);
            });
        }

        [Test]
        public void PersonBuyProduct_ShopDoNotHasThisProduct_ThrowException()
        {
            Shop shop = _shopManager.AddShop("Shop", "Address");
            Product product = _shopManager.RegisterProduct("test");
            Product extraProduct = _shopManager.RegisterProduct("extra");
            var person = new Person(100);

            shop.AddProducts(product, 1, 1);
            Assert.Catch<ShopException>(() =>
            {
                shop.Buy(person, extraProduct, 1);
            });
        }

        [Test]
        public void FindShopWithLowestPrice()
        {
            Product product = _shopManager.RegisterProduct("test");
            for (int i = 1; i <= 10; i++)
            {
                _shopManager.AddShop(i.ToString(), "Address").AddProducts(product, i, i);
            }

            Shop shop = _shopManager.AddShop("lowestPrice", "Adress");
            shop.AddProducts(product, 4, 5);
            Assert.AreEqual(shop, _shopManager.LowestPrice(product, 5));
        }
    }
}