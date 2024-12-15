using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Melon.Model
{
    public class Review
    {
        public int ReviewID { get; set; }

        [ForeignKey("ProductID")]
        public Product Product { get; set; }
        public int ProductID { get; set; }

        [ForeignKey("UserID")]
        public User User { get; set; }
        public int UserID { get; set; }

        public int Rating { get; set; }
        public string Comment { get; set; }
        public DateTime CreatedAt { get; set; }

        public string ReviewType { get; set; } 

        public string Reply { get; set; }

    }
}
