using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace InventoryApp
{
    public partial class AddEditWindow : Window
    {
        public Dictionary<string, string> Properties { get; private set; } // Словарь для хранения введенных данных
        public List<string> headers { get; private set; }
        public List<string> ids { get; private set; }
        public List<string> categories { get; private set; }

        // Конструктор для добавления нового элемента
        public AddEditWindow(List<string> head, List<string> ids, List<string> categories) // заголовки, Id, категории находящиеся в таблице
        {
            InitializeComponent();
            Properties = new Dictionary<string, string>();
            headers = head;
            this.ids = ids;
            this.categories = categories;
            CreateFields();
        }

        // Конструктор для редактирования существующего элемента
        public AddEditWindow(Dictionary<string, string> existing_properties, List<string> head, List<string> ids, List<string> categories) 
            // существующий товар, заголовки, Id, категории находящиеся в таблице
        {
            InitializeComponent();
            Properties = new Dictionary<string, string>(existing_properties);
            headers = head;
            this.ids = ids;
            if (existing_properties.Keys.Contains("Id")) { ids.Remove(existing_properties["Id"]); }
            this.categories = categories;
            CreateFields();
        }

        // Создание полей для ввода данных
        private void CreateFields()
        {
            int row = 0; // Индекс строки в Grid

            foreach (var key in headers)
            {
                // Добавляем новую строку в Grid
                FieldsGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });

                if (key != "Category")
                {
                    // TextBlock
                    var textBlock = new TextBlock
                    {
                        Text = key + ":",
                        Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#D1D090")),
                        VerticalAlignment = VerticalAlignment.Center,
                        Margin = new Thickness(5)
                    };
                    Grid.SetRow(textBlock, row);
                    Grid.SetColumn(textBlock, 0);
                    FieldsGrid.Children.Add(textBlock);

                    // TextBox
                    var textBox = new TextBox
                    {
                        VerticalAlignment = VerticalAlignment.Center,
                        Margin = new Thickness(5),
                        Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FBB672")),
                        Tag = key // Сохраняем ключ для идентификации TextBox
                    };
                    if (Properties.ContainsKey(key))
                    {
                        textBox.Text = Properties[key]; // Заполнение поля, если данные уже есть
                    }
                    Grid.SetRow(textBox, row);
                    Grid.SetColumn(textBox, 1);
                    FieldsGrid.Children.Add(textBox);

                    row++; // Переход к следующей строке
                }
                else
                {
                    // TextBlock
                    var textBlock = new TextBlock
                    {
                        Text = key + ":",
                        Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#D1D090")),
                        VerticalAlignment = VerticalAlignment.Center,
                        Margin = new Thickness(5)
                    };
                    Grid.SetRow(textBlock, row);
                    Grid.SetColumn(textBlock, 0);
                    FieldsGrid.Children.Add(textBlock);

                    // ComboBox
                    var comboBox = new ComboBox
                    {
                        VerticalAlignment = VerticalAlignment.Center,
                        Margin = new Thickness(5),
                        IsEditable = true, // Для произвольного текста
                        IsTextSearchEnabled = true, // автодополнение
                        ItemsSource = categories,
                        Tag = key // Сохраняем ключ для идентификации ComboBox
                    };
                    if (Properties.ContainsKey(key))
                    {
                        comboBox.Text = Properties[key]; // Заполнение поля, если данные уже есть
                    }
                    Grid.SetRow(comboBox, row);
                    Grid.SetColumn(comboBox, 1);
                    FieldsGrid.Children.Add(comboBox);

                    row++; // Переход к следующей строке
                }
            }
        }

        // Обработчик кнопки "Сохранить"
        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            // Сбор данных из полей
            foreach (var child in FieldsGrid.Children)
            {
                if (child is TextBox textBox)
                {
                    var key = textBox.Tag as string;
                    if (key == "Quantity" && !int.TryParse(textBox.Text, out _))
                    {
                        MessageBox.Show("Поле 'Quantity' должно быть числом.");
                        return;
                    }

                    if (key == "Quantity" && int.TryParse(textBox.Text, out _) && textBox.Text.StartsWith("-"))
                    {
                        MessageBox.Show("Поле 'Quantity' должно быть больше или равно нулю.");
                        return;
                    }

                    if (key == "Price" && !decimal.TryParse(textBox.Text, out _))
                    {
                        MessageBox.Show("Поле 'Price' должно быть числом.");
                        return;
                    }

                    if (key == "Price" && decimal.TryParse(textBox.Text, out _) && 
                        (textBox.Text.StartsWith("-") || (textBox.Text.StartsWith("0") && !textBox.Text.StartsWith("0,"))))
                    {
                        MessageBox.Show("Поле 'Price' должно быть больше нуля.");
                        return;
                    }

                    if (key == "Id" && ids.Contains(textBox.Text))
                    {
                        MessageBox.Show("Поле 'Id' должно быть уникальным.");
                        return;
                    }

                    Properties[key] = textBox.Text;
                }
                if (child is ComboBox comboBox)
                {
                    var key = comboBox.Tag as string;
                    Properties[key] = comboBox.Text;
                }
            }

            DialogResult = true;
            Close();
        }

        // Обработчик кнопки "Отмена"
        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}