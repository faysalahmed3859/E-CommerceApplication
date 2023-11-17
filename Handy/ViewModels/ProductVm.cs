using Handy.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Handy.ViewModels
{
    public class ProductVm
    {
        public Guid Id { get; set; }
        [Required]
        public string Name { get; set; }
        public string Description { get; set; }
        public string ImagePath { get; set; }
        public string ProductSerial { get; set; }
        public string ProductStatus { get; set; }
        public decimal AvailablePrice { get; set; }

        public decimal PreviousPrice { get; set; }

        public string ProductColor { get; set; }

        public int Quantity { get; set; }

        public DateTime Date { get; set; }
        //Navigation
        public Guid CategoryId { get; set; }
        public Guid SubCategoryId { get; set; }
        public Guid BrandId { get; set; }
        public List<IFormFile> Galleries { get; set; }
        public List<Gallery> GalleryImagesPath { get; set; }
    }
}
