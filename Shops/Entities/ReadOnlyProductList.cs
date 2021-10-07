using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Shops.Tools;

namespace Shops.Entities
{
    public class ReadOnlyProductList
    {
        private ReadOnlyDictionary<Product, ProductInfo> _products;
        public ReadOnlyProductList(Dictionary<Product, ProductInfo> products)
        {
            _products = new ReadOnlyDictionary<Product, ProductInfo>(products);
        }

        public ICollection Keys => _products.Keys;
        public ProductInfo this[Product product] => _products[product];

        public bool ContainsKey(Product product) => _products.ContainsKey(product);
    }
}