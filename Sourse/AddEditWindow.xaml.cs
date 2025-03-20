using System.Collections.Generic;
using System.Windows;

namespace InventoryApp
{
    public partial class AddEditWindow : Window
    {
        public Product Product { get; private set; }
        public List<int> ids = new List<int> { }; // все ID товаров

        public AddEditWindow(List<int> Ids) // Конструктор для добавления товара
        {
            InitializeComponent();
            Product = new Product(0, "", 0, 0);
            DataContext = Product;
            ids = Ids;
            Id_TextBox.IsReadOnly = false;
        }

        public AddEditWindow(Product product) // Конструктор для изменения товара
        {
            InitializeComponent();
            Product = product;
            DataContext = Product;
            ids.Clear();
            Id_TextBox.IsReadOnly = true;
        }

        // Обработчик кнопки "Сохранить"
        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (ids.Contains(Product.Id))
            {
                MessageBox.Show("Id уже занят.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (Product.Id < 1)
            {
                MessageBox.Show("Id не может быть меньше единицы.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (string.IsNullOrWhiteSpace(Product.Name))
            {
                MessageBox.Show("Название товара не может быть пустым.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (Product.Quantity < 0)
            {
                MessageBox.Show("Количество товара не может быть отрицательным.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (Product.Price < 0)
            {
                MessageBox.Show("Цена товара не может быть отрицательной.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            DialogResult = true; 
            Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false; 
            Close();
        }
    }
}
