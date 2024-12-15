using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Melon.Model
{
    public class Product
    {
        public int ProductID { get; set; }

        [ForeignKey("SellerID")]
        public User Seller { get; set; }
        public int SellerID { get; set; }

        public string ProductName { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public int Stock { get; set; }
        public string ImageURL { get; set; }
        public string Status { get; set; }
        public DateTime CreatedAt { get; set; }

        public ICollection<Review> Reviews { get; set; }
        public ICollection<OrderDetail> OrderDetails { get; set; }
        public ICollection<Complaint> Complaints { get; set; }
    }
}
