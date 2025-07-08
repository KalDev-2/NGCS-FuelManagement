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
using DocumentFormat.OpenXml.Bibliography;
using DocumentFormat.OpenXml.Office2010.Excel;
using DocumentFormat.OpenXml.Wordprocessing;
using Microsoft.AspNetCore.Identity;

namespace FCS.Areas.Administrator.Controllers
    {
        [Area("Administrator")]
        public class GasStationsController : Controller
         {
            private readonly FCSContext _context;

        public GasStationsController(FCSContext context)
        {
            _context = context;
        }

        [HttpPost]
        public IActionResult GetGasStations ()
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
                var returnData = (from manudata in _context.GasStations
                                  
                                  .Include(x=>x.GasStationEntries)
                                  .Include(x=>x.City)
                                  .Where(x=>x.IsDeleted==false)
                                  .Select(s => new { 
                                  s.Id,
                                  s.Name,
                                  City=s.City.Name,
                                  s.Location,
                                  s.IsActive,
                                  s.IsDeleted
                                  })

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

       
        public IActionResult Index()
        {  
            
            return View();
        }
       
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.GasStations == null)
            {
                TempData["Error"] = "The information you're looking for was not found!";
                return RedirectToAction("Index");
            }

            var gasStation = await _context.GasStations
               
                .Include(g => g.City)
                .Include(g => g.GasStationFuelEntries).ThenInclude(g => g.FuelType)
                .Include(g => g.GasStationFuelEntries).ThenInclude(g => g.ActionByNavigation)
                .Include(g => g.GasStationSchedules)
                .ThenInclude(g => g.VehicleType)
                .Include(g => g.GasStationSchedules)
                .ThenInclude(g => g.PlateNumberCategory)

                .FirstOrDefaultAsync(m => m.Id == id);

            if (gasStation == null)
            {
                TempData["Error"] = "The information you're looking for was not found!";return RedirectToAction("Index");
            }
            ViewBag.GasStationId = new SelectList(_context.GasStations.Where(x => x.IsDeleted == false), "Id", "Name");
            ViewBag.FuelTypeId = new SelectList(_context.FuelTypes.Where(x => x.IsDeleted == false), "Id", "Name");


            return View(gasStation);
        }

        
        public IActionResult Create()
        {   
            ViewData["CityId"] = new SelectList(_context.Cities.Where(x => x.IsDeleted == false), "Id", "Name");
            return View();

        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(string name, string location, int cityId)
        {
            try
            {

                var gasStation = new GasStation
                {   
                    Name = name,
                    Location = location,
                    CityId = cityId,
                    IsActive = true,
                    IsDeleted = false,

                };

                _context.GasStations.Add(gasStation);
                _context.SaveChanges();

                TempData["Success"] = " GasStation Created successfully!!";
                return RedirectToAction(nameof(Index));

            }

            catch (Exception ex)
            {
                TempData["Error"] = "Not Created.";
                return View();
            }

        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult FuelEntries(int gasStationId, int fuelTypeId, int quantity)
        {
            int? userId = HttpContext.Session.GetInt32("ActionBy");
            if (!userId.HasValue)
            {
                TempData["error"] = "User is not authenticated.";
                return RedirectToAction(nameof(Index));
            }
            try
            {
                var fuelEntries = new GasStationFuelEntry
                {
                    GasStationId = gasStationId,
                    FuelTypeId = fuelTypeId,
                    Quantity = quantity,
                    ActionBy = userId.Value,
                    Date = DateTime.Now,
                    IsActive = true,
                    IsDeleted = false
                };
                _context.GasStationFuelEntries.Add(fuelEntries);
                _context.SaveChanges();

                TempData["Success"] = " GasStation Created successfully!!";
                return RedirectToAction(nameof(Details));
            }
            
            catch (Exception ex)
            {
                TempData["Error"] = "Not Created.";
                return View();
            }

        }


        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.GasStations == null)
            {
                TempData["Error"] = "The information you're looking for was not found!";
                return RedirectToAction("Index");
            }

            var gasStation = await _context.GasStations.FindAsync(id);
            if (gasStation == null)
            {
                TempData["Error"] = "The information you're looking for was not found!";
                return RedirectToAction("Index");
            }
            ViewData["CityId"] = new SelectList(_context.Cities  .Where(x=>x.IsDeleted==false), "Id", "Name", gasStation.CityId);
            return View(gasStation);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Location,CityId,IsActive,IsDeleted")] GasStation gasStation)
        {
            if (id != gasStation.Id)
            {
                TempData["Error"] = "The information you're looking for was not found!";
                return RedirectToAction("Index");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(gasStation);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!GasStationExists(gasStation.Id))
                    {
                        TempData["Error"] = "The information you're looking for was not found!";
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        throw;
                    }
                }
                TempData["Success"] = "gasStation saved successfully.";
                return RedirectToAction(nameof(Index));
            }
            TempData["Error"] = "An error occured while saving gasStation. Please review your input.";
            ViewData["CityId"] = new SelectList(_context.Cities  .Where(x=>x.IsDeleted==false), "Id", "Name", gasStation.CityId);
            return View(gasStation);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.GasStations == null)
            {
                TempData["Error"] = "The information you're looking for was not found!";
                return RedirectToAction("Index");
            }

            var gasStation = await _context.GasStations
                .Include(g => g.City)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (gasStation == null)
            {
                TempData["Error"] = "The information you're looking for was not found!";
                return RedirectToAction("Index");
            }
            return View(gasStation);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.GasStations == null)
            {
                return Problem("Entity set 'FCSContext.GasStations'  is null.");
            }
            var gasStation = await _context.GasStations.FindAsync(id);
            if (gasStation != null)
            {
                 gasStation.IsActive = false;
                 gasStation.IsDeleted = true;
                _context.Update(gasStation);
            }
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

      
        private bool GasStationExists(int id)
        {
          return _context.GasStations.Any(e => e.Id == id);
        }
    }
}
