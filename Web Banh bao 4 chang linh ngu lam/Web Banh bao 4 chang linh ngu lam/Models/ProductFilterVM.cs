using System.Collections.Generic;

namespace Web_Banh_bao_4_chang_linh_ngu_lam.Models
{
    public class ProductFilterVM
    {
        public IEnumerable<Category> Categories { get; set; }
        public IEnumerable<Product> Products { get; set; }

        // Danh mục đang chọn
        public int? SelectedCategoryId { get; set; }
    }
}