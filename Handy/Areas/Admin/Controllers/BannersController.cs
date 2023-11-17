using Handy.Data;
using Handy.Extensions;
using Handy.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using static Handy.Extensions.Helper;

namespace ERPManagementSystem.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class BannersController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public BannersController(ApplicationDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        public async Task<IActionResult> Index(int pg, string sortOrder, string searchString)
        {
            ViewBag.bannernam = string.IsNullOrEmpty(sortOrder) ? "prod_desc" : "";
            var banner = _context.Banners.Where(c => c.BannerStatus == "Enable");
            switch (sortOrder)
            {
                case "prod_desc":
                    banner = banner.OrderByDescending(n => n.Name);
                    break;
                default:
                    banner = banner.OrderBy(n => n.Name);
                    break;
            }
            if (!string.IsNullOrEmpty(searchString))
            {
                banner = _context.Banners.Where(c => c.BannerStatus == "Enable" && c.Name.ToLower().Contains(searchString.ToLower()));
            }
            const int pageSize = 10;
            if (pg < 1)
            {
                pg = 1;
            }
            var resCount = banner.Count();
            ViewBag.TotalRecord = resCount;
            var pager = new Pager(resCount, pg, pageSize);
            int resSkip = (pg - 1) * pageSize;
            var data = banner.Skip(resSkip).Take(pager.PageSize);
            ViewBag.Pager = pager;
            return View(await data.ToListAsync());
        }

        //AddOrEdit
        [NoDirectAccess]
        public async Task<IActionResult> AddOrEdit(Guid id)
        {
            if (id == Guid.Parse("00000000-0000-0000-0000-000000000000"))
            {
               
                return View(new Banner());
            }

            else
            {
                var banner = await _context.Banners.FindAsync(id);
                if (banner == null)
                {
                    return NotFound();
                }
                
                return View(banner);
            }

        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddOrEdit(Guid id, Banner banner, IFormFile ImagePath)
        {
            if (ModelState.IsValid)
            {
                Banner entity;
                string uniqueFileNAme = null;
                string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "uploadimages");
                if (id == Guid.Parse("00000000-0000-0000-0000-000000000000"))
                {
                    entity = new Banner();
                    entity.Id = Guid.NewGuid();
                    entity.Name = banner.Name;
                    entity.Description=banner.Description;
                    entity.BannerStatus = banner.BannerStatus;
                    
                    
                    if (ImagePath != null)
                    {
                            uniqueFileNAme = Guid.NewGuid().ToString() + "_" + ImagePath.FileName;
                            string filePath = Path.Combine(uploadsFolder, uniqueFileNAme);
                            await ImagePath.CopyToAsync(new FileStream(filePath, FileMode.Create));                                        
                            entity.ImagePath = "uploadimages/" + uniqueFileNAme;
                            
                    }
                    else
                    {
                        entity.ImagePath = "uploadimages/noimage.jpg";
                    }
                    _context.Add(entity);
                    await _context.SaveChangesAsync();
                }

                else
                {
                    try
                    {
                        entity = await _context.Banners.FindAsync(banner.Id);
                        entity.Name = banner.Name;
                        entity.Description = banner.Description;
                        entity.BannerStatus = banner.BannerStatus;
                        if (ImagePath != null)
                        {
                            uniqueFileNAme = Guid.NewGuid().ToString() + "_" + ImagePath.FileName;
                            string filePath = Path.Combine(uploadsFolder, uniqueFileNAme);
                            await ImagePath.CopyToAsync(new FileStream(filePath, FileMode.Create));
                            entity.ImagePath = "uploadimages/" + uniqueFileNAme;                         
                        }

                        _context.Update(entity);
                        await _context.SaveChangesAsync();
                    }
                    catch (DbUpdateConcurrencyException ex)
                    {

                    }
                }
                var bannerData = _context.Banners.Where(c => c.BannerStatus == "Enable");
                int pg = 1;
                const int pageSize = 10;
                if (pg < 1)
                {
                    pg = 1;
                }
                var resCount = bannerData.Count();
                ViewBag.TotalRecord = resCount;
                var pager = new Pager(resCount, pg, pageSize);
                int resSkip = (pg - 1) * pageSize;
                var data = bannerData.Skip(resSkip).Take(pager.PageSize);
                ViewBag.Pager = pager;
                return Json(new { isValid = true, html = Helper.RenderRazorViewToString(this, "_ViewAllBanner", data) });
            }
            return Json(new { isValid = false, html = Helper.RenderRazorViewToString(this, "AddOrEdit", banner) });

        }
        //delete category
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var banner = await _context.Banners.FindAsync(id);
            banner.BannerStatus = "Disable";
            await _context.SaveChangesAsync();

            return Json(new { isValid = true, html = Helper.RenderRazorViewToString(this, "_ViewAllBanner", _context.Banners.Where(c => c.BannerStatus == "Enable").ToList()) });

        }
    }
}
