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
        //Function to retrieve memories from the database
        public ActionResult GetMemories()
        {
            ViewBag.Message = "Viewing memories";

            //ViewMemories();

            return View();
        }

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

                //If there are files to save...
                if (model.Photo != null)
                {
                    //Get the file
                   HttpPostedFileBase photoUpload = model.Photo;

                    //hasPhoto is true, set it to 1
                    hasPhoto = 1;

                    //Get the file size
                    fileSize = photoUpload.ContentLength / 1024;

                    //Set a random filename and make a path
                    string fileName = photoUpload.FileName;
                    path = Path.Combine(Server.MapPath("~/upload/" + fileName));

                    //Save the file
                    photoUpload.SaveAs(path);


                    //If the note already exists, update it
                    if (model.NoteID > 0)
                    {
                        if (UpdateMemory(model.NoteID, model.Date, model.Note, hasPhoto, path, fileSize) == 1)
                        {
                            return RedirectToAction("Index", "Home");
                        }

                        else
                        {
                            ModelState.AddModelError("", "Failure updating the note");
                        }
                    }

                    //If the note does not exist, then create a new one
                    else
                    {
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

                }

                //If the user has not submitted a file
                else
                {
                    hasPhoto = 0;
                    path = null;
                    fileSize = 0;

                    //If the note already exists, update it
                    if (model.NoteID > 0)
                    {
                        if (UpdateMemory(model.NoteID, model.Date, model.Note, hasPhoto, path, fileSize) == 1)
                        {
                            return RedirectToAction("Index", "Home");
                        }

                        else
                        {
                            ModelState.AddModelError("", "Failure updating the note");
                        }
                    }

                    //If the note does not exist, then create a new one
                    else
                    {
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

                }

                return RedirectToAction("Index", "Home");
            }
            return View(model);
        }
    }
}