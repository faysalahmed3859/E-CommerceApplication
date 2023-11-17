using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Handy.Models
{
    public class Banner
    {
        public Guid Id { get; set; }
        [Required]
        public string Name { get; set; }
        public string ImagePath { get; set; }
        public string Description { get; set; }
        public string BannerStatus { get; set; }
    }
}
