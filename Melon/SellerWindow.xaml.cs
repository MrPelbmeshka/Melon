using Microsoft.Win32;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using Melon.Model;
using Microsoft.EntityFrameworkCore;
using MahApps.Metro.Controls.Dialogs;
using MaterialDesignThemes.Wpf;
using Microsoft.Identity.Client;

namespace Melon
{
    public partial class SellerWindow : Window
    {
        private AppDbContext _dbContext;
        private int sellerId;
        public string InputText { get; private set; }

        public SellerWindow(int sellerId)
        {
            InitializeComponent();
            this.sellerId = sellerId;
            _dbContext = new AppDbContext();
            LoadProducts();

            LoadSellerData();
        }

        private void LoadSellerData()
        {
            using (var db = new AppDbContext())
            {
                var products = db.Products
                    .Where(p => p.SellerID == sellerId)
                    .Select(p => new
                    {
                        p.ProductID,
                        p.ProductName,
                        p.ImageURL,
                        SoldQuantity = db.OrderDetails
                            .Where(od => od.ProductID == p.ProductID)
                            .Sum(od => (int?)od.Quantity) ?? 0,
                        RemainingQuantity = p.Stock,
                        AmountAfterCommission = (p.Price * db.OrderDetails
                            .Where(od => od.ProductID == p.ProductID)
                            .Sum(od => (int?)od.Quantity) ?? 0) * 0.87m
                    })
                    .ToList();

                myProductsList.ItemsSource = products;
            }
        }



        private void MenuOneClick(object sender, RoutedEventArgs e)
        {
            CanvasOne.Visibility = Visibility.Visible;
            CanvasTwo.Visibility = Visibility.Collapsed;
            CanvasThree.Visibility = Visibility.Collapsed;
        }

        private void MenuTwoClick(object sender, RoutedEventArgs e)
        {
            CanvasOne.Visibility = Visibility.Collapsed;
            CanvasTwo.Visibility = Visibility.Visible;
            CanvasThree.Visibility = Visibility.Collapsed;
        }

        private void MenuThreeClick(object sender, RoutedEventArgs e)
        {
            CanvasOne.Visibility = Visibility.Collapsed;
            CanvasTwo.Visibility = Visibility.Collapsed;
            CanvasThree.Visibility = Visibility.Visible;
        }

        private void MyProductsList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Get the selected product
            var selectedProduct = myProductsList.SelectedItem as dynamic;

            if (selectedProduct == null) return;

            // Update the UI with the selected product's detailed information
            Statistik1.Text = selectedProduct.ProductName;
            Statistik2.Text = "Продано: " + selectedProduct.SoldQuantity;
            Statistik3.Text = "Осталось: " + selectedProduct.RemainingQuantity;
            Statistik4.Text = "Сумма к получению (13%): " + selectedProduct.AmountAfterCommission.ToString("C");
        }


        private void LoadProducts()
        {
            var products = _dbContext.Products.Where(p => p.SellerID == sellerId).ToList();
            productsTable.ItemsSource = products;
        }

        private void AddProductClick(object sender, RoutedEventArgs e)
        {
            var product = new Product
            {
                ProductName = productName.Text,
                Description = productDescription.Text,
                Price = decimal.Parse(productPrice.Text),
                Stock = int.Parse(productQuantity.Text),
                ImageURL = imageURL.Text,
                Status = "Ожидает",
                CreatedAt = DateTime.Now,
                SellerID = sellerId // Замените CurrentUser.ID на ID текущего пользователя
            };

            _dbContext.Products.Add(product);
            _dbContext.SaveChanges();
            LoadProducts();
            MessageBox.Show("Продукт добавлен!");
        }

        private void EditProductClick(object sender, RoutedEventArgs e)
        {
            if (productsTable.SelectedItem is Product selectedProduct)
            {
                selectedProduct.ProductName = productName.Text;
                selectedProduct.Description = productDescription.Text;
                selectedProduct.Price = decimal.Parse(productPrice.Text);
                selectedProduct.Stock = int.Parse(productQuantity.Text);
                selectedProduct.ImageURL = imageURL.Text;

                _dbContext.SaveChanges();
                LoadProducts();
                MessageBox.Show("Продукт обновлён!");
            }
        }

        private void DeleteProductClick(object sender, RoutedEventArgs e)
        {
            if (productsTable.SelectedItem is Product selectedProduct)
            {
                _dbContext.Products.Remove(selectedProduct);
                _dbContext.SaveChanges();
                LoadProducts();
                MessageBox.Show("Продукт удалён!");
            }
        }

        private void BrowseImageClick(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "Image Files|*.jpg;*.jpeg;*.png;"
            };
            if (openFileDialog.ShowDialog() == true)
            {
                imageURL.Text = openFileDialog.FileName;
                productImagePreview.Source = new BitmapImage(new Uri(openFileDialog.FileName));
            }
        }

        private void SetCreationDateClick(object sender, RoutedEventArgs e)
        {
            creationDate.Text = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        }

        private void ProductsTable_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (productsTable.SelectedItem is Product selectedProduct)
            {
                productName.Text = selectedProduct.ProductName;
                productDescription.Text = selectedProduct.Description;
                productPrice.Text = selectedProduct.Price.ToString();
                productQuantity.Text = selectedProduct.Stock.ToString();
                imageURL.Text = selectedProduct.ImageURL;
                creationDate.Text = selectedProduct.CreatedAt.ToString("yyyy-MM-dd HH:mm:ss");

                if (!string.IsNullOrEmpty(selectedProduct.ImageURL))
                {
                    productImagePreview.Source = new BitmapImage(new Uri(selectedProduct.ImageURL));
                }
            }
        }
        private void SearchReviews(object sender, RoutedEventArgs e)
        {
            using (var dbContext = new AppDbContext())
            {
                string selectedType = (filterTypeComboBox.SelectedItem as ComboBoxItem)?.Content.ToString();
                DateTime? fromDate = filterFromDate.SelectedDate;
                DateTime? toDate = filterToDate.SelectedDate;

                var reviews = dbContext.Reviews
                    .Include(r => r.User)
                    .Include(r => r.Product)
                    .Where(r =>
                        (selectedType == "Все" || (selectedType == "Отзыв" && r.ReviewType == "Отзыв") || (selectedType == "Вопрос" && r.ReviewType == "Вопрос")) &&
                        (!fromDate.HasValue || r.CreatedAt >= fromDate.Value) &&
                        (!toDate.HasValue || r.CreatedAt <= toDate.Value))
                    .Select(r => new
                    {
                        r.ReviewID,
                        BuyerName = r.User.FirstName + " " + r.User.LastName,
                        ProductName = r.Product.ProductName,
                        r.CreatedAt
                    })
                    .ToList();

                reviewsDataGrid.ItemsSource = reviews;
            }
        }


        private void ReviewsListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectedReview = reviewsDataGrid.SelectedItem as dynamic;
            if (selectedReview == null) return;

            int reviewId = selectedReview.ReviewID;

            using (var dbContext = new AppDbContext())
            {
                var reviewDetails = dbContext.Reviews
                    .Include(r => r.User)
                    .Include(r => r.Product)
                    .Where(r => r.ReviewID == reviewId)
                    .Select(r => new
                    {
                        ReviewerName = r.User.FirstName + " " + r.User.LastName,
                        r.Comment,
                        r.Product.ImageURL
                    })
                    .FirstOrDefault();

                if (reviewDetails != null)
                {
                    selectedReviewerName.Text = reviewDetails.ReviewerName;
                    selectedReviewText.Text = reviewDetails.Comment;

                    if (!string.IsNullOrEmpty(reviewDetails.ImageURL))
                    {
                        selectedProductImage.Source = new BitmapImage(new Uri(reviewDetails.ImageURL, UriKind.RelativeOrAbsolute));
                    }
                }
            }
        }

        private void SaveReply(object sender, RoutedEventArgs e)
        {
            // Проверяем, выбран ли отзыв
            var selectedReview = ChoiceReview;
            if (selectedReview == null || string.IsNullOrEmpty(selectedReview.Text))
            {
                MessageBox.Show("Выберите отзыв, чтобы ответить.");
                return;
            }

            // Получаем ID выбранного отзыва
            int reviewId = int.Parse(selectedReview.Text);

            // Получаем текст из TextBox
            string replyText = replyTextBox.Text.Trim();
            if (string.IsNullOrEmpty(replyText))
            {
                MessageBox.Show("Введите текст ответа перед сохранением.");
                return;
            }

            using (var dbContext = new AppDbContext())
            {
                // Находим отзыв по ID
                var review = dbContext.Reviews.FirstOrDefault(r => r.ReviewID == reviewId);

                if (review != null)
                {
                    // Проверяем, есть ли уже ответ на этот отзыв
                    if (!string.IsNullOrEmpty(review.Reply))
                    {
                        MessageBox.Show("Ответ уже существует на этот отзыв.");
                        return; // Прерываем выполнение метода, если ответ уже есть
                    }

                    // Если ответа нет, сохраняем новый ответ
                    review.Reply = replyText;
                    dbContext.SaveChanges();

                    MessageBox.Show("Ответ сохранён.");
                    replyTextBox.Clear(); // Очищаем поле после сохранения
                }
                else
                {
                    MessageBox.Show("Не удалось найти отзыв в базе данных.");
                }
            }
        }

        private void ExitClick(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
    }
}
