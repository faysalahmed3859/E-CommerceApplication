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

namespace Handy.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class BrandsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public BrandsController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(int pg, string sortOrder, string searchString)
        {
            ViewBag.brandnam = string.IsNullOrEmpty(sortOrder) ? "prod_desc" : "";
            var brand = _context.Brands.Include(d => d.SubCategory).Where(c => c.BrandStatus == "Enable");
            switch (sortOrder)
            {
                case "prod_desc":
                    brand = brand.OrderByDescending(n => n.Name);
                    break;
                default:
                    brand = brand.OrderBy(n => n.Name);
                    break;
            }
            if (!string.IsNullOrEmpty(searchString))
            {
                brand = _context.Brands.Include(d => d.SubCategory).Where(c => c.BrandStatus == "Enable" && c.Name.ToLower().Contains(searchString.ToLower()));
            }
            const int pageSize = 10;
            if (pg < 1)
            {
                pg = 1;
            }
            var resCount = brand.Count();
            ViewBag.TotalRecord = resCount;
            var pager = new Pager(resCount, pg, pageSize);
            int resSkip = (pg - 1) * pageSize;
            var data = brand.Skip(resSkip).Take(pager.PageSize);
            ViewBag.Pager = pager;
            return View(await data.ToListAsync());
        }

        //AddOrEdit
        [NoDirectAccess]
        public async Task<IActionResult> AddOrEdit(Guid id)
        {
            if (id == Guid.Parse("00000000-0000-0000-0000-000000000000"))
            {
                ViewData["SubCategory"] = new SelectList(_context.SubCategories.ToList(), "Id", "Name");
                return View(new Brand());
            }

            else
            {
                var brand = await _context.Brands.FindAsync(id);
                if (brand == null)
                {
                    return NotFound();
                }
                ViewData["SubCategory"] = new SelectList(_context.SubCategories.ToList(), "Id", "Name");
                return View(brand);
            }

        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddOrEdit(Guid id, Brand brand)
        {
            if (ModelState.IsValid)
            {
                Brand entity;
                if (id == Guid.Parse("00000000-0000-0000-0000-000000000000"))
                {
                    entity = new Brand();
                    entity.Id = Guid.NewGuid();
                    entity.Name = brand.Name;
                    entity.SubCategoryId = brand.SubCategoryId;
                    entity.BrandStatus = brand.BrandStatus;
                    _context.Add(entity);
                    await _context.SaveChangesAsync();
                }

                else
                {
                    try
                    {
                        entity = await _context.Brands.FindAsync(brand.Id);
                        entity.Name = brand.Name;
                        entity.SubCategoryId = brand.SubCategoryId;
                        entity.BrandStatus = brand.BrandStatus;
                        _context.Update(entity);
                        await _context.SaveChangesAsync();
                    }
                    catch (DbUpdateConcurrencyException ex)
                    {

                    }
                }
                var brandData = _context.Brands.Include(d => d.SubCategory).Where(c => c.BrandStatus == "Enable");
                int pg = 1;
                const int pageSize = 10;
                if (pg < 1)
                {
                    pg = 1;
                }
                var resCount = brandData.Count();
                ViewBag.TotalRecord = resCount;
                var pager = new Pager(resCount, pg, pageSize);
                int resSkip = (pg - 1) * pageSize;
                var data = brandData.Skip(resSkip).Take(pager.PageSize);
                ViewBag.Pager = pager;
                return Json(new { isValid = true, html = Helper.RenderRazorViewToString(this, "_ViewAllBrand", data) });
            }
            return Json(new { isValid = false, html = Helper.RenderRazorViewToString(this, "AddOrEdit", brand) });

        }
        //delete category
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var brand = await _context.Brands.FindAsync(id);
            brand.BrandStatus = "Disable";
            await _context.SaveChangesAsync();

            return Json(new { isValid = true, html = Helper.RenderRazorViewToString(this, "_ViewAllBrand", _context.Brands.Where(c => c.BrandStatus == "Enable").ToList()) });

        }
    }
}
