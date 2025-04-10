using NEXT_BMS.Models;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.Data;
using System.Data.Entity;
using System.Security.Claims;
using static System.Net.Mime.MediaTypeNames;

namespace NEXT_BMS.Utilities
{
    // public class UserHandler : IUserHandler

    public class UserHandler
    {
        NEXT_BMSContext _context;
        //public int _userId;
        private ISession _session;
        IHttpContextAccessor _contextAccessor;
        public UserHandler(NEXT_BMSContext context, IHttpContextAccessor httpContextAccessor)
        {
            _contextAccessor = httpContextAccessor;
            _context = context;
            _session = httpContextAccessor.HttpContext.Session;
        }
        //public int CheckLocalUser(string _uuId, string _name, string _fatherName, string _email)
        //{
        //	var user = _context.Users.FirstOrDefault(x => x.Uuid == _uuId);

        //	if (user == null)
        //	{
        //		User newUser = new User();
        //		newUser.Uuid = _uuId;
        //		newUser.Name = _name;
        //		newUser.FatherName = _fatherName;
        //		newUser.Email = _email;
        //		newUser.IsActive = false;
        //		_context.Users.Add(newUser);
        //		_context.SaveChanges();
        //		_session.SetString("UserActivated", newUser.IsActive.ToString());
        //		return newUser.Id;
        //	}
        //	else { _session.SetString("UserActivated", user.IsActive.ToString()); return user.Id; }
        //}

        //public string UserInstituteId(int _id)
        //{
        //	return string.Join(",", _context.UserInstitutes.Where(u => u.UserId == _id && u.IsActive == true).AsNoTracking().Select(x => x.InstituteId).ToList());
        //}

        //public void LoadDisplayMenus()
        //{
        //	string _roles = _session.GetString("Roles");
        //	if (_roles == "") { return; }
        //	List<string> roleIds = new List<string>();
        //	//List<DisplayMenus> Menus = new List<DisplayMenus>();
        //	List<CategoryDisplay> MenuCategories = new List<CategoryDisplay>();

        //	List<Menu> AssignedPriviledges;

        //	if (_roles != null)
        //	{
        //		if (_roles.Split(",").Contains("Super_Administrator"))
        //		{
        //			AssignedPriviledges = _context.Menus.Include(x => x.MenuCategory).Where(m => m.IsActive).ToList();
        //			MenuCategories = _context.Menus.Include(x => x.MenuCategory)
        //				.Select(x => x.MenuCategory).Distinct().Select(x => new CategoryDisplay
        //				{
        //					Id = x.Id,
        //					Name = x.Name,
        //					Icon = x.Icon,
        //					OrderNumber = x.OrderNumber,
        //					MenuTypeId = x.MenuTypeId
        //				}).Distinct().ToList();
        //              }
        //		else
        //		{
        //			var apps = _roles.Split(",");

        //			foreach (var app in apps)
        //			{
        //				roleIds.Add(app);
        //			}

        //			AssignedPriviledges = _context.Menus.Include(x => x.MenuCategory).Where(m => m.RolesMenus.Any(x => roleIds.Contains(x.Role.Name) && x.IsActive) && m.IsActive && m.IsDeleted == false).ToList();
        //			MenuCategories = _context.Menus.Include(x => x.MenuCategory).Where(m => m.RolesMenus.Any(x => roleIds.Contains(x.Role.Name) && x.IsActive) && m.IsActive && m.IsDeleted == false && m.IsMenu)
        //				.Select(x => x.MenuCategory).Distinct().Select(x => new CategoryDisplay
        //				{
        //					Id = x.Id,
        //					Name = x.Name,
        //					Icon = x.Icon,
        //					OrderNumber = x.OrderNumber,
        //					MenuTypeId = x.MenuTypeId
        //				}).Distinct().ToList();
        //		}

        //		string menuForDisplay = "";
        //		foreach (var category in MenuCategories.OrderBy(p=>p.OrderNumber))
        //		{
        //			menuForDisplay += "<li class='nav-item nav-item-submenu'><a href='#' class='nav-link'>" +
        //							  "<i class='" + category.Icon + "'></i><span>" +
        //							  category.Name + "</span></a><ul class='nav-group-sub collapse'>";

        //			foreach (var menu in AssignedPriviledges.Where(x => x.MenuCategoryId == category.Id && x.IsMenu==true).Distinct())
        //			{
        //				menuForDisplay += "<li class='nav-item'><a href='/"+menu.Controller+"/"+menu.Action+"' class='nav-link text-dark'><i class='icon-circle-small me-1'></i>"+menu.Title+"</a></li>";
        //			}
        //			menuForDisplay += "</ul></li>";

        //		}

        //		_session.SetString("DisplayMenus", menuForDisplay);
        //              var priviledge = AssignedPriviledges.Where(x => x.IsActive == true && x.IsDeleted == false)
        //                       .Select(x => new
        //                       Priviledge
        //                       {
        //                           URL = x.Controller + ":" + x.Action
        //                       }).ToList();
        //              _session.SetString("AssignedPriviledges", JsonConvert.SerializeObject(priviledge));
        //          }
        //}
    }

    internal class CategoryDisplay
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Icon { get; set; }
        public int? OrderNumber { get; set; }
        public int MenuTypeId { get; set; }
    }

    public class DisplayMenus
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Controller { get; set; }
        public string Action { get; set; }
        public int MenuCategoryId { get; set; }
    }

    public class Priviledge
    {
        public string URL { get; set; }
    }

    public interface IUserHandler
    {
        public string UserInstituteId(int _id);
        public int CheckLocalUser(string _uuId, string _name, string _fatherName, string _email);

        public void LoadDisplayMenus();
    }
}
