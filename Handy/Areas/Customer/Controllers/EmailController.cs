using Handy.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;

namespace Handy.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class EmailController : Controller
    {
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Index(SendMail em)
        {
            if (!ModelState.IsValid) return View();
            try
            {
                string to = em.To;
                string subject = em.Subject;
                string body = em.Body;
                MailMessage mm = new MailMessage();
                mm.To.Add(to);
                mm.Subject = subject;
                mm.Body = body;
                mm.From = new MailAddress("faysalahmed2235@gmail.com");
                mm.IsBodyHtml = false;
                SmtpClient smtp = new SmtpClient("smtp.gmail.com");
                smtp.Port = 587;
                smtp.UseDefaultCredentials = true;
                smtp.EnableSsl = true;
                smtp.Credentials = new System.Net.NetworkCredential("faysalahmed2235@gmail.com", "pppp");
                smtp.Send(mm);
                ViewBag.Message = "The Mail Was Send Successfully";
                ModelState.Clear();
            }
            catch (Exception ex)
            {
                //If any error occured it will show
                ViewBag.Message = ex.Message.ToString();
            }

            return View();
        }
    }
}
