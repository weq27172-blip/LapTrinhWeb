using System.Collections.Generic;

namespace Web_Banh_bao_4_chang_linh_ngu_lam.Models
{
    public class ProductFilterVM
    {
        public IEnumerable<Category> Categories { get; set; }
        public IEnumerable<Product> Products { get; set; }
        public int CurrentPage { get; set; }
        public int PageSize { get; set; }
        public int TotalProducts { get; set; }
        public int? SelectedCategoryId { get; set; }
    }
}