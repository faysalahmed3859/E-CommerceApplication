using Handy.Data;
using Handy.Extensions;
using Handy.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static Handy.Extensions.Helper;

namespace ERPManagementSystem.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CategoriesController : Controller
    {
        private readonly ApplicationDbContext _context;
        public CategoriesController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(int pg, string sortOrder, string searchString)
        {
            ViewBag.categorynam = string.IsNullOrEmpty(sortOrder) ? "prod_desc" : "";
            var category = _context.Categories.Where(c => c.CategoryStatus == "Enable");
            switch (sortOrder)
            {
                case "prod_desc":
                    category = category.OrderByDescending(n => n.Name);
                    break;
                default:
                    category = category.OrderBy(n => n.Name);
                    break;
            }
            if (!string.IsNullOrEmpty(searchString))
            {
                category = _context.Categories.Where(c => c.CategoryStatus == "Enable" && c.Name.ToLower().Contains(searchString.ToLower()));
            }
            const int pageSize = 5;
            if (pg < 1)
            {
                pg = 1;
            }
            var resCount = category.Count();
            ViewBag.TotalRecord = resCount;
            var pager = new Pager(resCount, pg, pageSize);
            int resSkip = (pg - 1) * pageSize;
            var data = category.Skip(resSkip).Take(pager.PageSize);
            ViewBag.Pager = pager;
            return View(await data.ToListAsync());
        }

        //AddOrEdit
        [NoDirectAccess]
        [HttpGet]
        public async Task<IActionResult> AddOrEdit(Guid id)
        {
            if (id == Guid.Parse("00000000-0000-0000-0000-000000000000"))
            {
                return View(new Category());
            }

            else
            {
                var category= await _context.Categories.FindAsync(id);
                if (category == null)
                {
                    return NotFound();
                }
                return View(category);
            }

        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddOrEdit(Guid id, Category category)
        {
            if (ModelState.IsValid)
            {
                Category entity ;
                if (id == Guid.Parse("00000000-0000-0000-0000-000000000000"))
                {
                    entity = new Category();
                    entity.Id = Guid.NewGuid();
                    entity.Name = category.Name;
                    entity.CategoryStatus = category.CategoryStatus;
                    _context.Add(entity);
                    await _context.SaveChangesAsync();
                }

                else
                {
                    try
                    {
                        entity =await _context.Categories.FindAsync(category.Id);
                        entity.Name = category.Name;
                        entity.CategoryStatus = category.CategoryStatus;
                        _context.Update(entity);
                        await _context.SaveChangesAsync();
                    }
                    catch (DbUpdateConcurrencyException ex)
                    {
                        
                    }
                }
                var categoryData = _context.Categories.Where(c => c.CategoryStatus == "Enable");
                int pg=1;
                const int pageSize = 5;
                if (pg < 1)
                {
                    pg = 1;
                }
                var resCount = categoryData.Count();
                ViewBag.TotalRecord = resCount;
                ViewBag.TotalRecord = resCount;
                var pager = new Pager(resCount, pg, pageSize);
                int resSkip = (pg - 1) * pageSize;
                var data = categoryData.Skip(resSkip).Take(pager.PageSize);
                ViewBag.Pager = pager;
                return Json(new { isValid = true, html = Helper.RenderRazorViewToString(this, "_ViewAllCategory", data) });
            }
            return Json(new { isValid = false, html = Helper.RenderRazorViewToString(this, "AddOrEdit", category) });

        }
        //delete category
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var category = await _context.Categories.FindAsync(id);
            category.CategoryStatus = "Disable";
            await _context.SaveChangesAsync();

            return Json(new { isValid = true, html = Helper.RenderRazorViewToString(this, "_ViewAllCategory", _context.Categories.Where(c=>c.CategoryStatus== "Enable").ToList()) });

        }

        public async Task<IActionResult> GetCategoryDetails(Guid id)
        {
            var SubCategoryList = await _context.Products.Where(x => x.CategoryId == id).ToListAsync();
            return View(SubCategoryList);
        }
    }
}
