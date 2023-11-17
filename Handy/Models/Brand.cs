using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Handy.Models
{
    public class Brand
    {
        public Guid Id { get; set; }
        [Required]
        public string Name { get; set; }
        public string BrandStatus { get; set; }
        [Required]
        public Guid SubCategoryId { get; set; }
        public SubCategory SubCategory { get; set; }

    }
}
