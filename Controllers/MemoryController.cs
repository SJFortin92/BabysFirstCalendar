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
    //Need to add UpdateMemory and DeleteMemory to here
    public class MemoryController : Controller
    {
        //The NewMemory view
        //Only displays if a user is logged in
        [Authorize]
        public ActionResult NewMemory()
        {
            ViewBag.Message = "Adding a new memory";
            return View();
        }

        //Processes if a user posts data
        //We will essentially have two pathways - one if the user uploaded a photo and one without
        //If a user did not upload a file, as in, file IS null, then all defaults go to null or 0.

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult NewMemory(MemoryModel model)
        {
            if (ModelState.IsValid)
            {
                //Declare the path, file size and photo bit so we can use it later
                string path;
                int fileSize;
                int hasPhoto;

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
                            return RedirectToAction("Index", "Home");
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