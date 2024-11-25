using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Melon
{
    /// <summary>
    /// Логика взаимодействия для ClientWindow.xaml
    /// </summary>
    public partial class ClientWindow : Window
    {
        public ClientWindow()
        {
            InitializeComponent();
        }

        private void CloseWindow_Click(object sender, RoutedEventArgs e)
        {

        }

        private void CategoriesButton_Click(object sender, RoutedEventArgs e)
        {
            // Переключение состояния Popup (открыть или закрыть)
            CategoriesPopup.IsOpen = !CategoriesPopup.IsOpen;
        }

        private void AddressButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Открывается список адресов!");
        }

        private void CartButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Открывается корзина!");
        }

        
    }
}
