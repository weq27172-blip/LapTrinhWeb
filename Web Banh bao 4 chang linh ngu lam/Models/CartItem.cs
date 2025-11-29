using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Web_Banh_bao_4_chang_linh_ngu_lam.Models
{
    public class CartItem
    {
        public Product Product { get; set; }
        public int Quantity { get; set; }
        public int FreeQuantity { get; set; } = 0;
        public decimal TotalPrice
        {
            get { return Product != null ? Product.Price * Quantity : 0; }
        }
    }
}