using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ClosedXML.Excel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Linq.Dynamic.Core;
using System.Data;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using FCS.Models;

namespace FCS.Areas.Administrator.Controllers
{
    [Area("Administrator")]
    public class MenuCategoriesController : Controller
    {
        private readonly FCSContext _context;
        public MenuCategoriesController(FCSContext context)
        {
            _context = context;
        }
        [HttpPost]
        public IActionResult GetMenuCategories()
        {
            try
            {
                var draw = Request.Form["draw"].FirstOrDefault();
                var start = Request.Form["start"].FirstOrDefault();
                var length = Request.Form["length"].FirstOrDefault();
                var sortColumn = Request.Form["columns[" + Request.Form["order[0][column]"].FirstOrDefault() + "][name]"].FirstOrDefault();
                var sortColumnDirection = Request.Form["order[0][dir]"].FirstOrDefault();
                var searchValue = Request.Form["search[value]"].FirstOrDefault();
                int pageSize = length != null ? Convert.ToInt32(length) : 0;
                int skip = start != null ? Convert.ToInt32(start) : 0;
                int recordsTotal = 0;
                var returnData = (from manudata in _context.MenuCategories
                                  .Include(a => a.MenuType)
                                   .Include(a => a.Application)
                                  .Where(x => !x.IsDeleted)
                                  .Select(s => new {
                                      s.Id,
                                      s.Name,
                                      s.Icon,
                                      MenuType = s.MenuType.Name,
                                      s.OrderNumber,
                                      Application = s.Application.Name,
                                      s.IsActive,
                                      s.IsDeleted
                                  }).OrderBy(a => a.Application).ThenBy(z => z.MenuType).ThenBy(a => a.OrderNumber)
                                  select manudata);
                if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDirection)))
                {
                    returnData = returnData.OrderBy(sortColumn + " " + sortColumnDirection);
                }
                if (!string.IsNullOrEmpty(searchValue))
                {
                    returnData = returnData.Where(m => m.Name.Contains(searchValue));
                }
                recordsTotal = returnData.Count();
                var data = returnData.Skip(skip).Take(pageSize).ToList();
                var jsonData = new { draw, recordsFiltered = recordsTotal, recordsTotal, data };
                return Ok(jsonData);
            }
            catch (Exception)
            {
                throw;
            }
        }

        // GET: MenuCategories


        public IActionResult Index()
        {
            return View();
        }
       

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.MenuCategories == null)
            {
                TempData["Error"] = "The information you're looking for was not found!";
                return RedirectToAction("Index");
            }

            var menuCategory = await _context.MenuCategories
                .Include(m => m.Application)
                .Include(m => m.MenuType)
                .Include(m => m.Parent)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (menuCategory == null)
            {
                TempData["Error"] = "The information you're looking for was not found!";return RedirectToAction("Index");
            }

            return View(menuCategory);
        }

        public IActionResult Create()
        {
            ViewData["ApplicationId"] = new SelectList(_context.Applications  .Where(x=>x.IsDeleted==false), "Id", "Id");
            ViewData["MenuTypeId"] = new SelectList(_context.MenuTypes  .Where(x=>x.IsDeleted==false), "Id", "Id");
            ViewData["ParentId"] = new SelectList(_context.MenuCategories  .Where(x=>x.IsDeleted==false), "Id", "Id");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,ApplicationId,MenuTypeId,Icon,ParentId,OrderNumber,IsActive")] MenuCategory menuCategory)
        {
            if (ModelState.IsValid)
            {
                _context.Add(menuCategory);
                await _context.SaveChangesAsync();
                TempData["Success"] = "menuCategory saved successfully.";
                return RedirectToAction(nameof(Index));
            }
            TempData["Error"] = "An error occured while saving menuCategory. Please review your input.";
            ViewData["ApplicationId"] = new SelectList(_context.Applications .Where(x=>x.IsDeleted==false), "Id", "Id", menuCategory.ApplicationId);
            ViewData["MenuTypeId"] = new SelectList(_context.MenuTypes .Where(x=>x.IsDeleted==false), "Id", "Id", menuCategory.MenuTypeId);
            ViewData["ParentId"] = new SelectList(_context.MenuCategories .Where(x=>x.IsDeleted==false), "Id", "Id", menuCategory.ParentId);
            return View(menuCategory);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.MenuCategories == null)
            {
                TempData["Error"] = "The information you're looking for was not found!";
                return RedirectToAction("Index");
            }

            var menuCategory = await _context.MenuCategories.FindAsync(id);
            if (menuCategory == null)
            {
                TempData["Error"] = "The information you're looking for was not found!";
                return RedirectToAction("Index");
            }
            ViewData["ApplicationId"] = new SelectList(_context.Applications  .Where(x=>x.IsDeleted==false), "Id", "Id", menuCategory.ApplicationId);
            ViewData["MenuTypeId"] = new SelectList(_context.MenuTypes  .Where(x=>x.IsDeleted==false), "Id", "Id", menuCategory.MenuTypeId);
            ViewData["ParentId"] = new SelectList(_context.MenuCategories  .Where(x=>x.IsDeleted==false), "Id", "Id", menuCategory.ParentId);
            return View(menuCategory);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,ApplicationId,MenuTypeId,Icon,ParentId,OrderNumber,IsActive")] MenuCategory menuCategory)
        {
            if (id != menuCategory.Id)
            {
                TempData["Error"] = "The information you're looking for was not found!";
                return RedirectToAction("Index");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(menuCategory);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MenuCategoryExists(menuCategory.Id))
                    {
                        TempData["Error"] = "The information you're looking for was not found!";
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        throw;
                    }
                }
                TempData["Success"] = "menuCategory saved successfully.";
                return RedirectToAction(nameof(Index));
            }
            TempData["Error"] = "An error occured while saving menuCategory. Please review your input.";
            ViewData["ApplicationId"] = new SelectList(_context.Applications  .Where(x=>x.IsDeleted==false), "Id", "Id", menuCategory.ApplicationId);
            ViewData["MenuTypeId"] = new SelectList(_context.MenuTypes  .Where(x=>x.IsDeleted==false), "Id", "Id", menuCategory.MenuTypeId);
            ViewData["ParentId"] = new SelectList(_context.MenuCategories  .Where(x=>x.IsDeleted==false), "Id", "Id", menuCategory.ParentId);
            return View(menuCategory);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.MenuCategories == null)
            {
                TempData["Error"] = "The information you're looking for was not found!";
                return RedirectToAction("Index");
            }

            var menuCategory = await _context.MenuCategories
                .Include(m => m.Application)
                .Include(m => m.MenuType)
                .Include(m => m.Parent)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (menuCategory == null)
            {
                TempData["Error"] = "The information you're looking for was not found!";
                return RedirectToAction("Index");
            }
            return View(menuCategory);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.MenuCategories == null)
            {
                return Problem("Entity set 'FCSContext.MenuCategories'  is null.");
            }
            var menuCategory = await _context.MenuCategories.FindAsync(id);
            if (menuCategory != null)
            {
                 menuCategory.IsActive = false;
                 menuCategory.IsDeleted = true;
                _context.Update(menuCategory);
            }
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool MenuCategoryExists(int id)
        {
          return _context.MenuCategories.Any(e => e.Id == id);
        }
    }
}
