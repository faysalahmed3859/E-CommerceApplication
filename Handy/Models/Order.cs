using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Handy.Models
{
    public class Order
    {
        public Order()
        {
            OrderDetails = new List<OrderDetails>();

        }
        public int Id { get; set; }
        //public Country Country { get; set; }

        //public int CountryID { get; set; }
        [Display(Name = "Order No")]
        public string OrderNo { get; set; }
        [Required]
        [Display(Name = "Fast Name")]
        public string Name { get; set; }
        [Display(Name = "Last Name")]
        public string LastName { get; set; }
        [Display(Name = "Company Name")]
        public string CompanyName { get; set; }
        [Display(Name = "Town/City")]
        public string TownOrCity { get; set; }
        [Display(Name = "State/Country")]
        public string StateOrCountry { get; set; }
        [Display(Name = "PostCode/Zip")]
        public string PostCodeOrZip { get; set; }
        [Required]
        [Display(Name = "Phone NUmber")]
        public string PhoneNo { get; set; }
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        public bool Status { get; set; }
        public string Address { get; set; }

        public DateTime Date { get; set; }
        public virtual List<OrderDetails> OrderDetails { get; set; }

    }
}
