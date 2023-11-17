using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Handy.Data;
using Handy.Extensions;
using Handy.Models;
using Handy.ViewModels;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using X.PagedList;
using static Handy.Extensions.Helper;

namespace Handy.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ProductsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public ProductsController(ApplicationDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        [HttpGet]
        public async Task<IActionResult> Index(int pg, string sortOrder, string searchString)
        {
            ViewBag.productnam = string.IsNullOrEmpty(sortOrder) ? "prod_desc" : "";
            var product = _context.Products.Include(c => c.Category).Include(d => d.SubCategory).Include(e => e.Brand).Where(c => c.ProductStatus == "Enable");
            switch (sortOrder)
            {
                case "prod_desc":
                    product = product.OrderByDescending(n => n.Name);
                    break;
                default:
                    product = product.OrderBy(n => n.Name);
                    break;
            }
            if (!string.IsNullOrEmpty(searchString))
            {
                product = _context.Products.Include(c => c.Category).Include(d => d.SubCategory).Include(e => e.Brand).Where(c => c.ProductStatus == "Enable" && c.Name.ToLower().Contains(searchString.ToLower()));
            }
            const int pageSize = 5;
            if (pg < 1)
            {
                pg = 1;
            }
            var resCount = product.Count();
            ViewBag.TotalRecord = resCount;
            var pager = new Pager(resCount, pg, pageSize);
            int resSkip = (pg - 1) * pageSize;
            var data = product.Skip(resSkip).Take(pager.PageSize);
            ViewBag.Pager = pager;

            return View(await data.ToListAsync());

        }
        //AddOrEdit
        [NoDirectAccess]
        public async Task<IActionResult> AddOrEdit(Guid id)
        {
            if (id == Guid.Parse("00000000-0000-0000-0000-000000000000"))
            {
                ViewData["Category"] = new SelectList(_context.Categories.ToList(), "Id", "Name");
                ViewData["SubCategory"] = new SelectList(_context.SubCategories.ToList(), "Id", "Name");
                ViewData["Brand"] = new SelectList(_context.Brands.ToList(), "Id", "Name");
                ProductVm vm = new ProductVm();
                var serialNo = _context.AutoGenerateSerialNumbers.Where(c => c.ModuleName == "PS").FirstOrDefault();
                if (serialNo == null)
                {
                    vm.ProductSerial = "N/A";

                }
                else
                {
                    vm.ProductSerial = serialNo.ModuleName + "-000" + serialNo.SeialNo.ToString();
                }


                return View(vm);
            }

            else
            {
                var product = await _context.Products.FindAsync(id);
                if (product == null)
                {

                    return NotFound();
                }
                ProductVm productVm = new ProductVm();
                var galleries = _context.Galleries.Where(c => c.ProductId == product.Id).ToList();
                productVm.Id = product.Id;
                productVm.Name = product.Name;
                productVm.ProductStatus = product.ProductStatus;
                productVm.Description = product.Description;
                productVm.CategoryId = product.CategoryId;
                productVm.SubCategoryId = product.SubCategoryId;
                productVm.AvailablePrice = product.AvailablePrice;
                productVm.ProductColor = product.ProductColor;
                productVm.PreviousPrice = product.PreviousPrice;
                //productVm.Quantity = product.Quantity;
                productVm.Date = product.Date;
                productVm.BrandId = product.BrandId;
                productVm.ImagePath = product.ImagePath;
                productVm.GalleryImagesPath = galleries;
                ViewData["Category"] = new SelectList(_context.Categories.ToList(), "Id", "Name");
                ViewData["SubCategory"] = new SelectList(_context.SubCategories.ToList(), "Id", "Name");
                ViewData["Brand"] = new SelectList(_context.Brands.ToList(), "Id", "Name");
                return View(productVm);
            }

        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddOrEdit(Guid id, ProductVm productVm)
        {
            if (ModelState.IsValid)
            {


                Product entity;
                string uniqueFileNAme = null;
                string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "uploadimages");
                if (id == Guid.Parse("00000000-0000-0000-0000-000000000000"))
                {
                    entity = new Product();
                    entity.Id = Guid.NewGuid();
                    entity.Name = productVm.Name;
                    entity.Description = productVm.Description;
                    entity.ProductStatus = productVm.ProductStatus;
                    entity.AvailablePrice = productVm.AvailablePrice;
                    entity.ProductColor = productVm.ProductColor;
                    entity.PreviousPrice = productVm.PreviousPrice;
                    entity.Quantity = productVm.Quantity;
                    entity.Date = productVm.Date;
                    entity.CategoryId = productVm.CategoryId;
                    entity.SubCategoryId = productVm.SubCategoryId;
                    entity.BrandId = productVm.BrandId;
                    entity.ProductSerial = productVm.ProductStatus;
                    _context.Add(entity);
                    await _context.SaveChangesAsync();


                    if (productVm.Galleries != null && productVm.Galleries.Count > 0)
                    {
                        foreach (IFormFile image in productVm.Galleries)
                        {
                            uniqueFileNAme = Guid.NewGuid().ToString() + "_" + image.FileName;
                            string filePath = Path.Combine(uploadsFolder, uniqueFileNAme);
                            await image.CopyToAsync(new FileStream(filePath, FileMode.Create));
                            var img = new Gallery();
                            img.ImagePath = "uploadimages/" + uniqueFileNAme;
                            img.ProductId = entity.Id;
                            _context.Galleries.Add(img);

                        }
                        IFormFile primaryImage = productVm.Galleries[0];
                        uniqueFileNAme = Guid.NewGuid().ToString() + "_" + primaryImage.FileName;
                        string primaryImgFilePath = Path.Combine(uploadsFolder, uniqueFileNAme);
                        await primaryImage.CopyToAsync(new FileStream(primaryImgFilePath, FileMode.Create));
                        entity.ImagePath = "uploadimages/" + uniqueFileNAme;
                        await _context.SaveChangesAsync();
                    }
                    else
                    {
                        entity.ImagePath = "uploadimages/noimage.jpg";
                    }
                }

                else
                {
                    try
                    {
                        entity = await _context.Products.FindAsync(productVm.Id);
                        entity.Name = productVm.Name;
                        entity.AvailablePrice = productVm.AvailablePrice;
                        entity.ProductColor = productVm.ProductColor;
                        entity.PreviousPrice = productVm.PreviousPrice;
                        entity.Quantity = productVm.Quantity;
                        entity.Date = productVm.Date;
                        entity.CategoryId = productVm.CategoryId;
                        entity.SubCategoryId = productVm.SubCategoryId;
                        entity.BrandId = productVm.BrandId;
                        entity.ProductStatus = productVm.ProductStatus;
                        if (productVm.Galleries != null && productVm.Galleries.Count > 0)
                        {
                            var oldProductImgcheck = _context.Galleries.Where(c => c.ProductId == entity.Id);
                            if (oldProductImgcheck.Count() > 0)
                            {
                                foreach (var item in oldProductImgcheck)
                                {
                                    _context.Galleries.Remove(item);
                                }
                            }

                            foreach (IFormFile image in productVm.Galleries)
                            {
                                uniqueFileNAme = Guid.NewGuid().ToString() + "_" + image.FileName;
                                string filePath = Path.Combine(uploadsFolder, uniqueFileNAme);
                                await image.CopyToAsync(new FileStream(filePath, FileMode.Create));
                                var img = new Gallery();
                                img.ImagePath = "uploadimages/" + uniqueFileNAme;
                                img.ProductId = entity.Id;
                                _context.Galleries.Add(img);

                            }
                            IFormFile primaryImage = productVm.Galleries[0];
                            uniqueFileNAme = Guid.NewGuid().ToString() + "_" + primaryImage.FileName;
                            string primaryImgFilePath = Path.Combine(uploadsFolder, uniqueFileNAme);
                            await primaryImage.CopyToAsync(new FileStream(primaryImgFilePath, FileMode.Create));
                            entity.ImagePath = "uploadimages/" + uniqueFileNAme;
                        }
                        //else
                        //{
                        //    entity.ImagePath = "uploadimages/noimage.jpg"; 
                        //}

                        _context.Update(entity);
                        await _context.SaveChangesAsync();
                    }
                    catch (DbUpdateConcurrencyException ex)
                    {

                    }
                }
                const int pageSize = 10;
                int pg = 1;
                if (pg < 1)
                {
                    pg = 1;
                }
                var resultProduct = _context.Products.Include(c => c.Category).Include(d => d.SubCategory).Include(e => e.Brand).Where(c => c.ProductStatus == "Enable").ToList();
                var resCount = resultProduct.Count();
                ViewBag.TotalRecord = resCount;
                var pager = new Pager(resCount, pg, pageSize);
                int resSkip = (pg - 1) * pageSize;
                ViewBag.Pager = pager;
                var data = resultProduct.Skip(resSkip).Take(pager.PageSize);

                return Json(new { isValid = true, html = Helper.RenderRazorViewToString(this, "_ViewAllProduct", data) });
            }
            return Json(new { isValid = false, html = Helper.RenderRazorViewToString(this, "AddOrEdit", productVm) });

        }
        [HttpGet("/Products/GetAllSubCategory")]
        public IActionResult GetAllSubCategory(Guid id)
        {
            var subCategory = _context.SubCategories.Where(c => c.CategoryId == id).ToList();
            return Json(subCategory);
        }
        [HttpGet("/Products/GetAllBrand")]
        public IActionResult GetAllBrand(Guid id)
        {
            var brand = _context.Brands.Where(c => c.SubCategoryId == id).ToList();
            return Json(brand);
        }
    }
}
