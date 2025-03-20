using Microsoft.Win32;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

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
            view_model.ExportToCsv(file_path);
            MessageBox.Show("Данные успешно сохранены!", "Сохранение завершено", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void ExportToCsv_Click(object sender, RoutedEventArgs e)
        {
            // Диалог для выбора места сохранения файла
            SaveFileDialog save_file_dialog = new SaveFileDialog
            {
                Filter = "All files (*.*)|*.*",
                DefaultExt = ".csv",
                FileName = "inventory_export.csv"
            };

            if (save_file_dialog.ShowDialog() == true)
            {
                view_model.ExportToCsv(save_file_dialog.FileName);
                MessageBox.Show("Данные успешно экспортированы в CSV!", "Экспорт завершен", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void ImportFromCsv_Click(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new OpenFileDialog
            {
                Filter = "CSV files (*.csv)|*.csv|All files (*.*)|*.*",
                DefaultExt = ".csv"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                file_path = openFileDialog.FileName;
                try
                {
                    view_model.ImportFromCsv(file_path);
                    MessageBox.Show("Данные успешно импортированы из CSV!", "Импорт завершен", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch { MessageBox.Show("Данные csv файл не доступен к чтению.", "Импорт завершен", MessageBoxButton.OK, MessageBoxImage.Error); }
            }
        }
        private void AddProduct_Click(object sender, RoutedEventArgs e)
        {
            var window = new AddEditWindow(view_model.products.Select(p => p.Id).ToList());
            if (window.ShowDialog() == true) { view_model.AddProduct(window.Product); }
        }

        private void EditProduct_Click(object sender, RoutedEventArgs e)
        {
            if (view_model.SelectedProduct != null)
            {
                var window = new AddEditWindow(view_model.SelectedProduct);
                if (window.ShowDialog() == true) { view_model.UpdateProduct(window.Product); }
            }
            else {  MessageBox.Show("Пожалуйста выберите товар для изменения", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning); }
        }
        private void RemoveProduct_Click(object sender, RoutedEventArgs e)
        {
            if (view_model.SelectedProduct != null)  {  view_model.RemoveProduct(view_model.SelectedProduct.Id); }
            else {  MessageBox.Show("Выберите товар для удаления.", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning); }
        }
    }
}
