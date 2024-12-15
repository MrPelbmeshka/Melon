using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Melon.Model
{
    public class Complaint
    {
        public int ComplaintID { get; set; }

        [ForeignKey("UserID")]
        public User User { get; set; }
        public int UserID { get; set; }

        [ForeignKey("ProductID")]
        public Product Product { get; set; }
        public int? ProductID { get; set; }

        [ForeignKey("ReviewID")]
        public Review Review { get; set; }
        public int? ReviewID { get; set; }

        public string ComplaintText { get; set; }
        public string Status { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
