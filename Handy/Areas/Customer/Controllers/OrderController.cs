using Handy.Data;
using Handy.Models;
using Handy.PdfProvider;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Handy.Extensions;
using MigraDoc.DocumentObjectModel;
using MigraDoc.DocumentObjectModel.Tables;
using System.IO;
using MigraDoc.DocumentObjectModel.Shapes;
using System.Net.Mail;
using Handy.PdfProvider.DataModel;
using Handy.ViewModels;
using System.Net;
using MigraDoc.Rendering;

namespace Handy.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class OrderController : Controller
    {
        private readonly IPdfSharpService _pdfService;

        private ApplicationDbContext _db;
        private readonly UserManager<IdentityUser> _userManager;
        public OrderController(ApplicationDbContext db, UserManager<IdentityUser> userManager)
        {
          

            _db = db;
            _userManager = userManager;

        }
        public IActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public IActionResult Checkout()
        {
            if (User.Identity.IsAuthenticated)
            {
                var id = _userManager.GetUserId(HttpContext.User);
                var s = _db.ApplicationUsers.FirstOrDefault(c => c.Id == id);
                Order order = new Order()
                {
                    Name = s.FirstName,
                    LastName = s.LastName,
                    Email = User.Identity.Name,
                    PhoneNo = s.PhoneNumber,
                    Address = s.Address,
                    Date = DateTime.Now,
                    //Status = false
                };
                return View(order);
            }
            return View();
        }


        //Post Checkout Action Method
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Checkout")]
        public async Task<IActionResult> Checkout(Order anOrder/*, EmailModel em*/)
        {
            try
            {
                var data = new PdfData
                {
                    DocumentTitle = "Title of the MigraDoc",
                    DocumentName = "MigraDocDocName",
                    CreatedBy = "Damien",
                    Description = "some data description which I have, and want to display in the PDF file..., This is another text, what is happening here, why is this text display...",
                    DisplayListItems = new List<ItemsToDisplay>
                {
                    new ItemsToDisplay{ Id = "Print Servers", Data1= "some data", Data2 = "more data to display"},
                    new ItemsToDisplay{ Id = "Network Stuff", Data1= "IP4", Data2 = "any left"},
                    new ItemsToDisplay{ Id = "Job details", Data1= "too many", Data2 = "say no"},
                    new ItemsToDisplay{ Id = "Firewall", Data1= "what", Data2 = "Let's burn it"}

                }
                };


                var path = CreateMigraDocPdf(data);

                var stream = new FileStream(path, FileMode.Open);



                List<LineItemViewModel> products = HttpContext.Session.Get<List<LineItemViewModel>>("products");


                HttpContext.Session.Set("products", new List<LineItemViewModel>());
                //email

                string to = anOrder.Email;
                


                //string to = anOrder.Email;
                //var email = User.Identity.Name;
                //string FROM = "swapnochura111815@gmail.com";
                //string FROMNAME = "Swapnochura";
                //string SMTP_USERNAME = "AKIAYNCPPLEW5X3NFOTL";
                //string SMTP_PASSWORD = "BH/hCepUBzGDRuns5a+XvTWNZ5IQh1L1PBDw+3hKssiR";
                //string HOST = "email-smtp.us-west-2.amazonaws.com";
                //int PORT = 587;
                //string SUBJECT = "No SUBJECT";
                //string BODY = "";
                if (to != null)
                {
                    try
                    {
                        string subject = "";
                        string body = "";
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
                        smtp.Credentials = new System.Net.NetworkCredential("faysalahmed2235@gmail.com", "ppp");
                        FileStreamResult stm = File(stream, "application/pdf");
                        var b = new byte[stream.Length];
                        stream.Read(b, 0, b.Length);
                        //Get some binary data
                        byte[] data_ = b;
                        //save the data to a memory stream
                        MemoryStream ms = new MemoryStream(data_);
                        //create the attachment from a stream. Be sure to name the data with a file and 
                        //media type that is respective of the data
                        mm.Attachments.Add(new Attachment(ms, "Test.pdf", "application/pdf"));
                        // Comment or delete the next line if you are not using a configuration set
                        // message.Headers.Add("X-SES-CONFIGURATION-SET", CONFIGSET);

                        smtp.Send(mm);
                        ViewBag.Message = "The Mail Was Send Successfully";
                        ModelState.Clear();


                        //MailMessage message = new MailMessage();
                        //message.IsBodyHtml = true;
                        //message.From = new MailAddress("swapnochura111815@gmail.com", "Swapnochura");
                        //message.To.Add(new MailAddress(to));
                        //message.Subject = SUBJECT;
                        //message.Body = BODY;
                        //FileStreamResult stm = File(stream, "application/pdf");
                        //var b = new byte[stream.Length];
                        //stream.Read(b, 0, b.Length);
                        ////Get some binary data
                        //byte[] data_ = b;
                        ////save the data to a memory stream
                        //MemoryStream ms = new MemoryStream(data_);
                        ////create the attachment from a stream. Be sure to name the data with a file and 
                        ////media type that is respective of the data
                        //message.Attachments.Add(new Attachment(ms, "Test.pdf", "application/pdf"));
                        //// Comment or delete the next line if you are not using a configuration set
                        //// message.Headers.Add("X-SES-CONFIGURATION-SET", CONFIGSET);

                        ////end attach
                        //using (var client = new System.Net.Mail.SmtpClient(HOST, PORT))
                        //{
                        //    // Pass SMTP credentials
                        //    client.Credentials =
                        //        new NetworkCredential("AKIAYNCPPLEW5X3NFOTL", "BH/hCepUBzGDRuns5a+XvTWNZ5IQh1L1PBDw+3hKssiR");


                        //    // Enable SSL encryption
                        //    client.EnableSsl = true;
                        //    client.Send(message);

                        //    // Try to send the message. Show status in console.

                        //}
                        if (products != null)
                        {
                            foreach (var product in products)
                            {
                                var pro = _db.Products.FirstOrDefault(c => c.Id == product.Id);
                                int A = product.Quantity;
                                var pr = new Product()
                                {
                                    Quantity = pro.Quantity

                                };
                                int B = pr.Quantity;
                                OrderDetails orderDetails = new OrderDetails();
                                orderDetails.ProductId = product.Id;
                                orderDetails.Price = product.AvailablePrice;
                                orderDetails.Quantity = product.Quantity;
                                orderDetails.Date = DateTime.Now;
                                anOrder.OrderDetails.Add(orderDetails);

                                int n;
                                if (B >= A)
                                {
                                    n = B - A;
                                    pro.Quantity = n;
                                    _db.Products.Update(pro);
                                    await _db.SaveChangesAsync();
                                }

                                else
                                {
                                    TempData["Success"] = "Not Available!";
                                    return RedirectToAction(nameof(Checkout));
                                }



                            }
                        }

                        anOrder.OrderNo = GetOrderNo();
                        anOrder.Date = DateTime.Now;
                        _db.Orders.Add(anOrder);
                        await _db.SaveChangesAsync();


                        return RedirectToAction(nameof(Complete));
                    }
                    catch
                    {
                        TempData["Email"] = "Wrong Email Address!";

                        return RedirectToAction(nameof(Checkout));

                    }
                }
                else
                {
                    return RedirectToAction(nameof(WithoutEmail));

                }


            }
            catch
            {
                TempData["Success"] = "Try again!";

                return RedirectToAction(nameof(Checkout));

            }

        }
        private readonly string _createdDocsPath = ".\\PdfProvider\\Created";


        public string CreateMigraDocPdf(PdfData pdfData)
        {
            // Create a MigraDoc document
            Document document = CreateDocument(pdfData);
            string mdddlName = $"{_createdDocsPath}/{pdfData.DocumentName}-{DateTime.UtcNow.ToOADate()}.mdddl";
            string docName = $"{_createdDocsPath}/{pdfData.DocumentName}-{DateTime.UtcNow.ToOADate()}.pdf";

            MigraDoc.DocumentObjectModel.IO.DdlWriter.WriteToFile(document, mdddlName);

            PdfDocumentRenderer renderer = new PdfDocumentRenderer(true);
            renderer.Document = document;
            renderer.RenderDocument();
            renderer.PdfDocument.Save(docName);

            return docName;
        }

        private Document CreateDocument(PdfData pdfData)
        {
            // Create a new MigraDoc document
            Document document = new Document();
            document.Info.Title = pdfData.DocumentTitle;
            document.Info.Subject = pdfData.Description;
            document.Info.Author = pdfData.CreatedBy;

            DefineStyles(document);
            CreatePage(document);
            DemonstrateSimpleTable(document);
            DemonstrateTableAlignment(document);
            //FillContent(document);

            return document;
        }
        private void DefineStyles(Document document)
        {
            // Get the predefined style Normal.
            Style style = document.Styles["Normal"];
            // Because all styles are derived from Normal, the next line changes the 
            // font of the whole document. Or, more exactly, it changes the font of
            // all styles and paragraphs that do not redefine the font.
            style.Font.Name = "Verdana";

            style = document.Styles[StyleNames.Header];
            style.ParagraphFormat.AddTabStop("16cm", TabAlignment.Right);

            style = document.Styles[StyleNames.Footer];
            style.ParagraphFormat.AddTabStop("8cm", TabAlignment.Center);

            // Create a new style called Table based on style Normal
            style = document.Styles.AddStyle("Table", "Normal");
            style.Font.Name = "Verdana";
            style.Font.Name = "Times New Roman";
            style.Font.Size = 9;

            // Create a new style called Reference based on style Normal
            style = document.Styles.AddStyle("Reference", "Normal");
            style.ParagraphFormat.SpaceBefore = "5mm";
            style.ParagraphFormat.SpaceAfter = "5mm";
            style.ParagraphFormat.TabStops.AddTabStop("16cm", TabAlignment.Right);
        }
        private void CreatePage(Document document)
        {

            // Each MigraDoc document needs at least one section.
            Section section = document.AddSection();




            // Put a logo in the header
            //string imageFilePath = System.Web.HttpContext.Current.Server.MapPath("..") + "/images/graphics/logo.png";

            //Image image = section.Headers.Primary.AddImage(imageFilePath);
            //Image image = section.Headers.Primary.AddImage("/img.png");
            //Image image = section.AddImage("C:\\inetpub\\wwwroot\\site\\logo.png");
            //image.Height = "2.5cm";
            //image.LockAspectRatio = true;
            //image.RelativeVertical = RelativeVertical.Line;
            //image.RelativeHorizontal = RelativeHorizontal.Margin;
            //image.Top = ShapePosition.Top;
            //image.Left = ShapePosition.Right;
            //image.WrapFormat.Style = WrapStyle.Through;



            Paragraph paragraph = section.Headers.Primary.AddParagraph();
            //paragraph.Format.SpaceBefore = "5cm";
            //paragraph.Style = "Reference";
            //paragraph.AddFormattedText("LOGO", TextFormat.Bold);
            //paragraph.Format.Font.Name = "Times New Roman";
            //paragraph.Format.Font.Size = 20;
            paragraph = section.AddParagraph("Swapnochura\n");
            paragraph.Format.Font.Size = 20;
            paragraph.Format.Font.Color = Colors.DarkRed;
            paragraph.Format.SpaceBefore = "0.5cm";
            paragraph.Format.SpaceAfter = "0.5cm";
            paragraph = section.AddParagraph("123 Main Street, Bangladesh, Bd 10030");
            paragraph = section.AddParagraph(" Date: ");
            paragraph.AddDateField("dd.MM.yyyy");

            // Create footer
            paragraph = section.Footers.Primary.AddParagraph();
            paragraph.AddText("Swapnochura Inc · Sample Street 42 · 56789 Dhaka · Bangladesh");
            paragraph.Format.Font.Size = 9;
            paragraph.Format.Alignment = ParagraphAlignment.Center;

            // Create the text frame for the address
            TextFrame addressFrame = section.AddTextFrame();

            addressFrame.Height = "3.0cm";
            addressFrame.Width = "7.0cm";
            addressFrame.Left = ShapePosition.Left;
            addressFrame.RelativeHorizontal = RelativeHorizontal.Margin;
            addressFrame.Top = "5.0cm";
            addressFrame.RelativeVertical = RelativeVertical.Page;

            //// Put sender in address frame
            //paragraph = addressFrame.AddParagraph("PowerBooks Inc · Sample Street 42 · 56789 Cologne");
            //paragraph.Format.Font.Name = "Times New Roman";
            //paragraph.Format.Font.Size = 7;
            //paragraph.Format.SpaceAfter = 3;

            // Add the print date field
            paragraph = section.AddParagraph();
            paragraph.Format.SpaceBefore = "2cm";
            paragraph.Style = "Reference";
            paragraph.AddFormattedText("INVOICE", TextFormat.Bold);
            paragraph.AddTab();
            paragraph.AddText("Dhaka, ");
            paragraph.AddDateField("dd.MM.yyyy");
        }

        private void DemonstrateTableAlignment(Document document)
        {
            document.LastSection.AddParagraph();

            Table table = new Table();
            table.Format.Alignment = ParagraphAlignment.Center;
            table.Borders.Width = 0.75;
            table.Borders.Visible = false;
            table.TopPadding = 5;
            table.BottomPadding = 5;

            Column column = table.AddColumn("2cm");
            column.Format.Alignment = ParagraphAlignment.Center;

            column = table.AddColumn("5cm");
            column.Format.Alignment = ParagraphAlignment.Center;

            column = table.AddColumn("3cm");
            column.Format.Alignment = ParagraphAlignment.Center;

            column = table.AddColumn("4cm");
            column.Format.Alignment = ParagraphAlignment.Center;

            column = table.AddColumn("3cm");
            column.Format.Alignment = ParagraphAlignment.Center;
            decimal total = 0;
            List<LineItemViewModel> products = HttpContext.Session.Get<List<LineItemViewModel>>("products");
            foreach (var product in products)
            {
                decimal single = product.AvailablePrice * product.Quantity;
                total = total + single;

            }
            Row row = table.AddRow();
            row.HeadingFormat = true;
            row.Format.Alignment = ParagraphAlignment.Center;
            row.VerticalAlignment = VerticalAlignment.Top;
            Cell cell = row.Cells[3];
            cell.AddParagraph("Total Product Price :");
            cell = row.Cells[4];
            cell.AddParagraph(total.ToString());

            decimal a = (decimal)(10.00 / 100.00);

            decimal vat = (a * total);

            row = table.AddRow();
            row.Format.Alignment = ParagraphAlignment.Center;
            cell = row.Cells[3];
            cell.AddParagraph("Vat 10% :");
            cell = row.Cells[4];
            cell.AddParagraph(vat.ToString());

            row = table.AddRow();
            row.Format.Alignment = ParagraphAlignment.Center;
            cell = row.Cells[3];
            cell.AddParagraph("Shipping Cost:");
            cell = row.Cells[4];
            cell.AddParagraph("50");

            row = table.AddRow();
            row.Format.Alignment = ParagraphAlignment.Center;
            cell = row.Cells[3];
            cell.AddParagraph("Total:");
            cell = row.Cells[4];
            cell.AddParagraph((total + vat + 50).ToString());



            document.LastSection.Add(table);

        }


        public void DemonstrateSimpleTable(Document document)
        {
            Table table = document.LastSection.AddTable();
            table.Borders.Visible = true;
            table.TopPadding = 5;
            table.BottomPadding = 5;

            Column column = table.AddColumn("2cm");
            column.Format.Alignment = ParagraphAlignment.Center;

            column = table.AddColumn("6cm");
            column.Format.Alignment = ParagraphAlignment.Center;

            column = table.AddColumn("3.5cm");
            column.Format.Alignment = ParagraphAlignment.Center;

            column = table.AddColumn("2cm");
            column.Format.Alignment = ParagraphAlignment.Center;

            column = table.AddColumn("3.5cm");
            column.Format.Alignment = ParagraphAlignment.Center;

            Row row = table.AddRow();
            row.Shading.Color = Colors.PaleGoldenrod;
            row.HeadingFormat = true;
            row.Format.Alignment = ParagraphAlignment.Center;
            row.Format.Font.Bold = true;
            row.VerticalAlignment = VerticalAlignment.Top;
            List<LineItemViewModel> products = HttpContext.Session.Get<List<LineItemViewModel>>("products");
            row.Cells[0].AddParagraph("Count");
            row.Cells[1].AddParagraph("Name");
            row.Cells[2].AddParagraph("Price");
            row.Cells[3].AddParagraph("quantity");
            row.Cells[4].AddParagraph("Total Price");

            int m = 0;

            foreach (var product in products)
            {
                m++;
                row = table.AddRow();
                row.Cells[0].AddParagraph(m.ToString());
                row.Cells[1].AddParagraph(product.Name.ToString());
                row.Cells[2].AddParagraph(product.AvailablePrice.ToString());
                row.Cells[3].AddParagraph(product.Quantity.ToString());
                row.Cells[4].AddParagraph((product.AvailablePrice * product.Quantity).ToString());

            }

            //row = table.AddRow();
            //row.Cells[0].AddParagraph("1");
            //row.Cells[1].AddParagraph("Apple Phone");
            //row.Cells[2].AddParagraph("75000");
            //row.Cells[3].AddParagraph("2");
            //row.Cells[4].AddParagraph("21005000");

            //row = table.AddRow();
            //row.Cells[0].AddParagraph("2");
            //row.Cells[1].AddParagraph("Laptop ");
            //row.Cells[2].AddParagraph("73335000");
            //row.Cells[3].AddParagraph("3");
            //row.Cells[4].AddParagraph("21555000");

            //row = table.AddRow();
            //row.Cells[0].AddParagraph("3");
            //row.Cells[1].AddParagraph("ffff ");
            //row.Cells[2].AddParagraph("74555000");
            //row.Cells[3].AddParagraph("10");
            //row.Cells[4].AddParagraph("215000");

        }


        private byte[] GetData()
        {

            //string pdfFilePath = "C:\\Users\\mdzah\\source\\repos\\EmailClient\\EmailClient\\Email.pdf";

            string pdfFilePath = "wwwroot\\Email\\Email.pdf";
            byte[] bytes = System.IO.File.ReadAllBytes(pdfFilePath);
            return bytes;
        }
        public IActionResult WithoutEmail()
        {
            return View();
        }
        public string GetOrderNo()
        {
            int rowCount = _db.Orders.ToList().Count() + 1;
            return rowCount.ToString("000");
        }
        public IActionResult Complete()
        {
            return View();
        }



    }
}
