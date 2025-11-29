using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Web_Banh_bao_4_chang_linh_ngu_lam.Models
{
    public class Reviews
    {
        [Key]
        public int ReviewId { get; set; }

        [Required]
        public int ProductId { get; set; }

        public int? UserId { get; set; } // Tương đương CustomerID ở DB

        [Required]
        [Range(1, 5)]
        public int Rating { get; set; }

        public string Comment { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        [ForeignKey("ProductId")]
        public virtual Product Product { get; set; }

        // Cần thêm Navigation Property cho Customer/User nếu có
    }
}