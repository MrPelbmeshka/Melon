using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Melon.Model
{
    public class Order
    {
        public int OrderID { get; set; }

        [ForeignKey("BuyerID")]
        public User Buyer { get; set; }
        public int BuyerID { get; set; }

        public decimal TotalPrice { get; set; }
        public string Status { get; set; }
        public DateTime OrderDate { get; set; }

        // Связь с PickupPoint
        [ForeignKey("PickupPointID")]
        public PickupPoint PickupPoint { get; set; }
        public int? PickupPointID { get; set; } // Внешний ключ

        public ICollection<OrderDetail> OrderDetails { get; set; }
    }

}
