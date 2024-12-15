using System;
using System.Linq;
using System.Windows;
using Melon.Model;
using BCrypt.Net;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace Melon
{
    public partial class Registration : Window
    {
        public Registration()
        {
            InitializeComponent();
        }

        private void RegisterBtn_Click(object sender, RoutedEventArgs e)
        {
            string email = nameText.Text;
            string password = passwordText.Password;
            string passwordRepeat = passwordTextRepeat.Password;

            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password) || string.IsNullOrEmpty(passwordRepeat))
            {
                MessageBox.Show("Все поля должны быть заполнены.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (password != passwordRepeat)
            {
                MessageBox.Show("Пароли не совпадают.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            using (var db = new AppDbContext())
            {
                if (db.Users.Any(u => u.Email == email))
                {
                    MessageBox.Show("Пользователь с такой почтой уже существует.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                // Хэширование пароля
                string passwordHash = BCrypt.Net.BCrypt.HashPassword(password);

                var user = new User
                {
                    Email = email,
                    PasswordHash = passwordHash,
                    Role = "buyer", // Роль по умолчанию
                    RegistrationDate = DateTime.Now,
                    LastName = " ",
                    FirstName = " ",
                    Address = "addres",
                    PickupPointID = 4
                    
                };

                db.Users.Add(user);
                try
                {
                    db.SaveChanges();
                }
                catch (DbUpdateException ex)
                {
                    MessageBox.Show($"Ошибка сохранения: {ex.InnerException?.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }

                MessageBox.Show("Регистрация прошла успешно!", "Регистрация", MessageBoxButton.OK, MessageBoxImage.Information);
                Authorization authorization = new Authorization();
                authorization.Show();
                this.Close();
            }
        }

        private void BackToLogin_Click(object sender, RoutedEventArgs e)
        {
            Authorization authorization = new Authorization();
            authorization.Show();
            this.Close();
        }
    }
}
