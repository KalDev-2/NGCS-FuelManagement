using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Text;
using NEXT_BMS.Models;
using NEXT_BMS.ViewModels;

namespace NEXT_BMS.Areas.Administrator.Controllers
{
    [Area("Administrator")]
    public class HomeController : Controller
    {
        private readonly NEXT_BMSContext _context;
        public HomeController(NEXT_BMSContext context)
        {
            _context = context;
        }
        public ActionResult Index()
        {
            ViewBag.MenuTypeId = 1;
            var userId = HttpContext.Session.GetInt32("UserId");

            if (userId==null)
            {
                return RedirectToAction("Login", "Home", new {area= "Administrator"});
            }
            var applicationCode= HttpContext.Session.GetString("ApplicationCode");
            if (applicationCode!=null)
            {
                return RedirectToAction(applicationCode, "Dashboard");
            }
            return View();
        }

        public ActionResult ChangeLayout(string Name)
        {
            TempData["info"] = "Layout has been changed successfully.";
            HttpContext.Session.SetString("Layout",Name);
            var referer = Request.Headers["Referer"].ToString();
            if (!string.IsNullOrEmpty(referer))
            {
                return Redirect(referer);
            }
            
            return RedirectToAction("Index", "Home");
        }

        public ActionResult ChangeLang(string lang = "en_US")
        {
            //Response.Cookies["CacheLang"].Value = lang;
            HttpContext.Session.SetString("lang", lang);
            TempData["info"] = "Language has been changed successfully.";

            var referer = Request.Headers["Referer"].ToString();
            if (!string.IsNullOrEmpty(referer))
            {
                return Redirect(referer);
            }
            return RedirectToAction("Index", "Home");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}