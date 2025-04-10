using Microsoft.AspNetCore.Mvc;
using NEXT_BMS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NEXT_BMS.Areas.Administrator.Controllers
{
    [Area("Administrator")]
    public class DashboardController : Controller
    {
       
        public ActionResult cfg01()
        {
            return View();
        }
          public ActionResult DMS01()
        {
            return View();
        }

        public ActionResult vms01()
        {
            return View();
        } 
        public ActionResult dlm01()
        {
            return View();
        } 
        public ActionResult pls001()
        {
            return View();
        }

        public ActionResult cms()
        {
            return View();
        }
    }
}