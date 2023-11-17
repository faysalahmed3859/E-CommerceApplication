//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Threading.Tasks;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.EntityFrameworkCore;


//using Handy.Models;
//using Handy.Extensions;
//using static Handy.Extensions.Helper;
//using Handy.Data;

//namespace OnlineShopFinal.Areas.Admin.Controllers
//{
//    [Area("Admin")]
//    public class CategoryController : Controller
//    {
//        private readonly ApplicationDbContext _context;
//        public CategoryController(ApplicationDbContext context)
//        {
//            _context = context;
//        }

//        public async Task<IActionResult> Index()
//        {
//            var categories = await _context.Categories.Where(c => c.IsActive == true).ToListAsync();
//            return View(categories);
//        }
//        [NoDirectAccess]
//        public async Task<IActionResult> AddOrEdit(int id)
//        {
//            if (id == 0)
//                return View(new Category());
//            else
//            {
//                var categoryModel = await _context.Categories.FindAsync(id);
//                if (categoryModel == null)
//                {
//                    return NotFound();
//                }
//                return View(categoryModel);
//            }
//        }
//        [HttpPost]
//        [ValidateAntiForgeryToken]
//        public async Task<IActionResult> AddOrEdit(int id, Category categoryModel)
//        {
//            if (ModelState.IsValid)
//            {
//                //Insert
//                if (id == 0)
//                {
//                    categoryModel.IsActive = true;
//                    _context.Add(categoryModel);
//                    await _context.SaveChangesAsync();

//                }
//                //Update
//                else
//                {
//                    try
//                    {
//                        var result = await _context.Categories.FindAsync(id);
//                        result.Name = categoryModel.Name;
//                        result.IsActive = true;
//                        _context.Update(result);
//                        await _context.SaveChangesAsync();
//                    }
//                    catch (DbUpdateConcurrencyException)
//                    {

//                    }
//                }
//                return Json(new { isValid = true, html = Helper.RenderRazorViewToString(this, "_ViewAll", _context.Categories.Where(c => c.IsActive == true).ToList()) });
//            }
//            return Json(new { isValid = false, html = Helper.RenderRazorViewToString(this, "AddOrEdit", categoryModel) });
//        }
//        public async Task<IActionResult> Delete(int id)
//        {
//            var categoryModel = await _context.Categories.FindAsync(id);
//            categoryModel.IsActive = false;
//            await _context.SaveChangesAsync();
//            return Json(new { isValid = true, html = Helper.RenderRazorViewToString(this, "_ViewAll", _context.Categories.Where(c => c.IsActive == true).ToList()) });
//        }

//        //public async Task<IActionResult> GetCategoryDetails(int id)
//        //{
//        //    var SubCategoryList = await _context.Product.Where(x => x.CategoryId == id).ToListAsync();
//        //    return View(SubCategoryList);
//        //}

//    }
//}