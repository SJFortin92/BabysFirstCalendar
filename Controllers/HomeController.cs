using BabysFirstCalendar.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using static BabysFirstCalendar.DatabaseBusinessLogic.AccountProcessor;

namespace BabysFirstCalendar.Controllers
{
    public class HomeController : Controller
    {
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

        //Function to retrieve memories from the database
        public JsonResult GetMemories()
        {
            using (NewbornCalendarModelEntities dc = new NewbornCalendarModelEntities())
            {
                var memories = dc.Notes.ToList();
                return new JsonResult { Data = memories, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
            }
        }
    }

}