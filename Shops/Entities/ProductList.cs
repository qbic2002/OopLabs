using System.Collections.Generic;
using System.Linq;
using Shops.Tools;

namespace Shops.Entities
{
    public class ProductList
    {
        public ProductList(Dictionary<Product, ProductInfo> products)
        {
            Products = products ?? throw new ShopException("Incorrect products");
        }

        public Dictionary<Product, ProductInfo> Products { get; }
        public ProductInfo this[Product product] => Products[product];
        public bool IsContains(params ProductSet[] productSets)
        {
            if (productSets is null || productSets.Length == 0)
                throw new ShopException("Incorrect set of products");
            var productSetsList = productSets.ToList();
            bool isContains = true;
            productSetsList.ForEach(productSet =>
            {
                if (!Products.ContainsKey(productSet.Product))
                    isContains = false;
                else if (Products[productSet.Product].Count < productSet.Count)
                    isContains = false;
            });
            return isContains;
        }

        public void Buy(params ProductSet[] productSets)
        {
            if (productSets is null || productSets.Length == 0)
                throw new ShopException("Incorrect set of products");
            var productSetsList = new List<ProductSet>(productSets);
            if (!IsContains(productSets))
                throw new ShopException("Can't find products");
            productSetsList.ForEach(productSet =>
            {
                if (Products[productSet.Product].Count == productSet.Count)
                {
                    Products.Remove(productSet.Product);
                    return;
                }

                if (Products[productSet.Product].Count > productSet.Count)
                {
                    Products[productSet.Product].RemoveCount(productSet.Count);
                }
            });
        }

        public decimal GetPrice(params ProductSet[] productSets)
        {
            if (productSets is null || productSets.Length == 0)
                throw new ShopException("Incorrect set of products");
            var productSetsList = new List<ProductSet>(productSets);
            if (!IsContains(productSets))
                throw new ShopException("Can't find products");
            decimal price = 0;
            productSetsList.ForEach(productSet => price += Products[productSet.Product].Price * productSet.Count);
            return price;
        }

        public void ChangePrice(Product product, decimal newPrice)
        {
            if (product is null)
                throw new ShopException("Incorrect product");
            if (newPrice <= 0)
                throw new ShopException("Incorrect price");
            if (!Products.ContainsKey(product))
                throw new ShopException("No product");
            Products[product].ChangePrice(newPrice);
        }

        public bool ContainsKey(Product product) => Products.ContainsKey(product);

        public void Add(Product product, ProductInfo productInfo) => Products.Add(product, productInfo);
    }
}