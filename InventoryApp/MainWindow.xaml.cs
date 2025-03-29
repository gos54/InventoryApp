using Microsoft.Win32;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace InventoryApp
{
    public partial class MainWindow : Window
    {
        private MainViewModel view_model;
        private string file_path = "";

        public MainWindow()
        {
            InitializeComponent();

            view_model = new MainViewModel();
            DataContext = view_model;
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            if (!(string.IsNullOrWhiteSpace(file_path) || string.IsNullOrEmpty(file_path)))
            {
                if (file_path.EndsWith(".json"))
                {
                    view_model.ExportToJson(file_path);
                    MessageBox.Show($"Данные экспортированы в файл: {file_path}", "Экспорт завершен", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                if (file_path.EndsWith(".csv"))
                {
                    view_model.ExportToCsv(file_path);
                    MessageBox.Show($"Данные экспортированы в файл: {file_path}", "Экспорт завершен", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
        }

        private void ExportToJson_Click(object sender, RoutedEventArgs e)
        {
            var saveFileDialog = new Microsoft.Win32.SaveFileDialog
            {
                Filter = "Json files (*.json)|*.json|All files (*.*)|*.*",
                DefaultExt = ".json",
                FileName = "exported_data.json"
            };

            if (saveFileDialog.ShowDialog() == true)
            {
                string file_path = saveFileDialog.FileName;
                view_model.ExportToJson(file_path);

                MessageBox.Show($"Данные экспортированы в файл: {file_path}", "Экспорт завершен", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void ExportToCsv_Click(object sender, RoutedEventArgs e)
        {
            var saveFileDialog = new Microsoft.Win32.SaveFileDialog
            {
                Filter = "Csv files (*.csv)|*.csv|All files (*.*)|*.*",
                DefaultExt = ".csv",
                FileName = "exported_data.csv"
            };

            if (saveFileDialog.ShowDialog() == true)
            {
                string file_path = saveFileDialog.FileName;
                view_model.ExportToCsv(file_path);

                MessageBox.Show($"Данные экспортированы в файл: {file_path}", "Экспорт завершен", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void ImportFromJson_Click(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new OpenFileDialog
            {
                Filter = "Json files (*.json)|*.json",
                DefaultExt = ".json"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                this.file_path = openFileDialog.FileName;
                view_model.ImportFromJson(file_path);

                // Очищаем старые колонки
                GridView.Columns.Clear();

                foreach (var key in view_model.headers)
                {
                    try
                    {
                        // Создание колонки
                        var column = new GridViewColumn
                        {
                            Header = key,
                            DisplayMemberBinding = new Binding($"Properties[{key}]") // Привязка к ключу словаря
                        };
                        GridView.Columns.Add(column);
                    }
                    catch { }
                }
                choose_comboBox.ItemsSource = view_model.headers;
                choose_comboBox.SelectedIndex = 0;
                MessageBox.Show($"Данные импортированны из файла: {file_path}", "Импорт завершен", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void ImportFromCSV_Click(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new OpenFileDialog
            {
                Filter = "Csv files (*.csv)|*.csv",
                DefaultExt = ".csv"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                this.file_path = openFileDialog.FileName;
                view_model.ImportFromCsv(file_path);

                // Очищаем старые колонки
                GridView.Columns.Clear();

                foreach (var key in view_model.headers)
                {
                    try
                    {
                        // Создание колонки
                        var column = new GridViewColumn
                        {
                            Header = key,
                            DisplayMemberBinding = new Binding($"Properties[{key}]") // Привязка к ключу словаря
                        };
                        GridView.Columns.Add(column);
                    }
                    catch { }
                }
                choose_comboBox.ItemsSource = view_model.headers;
                choose_comboBox.SelectedIndex = 0;
                MessageBox.Show($"Данные импортированны из файла: {file_path}", "Импорт завершен", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void AddProduct_Click(object sender, RoutedEventArgs e)
        {
            if (view_model.headers.Count > 0) 
            {
                var addWindow = new AddEditWindow(view_model.headers, view_model.GetAllIds(), view_model.GetAllCatygories());
                if (addWindow.ShowDialog() == true)
                {
                    // Добавление нового элемента
                    var newItem = new Product { Properties = addWindow.Properties };
                    view_model.AddProduct(newItem);
                }
            }
            else
            {
                MessageBox.Show($"Импортируйте сначала json или csv файл", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void EditProduct_Click(object sender, RoutedEventArgs e)
        {
            if (view_model.headers.Count > 0)
            {
                try
                {
                    var editWindow = new AddEditWindow(view_model.selected_product.Properties, view_model.headers, view_model.GetAllIds(), view_model.GetAllCatygories());
                    if (editWindow.ShowDialog() == true)
                    {
                        // Добавление нового элемента
                        var item = new Product { Properties = editWindow.Properties };
                        view_model.UpdateProduct(item);
                    }
                }
                catch
                {
                    MessageBox.Show($"Выберите строку в таблице", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                MessageBox.Show($"Импортируйте сначала json или csv файл", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void RemoveProduct_Click(object sender, RoutedEventArgs e)
        {
            if (view_model.headers.Count > 0)
            {
                if (view_model.selected_product != null)
                {
                    view_model.RemoveProduct();
                }
                else
                {
                    MessageBox.Show($"Выберите строку в таблице", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                MessageBox.Show($"Импортируйте сначала json или csv файл", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void HelpWindow_Click(object sender, RoutedEventArgs e)
        {
            var helpWindow = new HelpWindow();
            helpWindow.ShowDialog();
        }
    }
}