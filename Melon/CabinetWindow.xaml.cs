using Melon.Model;
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
    /// Логика взаимодействия для CabinetWindow.xaml
    /// </summary>
    public partial class CabinetWindow : Window
    {
        private int _userId; // ID текущего пользователя

        public CabinetWindow(int userId)
        {
            InitializeComponent();
            _userId = userId; // Сохраняем ID для использования
            LoadUserData(); // Загрузка данных пользователя
        }

        private void LoadUserData()
        {
            // Загрузка данных пользователя по ID (например, имя, фамилия, пункт выдачи)
            using (var context = new AppDbContext())
            {
                var user = context.Users.FirstOrDefault(u => u.UserID == _userId);
                if (user != null)
                {
                    NameTextBox.Text = user.FirstName;
                    SurnameTextBox.Text = user.LastName;

                    // Загрузка доступных пунктов выдачи
                    var pickupPoints = context.PickupPoints.ToList();
                    PickupPointListBox.ItemsSource = pickupPoints;

                    // Установка текущего выбранного пункта выдачи
                    PickupPointListBox.SelectedItem = pickupPoints.FirstOrDefault(p => p.PickupPointID == user.PickupPointID);
                }
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            using (var context = new AppDbContext())
            {
                var user = context.Users.FirstOrDefault(u => u.UserID == _userId);
                if (user != null)
                {
                    user.FirstName = NameTextBox.Text;
                    user.LastName = SurnameTextBox.Text;

                    if (PickupPointListBox.SelectedItem is PickupPoint selectedPickupPoint)
                    {
                        user.PickupPointID = selectedPickupPoint.PickupPointID;
                    }

                    context.SaveChanges();
                    MessageBox.Show("Данные успешно сохранены!");
                }
            }
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            var clientWindow = new ClientWindow(_userId); // Открываем клиентское окно
            clientWindow.Show();
            this.Close(); // Закрываем текущее окно
        }
        

    }

}
