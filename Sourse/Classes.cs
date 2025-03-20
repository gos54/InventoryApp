using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

namespace InventoryApp
{
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; } 
        public int Quantity { get; set; }
        public decimal Price { get; set; } 

        public Product(int id, string name, int quantity, decimal price)
        {
            Id = id;
            Name = name;
            Quantity = quantity;
            Price = price;
        }
    }
    public class InventoryRepository
    {
        private List<Product> products = new List<Product>();

        public void AddProduct(Product product) { products.Add(product); }

        public void RemoveProduct(int id)
        {
            var product = products.FirstOrDefault(p => p.Id == id);
            if (product != null)
            {
                products.Remove(product);
            }
        }

        public List<Product> SearchProduct(string name) { return products.Where(p => p.Name.Contains(name)).ToList(); }

        public List<Product> GetAllProducts() { return products; }

        public void ImportFromCsv(string filePath)
        {
            var result = new List<Product>();
            int num = 0;
            try
            {
                using (var reader = new StreamReader(filePath))
                {
                    string[] headers = null;

                    while (!reader.EndOfStream)
                    {
                        var line = reader.ReadLine();

                        if (headers == null) { headers = line.Split(','); } // Получение заголовков
                        else
                        {
                            var values = line.Split(',');
                            num += 1;
                            if (values.Length == headers.Length)
                            {
                                var product = new Product(num, $"Новый товар {num}", 0, 0); // Базовый товар

                                for (int i = 0; i < headers.Length; i++)
                                {
                                    var header = headers[i];
                                    var value = values[i];

                                    switch (header) // Добавление информации в товар в определённое поле
                                    {
                                        case "Id":
                                            if (int.TryParse(value, out int id))
                                                product.Id = id;
                                            break;
                                        case "Name":
                                            product.Name = value;
                                            break;
                                        case "Quantity":
                                            if (int.TryParse(value, out int quantity))
                                                product.Quantity = quantity;
                                            break;
                                        case "Price":
                                            if (decimal.TryParse(value, out decimal price))
                                                product.Price = price;
                                            break;
                                    }
                                }

                                result.Add(product);
                            }
                        }
                    }
                    products = result;
                }
            }
            catch { }
            if (products.Count == 0) throw new InvalidOperationException("Csv file can`t read");
        }
    }
}
