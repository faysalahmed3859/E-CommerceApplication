using System.ComponentModel.DataAnnotations;

namespace Handy.Models
{
    public class SendMail
    {
        [Required]
        public string To { get; set; }
        [Required]
        public string Subject { get; set; }
        [Required]
        public string Body { get; set; }
    }
}
