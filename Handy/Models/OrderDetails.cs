using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Handy.Models
{
    public class OrderDetails
    {
        public int Id { get; set; }
        [Display(Name = "Order")]
        public int OrderId { get; set; }
        [Display(Name = "Product")]
       

        [ForeignKey("OrderId")]
        public Order Order { get; set; }

        public Guid ProductId { get; set; }
        public Product Product { get; set; }

        //[Required(ErrorMessage = "Provied Price")]
        //[DataType(DataType.Currency)]
        public decimal Price { get; set; }

        //[Required(ErrorMessage = "Provied Quantity")]
        //[DataType(DataType.PhoneNumber)]
        public int Quantity { get; set; }

        public DateTime Date { get; set; }
    }
}
