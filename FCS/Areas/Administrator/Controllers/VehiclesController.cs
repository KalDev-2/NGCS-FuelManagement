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
using DocumentFormat.OpenXml.Wordprocessing;

namespace FCS.Areas.Administrator.Controllers
{
    [Area("Administrator")]
    public class VehiclesController : Controller
    {
        private readonly FCSContext _context;
        public VehiclesController(FCSContext context)
        {
            _context = context;
        }

        [HttpPost]
        public IActionResult GetVehicles()
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
                var returnData = (from manudata in _context.Vehicles
                                  .Include(g => g.VehicleStatus)
                                  .Include(g => g.FuelType)
                                  .Include(g => g.VehicleType)
                                  .Include(g => g.City)
                                  .Include(g => g.Code)
                                   .Where(x => x.IsDeleted == false)
                                  .Select(s => new {
                                      s.Id,
                                      s.Name,
                                      s.PlateNumber,
                                      City = s.City.Name,
                                      Code = s.Code.Name,
                                       Area = s.Area.Name,
                                      VehicleStatus = s.VehicleStatus.Name,
                                      FuelType = s.FuelType.Name,
                                      VehicleType = s.VehicleType.Name,
                                      s.IsDeleted,
                                      s.IsActive,

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
            if (id == null || _context.Vehicles == null)
            {
                TempData["Error"] = "The information you're looking for was not found!";
                return RedirectToAction("Index");
            }

            var vehicle = await _context.Vehicles
                .Include(v => v.Area)
                .Include(v => v.City)
                .Include(v => v.Code)
                .Include(v => v.VehicleStatus)
                .Include(v => v.VehicleType)
                .Include(v => v.FuelType)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (vehicle == null)
            {
                TempData["Error"] = "The information you're looking for was not found!";
                return RedirectToAction("Index");
            }

            return View(vehicle);
        }

        public IActionResult Create()
        {
            ViewData["AreaId"] = new SelectList(_context.Areas  .Where(x=>x.IsDeleted==false), "Id", "Name");
            ViewData["CityId"] = new SelectList(_context.Cities  .Where(x=>x.IsDeleted==false), "Id", "Name");
            ViewData["CodeId"] = new SelectList(_context.Codes  .Where(x=>x.IsDeleted==false), "Id", "Name");
            ViewData["VehicleStatusId"] = new SelectList(_context.VehicleStatuses  .Where(x=>x.IsDeleted==false), "Id", "Name");
            ViewData["VehicleTypeId"] = new SelectList(_context.VehicleTypes  .Where(x=>x.IsDeleted==false), "Id", "Name");
            ViewData["FuelTypeId"] = new SelectList(_context.FuelTypes  .Where(x=>x.IsDeleted==false), "Id", "Name");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(string Name,int CityId,int VehicleTypeId,int FuelTypeId ,int AreaId,int CodeId,int VehicleStatusId,string PlateNumber)
        {
            try
            {
                var vehicle = new Vehicle
                {
                    Name = Name,
                    VehicleTypeId = VehicleTypeId,
                    AreaId = AreaId,
                    CodeId = CodeId,
                    VehicleStatusId = VehicleStatusId,
                    FuelTypeId = FuelTypeId,
                    PlateNumber = PlateNumber,
                    CityId = CityId,
                    IsActive = true,
                    IsDeleted = false,

                };

                _context.Vehicles.Add(vehicle);
                _context.SaveChanges();
                TempData["Success"] = "Vehicle Created successfully!!";
                return RedirectToAction(nameof(Index));

            }

            catch (Exception ex)
            {
                TempData["Error"] = "Not Created.";
                return View();
            }
           
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Vehicles == null)
            {
                TempData["Error"] = "The information you're looking for was not found!";
                return RedirectToAction("Index");
            }

            var vehicle = await _context.Vehicles.FindAsync(id);
            if (vehicle == null)
            {
                TempData["Error"] = "The information you're looking for was not found!";
                return RedirectToAction("Index");
            }
            ViewData["AreaId"] = new SelectList(_context.Areas  .Where(x=>x.IsDeleted==false), "Id", "Name", vehicle.AreaId);
            ViewData["CityId"] = new SelectList(_context.Cities  .Where(x=>x.IsDeleted==false), "Id", "Name", vehicle.CityId);
            ViewData["CodeId"] = new SelectList(_context.Codes  .Where(x=>x.IsDeleted==false), "Id", "Name", vehicle.CodeId);
            ViewData["VehicleStatusId"] = new SelectList(_context.VehicleStatuses  .Where(x=>x.IsDeleted==false), "Id", "Name", vehicle.VehicleStatusId);
            ViewData["VehicleTypeId"] = new SelectList(_context.VehicleTypes  .Where(x=>x.IsDeleted==false), "Id", "Name", vehicle.VehicleTypeId);
            return View(vehicle);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,CityId,VehicleTypeId,AreaId,CodeId,VehicleStatusId,PlateNumber,IsActive,IsDeleted")] Vehicle vehicle)
        {
            if (id != vehicle.Id)
            {
                TempData["Error"] = "The information you're looking for was not found!";
                return RedirectToAction("Index");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(vehicle);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!VehicleExists(vehicle.Id))
                    {
                        TempData["Error"] = "The information you're looking for was not found!";
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        throw;
                    }
                }
                TempData["Success"] = "vehicle saved successfu lly.";
                return RedirectToAction(nameof(Index));
            }
            TempData["Error"] = "An error occured while saving vehicle. Please review your input.";
            ViewData["AreaId"] = new SelectList(_context.Areas  .Where(x=>x.IsDeleted==false), "Id", "Name", vehicle.AreaId);
            ViewData["CityId"] = new SelectList(_context.Cities  .Where(x=>x.IsDeleted==false), "Id", "Name", vehicle.CityId);
            ViewData["CodeId"] = new SelectList(_context.Codes  .Where(x=>x.IsDeleted==false), "Id", "Name", vehicle.CodeId);
            ViewData["VehicleStatusId"] = new SelectList(_context.VehicleStatuses  .Where(x=>x.IsDeleted==false), "Id", "Name", vehicle.VehicleStatusId);
            ViewData["VehicleTypeId"] = new SelectList(_context.VehicleTypes  .Where(x=>x.IsDeleted==false), "Id", "Name", vehicle.VehicleTypeId);
            return View(vehicle);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Vehicles == null)
            {
                TempData["Error"] = "The information you're looking for was not found!";
                return RedirectToAction("Index");
            }

            var vehicle = await _context.Vehicles
                .Include(v => v.Area)
                .Include(v => v.City)
                .Include(v => v.Code)
                .Include(v => v.VehicleStatus)
                .Include(v => v.VehicleType)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (vehicle == null)
            {
                TempData["Error"] = "The information you're looking for was not found!";
                return RedirectToAction("Index");
            }
            return View(vehicle);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Vehicles == null)
            {
                return Problem("Entity set 'FCSContext.Vehicles'  is null.");
            }
            var vehicle = await _context.Vehicles.FindAsync(id);
            if (vehicle != null)
            {
                 vehicle.IsActive = false;
                 vehicle.IsDeleted = true;
                _context.Update(vehicle);
            }
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool VehicleExists(int id)
        {
          return _context.Vehicles.Any(e => e.Id == id);
        }
    }
}
