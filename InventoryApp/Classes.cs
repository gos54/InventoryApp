using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows.Input;
using Newtonsoft.Json.Linq;

namespace InventoryApp
{
    public class Product : INotifyPropertyChanged
    {
        private Dictionary<string, string> properties;

        public Dictionary<string, string> Properties
        {
            get { return properties; }
            set
            {
                properties = value;
                OnPropertyChanged(nameof(Properties));
            }
        }

        public Product()
        {
            Properties = new Dictionary<string, string>();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class InventoryRepository
    {
        private List<Product> products = new List<Product>();
        private List<string> headers;

        public void AddProduct(Product product) 
        {
            products.Add(product);
        }

        public void RemoveProduct(Product product)
        {
            products.Remove(product);
        }

        public List<Product> SearchProduct(string name) 
        { 
            return new List<Product> { }; 
        }

        public List<Product> GetAllProducts() { return products; }
        public List<string> GetAllHeaders() 
        {
            return headers;
        }

        public List<string> GetAllHeader(string header) // Функция для получение значений определённого столбца
        {
            try
            {
                var data = products.Select(item => item.Properties).ToList();
                return data.Select(d => d.TryGetValue(header, out string value) ? value : "").ToList();
            }
            catch { return new List<string>(); }
        }

        public void ImportFromJson(string filePath)
        {
            string json = File.ReadAllText(filePath);
            products = new List<Product>();

            // Парсинг JSON
            var jsonArray = JArray.Parse(json);

            foreach (var item in jsonArray)
            {
                var dict = new Dictionary<string, string>();
                foreach (var property in ((JObject)item).Properties())
                {
                    // Преобразуем значение в строку
                    dict[property.Name] = property.Value.ToString();
                }
                products.Add(new Product { Properties = dict });
            }
            headers = products.SelectMany(dict => dict.Properties.Keys).Distinct().ToList();
        }

        public void ImportFromCsv(string filePath)
        {
            products = new List<Product>();
            var lines = File.ReadAllLines(filePath);
            headers = lines[0].Split(',').ToList();
            for (int i = 1; i < lines.Length; i++)
            {
                var values = lines[i].Split(',');
                var dict = new Dictionary<string, string>();

                // Сопоставляем заголовки со значениями
                for (int j = 0; j < headers.Count && j < values.Length; j++)
                {
                    string value = j < values.Length ? values[j].Trim() : "";
                    dict[headers[j].Trim()] = value;
                }

                products.Add(new Product { Properties = dict });
            }
        }
    }
    public class RelayCommand<T> : ICommand
    {
        private readonly Action<T> _execute;
        private readonly Func<T, bool> _canExecute;

        public RelayCommand(Action<T> execute, Func<T, bool> canExecute = null)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute;
        }

        public bool CanExecute(object parameter) => _canExecute == null || _canExecute((T)parameter);

        public void Execute(object parameter) => _execute((T)parameter);

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }
    }
}