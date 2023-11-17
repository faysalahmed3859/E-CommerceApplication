using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Handy.Models
{
    public class detailViewModel
    {
        public Guid Id { get; set; }
       
        public string Name { get; set; }
        public string ProductSerial { get; set; }
        public string ImagePath { get; set; }
        public string Description { get; set; }
        public string ProductStatus { get; set; }
        public decimal AvailablePrice { get; set; }

        public decimal PreviousPrice { get; set; }

        public string ProductColor { get; set; }

        public int Quantity { get; set; }

        public DateTime Date { get; set; }
    }
}
