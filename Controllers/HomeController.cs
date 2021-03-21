using BabysFirstCalendar.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using System.Web.Security;
using static BabysFirstCalendar.DatabaseBusinessLogic.AccountProcessor;
using static BabysFirstCalendar.DatabaseBusinessLogic.MemoryProcessor;

namespace BabysFirstCalendar.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        //Json result to display the memories
        public JsonResult DisplayMemories()
        {
            var memories = ViewMemories();

            //Convert the SQL memory date to a string
            foreach (var memory in memories)
            {
                memory.StringDate = memory.Date.ToShortDateString();
            }

            //Return the results
            return new JsonResult { Data = memories, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }


        //Add a new memory using the Fullcalendar on Home page
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult NewMemory(MemoryModel model)
        {
            if (ModelState.IsValid)
            {
                //Declare the path, file size and photo bit so we can use it later
                string path;
                int fileSize;
                int hasPhoto;
                var status = false;

                WebImage PhotoUpload = WebImage.GetImageFromRequest();

                //If the submitted file is not an accepted WebImage type, then we ask the user
                //to submit a photo.
                if (PhotoUpload == null && model.Photo != null)
                {
                    ModelState.AddModelError("", "Please upload an image");
                }

                //If we have an approved WebImage type
                else if (PhotoUpload != null)
                {
                    //Get the file name
                    string name = Path.GetFileName(PhotoUpload.FileName);

                    //hasPhoto is true
                    hasPhoto = 1;

                    //Get the file size
                    fileSize = PhotoUpload.GetBytes().Length / 1024;

                    //Set the path equal to the upload folder in the project
                    path = "~/upload/" + name;

                    //Save the photo
                    PhotoUpload.Save(Server.MapPath(path));

                    //Call CreateMemory from MemoryProcessor in DatabaseBusinessLogic
                    if (CreateMemory(model.Date, model.Note, hasPhoto, path, fileSize) == 1)
                    {
                        status = true;
                        return new JsonResult { Data = new { status = status } };
                    }

                    else
                    {
                        ModelState.AddModelError("", "Error when adding a new memory");
                    }

                }

                //If the user has not submitted a file
                else
                {
                    hasPhoto = 0;
                    path = null;
                    fileSize = 0;

                    //Call CreateMemory from the MemoryProcessor class in the DatabaseBusinessLogic folder
                    if (CreateMemory(model.Date, model.Note, hasPhoto, path, fileSize) == 1)
                    {
                        status = true;
                        return new JsonResult { Data = new { status = status } };
                    }

                    else
                    {
                        ModelState.AddModelError("", "Error when adding a new memory");
                    }

                }
            }
            return Json(model);
        }

        public ActionResult TestLogic()
        {
            var data = ViewMemories();

            foreach (var memory in data)
            {
                memory.StringDate = memory.Date.ToShortDateString();
            }


            return View(data);
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

    }

}