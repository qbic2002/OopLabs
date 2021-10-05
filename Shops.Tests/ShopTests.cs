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
            Assert.AreEqual(firstPrice, shop.GetPrice(new ProductSet(product)));
            shop.ChangePrice(product, newPrice);
            Assert.AreEqual(newPrice, shop.GetPrice(new ProductSet(product)));
        }

        [TestCase(1000,10,5,3)]
        [TestCase(1000,20,7,7)]
        public void PersonBuyProducts_MoneyReducedAndProductsRemoved(decimal firstMoney, decimal productPrice, int productsCount, int productsToBuyCount)
        {
            Shop shop = _shopManager.AddShop("Shop", "Address");
            Product product = _shopManager.RegisterProduct("test");
            Product anotherProduct = _shopManager.RegisterProduct("anotherTest");
            var person = new Person(firstMoney);

            shop.AddProducts(product, productPrice, productsCount);
            shop.AddProducts(anotherProduct, productPrice, productsCount);
            shop.Buy(person, new ProductSet(product, productsToBuyCount), new ProductSet(anotherProduct, productsToBuyCount));

            Assert.AreEqual(firstMoney - 2 * (productPrice * productsToBuyCount), person.Money);
            if (productsCount == productsToBuyCount)
            {
                if (shop.Products.ContainsKey(product) || shop.Products.ContainsKey(anotherProduct))
                    Assert.Fail();
            }

            if (productsCount > productsToBuyCount)
            {
                Assert.AreEqual(productsCount - productsToBuyCount, shop.Products[product].Count);
                Assert.AreEqual(productsCount - productsToBuyCount, shop.Products[anotherProduct].Count);
            }
        }

        [TestCase(100,100.5,1,1,1)]
        [TestCase(100, 30, 1,4,1)]
        [TestCase(199, 50, 50, 2,2)]
        public void PersonBuyProduct_PersonDoNotHasEnoughMoney_ThrowException(decimal money, decimal firstPrice, decimal secondPrice, int firstCount, int secondCount)
        {
            Shop shop = _shopManager.AddShop("Shop", "Address");
            Product product = _shopManager.RegisterProduct("test");
            Product anotherProduct = _shopManager.RegisterProduct("anotherTest");
            var person = new Person(money);

            shop.AddProducts(product, firstPrice, firstCount);
            shop.AddProducts(anotherProduct, secondPrice, secondCount);

            Assert.Catch<ShopException>(() =>
            {
                shop.Buy(person, new ProductSet(product, firstCount), new ProductSet(anotherProduct, secondCount));
            });
        }
        
        [TestCase(100,1,5,10, 1)]
        [TestCase(100,1,5,6, 6)]
        [TestCase(100, 1, 5, 5, 7)]
        [TestCase(100, 1, 5, 3, 7)]
        public void PersonBuyProduct_ShopDoNotHasEnoughProducts_ThrowException(decimal money, decimal price, int countProducts, int countFirstProductsToBuy, int countSecondProductsToBuy)
        {
            Shop shop = _shopManager.AddShop("Shop", "Address");
            Product product = _shopManager.RegisterProduct("test");
            Product anotherProduct = _shopManager.RegisterProduct("anotherTest");
            var person = new Person(money);

            shop.AddProducts(product, price, countProducts);
            shop.AddProducts(anotherProduct, price, countProducts);
            Assert.Catch<ShopException>(() =>
            {
                shop.Buy(person, new ProductSet(product, countFirstProductsToBuy), new ProductSet(anotherProduct, countSecondProductsToBuy));
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
                shop.Buy(person, new ProductSet(extraProduct));
            });
        }

        [Test]
        public void FindShopWithLowestPrice()
        {
            Shop shop1 = _shopManager.AddShop("test1", "street1");
            Shop shop2 = _shopManager.AddShop("test2", "street2");
            Shop shop3 = _shopManager.AddShop("test3", "street3");

            Product product1 = _shopManager.RegisterProduct("first");
            Product product2 = _shopManager.RegisterProduct("second");
            Product product3 = _shopManager.RegisterProduct("third");
            
            shop1.AddProducts(product1, 100, 100);
            shop1.AddProducts(product2, 10, 100);
            shop1.AddProducts(product3, 99, 10);
            
            shop2.AddProducts(product1, 10, 100);
            shop2.AddProducts(product2, 100, 100);
            shop2.AddProducts(product3, 100, 11);
            
            shop3.AddProducts(product1, 100, 10);
            shop3.AddProducts(product2, 10, 100);
            shop3.AddProducts(product3, 100, 100);

            var set1 = new List<ProductSet>
            {
                new ProductSet(product1, 100),
                new ProductSet(product2, 100),
                new ProductSet(product3, 10)
            };

            var set2 = new List<ProductSet>
            {
                new ProductSet(product1, 100),
                new ProductSet(product2, 100),
                new ProductSet(product3, 11)
            };

            var set3 = new List<ProductSet>
            {
                new ProductSet(product1, 10),
                new ProductSet(product2, 100),
                new ProductSet(product3, 100)
            };

            Assert.AreEqual(shop1, _shopManager.LowestPrice(set1.ToArray()));
            Assert.AreEqual(shop2, _shopManager.LowestPrice(set2.ToArray()));
            Assert.AreEqual(shop3, _shopManager.LowestPrice(set3.ToArray()));
        }
    }
}