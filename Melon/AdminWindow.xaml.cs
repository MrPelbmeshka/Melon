using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using Melon.Model;
using Microsoft.EntityFrameworkCore;

namespace Melon
{
    public partial class AdminWindow : Window
    {
        private AppDbContext _dbContext;

        public AdminWindow()
        {
            InitializeComponent();
            _dbContext = new AppDbContext();
            LoadComplaints();
            LoadProducts();
        }

        private void LoadProducts()
        {
            using (var dbContext = new AppDbContext())
            {
                var products = dbContext.Products
                    .Include(p => p.Seller)
                    .Select(p => new
                    {
                        p.ProductID,
                        p.ProductName,
                        SellerName = p.Seller.FirstName + " " + p.Seller.LastName,
                        p.Price,
                        p.Status,
                        p.Description,
                        p.ImageURL
                    })
                    .ToList();

                productsDataGrid.ItemsSource = products;
            }
        }

        private void ApproveProduct_Click(object sender, RoutedEventArgs e)
        {
            var selectedProduct = productsDataGrid.SelectedItem;
            if (selectedProduct == null)
            {
                MessageBox.Show("Выберите продукт для одобрения.");
                return;
            }

            // Приводим выбранный элемент к конкретному типу
            var productId = (int)selectedProduct.GetType().GetProperty("ProductID").GetValue(selectedProduct);

            using (var dbContext = new AppDbContext())
            {
                var product = dbContext.Products.FirstOrDefault(p => p.ProductID == productId);
                if (product != null)
                {
                    product.Status = "Одобрено";
                    dbContext.SaveChanges();
                    MessageBox.Show("Продукт успешно одобрен.");
                    LoadProducts(); // Обновление списка продуктов
                }
                else
                {
                    MessageBox.Show("Не удалось найти продукт в базе данных.");
                }
            }
        }


        private void RejectProduct_Click(object sender, RoutedEventArgs e)
        {
            var selectedProduct = productsDataGrid.SelectedItem;
            if (selectedProduct == null)
            {
                MessageBox.Show("Выберите продукт для блокировки.");
                return;
            }

            // Приводим выбранный элемент к конкретному типу
            var productId = (int)selectedProduct.GetType().GetProperty("ProductID").GetValue(selectedProduct);

            using (var dbContext = new AppDbContext())
            {
                var product = dbContext.Products.FirstOrDefault(p => p.ProductID == productId);
                if (product != null)
                {
                    product.Status = "Не одобрено";
                    dbContext.SaveChanges();
                    MessageBox.Show("Продукт заблокирован.");
                    LoadProducts(); // Обновить список продуктов
                }
                else
                {
                    MessageBox.Show("Не удалось найти продукт в базе данных.");
                }
            }
        }



        private void ProductsDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectedProduct = productsDataGrid.SelectedItem as dynamic;
            if (selectedProduct == null) return;

            selectedProductName2.Text = selectedProduct.ProductName;
            selectedProductDescription.Text = selectedProduct.Description;

            if (!string.IsNullOrEmpty(selectedProduct.ImageURL))
            {
                selectedProductImage2.Source = new BitmapImage(new Uri(selectedProduct.ImageURL, UriKind.RelativeOrAbsolute));
            }
            else
            {
                selectedProductImage.Source = null; // Если изображение отсутствует
            }
        }


        private void LoadComplaints()
        {
            using (var dbContext = new AppDbContext())
            {
                var complaints = dbContext.Complaints
                    .Include(c => c.User)
                    .Include(c => c.Product)
                    .Include(c => c.Review)
                    .Select(c => new
                    {
                        c.ComplaintID,
                        BuyerName = c.User.FirstName + " " + c.User.LastName,
                        ProductName = c.Product != null ? c.Product.ProductName : "Без товара",
                        c.CreatedAt,
                        ComplaintText = c.ComplaintText,
                        ProductImageUrl = c.Product != null ? c.Product.ImageURL : null,
                        ReviewComment = c.Review != null ? c.Review.Comment : "Комментарий отсутствует",
                        ReviewID = c.ReviewID,
                    })
                    .ToList();

                complaintsDataGrid.ItemsSource = complaints;
            }
        }


        private void ComplaintsDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectedComplaint = complaintsDataGrid.SelectedItem as dynamic;
            if (selectedComplaint == null) return;

            selectedComplaintText.Text = selectedComplaint.ComplaintText;
            selectedProductName.Text = selectedComplaint.ProductName;
            selectedReviewText.Text = selectedComplaint.ReviewComment;

            if (!string.IsNullOrEmpty(selectedComplaint.ProductImageUrl))
            {
                selectedProductImage.Source = new BitmapImage(new Uri(selectedComplaint.ProductImageUrl, UriKind.RelativeOrAbsolute));
            }
            else
            {
                selectedProductImage.Source = null; // Если изображение отсутствует
            }
        }

        // Метод для удаления только жалобы
        private void DeleteComplaintClick(object sender, RoutedEventArgs e)
        {
            var selectedComplaint = complaintsDataGrid.SelectedItem as dynamic;
            if (selectedComplaint == null)
            {
                MessageBox.Show("Выберите жалобу для удаления.");
                return;
            }

            int complaintId = selectedComplaint.ComplaintID;

            // Удаляем только жалобу
            using (var dbContext = new AppDbContext())
            {
                var complaint = dbContext.Complaints.FirstOrDefault(c => c.ComplaintID == complaintId);
                if (complaint != null)
                {
                    dbContext.Complaints.Remove(complaint);
                    dbContext.SaveChanges();
                    MessageBox.Show("Жалоба успешно удалена.");
                    LoadComplaints(); // Обновление списка жалоб
                }
                else
                {
                    MessageBox.Show("Не удалось найти жалобу в базе данных.");
                }
            }
        }

        // Метод для удаления жалобы и отзыва
        private void DeleteReviewComplaintClick(object sender, RoutedEventArgs e)
        {
            var selectedComplaint = complaintsDataGrid.SelectedItem as dynamic;
            if (selectedComplaint == null)
            {
                MessageBox.Show("Выберите жалобу для удаления.");
                return;
            }

            int complaintId = selectedComplaint.ComplaintID;
            int? reviewId = selectedComplaint.ReviewID; // Получаем ID отзыва

            using (var dbContext = new AppDbContext())
            {
                // Удаляем жалобу, которая ссылается на отзыв
                var complaint = dbContext.Complaints.FirstOrDefault(c => c.ComplaintID == complaintId);
                if (complaint != null)
                {
                    dbContext.Complaints.Remove(complaint);
                }

                // Удаляем отзыв, если он существует
                if (reviewId != null)
                {
                    var review = dbContext.Reviews.FirstOrDefault(r => r.ReviewID == reviewId.Value);
                    if (review != null)
                    {
                        dbContext.Reviews.Remove(review);
                    }
                }

                // Сохраняем изменения в базе данных
                try
                {
                    dbContext.SaveChanges();
                    MessageBox.Show("Жалоба и отзыв успешно удалены.");
                    LoadComplaints(); // Обновление списка жалоб
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при удалении: {ex.Message}");
                }
            }
        }



    }
}
