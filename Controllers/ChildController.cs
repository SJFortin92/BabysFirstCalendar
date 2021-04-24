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

        //The Child view
        //Add functionality to return the selected child's information
        //Will only display if a user is logged in
        
        [Authorize]
        public ActionResult Child()
        {
            ViewBag.Message = "Add a new child to your account";
            return View();
        }

        //Processes if a user posts data
        
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Child(ChildModel model)
        {
            if (ModelState.IsValid)
            {
                //Calls CreateChild from the ChildProcessor class in DatabaseBusinessLogic
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