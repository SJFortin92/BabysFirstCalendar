using BabysFirstCalendar.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Web;
using System.Web.Mvc;
using static BabysFirstCalendar.DatabaseBusinessLogic.MemoryProcessor;
using System.Web.Helpers;

namespace BabysFirstCalendar.Controllers
{
    public class MemoryController : Controller
    {
        [Authorize]

        public ActionResult NewMemory()
        {
            ViewBag.Message = "Adding a new memory";
            return View();
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]

        //We will essentially have two pathways - one if the user uploaded a photo and one without
        //If a user did not upload a file, as in, file IS null, then all defaults go to null or 0.
        public ActionResult NewMemory(MemoryModel model)
        {
            if (ModelState.IsValid)
            {
                //Declare the path, file size and photo bit so we can use it later
                string path;
                int fileSize;
                int hasPhoto;

                WebImage PhotoUpload = WebImage.GetImageFromRequest();

                if (PhotoUpload == null && model.Photo != null)
                {
                    ModelState.AddModelError("", "Please upload an image");
                }

                else if (PhotoUpload != null)
                {
                    string name = Path.GetFileName(PhotoUpload.FileName);
                    hasPhoto = 1;
                    fileSize = PhotoUpload.GetBytes().Length / 1024;
                    path = "~/upload/" + name;

                    PhotoUpload.Save(Server.MapPath(path));

                        if (CreateMemory(model.Date, model.Note, hasPhoto, path, fileSize) == 1)
                        {
                            return RedirectToAction("Index", "Home");
                        }

                        else
                        {
                            ModelState.AddModelError("", "Error when adding a new memory");
                        }

                }

                else
                {
                    hasPhoto = 0;
                    path = null;
                    fileSize = 0;

                    if (CreateMemory(model.Date, model.Note, hasPhoto, path, fileSize) == 1)
                    {
                        return RedirectToAction("Index", "Home");
                    }

                    else
                    {
                        ModelState.AddModelError("", "Error when adding a new memory");
                    }

                }
            }
            return View(model);
        }
    }
}