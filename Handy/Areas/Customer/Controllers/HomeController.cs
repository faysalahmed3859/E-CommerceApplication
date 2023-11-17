using Handy.Data;
using Handy.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using cloudscribe.Pagination.Models;
using Microsoft.EntityFrameworkCore;
using Handy.Extensions;
using Handy.ViewModels;

namespace Handy.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;      
        public HomeController(ApplicationDbContext context)
        {
            _context = context;          
        }

        [HttpGet("/Home/GetData")]
        public ActionResult GetData(Guid id)
        {
            var Quantity = _context.Products.Where(c => c.Id == id).FirstOrDefault();
            return Json(Quantity.Quantity);

        }
        public async Task<IActionResult> GetAllProductByCategory(int pg, Guid categoryId, int pageNumber = 1, int pageSize = 8)
        {
            int ExcludeRecords = (pageSize * pageNumber) - pageSize;
            var product =  _context.Products.Where(c => c.CategoryId == categoryId)
                .Skip(ExcludeRecords)
                .Take(pageSize);
            var result = new PagedResult<Product>
            {
                Data = product.AsNoTracking().ToList(),
                TotalItems = _context.Products.Count(),
                PageNumber = pageNumber,
                PageSize = pageSize

            };
            return View(result);
        }
        //public async Task<IActionResult> GetCategoryDetails(String name)
        //{
        //    var SubCategoryList =  _context.Products.Where(x => x.Name == name).ToList();
        //    return View(SubCategoryList);
        //}

        public IActionResult Index()
        {
            var product = Getproducts();
            var banner = GetBanner();
            IndexVM model = new IndexVM();
            model.Banners = banner;
            model.Products = product;

            return View(model);
        }
        private List<Product> Getproducts()
        {
           
            return (_context.Products.Include(c => c.Category).Include(d => d.SubCategory).Include(e => e.Brand).ToList());
        }
        private List<Banner> GetBanner()
        {
            return (_context.Banners.Where(c => c.BannerStatus == "Enable").ToList());
        }

        //GET product detail acation method

        public ActionResult Detail(Guid id)
        {

            if (id == null)
            {
                return NotFound();
            }
            var product = _context.Products.Include(c => c.Category).Include(d => d.SubCategory).Include(e => e.Brand).FirstOrDefault(c => c.Id == id);

            if (product == null)
            {
                return NotFound();
            }
            ViewBag.gallery = _context.Galleries.Where(c => c.ProductId == product.Id);
            return View(product);
        }

        //POST product detail acation method
        [HttpPost]
        [ActionName("Detail")]
        public ActionResult ProductDetail(Guid id, Product pro)
        {
            List<Product> products = new List<Product>();
            if (id == null)
            {
                return NotFound();
            }

            var product = _context.Products.Include(c => c.Category).Include(d => d.SubCategory).Include(e => e.Brand).FirstOrDefault(c => c.Id == id);
            if (product == null)
            {
                return NotFound();
            }
            product.Quantity = pro.Quantity;
            product.Subtotal = pro.Quantity * product.AvailablePrice;
            products = HttpContext.Session.Get<List<Product>>("products");
            if (products == null)
            {
                products = new List<Product>();
            }
            products.Add(product);
            HttpContext.Session.Set("products", products);
            return RedirectToAction(nameof(Index));
        }



        //GET Remove action methdo
        [ActionName("Remove")]
        public IActionResult RemoveToCart(Guid? id)
        {
            List<Product> products = HttpContext.Session.Get<List<Product>>("products");
            if (products != null)
            {
                var product = products.FirstOrDefault(c => c.Id == id);
                if (product != null)
                {
                    products.Remove(product);
                    HttpContext.Session.Set("products", products);
                }
            }
            return RedirectToAction(nameof(Cart));
        }

        [HttpPost]

        public IActionResult Remove(Guid? id)
        {
            List<Product> products = HttpContext.Session.Get<List<Product>>("products");
            if (products != null)
            {
                var product = products.FirstOrDefault(c => c.Id == id);
                if (product != null)
                {
                    products.Remove(product);
                    HttpContext.Session.Set("products", products);
                }
            }
            return RedirectToAction(nameof(Cart));
        }

        //GET product Cart action method

        

        public IActionResult Shop(int pageNumber = 1, int pageSize = 8)
        {
           

            int ExcludeRecords = (pageSize * pageNumber) - pageSize;

            var product = _context.Products.Include(c => c.Category).Include(d => d.SubCategory).Include(e => e.Brand)
                .Skip(ExcludeRecords)
                .Take(pageSize);
            var result = new PagedResult<Product>
            {
                Data = product.AsNoTracking().ToList(),
                TotalItems = _context.Products.Count(),
                PageNumber = pageNumber,
                PageSize = pageSize

            };
            return View(result);
        }

        public IActionResult About()
        {          
            return View();
        }

        [HttpGet("/Home/Cart")]
        public IActionResult Cart()
        {
            List<Product> products = HttpContext.Session.Get<List<Product>>("products");
            if (products == null)
            {
                products = new List<Product>();
            }
            return View(products);
        }
        [HttpPost("/Home/Cart")]
        public IActionResult Cart(List<LineItemViewModel> CartItem, string number1, string number2, Guid number3)
        {
            int qt = Convert.ToInt32(number1);
            decimal sbtotal = Convert.ToDecimal(number2);
            Guid id = number3;
            if (number1 == null && number2 == null && number3 == null)
            {
                List<LineItemViewModel> products = HttpContext.Session.Get<List<LineItemViewModel>>("products");
                foreach (var i in CartItem)
                {
                    var item = products.SingleOrDefault(X => X.Id.Equals(i.Id));
                    item.Quantity = i.Quantity;
                    item.Subtotal = i.Subtotal;
                    HttpContext.Session.Set("products", products);

                }
                return RedirectToAction("Checkout", "Order", new { area = "Customer" });


            }
            else
            {
                List<LineItemViewModel> products = HttpContext.Session.Get<List<LineItemViewModel>>("products");
                var item = products.SingleOrDefault(X => X.Id.Equals(id));
                item.Quantity = qt;
                item.Subtotal = sbtotal;
                HttpContext.Session.Set("products", products);
                return RedirectToAction("Cart", "Home", new { area = "Customer" });
            }

        }


    }
}
