using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Linq.Dynamic.Core;
using FCS.Models;
using System.Reflection;

namespace FCS.Areas.Administrator.Controllers
{
    [Area("Administrator")]
    public class MenusController : Controller
    {
        private readonly FCSContext _context;

        public MenusController(FCSContext context)
        {
            _context = context;
        }
        [HttpPost]
        public IActionResult GetMenus ()
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
                var returnData = (from manudata in _context.Menus.Where(x=>x.IsActive)
                                   .Include(x => x.MenuCategory).ThenInclude(x => x.Application)
                                   .Select(s => new { 
                                        s.Id,
                                        s.Title,
                                        s.Action,
                                        s.Controller,
                                        MenuCategory= s.MenuCategory.Name,
                                        Application = s.MenuCategory.Application.Name,
                                        s.IsTraceable,
                                        s.IsMenu,
                                        s.IsActive
                                   })
                                  select manudata);
                if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDirection)))
                {
                    returnData = returnData.OrderBy(sortColumn + " " + sortColumnDirection);
                }
                if (!string.IsNullOrEmpty(searchValue))
                {
                    returnData = returnData.Where(m => m.Title.Contains(searchValue));
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

        // GET: Menus       
        public IActionResult Index()
        {
            return View();
        }
        //public async Task<IActionResult> Index()
        //{
            //var examAdminContext = _context.Menus.Include(m => m.MenuCategory);
            //return View(await examAdminContext.ToListAsync());
        //}


        // GET: Menus/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Menus == null)
            {
                TempData["Error"] = "The information you're looking for was not found!";return RedirectToAction("Index");
            }

            var menu = await _context.Menus
                .Include(m => m.MenuCategory)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (menu == null)
            {
                TempData["Error"] = "The information you're looking for was not found!";return RedirectToAction("Index");
            }

            return View(menu);
        }

        // GET: Menus/Create
        public IActionResult Create()
        {
            ViewData["MenuCategoryId"] = new SelectList(_context.MenuCategories, "Id", "Name");
            return View();
        }

        // POST: Menus/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,MenuCategoryId,Title,Controller,Action,IsMenu,IsTraceable,IsActive")] Menu menu)
        {
            if (ModelState.IsValid)
            {
                _context.Add(menu);
                await _context.SaveChangesAsync();
                TempData["Success"] = "menu saved successfully.";
                return RedirectToAction(nameof(Index));
            }
            TempData["Error"] = "An error occured while saving menu. Please review your input.";
            ViewData["MenuCategoryId"] = new SelectList(_context.MenuCategories, "Id", "Name", menu.MenuCategoryId);
            return View(menu);
        }

        // GET: Menus/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Menus == null)
            {
                TempData["Error"] = "The information you're looking for was not found!";return RedirectToAction("Index");
            }

            var menu = await _context.Menus.FindAsync(id);
            if (menu == null)
            {
                TempData["Error"] = "The information you're looking for was not found!";return RedirectToAction("Index");
            }
            ViewData["MenuCategoryId"] = new SelectList(_context.MenuCategories, "Id", "Name", menu.MenuCategoryId);
            return View(menu);
        }

        // POST: Menus/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,MenuCategoryId,Title,Controller,Action,IsMenu,IsTraceable,IsActive")] Menu menu)
        {
            if (id != menu.Id)
            {
                TempData["Error"] = "The information you're looking for was not found!";return RedirectToAction("Index");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(menu);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MenuExists(menu.Id))
                    {
                        TempData["Error"] = "The information you're looking for was not found!";return RedirectToAction("Index");
                    }
                    else
                    {
                        throw;
                    }
                }
                TempData["Success"] = "menu saved successfully.";
                return RedirectToAction(nameof(Index));
            }
            TempData["Error"] = "An error occured while saving menu. Please review your input.";
            ViewData["MenuCategoryId"] = new SelectList(_context.MenuCategories, "Id", "Name", menu.MenuCategoryId);
            return View(menu);
        }

        // GET: Menus/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Menus == null)
            {
                TempData["Error"] = "The information you're looking for was not found!";return RedirectToAction("Index");
            }

            var menu = await _context.Menus
                .Include(m => m.MenuCategory)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (menu == null)
            {
                TempData["Error"] = "The information you're looking for was not found!";return RedirectToAction("Index");
            }

            return View(menu);
        }

        // POST: Menus/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Menus == null)
            {
                return Problem("Entity set 'ExamAdminContext.Menus'  is null.");
            }
            var menu = await _context.Menus.FindAsync(id);
            if (menu != null)
            {
                _context.Menus.Remove(menu);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool MenuExists(int id)
        {
          return _context.Menus.Any(e => e.Id == id);
        }

        public IActionResult Generate()
        {
            var result = Assembly.GetExecutingAssembly()
            .GetTypes()
            .Where(type => typeof(Controller).IsAssignableFrom(type))
            .SelectMany(type => type.GetMethods(BindingFlags.Instance | BindingFlags.DeclaredOnly | BindingFlags.Public))
            .Where(m => !m.GetCustomAttributes(typeof(System.Runtime.CompilerServices.CompilerGeneratedAttribute), true).Any())
            .GroupBy(x => x.DeclaringType.Name)
            .Select(x => new Controlr { Name = x.Key.Replace("Controller",""), ctrlMenus = x.Select(s =>new CtrlMenu { Name = s.Name }).DistinctBy(x=>x.Name).ToList() })
            .ToList();
            ViewData["MenuCategoryId"] = new SelectList(_context.MenuCategories, "Id", "Name");
            ViewData["Menus"]=_context.Menus.ToList();
            return View(result);
        }

        public JsonResult saveMenu(int? id, int menuCategoryId, string title, string controller, string action, bool isMenu, bool isTracable = false)
        {
            var oldMenu = _context.Menus.FirstOrDefault(x => x.Controller == controller && x.Action == action);

            if (oldMenu != null)
            {
                // var editMenu = _context.Menus.Find(id);
                oldMenu.Controller = controller;
                oldMenu.Action = action;
                oldMenu.MenuCategoryId = menuCategoryId;
                oldMenu.Title = title;
                oldMenu.IsMenu = isMenu;
                oldMenu.IsTraceable = isTracable;
                oldMenu.IsActive=true;
                _context.Update(oldMenu);
            }
            else
            {
                Menu menu = new Menu
                {
                    Title = title,
                    Controller = controller,
                    Action = action,
                    MenuCategoryId = menuCategoryId,
                    IsMenu = isMenu,
                    IsTraceable = isTracable,
                    IsActive=true
                };
                _context.Menus.Add(menu);
            }

            if (action == "Index")
            {
                var controlleractionlist = Assembly.GetExecutingAssembly()
                       .GetTypes()
                       .Where(type => typeof(Controller).IsAssignableFrom(type))
                       .SelectMany(type => type.GetMethods(BindingFlags.Instance | BindingFlags.DeclaredOnly | BindingFlags.Public))
                       .Where(m => !m.GetCustomAttributes(typeof(System.Runtime.CompilerServices.CompilerGeneratedAttribute), true).Any())
                       .GroupBy(x => x.DeclaringType.Name).Where(x=>x.Key.StartsWith(controller))
                       .Select(x => new Controlr { Name = x.Key.Replace("Controller", ""), ctrlMenus = x.Select(s => new CtrlMenu { Name = s.Name }).DistinctBy(x => x.Name).ToList() })
                       .ToList();
                foreach (var sys_controller in controlleractionlist.Distinct())
                {
                    foreach (var sys_action in sys_controller.ctrlMenus)
                    {
                        if (sys_action.Name.Trim() == "DeleteConfirmed" || sys_action.Name.Trim() == "Index")
                        {
                            continue;
                        }
                        string newTitle = sys_action.Name + " " + sys_controller.Name;
                        if (newTitle.Length >= 50)
                            newTitle = newTitle.Substring(0, 50);

                        if (!_context.Menus.Any(x => x.Controller == sys_controller.Name && x.Action == sys_action.Name))
                        {
                            try
                            {
                                Menu newmenu = new Menu
                                {
                                    Title = newTitle,
                                    Controller = sys_controller.Name.Replace("Controller", ""),
                                    Action = sys_action.Name,
                                    MenuCategoryId = menuCategoryId,
                                    IsMenu = false,
                                    IsTraceable = false,
                                    IsActive = true
                                };
                                _context.Menus.Add(newmenu);
                            }
                            catch (Exception)
                            { }
                        }
                        else
                        {
                            var prevSavedMenu = _context.Menus.FirstOrDefault(x => x.Controller == sys_controller.Name && x.Action == sys_action.Name);
                            prevSavedMenu.Controller = sys_controller.Name;
                            prevSavedMenu.Action = sys_action.Name;
                            prevSavedMenu.MenuCategoryId = menuCategoryId;
                            prevSavedMenu.Title = newTitle;
                            prevSavedMenu.IsMenu = false;
                            prevSavedMenu.IsTraceable = false;
                            prevSavedMenu.IsActive = true;
                            _context.Update(prevSavedMenu);
                        }
                    }
                }
            }
            _context.SaveChanges();
            return Json("Ok");
        }
    }
    public class Controlr
    {
        public string Name { get; set; }
        public virtual ICollection<CtrlMenu> ctrlMenus { get; set; } = new List<CtrlMenu>();
    }
    public class CtrlMenu
    {
        public string Name { get; set; }
        public virtual Controlr Controlr { get; set; }
    }
}
