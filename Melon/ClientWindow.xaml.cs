using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Printing;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Melon.Model;
using Microsoft.EntityFrameworkCore;

namespace Melon
{
    public partial class ClientWindow : Window, INotifyPropertyChanged
    {
        public ICommand PlaceOrderCommand { get; }
        private List<Product> _favorites; // Список избранных товаров
        private int _userId;

        public ICommand ViewProductCommand { get; }
        public ObservableCollection<Product> Products { get; set; }

        private string _searchQuery;
        public string SearchQuery
        {
            get => _searchQuery;
            set
            {
                if (_searchQuery != value)
                {
                    _searchQuery = value;
                    OnPropertyChanged();
                    PerformSearch(); // Выполняем фильтрацию при изменении строки поиска
                }
            }
        }

        private ObservableCollection<Product> _filteredProducts;
        public ObservableCollection<Product> FilteredProducts
        {
            get => _filteredProducts;
            set
            {
                if (_filteredProducts != value)
                {
                    _filteredProducts = value;
                    OnPropertyChanged();
                }
            }
        }
        private ObservableCollection<OrderDetail> _cartItems;
        public ObservableCollection<OrderDetail> CartItems
        {
            get => _cartItems;
            set
            {
                if (_cartItems != value)
                {
                    _cartItems = value;
                    OnPropertyChanged();
                }
            }
        }


        private ObservableCollection<PickupPoint> _pickupPoints;
        public ObservableCollection<PickupPoint> PickupPoints
        {
            get => _pickupPoints;
            set
            {
                if (_pickupPoints != value)
                {
                    _pickupPoints = value;
                    OnPropertyChanged();
                }
            }
        }

        public ClientWindow(int userId)
        {
            InitializeComponent();
            _favorites = new List<Product>(); // Инициализация списка избранных товаров
            LoadPickupPoints();
            LoadProducts(); // Загружаем список товаров
            ViewProductCommand = new RelayCommand(ViewProductDetails);
            _userId = userId;

            FilteredProducts = new ObservableCollection<Product>(Products ?? Enumerable.Empty<Product>());
            PlaceOrderCommand = new RelayCommand(PlaceOrder, CanPlaceOrder);
            LoadPickupPoints();
            LoadProducts();
            LoadCartItems();
            SearchQuery = "";
            DataContext = this; // Устанавливаем DataContext для привязок
        }

        private bool CanPlaceOrder(object parameter)
        {
            return CartItems != null && CartItems.Count > 0;
        }

        private void PlaceOrder(object parameter)
        {
            using (var context = new AppDbContext())
            {
                // Находим заказ со статусом "Корзина" для текущего пользователя
                var order = context.Orders.FirstOrDefault(o => o.BuyerID == _userId && o.Status == "Корзина");

                if (order != null)
                {
                    // Меняем статус заказа
                    order.Status = "Обработка";
                    context.SaveChanges();

                    MessageBox.Show("Заказ успешно оформлен!");
                    LoadCartItems(); // Обновляем список корзины
                }
                else
                {
                    MessageBox.Show("Корзина пуста!");
                }
            }
        }

        private void LoadProducts()
        {
            using (var context = new AppDbContext())
            {
                Products = new ObservableCollection<Product>(
                    context.Products.Where(product => product.Status == "Одобрено").ToList()
                );
            }

            FilteredProducts = new ObservableCollection<Product>(Products); // Инициализация фильтрованных данных
        }

        private void PerformSearch()
        {
            if (string.IsNullOrWhiteSpace(SearchQuery))
            {
                FilteredProducts = new ObservableCollection<Product>(Products);
            }
            else
            {
                var lowerQuery = SearchQuery.ToLower();

                // Фильтрация по названию товаров
                FilteredProducts = new ObservableCollection<Product>(
                    Products.Where(product =>
                        !string.IsNullOrEmpty(product.ProductName) &&
                        product.ProductName.ToLower().Contains(lowerQuery)
                    )
                );
            }
        }

        private void LoadPickupPoints()
        {
            using (var context = new AppDbContext())
            {
                PickupPoints = new ObservableCollection<PickupPoint>(context.PickupPoints.ToList());
            }
        }

        private void LoadCartItems()
        {
            using (var context = new AppDbContext())
            {
                // Ищем заказ "Корзина"
                var order = context.Orders
                    .FirstOrDefault(o => o.BuyerID == _userId && o.Status == "Корзина");

                if (order != null)
                {
                    // Загружаем детали заказа с продуктами
                    var items = context.OrderDetails
                        .Where(od => od.OrderID == order.OrderID)
                        .Include(od => od.Product) // Подгружаем связанные данные
                        .ToList();

                    CartItems = new ObservableCollection<OrderDetail>(items);
                }
                else
                {
                    CartItems = new ObservableCollection<OrderDetail>(); // Если корзина пуста
                }
            }
        }



        // Открытие деталей товара
        private void ViewProductDetails(object parameter)
        {
            if (parameter is Product product)
            {
                // Здесь передаем product и _userId
                var seeMoreWindow = new SeeMoreWindow(product, _userId);
                seeMoreWindow.Show();
                this.Close(); // Закрываем текущее окно, если необходимо
            }
        }

        // Кнопка закрытия окна
        private void CloseWindow_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Cabinet_Click(object sender, RoutedEventArgs e)
        {
            // Передаем ID текущего пользователя
            CabinetWindow cabinetWindow = new CabinetWindow(_userId);
            cabinetWindow.Show();
            this.Close(); // Закрываем текущее окно, если необходимо
        }


        // Кнопка открытия пунктов выдачи
        private void PickupPointsButton_Click(object sender, RoutedEventArgs e)
        {
            //PickupPointsPopup.IsOpen = !PickupPointsPopup.IsOpen; // Открываем/закрываем Popup
        }

        // Кнопка корзины
        private void CartButton_Click(object sender, RoutedEventArgs e)
        {
            LoadCartItems(); // Загружаем товары из корзины
            basketPopup.IsOpen = !basketPopup.IsOpen;
        }

        // Реализация INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        // Реализация команды
        public class RelayCommand : ICommand
        {
            private readonly Action<object> _execute;
            private readonly Func<object, bool> _canExecute;

            public RelayCommand(Action<object> execute, Func<object, bool> canExecute = null)
            {
                _execute = execute;
                _canExecute = canExecute;
            }

            public event EventHandler CanExecuteChanged;

            public bool CanExecute(object parameter) => _canExecute == null || _canExecute(parameter);

            public void Execute(object parameter) => _execute(parameter);

            public void RaiseCanExecuteChanged() => CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
