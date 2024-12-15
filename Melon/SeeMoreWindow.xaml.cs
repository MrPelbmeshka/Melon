using Melon.Model;
using System.Windows;

namespace Melon
{
    public partial class SeeMoreWindow : Window
    {
        private int _userId; // Хранение ID пользователя

        public SeeMoreWindow(Product product, int userId)
        {
            InitializeComponent();
            DataContext = product; // Устанавливаем контекст данных на переданный продукт
            _userId = userId; // Сохраняем ID пользователя
        }

        private void ReturnToClientWindow_Click(object sender, RoutedEventArgs e)
        {
            var clientWindow = new ClientWindow(_userId);
            clientWindow.Show();
            this.Close();
        }


        private void AddToCartButton_Click(object sender, RoutedEventArgs e)
        {
            using (var context = new AppDbContext())
            {
                // Получаем пользователя для определения PickupPointID
                var user = context.Users.FirstOrDefault(u => u.UserID == _userId);
                if (user == null)
                {
                    MessageBox.Show("Ошибка: пользователь не найден.");
                    return;
                }

                // Проверка существующего заказа "Корзина" для пользователя
                var existingOrder = context.Orders.FirstOrDefault(o => o.BuyerID == _userId && o.Status == "Корзина");

                // Если заказа нет, создаем новый
                if (existingOrder == null)
                {
                    existingOrder = new Order
                    {
                        BuyerID = _userId,
                        PickupPointID = user.PickupPointID, // Привязываем ID пункта выдачи из пользователя
                        TotalPrice = 0, // Пересчитается при добавлении товаров
                        Status = "Корзина",
                        OrderDate = DateTime.Now
                    };
                    context.Orders.Add(existingOrder);
                    context.SaveChanges(); // Сохраняем, чтобы получить OrderID
                }

                // Проверка переданного продукта
                var product = DataContext as Product;
                if (product == null)
                {
                    MessageBox.Show("Ошибка: продукт не выбран.");
                    return;
                }

                // Добавление в OrderDetails
                var orderDetail = context.OrderDetails.FirstOrDefault(od => od.OrderID == existingOrder.OrderID && od.ProductID == product.ProductID);
                if (orderDetail == null)
                {
                    orderDetail = new OrderDetail
                    {
                        OrderID = existingOrder.OrderID,
                        ProductID = product.ProductID,
                        Quantity = 1,
                        Price = product.Price
                    };
                    context.OrderDetails.Add(orderDetail);
                }
                else
                {
                    // Если товар уже в заказе, увеличиваем количество
                    orderDetail.Quantity++;
                    orderDetail.Price = product.Price * orderDetail.Quantity;
                }

                // Пересчет итоговой суммы заказа
                existingOrder.TotalPrice = context.OrderDetails
                    .Where(od => od.OrderID == existingOrder.OrderID)
                    .Sum(od => od.Price);

                context.SaveChanges();
                MessageBox.Show("Товар добавлен в корзину!");
            }
        }


        private void AddToFavoriteButton_Click(object sender, RoutedEventArgs e)
        {
            using (var context = new AppDbContext())
            {
                // Проверка переданного продукта
                var product = DataContext as Product;
                if (product == null)
                {
                    MessageBox.Show("Ошибка: продукт не выбран.");
                    return;
                }

                // Проверка, не добавлен ли уже продукт в избранное
                var existingFavorite = context.Favorites.FirstOrDefault(f => f.UserID == _userId && f.ProductID == product.ProductID);
                if (existingFavorite != null)
                {
                    MessageBox.Show("Этот товар уже находится в избранном!");
                    return;
                }

                // Добавление в избранное
                var favorite = new Favorite
                {
                    UserID = _userId,
                    ProductID = product.ProductID
                };

                context.Favorites.Add(favorite);
                context.SaveChanges();
                MessageBox.Show("Товар добавлен в избранное!");
            }
        }


    }
}
