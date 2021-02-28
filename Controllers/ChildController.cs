using BabysFirstCalendar.DatabaseBusinessLogic;
using BabysFirstCalendar.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BabysFirstCalendar.Controllers
{
    public class ChildController : Controller
    {
        // GET: Child
        [Authorize]
        public ActionResult Child()
        {
            ViewBag.Message = "Your child's information page";
            return View();
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Child(ChildModel model)
        {
            if (ModelState.IsValid)
            {
                if (ChildProcessor.CreateChild(model.FirstName, model.LastName, model.DateOfBirth) == 1)
                {
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ModelState.AddModelError("", "Error when adding a new child");
                }
            }

            return View(model);
        }

    }
}