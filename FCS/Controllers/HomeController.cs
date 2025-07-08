using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using FCS.Models;
using FCS.ViewModels;

namespace FCS.Controllers
{
    public class HomeController : Controller
    {
        private readonly FCSContext _context;
       
        public HomeController(FCSContext context)
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