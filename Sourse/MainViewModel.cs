using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;

namespace InventoryApp
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private InventoryRepository repository = new InventoryRepository();
        public ObservableCollection<Product> products; // Список продуктов
        private ObservableCollection<Product> filtered_products; // Список отсортированных продуктов
        private string search_text; // Поле для сохранения введённого текста в поиске
        private Product selected_product; // Поля для сохранения данных из выбранной ячейки таблицы

        public ObservableCollection<Product> Products
        {
            get => products;
            set
            {
                products = value;
                OnPropertyChanged(nameof(Products));
            }
        }
        public ObservableCollection<Product> FilteredProducts
        {
            get => filtered_products;
            set
            {
                filtered_products = value;
                OnPropertyChanged(nameof(FilteredProducts));
            }
        }

        public string SearchText
        {
            get => search_text;
            set
            {
                search_text = value;
                SearchProducts();
                OnPropertyChanged(nameof(SearchText));
            }
        }

        public Product SelectedProduct
        {
            get => selected_product;
            set
            {
                selected_product = value;
                OnPropertyChanged(nameof(SelectedProduct));
            }
        }

        public MainViewModel()
        {
            Products = new ObservableCollection<Product>(repository.GetAllProducts()); 
            FilteredProducts = new ObservableCollection<Product>(products);
        }

        // Поиск товара
        public void SearchProducts()
        {
            if (string.IsNullOrWhiteSpace(SearchText))
            {
                FilteredProducts = new ObservableCollection<Product>(products);
            }
            else
            {
                var filtered = products.Where(p => p.Name.Contains(SearchText)).ToList();
                FilteredProducts = new ObservableCollection<Product>(filtered);
            }
        }

        // Добавление товара
        public void AddProduct(Product product)
        {
            repository.AddProduct(product);
            Products = new ObservableCollection<Product>(repository.GetAllProducts());
            FilteredProducts = new ObservableCollection<Product>(products);
            SearchProducts();
        }

        // Удаление товара
        public void RemoveProduct(int id)
        {
            var product = products.FirstOrDefault(p => p.Id == id);
            if (product != null)
            {
                products.Remove(product);
                OnPropertyChanged(nameof(Products));
            }
            FilteredProducts = new ObservableCollection<Product>(products);
            SearchProducts();
        }

        // Обновление товара
        public void UpdateProduct(Product updated_product)
        {
            var product = products.FirstOrDefault(p => p.Id == updated_product.Id);
            if (product != null)
            {
                product.Name = updated_product.Name;
                product.Quantity = updated_product.Quantity;
                product.Price = updated_product.Price;
                OnPropertyChanged(nameof(Products)); 
            }
            FilteredProducts = new ObservableCollection<Product>(products);
            SearchProducts();
        }

        // Экспорт данных в CSV
        public void ExportToCsv(string file_path)
        {
            using (var writer = new StreamWriter(file_path))
            {
                writer.WriteLine("ID,Name,Quantity,Price");
                foreach (var product in products)
                {
                    writer.WriteLine($"{product.Id},{product.Name},{product.Quantity},{product.Price}");
                }
            }
        }

        // Импорт данных из CSV
        public void ImportFromCsv(string file_path)
        {
            repository.ImportFromCsv(file_path);
            Products = new ObservableCollection<Product>(repository.GetAllProducts());
            FilteredProducts = new ObservableCollection<Product>(products);
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
