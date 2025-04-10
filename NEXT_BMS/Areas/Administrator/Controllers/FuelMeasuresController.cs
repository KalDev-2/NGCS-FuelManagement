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
using NEXT_BMS.Models;
using Org.BouncyCastle.Bcpg;

namespace NEXT_BMS.Areas.Administrator.Controllers
{
    [Area("Administrator")]
    public class FuelMeasuresController : Controller
    {
        private readonly NEXT_BMSContext _context;
        public FuelMeasuresController(NEXT_BMSContext context)
        {
            _context = context;
        }
        [HttpPost]
        public IActionResult FuelMeasures ()
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
                var returnData = (from manudata in _context.FuelMeasures
                                  .Include(g => g.ActionByNavigation)
                                  .Include(g => g.ApprovedByNavigation)
                                  .Include(g => g.GasStation)
                                  .Include(g => g.FuelType)

                                  .Where(x=>x.IsDeleted==false)
                                   .Select(s => new {
                                       s.Id,
                                      
                                       FuelType = s.FuelType.Name,
                                       GasStation = s.GasStation.Name,
                                       s.Remark,
                                       s.Quantity,
                                       s.Date,
                                       s.IsActive,
                                      ActionBy =$"{s.ActionByNavigation.FirstName} {s.ActionByNavigation.MiddleName} "  

                                   })
                                  select manudata);
                if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDirection)))
                {
                    returnData = returnData.OrderBy(sortColumn + " " + sortColumnDirection);
                }
                //if (!string.IsNullOrEmpty(searchValue))
                //{
                //    returnData = returnData.Where(m => m.GasStation.Name.Contains(searchValue));
                //}
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

        // GET: Administrator/FuelMeasures

       
        public IActionResult Index()
        {
            return View();
        }
        //public async Task<IActionResult> Index()
        //{
            //var nEXT_BMSContext = _context.FuelMeasures.Include(f => f.ActionByNavigation).Include(f => f.ApprovedByNavigation).Include(f => f.FuelType).Include(f => f.GasStation);
            //return View(await nEXT_BMSContext.ToListAsync());
        //}

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.FuelMeasures == null)
            {
                TempData["Error"] = "The information you're looking for was not found!";
                return RedirectToAction("Index");
            }

            var fuelMeasure = await _context.FuelMeasures
                .Include(f => f.ActionByNavigation)
                .Include(f => f.ApprovedByNavigation)
                .Include(f => f.FuelType)
                .Include(f => f.GasStation)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (fuelMeasure == null)
            {
                TempData["Error"] = "The information you're looking for was not found!";return RedirectToAction("Index");
            }

            return View(fuelMeasure);
        }

        public IActionResult Create()
        {
            ViewData["ActionBy"] = new SelectList(_context.Users  .Where(x=>x.IsDeleted==false), "Id", "FirstName");
            ViewData["ApprovedBy"] = new SelectList(_context.Users  .Where(x=>x.IsDeleted==false), "Id", "FirstName");
            ViewData["FuelTypeId"] = new SelectList(_context.FuelTypes  .Where(x=>x.IsDeleted==false), "Id", "Name");
            ViewData["GasStationId"] = new SelectList(_context.GasStations  .Where(x=>x.IsDeleted==false), "Id", "Name");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,GasStationId,FuelTypeId,Quantity")] FuelMeasure fuelMeasure)
        {
            int? userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                TempData["Error"] = "User session expired. Please log in again.";
                return RedirectToAction("LogIn", "Account");
            }
            if (ModelState.IsValid)
            {
                fuelMeasure.Date = DateTime.Now;
                fuelMeasure.ActionBy = userId.Value;
                fuelMeasure.IsActive = true;
                fuelMeasure.IsDeleted = false;
                _context.Add(fuelMeasure);
                await _context.SaveChangesAsync();
                TempData["Success"] = "fuelMeasure saved successfully.";
                return RedirectToAction(nameof(Index));
            }
            TempData["Error"] = "An error occured while saving fuelMeasure. Please review your input.";
            ViewData["ActionBy"] = new SelectList(_context.Users .Where(x=>x.IsDeleted==false), "Id", "FirstName", fuelMeasure.ActionBy);
            ViewData["ApprovedBy"] = new SelectList(_context.Users .Where(x=>x.IsDeleted==false), "Id", "FirstName", fuelMeasure.ApprovedBy);
            ViewData["FuelTypeId"] = new SelectList(_context.FuelTypes .Where(x=>x.IsDeleted==false), "Id", "Name", fuelMeasure.FuelTypeId);
            ViewData["GasStationId"] = new SelectList(_context.GasStations .Where(x=>x.IsDeleted==false), "Id", "Name", fuelMeasure.GasStationId);
            return View(fuelMeasure);
        }

        [HttpPost]
        public IActionResult Approve(int id, string Remark)
        {
            int? userId = HttpContext.Session.GetInt32("UserId");
            string userRole = HttpContext.Session.GetString("RoleName");

            if (userId == null || userRole != "Admin")
            {
                TempData["Error"] = "You are not authorized to perform this action.";
                return RedirectToAction("Details", new { id = id });
            }

            var fuelMeasure = _context.FuelMeasures.FirstOrDefault(f => f.Id == id);
            if (fuelMeasure == null)
            {
                TempData["Error"] = "Fuel measure not found.";
                return RedirectToAction("Index");
            }

            fuelMeasure.ApprovedBy = userId.Value;
            fuelMeasure.ApprovedDate = DateTime.Now;
            fuelMeasure.Remark = Remark; 

            _context.Update(fuelMeasure);
            _context.SaveChanges();

            TempData["Success"] = "Fuel measure approved.";
            return RedirectToAction("Details", new { id = id });
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.FuelMeasures == null)
            {
                TempData["Error"] = "The information you're looking for was not found!";
                return RedirectToAction("Index");
            }

            var fuelMeasure = await _context.FuelMeasures.FindAsync(id);
            if (fuelMeasure == null)
            {
                TempData["Error"] = "The information you're looking for was not found!";
                return RedirectToAction("Index");
            }
            ViewData["ActionBy"] = new SelectList(_context.Users  .Where(x=>x.IsDeleted==false), "Id", "FirstName", fuelMeasure.ActionBy);
            ViewData["ApprovedBy"] = new SelectList(_context.Users  .Where(x=>x.IsDeleted==false), "Id", "FirstName", fuelMeasure.ApprovedBy);
            ViewData["FuelTypeId"] = new SelectList(_context.FuelTypes  .Where(x=>x.IsDeleted==false), "Id", "Name", fuelMeasure.FuelTypeId);
            ViewData["GasStationId"] = new SelectList(_context.GasStations  .Where(x=>x.IsDeleted==false), "Id", "Location", fuelMeasure.GasStationId);
            return View(fuelMeasure);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,GasStationId,FuelTypeId,Quantity,Date,ActionBy,ApprovedBy,IsActive,IsDeleted")] FuelMeasure fuelMeasure)
        {
            if (id != fuelMeasure.Id)
            {
                TempData["Error"] = "The information you're looking for was not found!";
                return RedirectToAction("Index");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(fuelMeasure);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!FuelMeasureExists(fuelMeasure.Id))
                    {
                        TempData["Error"] = "The information you're looking for was not found!";
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        throw;
                    }
                }
                TempData["Success"] = "fuelMeasure saved successfully.";
                return RedirectToAction(nameof(Index));
            }
            TempData["Error"] = "An error occured while saving fuelMeasure. Please review your input.";
            ViewData["ActionBy"] = new SelectList(_context.Users  .Where(x=>x.IsDeleted==false), "Id", "FirstName", fuelMeasure.ActionBy);
            ViewData["ApprovedBy"] = new SelectList(_context.Users  .Where(x=>x.IsDeleted==false), "Id", "FirstName", fuelMeasure.ApprovedBy);
            ViewData["FuelTypeId"] = new SelectList(_context.FuelTypes  .Where(x=>x.IsDeleted==false), "Id", "Name", fuelMeasure.FuelTypeId);
            ViewData["GasStationId"] = new SelectList(_context.GasStations  .Where(x=>x.IsDeleted==false), "Id", "Location", fuelMeasure.GasStationId);
            return View(fuelMeasure);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.FuelMeasures == null)
            {
                TempData["Error"] = "The information you're looking for was not found!";
                return RedirectToAction("Index");
            }

            var fuelMeasure = await _context.FuelMeasures
                .Include(f => f.ActionByNavigation)
                .Include(f => f.ApprovedByNavigation)
                .Include(f => f.FuelType)
                .Include(f => f.GasStation)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (fuelMeasure == null)
            {
                TempData["Error"] = "The information you're looking for was not found!";
                return RedirectToAction("Index");
            }
            return View(fuelMeasure);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.FuelMeasures == null)
            {
                return Problem("Entity set 'NEXT_BMSContext.FuelMeasures'  is null.");
            }
            var fuelMeasure = await _context.FuelMeasures.FindAsync(id);
            if (fuelMeasure != null)
            {
                 fuelMeasure.IsActive = false;
                 fuelMeasure.IsDeleted = true;
                _context.Update(fuelMeasure);
            }
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool FuelMeasureExists(int id)
        {
          return _context.FuelMeasures.Any(e => e.Id == id);
        }
    }
}
