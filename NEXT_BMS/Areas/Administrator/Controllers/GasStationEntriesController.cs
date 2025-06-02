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
using DocumentFormat.OpenXml.Bibliography;
using DocumentFormat.OpenXml.Wordprocessing;

namespace NEXT_BMS.Areas.Administrator.Controllers
{
    [Area("Administrator")]
    public class GasStationEntriesController : Controller
    {
        private readonly NEXT_BMSContext _context;
        public GasStationEntriesController(NEXT_BMSContext context)
        {
            _context = context;
        }
        [HttpPost]
        public IActionResult GasStationEntries()
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
                var gasStationEntries = _context.GasStationEntries
                                  .Include(g => g.GasStation)
                                  .Include(g => g.FuelType)
                                  .Include(g => g.User)
                                  .Include(g => g.Vehicle).ThenInclude(g => g.VehicleType)

                                  .Where(x => x.IsDeleted == false)
                                    .Select(s => new {
                                        s.Id,
                                        Vehicle = s.Vehicle.Name,
                                        GasStation = s.GasStation.Name,
                                        FuelType = s.FuelType.Name,
                                        s.PlateNumber,
                                        s.Quantity,
                                        User = s.User.FirstName,
                                        s.EntryDate,
                                        s.IsActive
                                    });

                var returnData = (from manudata in gasStationEntries
                                  select manudata);
                if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDirection)))
                {
                    returnData = returnData.OrderBy(sortColumn + " " + sortColumnDirection);
                }
                if (!string.IsNullOrEmpty(searchValue))
                {
                    returnData = returnData.Where(m => m.PlateNumber.Contains(searchValue));
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
            if (id == null || _context.GasStationEntries == null)
            {
                TempData["Error"] = "The information you're looking for was not found!";
                return RedirectToAction("Index");
            }

            var gasStationEntry = await _context.GasStationEntries
                .Include(g => g.FuelType)
                .Include(g => g.GasStation)
                .Include(g => g.User)
                .Include(g => g.Vehicle)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (gasStationEntry == null)
            {
                TempData["Error"] = "The information you're looking for was not found!";
                return RedirectToAction("Index");
            }

            return View(gasStationEntry);
        }

        
        [HttpPost]
        public JsonResult SetSession([FromBody] GasStationEntry gasStationEntry, Vehicle vehicle)
        {
            
            HttpContext.Session.SetInt32("VehicleTypeId", vehicle.VehicleTypeId);
            HttpContext.Session.SetInt32("FuelTypeId", gasStationEntry.FuelTypeId);
            HttpContext.Session.SetInt32("GasStationId", gasStationEntry.GasStationId);
           
            return Json(new { success = true });
        }


        [HttpPost]
        public IActionResult SearchByPlateNumber([FromBody] PlateNumberRequest request)
        {
            if (request == null || string.IsNullOrEmpty(request.PlateNumber))
            {
                return Json(new { message = "Plate number is required." });
            }

            var vehicle = _context.Vehicles
                                   .Where(v => v.PlateNumber == request.PlateNumber && !v.IsDeleted)
                                   .Select(v => new
                                   {
                                       v.Id,
                                       v.PlateNumber,
                                       v.Name
                                   })
                                   .FirstOrDefault(); 
            if (vehicle == null)
            {
                return Json(new { message = "No vehicle found for the given plate number." });
            }

            return Json(vehicle);
        }

        public class PlateNumberRequest
        {
            public string PlateNumber { get; set; }
        }

        public async Task<IActionResult> Create()
        {
            
            int? vehicleTypeId = HttpContext.Session.GetInt32("VehicleTypeId") ?? 0;
            int? fuelTypeId = HttpContext.Session.GetInt32("FuelTypeId") ?? 0;
            int? gasStationId = HttpContext.Session.GetInt32("GasStationId") ?? 0;

            ViewData["FuelTypeId"] = new SelectList(_context.FuelTypes.Where(x => x.IsDeleted == false), "Id", "Name", fuelTypeId);
            ViewData["GasStationId"] = new SelectList(_context.GasStations.Where(x => x.IsDeleted == false), "Id", "Name", gasStationId);
            ViewData["VehicleTypeId"] = new SelectList(_context.VehicleTypes.Where(x => x.IsDeleted == false), "Id", "Name", vehicleTypeId);

            var recentEntries = await _context.GasStationEntries
            .Include(g => g.GasStation).ThenInclude(g => g.EntryAttempts)
            .Include(g => g.Vehicle).ThenInclude(v => v.VehicleType)
            .Include(g => g.FuelType)
            .Include(g => g.User)
           
            .OrderByDescending(entry => entry.Id)
            .Take(10)
            .ToListAsync();


            ViewBag.RecentEntries = recentEntries;

            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,UserId,VehicleId,GasStationId,FuelTypeId,PlateNumber,EntryDate,Quantity,IsActive,IsDeleted")] GasStationEntry gasStationEntry)
        {   

            int? userId = HttpContext.Session.GetInt32("UserId");
            int? gasStationId = HttpContext.Session.GetInt32("GasStationId");
            int? fuelTypeId = HttpContext.Session.GetInt32("FuelTypeId");

            if (!userId.HasValue)
            {
                TempData["error"] = "User is not authenticated.";
                return RedirectToAction(nameof(Index));
            }

            var checkEntry =  CheckEntry(gasStationEntry.PlateNumber);

            if (checkEntry != "")
            {
               
                TempData["Warning"] = checkEntry;
                
            }
            else 
            {
                if (ModelState.IsValid)
                {
                    gasStationEntry.UserId = userId.Value;
                    gasStationEntry.FuelTypeId = fuelTypeId.Value;
                    gasStationEntry.GasStationId = gasStationId.Value;
                    gasStationEntry.IsActive = true;
                    gasStationEntry.IsDeleted = false;
                    gasStationEntry.EntryDate = DateTime.Now;

                    _context.Add(gasStationEntry);
                    await _context.SaveChangesAsync();


                    var entryAttempt = new EntryAttempt
                    {
                        UserId = gasStationEntry.UserId,
                        VehicleId = gasStationEntry.VehicleId,
                        GasStationId = gasStationEntry.GasStationId,
                        PlateNumber = gasStationEntry.PlateNumber,
                        AttemptedDate = DateOnly.FromDateTime(DateTime.Now),
                        IsActive = true,
                        IsDeleted = false
                    };

                    _context.EntryAttempts.Add(entryAttempt);
                    await _context.SaveChangesAsync();

                    TempData["Success"] = "Gas station entry saved successfully.";
                    return RedirectToAction(nameof(Create));

                }
                else
                {
                    TempData["Error"] = "An error occurred while saving the gas station entry. Please review your input.";
                }
            }

             
            ViewData["FuelTypeId"] = new SelectList(_context.FuelTypes.Where(x => x.IsDeleted == false), "Id", "Name", gasStationEntry.FuelTypeId);
            ViewData["GasStationId"] = new SelectList(_context.GasStations.Where(x => x.IsDeleted == false), "Id", "Name", gasStationEntry.GasStationId);
            ViewData["UserId"] = new SelectList(_context.Users.Where(x => x.IsDeleted == false), "Id", "FirstName", gasStationEntry.UserId);
            ViewData["VehicleId"] = new SelectList(_context.Vehicles.Where(x => x.IsDeleted == false), "Id", "Name", gasStationEntry.VehicleId);

            var recentEntries = await _context.GasStationEntries
                        .Include(g => g.GasStation).ThenInclude(g => g.EntryAttempts)
                        .Include(g => g.Vehicle).ThenInclude(v => v.VehicleType)
                        .Include(g => g.FuelType)
                        .Include(g => g.User)

                        .OrderByDescending(entry => entry.EntryDate)
                        .Take(10)
                        .ToListAsync();


            ViewBag.RecentEntries = recentEntries;

            return View(gasStationEntry);
        }


        public string CheckEntry(string platenumber)
        {
            var currentTime = DateTime.Today;
            var checkpoint = currentTime.AddDays(-3);

            var entry =  _context.GasStationEntries
                .Include(g => g.GasStation)
                .Where(x => x.EntryDate >= checkpoint && x.PlateNumber == platenumber)
                .FirstOrDefault();


            if (entry != null)
            {
                return $"This vehicle (PN = {platenumber}) was fueled on {entry.EntryDate.ToShortDateString()} at {entry.GasStation.Name}.";
            }


            return "";
        }




        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.GasStationEntries == null)
            {
                TempData["Error"] = "The information you're looking for was not found!";
                return RedirectToAction("Index");
            }

            var gasStationEntry = await _context.GasStationEntries.FindAsync(id);
            if (gasStationEntry == null)
            {
                TempData["Error"] = "The information you're looking for was not found!";
                return RedirectToAction("Index");
            }
            ViewData["FuelTypeId"] = new SelectList(_context.FuelTypes  .Where(x=>x.IsDeleted==false), "Id", "Name", gasStationEntry.FuelTypeId);
            ViewData["GasStationId"] = new SelectList(_context.GasStations  .Where(x=>x.IsDeleted==false), "Id", "Name", gasStationEntry.GasStationId);
            ViewData["UserId"] = new SelectList(_context.Users  .Where(x=>x.IsDeleted==false), "Id", "FirstName", gasStationEntry.UserId);
            ViewData["VehicleId"] = new SelectList(_context.Vehicles  .Where(x=>x.IsDeleted==false), "Id", "Name", gasStationEntry.VehicleId);
            return View(gasStationEntry);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,UserId,VehicleId,GasStationId,FuelTypeId,PlateNumber,EntryDate,Quantity,IsActive,IsDeleted")] GasStationEntry gasStationEntry)
        {
            if (id != gasStationEntry.Id)
            {
                TempData["Error"] = "The information you're looking for was not found!";
                return RedirectToAction("Index");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(gasStationEntry);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!GasStationEntryExists(gasStationEntry.Id))
                    {
                        TempData["Error"] = "The information you're looking for was not found!";
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        throw;
                    }
                }
                TempData["Success"] = "gasStationEntry saved successfully.";
                return RedirectToAction(nameof(Index));
            }
            TempData["Error"] = "An error occured while saving gasStationEntry. Please review your input.";
            ViewData["FuelTypeId"] = new SelectList(_context.FuelTypes  .Where(x=>x.IsDeleted==false), "Id", "Name", gasStationEntry.FuelTypeId);
            ViewData["GasStationId"] = new SelectList(_context.GasStations  .Where(x=>x.IsDeleted==false), "Id", "Location", gasStationEntry.GasStationId);
            ViewData["UserId"] = new SelectList(_context.Users  .Where(x=>x.IsDeleted==false), "Id", "FirstName", gasStationEntry.UserId);
            ViewData["VehicleId"] = new SelectList(_context.Vehicles  .Where(x=>x.IsDeleted==false), "Id", "Name", gasStationEntry.VehicleId);
            return View(gasStationEntry);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.GasStationEntries == null)
            {
                TempData["Error"] = "The information you're looking for was not found!";
                return RedirectToAction("Index");
            }

            var gasStationEntry = await _context.GasStationEntries
                .Include(g => g.FuelType)
                .Include(g => g.GasStation)
                .Include(g => g.User)
                .Include(g => g.Vehicle)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (gasStationEntry == null)
            {
                TempData["Error"] = "The information you're looking for was not found!";
                return RedirectToAction("Index");
            }
            return View(gasStationEntry);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.GasStationEntries == null)
            {
                return Problem("Entity set 'NEXT_BMSContext.GasStationEntries'  is null.");
            }
            var gasStationEntry = await _context.GasStationEntries.FindAsync(id);
            if (gasStationEntry != null)
            {
                 gasStationEntry.IsActive = false;
                 gasStationEntry.IsDeleted = true;
                _context.Update(gasStationEntry);
            }
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool GasStationEntryExists(int id)
        {
          return _context.GasStationEntries.Any(e => e.Id == id);
        }
    }
}
