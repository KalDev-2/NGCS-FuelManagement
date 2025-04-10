using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using NEXT_BMS.Models;
using NEXT_BMS.ViewModels;

namespace NEXT_BMS.Controllers
{
    public class HomeController : Controller
    {
        private readonly NEXT_BMSContext _context;
       
        public HomeController(NEXT_BMSContext context)
        {
            _context = context;
          
        }

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";
            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
        public ActionResult Privacy()
        {
            ViewBag.Message = "Your Privacy.";

            return View();
        }

    }
}