using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Project1
{
    public partial class MainWindow : Window
    {
        public class MenuItem
        {
            public string Name { get; set; } = string.Empty; // 初始化為空字串
            public double Price { get; set; }
        }

        public class ShoppingCartItem
        {
            public string Name { get; set; } = string.Empty; // 初始化為空字串
            public double Price { get; set; }
            public int Quantity { get; set; } = 1; // 默認數量為1
            public double SubTotal => Price * Quantity; // 小計
            public string Display => $"{Name} x{Quantity} - ${SubTotal}";
        }

        private List<MenuItem> menuItems = new List<MenuItem>(); // 初始化為空列表
        private List<ShoppingCartItem> cartItems = new List<ShoppingCartItem>();

        public MainWindow()
        {
            InitializeComponent();
            InitializeMenuItems();
            MenuItemsControl.ItemsSource = menuItems;
        }

        private void InitializeMenuItems()
        {
            menuItems = new List<MenuItem>
            {
                new MenuItem { Name = "經典漢堡", Price = 80 },
                new MenuItem { Name = "雙層芝士漢堡", Price = 100 },
                new MenuItem { Name = "薯條", Price = 40 },
                new MenuItem { Name = "可樂", Price = 30 },
                new MenuItem { Name = "冰沙", Price = 50 }
            };
            // 更新 ItemsSource
            MenuItemsControl.ItemsSource = menuItems;
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is MenuItem item)
            {
                // 獲取數量
                var parentStackPanel = FindParent<StackPanel>(btn);
                if (parentStackPanel != null)
                {
                    var quantityStackPanel = parentStackPanel.Children.OfType<StackPanel>().FirstOrDefault();
                    if (quantityStackPanel != null)
                    {
                        var quantityTextBox = quantityStackPanel.Children.OfType<TextBox>().FirstOrDefault();
                        if (quantityTextBox != null && int.TryParse(quantityTextBox.Text, out int quantity))
                        {
                            if (quantity <= 0)
                            {
                                MessageBox.Show("數量必須大於0。", "無效數量", MessageBoxButton.OK, MessageBoxImage.Warning);
                                return;
                            }

                            // 檢查購物車中是否已存在該項目
                            var existingItem = cartItems.FirstOrDefault(ci => ci.Name == item.Name);
                            if (existingItem != null)
                            {
                                existingItem.Quantity += quantity;
                            }
                            else
                            {
                                cartItems.Add(new ShoppingCartItem { Name = item.Name, Price = item.Price, Quantity = quantity });
                            }

                            UpdateCart();

                            // 重置數量選擇控制項的值為1
                            quantityTextBox.Text = "1";
                        }
                    }
                }
            }
        }

        private void UpdateCart()
        {
            CartListBox.ItemsSource = null;
            CartListBox.ItemsSource = cartItems;
            double total = cartItems.Sum(ci => ci.SubTotal);
            TotalText.Text = $"總計: ${total}";
        }

        private void IncreaseQuantity_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn)
            {
                var parentStackPanel = FindParent<StackPanel>(btn);
                if (parentStackPanel != null)
                {
                    var quantityTextBox = parentStackPanel.Children.OfType<TextBox>().FirstOrDefault();
                    if (quantityTextBox != null && int.TryParse(quantityTextBox.Text, out int currentQuantity))
                    {
                        currentQuantity++;
                        quantityTextBox.Text = currentQuantity.ToString();
                    }
                }
            }
        }

        private void DecreaseQuantity_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn)
            {
                var parentStackPanel = FindParent<StackPanel>(btn);
                if (parentStackPanel != null)
                {
                    var quantityTextBox = parentStackPanel.Children.OfType<TextBox>().FirstOrDefault();
                    if (quantityTextBox != null && int.TryParse(quantityTextBox.Text, out int currentQuantity))
                    {
                        if (currentQuantity > 1)
                        {
                            currentQuantity--;
                            quantityTextBox.Text = currentQuantity.ToString();
                        }
                    }
                }
            }
        }

        private void QuantityTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            // 只允許數字輸入
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void RemoveItem_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is ShoppingCartItem item)
            {
                cartItems.Remove(item);
                UpdateCart();
            }
        }

        // 泛型方法來找到指定類型的父元素
        private T FindParent<T>(DependencyObject child) where T : DependencyObject
        {
            DependencyObject parentObject = VisualTreeHelper.GetParent(child);

            if (parentObject == null) return null;

            T parent = parentObject as T;
            if (parent != null)
                return parent;
            else
                return FindParent<T>(parentObject);
        }
    }
}
