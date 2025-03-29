using Newtonsoft.Json;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows.Data;
using System.Windows.Input;

namespace InventoryApp
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private InventoryRepository repository = new InventoryRepository();
        private ObservableCollection<Product> filtered_products; // Список отсортированных продуктов
        public List<string> headers = new List<string>(); // Заголовки таблицы
        private string search_text; // Поле для сохранения введённого текста в поиске
        public Product selected_product; // Поля для сохранения данных из выбранной ячейки таблицы
        public string selected_header; // Выбраный заголовок таблицы для поиска
        public ICommand SortCommand { get; }

        public ObservableCollection<Product> FilteredProducts
        {
            get => filtered_products;
            set
            {
                filtered_products = value;
                OnPropertyChanged(nameof(FilteredProducts));
            }
        }

        public string SelectedHeader
        {
            get => selected_header;
            set
            {
                selected_header = value;
                OnPropertyChanged(nameof(SearchText));
            }
        }
        public string SearchText
        {
            get => search_text;
            set
            {
                search_text = value;
                OnPropertyChanged(nameof(SearchText));
                SearchProducts();
            }
        }

        public List<string> Headers
        {
            get => headers;
            set
            {
                headers = value;
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
            FilteredProducts = new ObservableCollection<Product>(repository.GetAllProducts());
            SortCommand = new RelayCommand<string>(Sort);
        }

        // Поиск товара
        public void SearchProducts()
        {
            if (string.IsNullOrWhiteSpace(search_text))
            {
                FilteredProducts = new ObservableCollection<Product>(repository.GetAllProducts());
            }
            else
            {
                filtered_products.Clear();
                foreach (var product in repository.GetAllProducts())
                {
                    try
                    {
                        if (product.Properties[selected_header].Contains(search_text))
                        {
                            filtered_products.Add(product);
                        }
                    }
                    catch { }
                }
            }
        }

        private void Sort(string propertyName)
        {
            var view = CollectionViewSource.GetDefaultView(filtered_products);
            view.SortDescriptions.Clear();
            view.SortDescriptions.Add(new SortDescription(propertyName, ListSortDirection.Ascending));
        }

        public List<string> GetAllIds()
        {
            return repository.GetAllHeader("Id");
        }
        public List<string> GetAllCatygories()
        {
            return repository.GetAllHeader("Category").Distinct().ToList();
        }

        // Добавление товара
        public void AddProduct(Product product)
        {
            repository.AddProduct(product);
            SearchProducts();
        }

        // Удаление товара
        public void RemoveProduct()
        {
            repository.RemoveProduct(selected_product);
            SearchProducts();
        }

        // Обновление товара
        public void UpdateProduct(Product updated_product)
        {
            var existingProduct = repository.GetAllProducts().FirstOrDefault(p => p.Properties == selected_product.Properties);
            if (existingProduct != null)
            {
                existingProduct.Properties = updated_product.Properties;
            }
            SearchProducts();
        }

        // Экспорт данных в json
        public void ExportToJson(string file_path)
        {
            var data = repository.GetAllProducts().Select(item => item.Properties).ToList();
            string json = JsonConvert.SerializeObject(data, Formatting.Indented);
            File.WriteAllText(file_path, json);
        }

        // Экспорт данных в csv
        public void ExportToCsv(string filePath)
        {
            var data = repository.GetAllProducts().Select(item => item.Properties).ToList();

            using (var writer = new StreamWriter(filePath))
            {
                // Записываем заголовки
                writer.WriteLine(string.Join(",", headers));

                // Записываем данные
                foreach (var dict in data)
                {
                    var line = new List<string>();
                    foreach (var header in headers)
                    {
                        dict.TryGetValue(header, out string value);
                        line.Add(EscapeCsvValue(value ?? ""));
                    }
                    writer.WriteLine(string.Join(",", line));
                }
            }
        }

        private static string EscapeCsvValue(string value) // функция для обработки пустых полей в словаре
        {
            if (value.Contains(",") || value.Contains("\"") || value.Contains("\n")) // проверка на спец знаки
            { return $"\"{value.Replace("\"", "\"\"")}\""; } // экранирование спец знаков
            return value;
        }

        // Импорт данных из json
        public void ImportFromJson(string file_path)
        {
            repository.ImportFromJson(file_path);
            headers = repository.GetAllHeaders();
            FilteredProducts = new ObservableCollection<Product>(repository.GetAllProducts());
        }
        // Импорт данных из csv
        public void ImportFromCsv(string file_path)
        {
            repository.ImportFromCsv(file_path);
            headers = repository.GetAllHeaders();
            FilteredProducts = new ObservableCollection<Product>(repository.GetAllProducts());
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}