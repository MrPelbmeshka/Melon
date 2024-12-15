    using Microsoft.EntityFrameworkCore;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    namespace Melon.Model
    {
        public class AppDbContext : DbContext
        {
            public DbSet<User> Users { get; set; }
            public DbSet<AdminAction> AdminActions { get; set; }
            public DbSet<Complaint> Complaints { get; set; }
            public DbSet<Order> Orders { get; set; }
            public DbSet<OrderDetail> OrderDetails { get; set; }
            public DbSet<Product> Products { get; set; }
            public DbSet<Review> Reviews { get; set; }
            public DbSet<Favorite> Favorites { get; set; }
            public DbSet<PickupPoint> PickupPoints { get; set; } // Добавлено

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            {
                optionsBuilder.UseSqlServer("Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=MarketPlace;Integrated Security=True;Connect Timeout=30;Encrypt=False;Trust Server Certificate=False;Application Intent=ReadWrite;Multi Subnet Failover=False");
            }

            protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
                base.OnModelCreating(modelBuilder);

                // Установка точности для TotalPrice в Order
                modelBuilder.Entity<Order>()
                    .Property(o => o.TotalPrice)
                    .HasPrecision(18, 2); // 18 цифр, 2 из которых после запятой

                modelBuilder.Entity<Order>()
                    .HasOne(o => o.PickupPoint)
                    .WithMany(pp => pp.Orders)
                    .HasForeignKey(o => o.PickupPointID)
                    .OnDelete(DeleteBehavior.Restrict);

                // Установка точности для Price в OrderDetail
                 modelBuilder.Entity<OrderDetail>()
                    .Property(od => od.Price)
                    .HasPrecision(18, 2); // 18 цифр, 2 из которых после запятой

                // Установка точности для Price в Product
                modelBuilder.Entity<Product>()
                    .Property(p => p.Price)
                    .HasPrecision(18, 2); // 18 цифр, 2 из которых после запятой

                // Конфигурация отношений

                // Связь User - Product (Продавец)
                modelBuilder.Entity<Product>()
                    .HasOne(p => p.Seller)
                    .WithMany(u => u.Products)
                    .HasForeignKey(p => p.SellerID)
                    .OnDelete(DeleteBehavior.Restrict);

                // Связь Order - User (Покупатель)
                modelBuilder.Entity<Order>()
                    .HasOne(o => o.Buyer)
                    .WithMany(u => u.Orders)
                    .HasForeignKey(o => o.BuyerID)
                    .OnDelete(DeleteBehavior.Restrict);

                // Связь OrderDetail - Order
                modelBuilder.Entity<OrderDetail>()
                    .HasOne(od => od.Order)
                    .WithMany(o => o.OrderDetails)
                    .HasForeignKey(od => od.OrderID)
                    .OnDelete(DeleteBehavior.Restrict);

                // Связь OrderDetail - Product
                modelBuilder.Entity<OrderDetail>()
                    .HasOne(od => od.Product)
                    .WithMany(p => p.OrderDetails)
                    .HasForeignKey(od => od.ProductID)
                    .OnDelete(DeleteBehavior.Restrict);

                // Связь Complaint - User
                modelBuilder.Entity<Complaint>()
                    .HasOne(c => c.User)
                    .WithMany(u => u.Complaints)
                    .HasForeignKey(c => c.UserID)
                    .OnDelete(DeleteBehavior.Restrict);

                // Связь Complaint - Product
                modelBuilder.Entity<Complaint>()
                    .HasOne(c => c.Product)
                    .WithMany(p => p.Complaints)
                    .HasForeignKey(c => c.ProductID)
                    .OnDelete(DeleteBehavior.Restrict);

                // Связь Complaint - Review
                modelBuilder.Entity<Complaint>()
                    .HasOne(c => c.Review)
                    .WithMany()
                    .HasForeignKey(c => c.ReviewID)
                    .OnDelete(DeleteBehavior.Restrict);

                // Связь Review - User
                modelBuilder.Entity<Review>()
                    .HasOne(r => r.User)
                    .WithMany()
                    .HasForeignKey(r => r.UserID)
                    .OnDelete(DeleteBehavior.Restrict);

                // Связь Review - Product
                modelBuilder.Entity<Review>()
                    .HasOne(r => r.Product)
                    .WithMany(p => p.Reviews)
                    .HasForeignKey(r => r.ProductID)
                    .OnDelete(DeleteBehavior.Restrict);

                // Связь AdminAction - User (Admin)
                modelBuilder.Entity<AdminAction>()
                    .HasOne(aa => aa.Admin)
                    .WithMany()
                    .HasForeignKey(aa => aa.AdminID)
                    .OnDelete(DeleteBehavior.Restrict);
            }
        }
    }
