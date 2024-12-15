using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Melon.Model
{
    public class PickupPoint
    {
        public int PickupPointID { get; set; }
        public string Location { get; set; } // Адрес или описание точки самовывоза
        public string Description { get; set; } // Дополнительная информация

        public ICollection<Order> Orders { get; set; } // Связь с заказами
    }
}
