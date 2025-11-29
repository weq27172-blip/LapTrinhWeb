using System.Collections.Generic;

namespace Web_Banh_bao_4_chang_linh_ngu_lam.Models
{
    public class ProductVM
    {
        public int ProductID { get; set; }
        public string NamePro { get; set; }
        public decimal Price { get; set; }
        public string DescriptionPro { get; set; }
        public string ImagePro { get; set; }
        public List<Review> Reviews { get; set; } = new List<Review>();

        // Điểm trung bình
        public double AverageRating
        {
            get
            {
                if (Reviews.Count == 0) return 0;
                double sum = 0;
                foreach (var r in Reviews)
                    sum += r.Rating;
                return sum / Reviews.Count;
            }
        }

    }
}