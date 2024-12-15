using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Melon.Model
{
    public class OrderDetail
    {
        public int OrderDetailID { get; set; }

        [ForeignKey("OrderID")]
        public Order Order { get; set; }
        public int OrderID { get; set; }

        [ForeignKey("ProductID")]
        public Product Product { get; set; }
        public int ProductID { get; set; }

        public int Quantity { get; set; }
        public decimal Price { get; set; }
    }
}
