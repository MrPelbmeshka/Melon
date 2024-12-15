using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Melon.Model
{
    public class Favorite
    {
        public int FavoriteID { get; set; }

        [ForeignKey("UserID")]
        public User User { get; set; }
        public int UserID { get; set; }

        [ForeignKey("ProductID")]
        public Product Product { get; set; }
        public int ProductID { get; set; }
    }
}

