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
    public class SubCategoriesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public SubCategoriesController(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index(int pg, string sortOrder, string searchString)
        {
            ViewBag.subCategorynam = string.IsNullOrEmpty(sortOrder) ? "prod_desc" : "";
            var subCategory = _context.SubCategories.Include(c => c.Category).Where(c => c.SubCategoryStatus == "Enable");
            switch (sortOrder)
            {
                case "prod_desc":
                    subCategory = subCategory.OrderByDescending(n => n.Name);
                    break;
                default:
                    subCategory = subCategory.OrderBy(n => n.Name);
                    break;
            }
            if (!string.IsNullOrEmpty(searchString))
            {
                subCategory = _context.SubCategories.Include(c => c.Category).Where(c => c.SubCategoryStatus == "Enable" && c.Name.ToLower().Contains(searchString.ToLower()));
            }
            const int pageSize = 10;
            if (pg < 1)
            {
                pg = 1;
            }
            var resCount = subCategory.Count();
            ViewBag.TotalRecord = resCount;
            var pager = new Pager(resCount, pg, pageSize);
            int resSkip = (pg - 1) * pageSize;
            var data = subCategory.Skip(resSkip).Take(pager.PageSize);
            ViewBag.Pager = pager;
            return View(await data.ToListAsync());
        }

        //AddOrEdit
        [NoDirectAccess]
        public async Task<IActionResult> AddOrEdit(Guid id)
        {
            if (id == Guid.Parse("00000000-0000-0000-0000-000000000000"))
            {
                ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name");
                return View(new SubCategory());
            }

            else
            {
                var subCategory = await _context.SubCategories.FindAsync(id);
                if (subCategory == null)
                {
                    return NotFound();
                }
                ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name");
                return View(subCategory);
            }

        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddOrEdit(Guid id, SubCategory subCategory)
        {
            if (ModelState.IsValid)
            {
                SubCategory entity;
                if (id == Guid.Parse("00000000-0000-0000-0000-000000000000"))
                {
                    entity = new SubCategory();
                    entity.Id = Guid.NewGuid();
                    entity.Name = subCategory.Name;
                    entity.CategoryId = subCategory.CategoryId;
                    entity.SubCategoryStatus = subCategory.SubCategoryStatus;
                    _context.Add(entity);
                    await _context.SaveChangesAsync();
                }

                else
                {
                    try
                    {
                        entity = await _context.SubCategories.FindAsync(subCategory.Id);
                        entity.Name = subCategory.Name;
                        entity.CategoryId = subCategory.CategoryId;
                        entity.SubCategoryStatus = subCategory.SubCategoryStatus;
                        _context.Update(entity);
                        await _context.SaveChangesAsync();
                    }
                    catch (DbUpdateConcurrencyException ex)
                    {

                    }
                }
                var subCategoryData = _context.SubCategories.Include(c => c.Category).Where(c => c.SubCategoryStatus == "Enable");
                int pg = 1;
                const int pageSize = 10;
                if (pg < 1)
                {
                    pg = 1;
                }
                var resCount = subCategoryData.Count();
                ViewBag.TotalRecord = resCount;
                var pager = new Pager(resCount, pg, pageSize);
                int resSkip = (pg - 1) * pageSize;
                var data = subCategoryData.Skip(resSkip).Take(pager.PageSize);
                ViewBag.Pager = pager;
                return Json(new { isValid = true, html = Helper.RenderRazorViewToString(this, "_ViewAllSubCategory", data) });
            }
            return Json(new { isValid = false, html = Helper.RenderRazorViewToString(this, "AddOrEdit", subCategory) });

        }
        //delete category
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var subCategory = await _context.SubCategories.FindAsync(id);
            subCategory.SubCategoryStatus = "Disable";
            await _context.SaveChangesAsync();

            return Json(new { isValid = true, html = Helper.RenderRazorViewToString(this, "_ViewAllSubCategory", _context.SubCategories.Where(c => c.SubCategoryStatus == "Enable").ToList()) });

        }
    }
}
