using Handy.Data;
using Handy.Extensions;
using Handy.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static Handy.Extensions.Helper;

namespace Handy.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class AutoGenerateSerialNumbersController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AutoGenerateSerialNumbersController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(int pg)
        {
            var autoGenerateSerialNumbers = _context.AutoGenerateSerialNumbers;
            const int pageSize = 10;
            if (pg < 1)
            {
                pg = 1;
            }

            var resCount = autoGenerateSerialNumbers.Count();
            var pager = new Pager(resCount, pg, pageSize);
            int resSkip = (pg - 1) * pageSize;
            var data = autoGenerateSerialNumbers.Skip(resSkip).Take(pager.PageSize);
            ViewBag.Pager = pager;
            return View(await data.ToListAsync());
        }

        //AddOrEdit
        [NoDirectAccess]
        public async Task<IActionResult> AddOrEdit(Guid id)
        {
            if (id == Guid.Parse("00000000-0000-0000-0000-000000000000"))
            {
               
                return View(new AutoGenerateSerialNumber());
            }

            else
            {
                var autoGenerateSerialNumbers = await _context.AutoGenerateSerialNumbers.FindAsync(id);
                if (autoGenerateSerialNumbers == null)
                {
                    return NotFound();
                }
                
                return View(autoGenerateSerialNumbers);
            }

        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddOrEdit(Guid id, AutoGenerateSerialNumber autoGenerateSerialNumbers)
        {
            if (ModelState.IsValid)
            {
                AutoGenerateSerialNumber entity;
                if (id == Guid.Parse("00000000-0000-0000-0000-000000000000"))
                {
                    entity = new AutoGenerateSerialNumber();
                    entity.Id = Guid.NewGuid();
                    entity.ModuleName = autoGenerateSerialNumbers.ModuleName;
                    entity.SeialNo = autoGenerateSerialNumbers.SeialNo;                   
                    _context.Add(entity);
                    await _context.SaveChangesAsync();
                }

                else
                {
                    try
                    {
                        entity = await _context.AutoGenerateSerialNumbers.FindAsync(autoGenerateSerialNumbers.Id);
                        entity.ModuleName = autoGenerateSerialNumbers.ModuleName;
                        entity.SeialNo = autoGenerateSerialNumbers.SeialNo;                      
                        _context.Update(entity);
                        await _context.SaveChangesAsync();
                    }
                    catch (DbUpdateConcurrencyException ex)
                    {

                    }
                }
                var autoGenerateSerialNumbersData = _context.AutoGenerateSerialNumbers;
                int pg = 1;
                const int pageSize = 10;
                if (pg < 1)
                {
                    pg = 1;
                }
                var resCount = autoGenerateSerialNumbersData.Count();
                ViewBag.TotalRecord = resCount;
                var pager = new Pager(resCount, pg, pageSize);
                int resSkip = (pg - 1) * pageSize;
                var data = autoGenerateSerialNumbersData.Skip(resSkip).Take(pager.PageSize);
                ViewBag.Pager = pager;
                return Json(new { isValid = true, html = Helper.RenderRazorViewToString(this, "_ViewAllAutoGenerateSerialNumber", data) });
            }
            return Json(new { isValid = false, html = Helper.RenderRazorViewToString(this, "AddOrEdit", autoGenerateSerialNumbers) });

        }
        
    }
}
