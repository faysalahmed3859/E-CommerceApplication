using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Handy.Models
{
    public class Gallery
    {
        public Guid Id { get; set; }
        public string ImagePath { get; set; }
        public string Status { get; set; }
        //Navigation
        public Guid ProductId { get; set; }
        public Product Product { get; set; }
    }
}
