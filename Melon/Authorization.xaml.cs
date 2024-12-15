using System.Linq;
using System.Windows;
using Melon.Model;
using BCrypt.Net;

namespace Melon
{
    public partial class Authorization : Window
    {
        public Authorization()
        {
            InitializeComponent();
        }

        private void LoginBtn_Click(object sender, RoutedEventArgs e)
        {
            string login = loginText.Text;
            string password = pas.Password;

            using (var db = new AppDbContext())
            {
                // Найти пользователя по email
                var user = db.Users.FirstOrDefault(u => u.Email == login);

                // Если пользователь найден и пароли совпадают
                if (user != null && BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
                {
                    MessageBox.Show("Успешный вход!", "Авторизация", MessageBoxButton.OK, MessageBoxImage.Information);

                    // Переход в зависимости от роли пользователя
                    if (user.Role == "admin")
                    {
                        // Открываем окно для администратора
                        AdminWindow adminWindow = new AdminWindow();
                        adminWindow.Show();
                    }
                    else if (user.Role == "seller")
                    {
                        // Открываем окно для продавца
                        SellerWindow sellerWindow = new SellerWindow(user.UserID);
                        sellerWindow.Show();
                    }
                    else if (user.Role == "buyer")
                    {
                        // Открываем окно для покупателя
                        ClientWindow buyerWindow = new ClientWindow(user.UserID);
                        buyerWindow.Show();
                    }

                    // Закрываем окно авторизации
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Неверный логин или пароль.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void RegisterBtn_Click(object sender, RoutedEventArgs e)
        {
            Registration registration = new Registration();
            registration.Show();
            this.Close();
        }
    }
}
