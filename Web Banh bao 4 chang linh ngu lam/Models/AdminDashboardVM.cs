using System.Collections.Generic;

namespace Web_Banh_bao_4_chang_linh_ngu_lam.Models
{
    public class TopProductVM
    {
        public string Product { get; set; }
        public int TotalQty { get; set; }
    }

    public class AdminDashboardVM
    {
        public int TotalProduct { get; set; }
        public int TotalOrder { get; set; }
        public int TotalCustomer { get; set; }
        public decimal TotalRevenue { get; set; }
        public List<TopProductVM> TopProducts { get; set; }
        public decimal ProfitToday { get; set; }
        public decimal ProfitThisMonth { get; set; }
        public string BestSellingDay { get; set; }
        public string BestSellingMonth { get; set; }
        public int BestSellingDayQuantity { get; set; }

        public int BestSellingMonthQuantity { get; set; }
    }
}
