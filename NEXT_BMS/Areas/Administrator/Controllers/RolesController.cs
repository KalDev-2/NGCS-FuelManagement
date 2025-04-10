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


namespace NEXT_BMS.Areas.Administrator.Controllers
{
    [Area("Administrator")]
    public class RolesController : Controller
    {
        private readonly NEXT_BMSContext _context;
        public RolesController(NEXT_BMSContext context)
        {
            _context = context;
        }
        public ActionResult Index(string Sorting_Order, string Search_Data, string Filter_Value, int? Page_No,string SizeOfPage)
        {
            var roles = _context.Roles.Where(s => s.IsDeleted == false);
            
            if (Search_Data != null)
            {
                Page_No = 1;
            }
            else
            {
                Search_Data = Filter_Value;
            }
            ViewBag.FilterValue = Search_Data;
			if (!String.IsNullOrEmpty(Search_Data))
            {
                roles = roles.Where(d => d.Name.ToUpper().Contains(Search_Data.ToUpper()));
            }
			
            return View(roles);

        }

        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return BadRequest();
            }
            Role role = _context.Roles
                .Include(x => x.RolesMenus).ThenInclude(a => a.Menu).ThenInclude(x=>x.MenuCategory).ThenInclude(x => x.Application)
                .FirstOrDefault(f=>f.Id==id);
            if (role == null)
            {
                return NotFound();
            }
            return View(role);
        }

        public ActionResult Create()
        {
            ViewBag.Applications = _context.Applications
                 .Include(x => x.MenuCategories).ThenInclude(x => x.Menus)
                .Where(x => x.IsDeleted == false);
            return View();
        }

        [HttpPost]
        public ActionResult Create(string name, string selectedMenus)
        {
            Role role = new Role
            {
                Name = name
            };

            _context.Roles.Add(role);
            _context.SaveChanges();
            var RoleMenu = _context.RolesMenus;
            List<string> mns = new List<string>();
            mns.AddRange(selectedMenus.Split(',').ToList<string>());
            foreach (string rm in mns)
            {
                RolesMenu newRoleMenu = new RolesMenu();
                newRoleMenu.RoleId = role.Id;
                newRoleMenu.MenuId = int.Parse(rm);
                newRoleMenu.IsActive = true;
                RoleMenu.Add(newRoleMenu);
            }
            _context.SaveChanges();
            TempData["Success"] = "OSC";
            return RedirectToAction("Index");

        }

        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return BadRequest();
            }
            Role role = _context.Roles
                .Include(a=>a.RolesMenus)
                .FirstOrDefault(f=>f.Id==id);
          
            ViewBag.Applications = _context.Applications
                .Include(x=>x.MenuCategories).ThenInclude(x=>x.Menus)
                .Where(x => x.IsDeleted == false);
           
            if (role == null)
            {
                return NotFound();
            }

            return View(role);
        }

        [HttpPost]
        public ActionResult Edit(int Id, string name, string selectedMenus, bool IsActive)
        {
            if (ModelState.IsValid)
            {
                var roleMenus = _context.RolesMenus.Where(rm => rm.RoleId == Id);
                _context.RolesMenus.RemoveRange(roleMenus);
                _context.SaveChanges();
                var RoleMenu = _context.RolesMenus;
                List<string> mns = new List<string>();
                mns.AddRange(selectedMenus.Split(',').ToList<string>());
                foreach (string rm in mns)
                {
                    RolesMenu newRoleMenu = new RolesMenu();
                    newRoleMenu.RoleId = Id;
                    newRoleMenu.MenuId = int.Parse(rm);
                    newRoleMenu.IsActive = true;
                    RoleMenu.Add(newRoleMenu);
                }
                Role role = _context.Roles.Find(Id);
                role.Name = name;
                role.IsActive = IsActive;
                _context.Update(role);
                _context.SaveChanges();
                TempData["Success"] = "OSC";
                return RedirectToAction("Index");
            }
            ViewBag.Applications = _context.Applications
                 .Include(x => x.MenuCategories).ThenInclude(x => x.Menus)
                .Where(x => x.IsDeleted == false);
            return View();
        }

        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return BadRequest();
            }
            Role role = _context.Roles
                .Include(x => x.RolesMenus).ThenInclude(a => a.Menu).ThenInclude(x => x.MenuCategory).ThenInclude(x => x.Application)
                .FirstOrDefault(f => f.Id == id);
            if (role == null)
            {
                return NotFound();
            }
            return View(role);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Role role = _context.Roles.Find(id);
            role.IsActive=false;
            role.IsDeleted=true;
            _context.Update(role);
            _context.SaveChanges();
            TempData["Success"] = "OSC";
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _context.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
