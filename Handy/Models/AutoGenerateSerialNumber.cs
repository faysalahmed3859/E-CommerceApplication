using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Handy.Models
{
    public class AutoGenerateSerialNumber
    {
        public Guid Id { get; set; }
        [Required]
        public string ModuleName { get; set; }
        [Required]
        public int SeialNo { get; set; }
    }
}
