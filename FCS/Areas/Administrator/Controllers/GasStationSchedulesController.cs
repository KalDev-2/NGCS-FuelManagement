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
using Microsoft.AspNetCore.Http;

namespace FCS.Areas.Administrator.Controllers
{
    [Area("Administrator")]
    public class GasStationSchedulesController : Controller
    {
        private readonly FCSContext _context;
        public GasStationSchedulesController(FCSContext context)
        {
            _context = context;
        }
        [HttpPost]
        public IActionResult GasStationSchedules()
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
                var returnData = (from manudata in _context.GasStationSchedules
                                  .Include(g => g.ActionByNavigation)

                                  .Include(g => g.City)
                                  .Include(g => g.PlateNumberCategory)
                                  .Include(g => g.VehicleType)
                                  .Include(g => g.FuelType)
                                  .Include(g => g.GasStation)
                                  .Where(x=>x.IsDeleted==false)
                                  .Select(s => new {
                                      s.Id,
                                      City = s.City.Name,
                                      FuelType = s.FuelType.Name,
                                      VehicleType = s.VehicleType.Name,
                                      PlateNumberCategory = s.PlateNumberCategory.Name,
                                      GasStation = s.GasStation.Name,
                                      s.ScheduleDate,
                                      s.IsActive

                                  })
                                  select manudata);
                if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDirection)))
                {
                    returnData = returnData.OrderBy(sortColumn + " " + sortColumnDirection);
                }

                if (searchValue != null)
                {
                    returnData = returnData.Where(m => m.City.Contains(searchValue) || m.FuelType.Contains(searchValue) || m.VehicleType.Contains(searchValue) || m.PlateNumberCategory.Contains(searchValue) || m.GasStation.Contains(searchValue));
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

        public IActionResult Index()
        {   

            return View();
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.GasStationSchedules == null)
            {
                TempData["Error"] = "The information you're looking for was not found!";
                return RedirectToAction("Index");
            }

            var gasStationSchedule = await _context.GasStationSchedules
                .Include(g => g.ActionByNavigation)
                .Include(g => g.City)
                .Include(g => g.FuelType)
                .Include(g => g.GasStation)
                .Include(g => g.PlateNumberCategory)
                .Include(g => g.VehicleType)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (gasStationSchedule == null)
            {
                TempData["Error"] = "The information you're looking for was not found!";
                return RedirectToAction("Index");
            }

            return View(gasStationSchedule);
        }

        public IActionResult Create()
        {
            ViewData["ActionBy"] = new SelectList(_context.Users  .Where(x=>x.IsDeleted==false), "Id", "FirstName");
            ViewData["CityId"] = new SelectList(_context.Cities  .Where(x=>x.IsDeleted==false), "Id", "Name");
            ViewData["FuelTypeId"] = new SelectList(_context.FuelTypes  .Where(x=>x.IsDeleted==false), "Id", "Name");
            ViewData["GasStationId"] = new SelectList(_context.GasStations  .Where(x=>x.IsDeleted==false), "Id", "Name");
            ViewData["PlateNumberCategoryId"] = new SelectList(_context.PlateNumberCategories  .Where(x=>x.IsDeleted==false), "Id", "Name");
            ViewData["VehicleTypeId"] = new SelectList(_context.VehicleTypes  .Where(x=>x.IsDeleted==false), "Id", "Name");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,CityId,GasStationId,VehicleTypeId,FuelTypeId,PlateNumberCategoryId,ScheduleDate,IsActive,IsDeleted")] GasStationSchedule gasStationSchedule)
        {
            int? userId = HttpContext.Session.GetInt32("UserId");
            if(userId == null)
            {
                TempData["Error"] = "User session expired. Please log in again.";
                return RedirectToAction("LogIn", "Account");
            }
            if (ModelState.IsValid)
            {
                gasStationSchedule.ActionBy= userId.Value;
                gasStationSchedule.IsDeleted = false;
                _context.Add(gasStationSchedule);
                await _context.SaveChangesAsync();
                TempData["Success"] = "GasStationSchedule saved successfully.";
                return RedirectToAction(nameof(Index));
            }
            TempData["Error"] = "An error occured. Please review your input.";
            ViewData["ActionBy"] = new SelectList(_context.Users .Where(x=>x.IsDeleted==false), "Id", "FirstName", gasStationSchedule.ActionBy);
            ViewData["CityId"] = new SelectList(_context.Cities .Where(x=>x.IsDeleted==false), "Id", "Name", gasStationSchedule.CityId);
            ViewData["FuelTypeId"] = new SelectList(_context.FuelTypes .Where(x=>x.IsDeleted==false), "Id", "Name", gasStationSchedule.FuelTypeId);
            ViewData["GasStationId"] = new SelectList(_context.GasStations .Where(x=>x.IsDeleted==false), "Id", "Name", gasStationSchedule.GasStationId);
            ViewData["PlateNumberCategoryId"] = new SelectList(_context.PlateNumberCategories .Where(x=>x.IsDeleted==false), "Id", "Name", gasStationSchedule.PlateNumberCategoryId);
            ViewData["VehicleTypeId"] = new SelectList(_context.VehicleTypes .Where(x=>x.IsDeleted==false), "Id", "Name", gasStationSchedule.VehicleTypeId);
            return View(gasStationSchedule);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.GasStationSchedules == null)
            {
                TempData["Error"] = "The information you're looking for was not found!";
                return RedirectToAction("Index");
            }

            var gasStationSchedule = await _context.GasStationSchedules.FindAsync(id);
            if (gasStationSchedule == null)
            {
                TempData["Error"] = "The information you're looking for was not found!";
                return RedirectToAction("Index");
            }

            ViewData["ActionBy"] = new SelectList(_context.Users  .Where(x=>x.IsDeleted==false), "Id", "FirstName", gasStationSchedule.ActionBy);
            ViewData["CityId"] = new SelectList(_context.Cities  .Where(x=>x.IsDeleted==false), "Id", "Name", gasStationSchedule.CityId);
            ViewData["FuelTypeId"] = new SelectList(_context.FuelTypes  .Where(x=>x.IsDeleted==false), "Id", "Name", gasStationSchedule.FuelTypeId);
            ViewData["GasStationId"] = new SelectList(_context.GasStations  .Where(x=>x.IsDeleted==false), "Id", "Location", gasStationSchedule.GasStationId);
            ViewData["PlateNumberCategoryId"] = new SelectList(_context.PlateNumberCategories  .Where(x=>x.IsDeleted==false), "Id", "Name", gasStationSchedule.PlateNumberCategoryId);
            ViewData["VehicleTypeId"] = new SelectList(_context.VehicleTypes  .Where(x=>x.IsDeleted==false), "Id", "Name", gasStationSchedule.VehicleTypeId);
            return View(gasStationSchedule);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,CityId,GasStationId,VehicleTypeId,FuelTypeId,PlateNumberCategoryId,ScheduleDate,ActionBy,IsActive,IsDeleted")] GasStationSchedule gasStationSchedule)
        {
            if (id != gasStationSchedule.Id)
            {
                TempData["Error"] = "The information you're looking for was not found!";
                return RedirectToAction("Index");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(gasStationSchedule);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!GasStationScheduleExists(gasStationSchedule.Id))
                    {
                        TempData["Error"] = "The information you're looking for was not found!";
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        throw;
                    }
                }
                TempData["Success"] = "gasStationSchedule saved successfully.";
                return RedirectToAction(nameof(Index));
            }
            TempData["Error"] = "An error occured while saving gasStationSchedule. Please review your input.";
            ViewData["ActionBy"] = new SelectList(_context.Users  .Where(x=>x.IsDeleted==false), "Id", "FirstName", gasStationSchedule.ActionBy);
            ViewData["CityId"] = new SelectList(_context.Cities  .Where(x=>x.IsDeleted==false), "Id", "Name", gasStationSchedule.CityId);
            ViewData["FuelTypeId"] = new SelectList(_context.FuelTypes  .Where(x=>x.IsDeleted==false), "Id", "Name", gasStationSchedule.FuelTypeId);
            ViewData["GasStationId"] = new SelectList(_context.GasStations  .Where(x=>x.IsDeleted==false), "Id", "Location", gasStationSchedule.GasStationId);
            ViewData["PlateNumberCategoryId"] = new SelectList(_context.PlateNumberCategories  .Where(x=>x.IsDeleted==false), "Id", "Name", gasStationSchedule.PlateNumberCategoryId);
            ViewData["VehicleTypeId"] = new SelectList(_context.VehicleTypes  .Where(x=>x.IsDeleted==false), "Id", "Name", gasStationSchedule.VehicleTypeId);
            return View(gasStationSchedule);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.GasStationSchedules == null)
            {
                TempData["Error"] = "The information you're looking for was not found!";
                return RedirectToAction("Index");
            }

            var gasStationSchedule = await _context.GasStationSchedules
                .Include(g => g.ActionByNavigation)
                .Include(g => g.City)
                .Include(g => g.FuelType)
                .Include(g => g.GasStation)
                .Include(g => g.PlateNumberCategory)
                .Include(g => g.VehicleType)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (gasStationSchedule == null)
            {
                TempData["Error"] = "The information you're looking for was not found!";
                return RedirectToAction("Index");
            }
            return View(gasStationSchedule);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.GasStationSchedules == null)
            {
                return Problem("Entity set 'FCSContext.GasStationSchedules'  is null.");
            }
            var gasStationSchedule = await _context.GasStationSchedules.FindAsync(id);
            if (gasStationSchedule != null)
            {
                 gasStationSchedule.IsActive = false;
                 gasStationSchedule.IsDeleted = true;
                _context.Update(gasStationSchedule);
            }
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        } 

        private bool GasStationScheduleExists(int id)
        {
          return _context.GasStationSchedules.Any(e => e.Id == id);
        }
    }
}
