using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Melon.Model
{
    public class AdminAction
    {
        public int AdminActionID { get; set; }
        public string Action { get; set; }

        [ForeignKey("AdminID")]
        public User Admin { get; set; }
        public int AdminID { get; set; } // Добавлено для связи

        public string ActionType { get; set; }
        public int? TargetID { get; set; }
        public string ActionDescription { get; set; }
        public DateTime ActionDate { get; set; }
    }
}
