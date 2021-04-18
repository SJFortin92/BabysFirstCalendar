﻿using BabysFirstCalendar.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using System.Web.Security;
using static BabysFirstCalendar.DatabaseBusinessLogic.RetrievalProcessor;
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

            //Convert data if necessary
            foreach (var memory in memories)
            {
                //Convert the SQL memory date to a string
                memory.StringDate = memory.Date.ToShortDateString();

                if (memory.PhotoLocationReference != null)
                {
                    //Code to trim the pathway and make it ~/upload/photoName
                    memory.PhotoLocationReference = RetrieveRelativeImagePath(memory.PhotoLocationReference);
                }
            }

            //Return the results
            return new JsonResult { Data = memories, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        [Authorize]
        [HttpPost]
        public JsonResult DeleteMemory(MemoryModel model)
        {
            var status = false;
            if (DeleteNote(model.NoteID) == 1)
            {
                status = true;
                return new JsonResult { Data = new { status = status } };
            }

            return new JsonResult { Data = new { status = status } };

        }

        //Update or save a memory using the Fullcalendar on Home page
        [Authorize]
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public JsonResult SaveMemory(MemoryModel model)
        {
            var status = false;

            //If there are files to save...
            if (Request.Files.Count > 0)
            {
                
                //Get the files
                HttpFileCollectionBase files = Request.Files;

                //Save the photos
                var memoryStruct = SavePhoto(files);

                //If the note already exists, update it
                if (model.NoteID > 0)
                {
                    if (UpdateMemory(model.NoteID, model.Date, model.Note, memoryStruct.hasPhoto, memoryStruct.path, memoryStruct.fileSize) == 1)
                    {
                        status = true;
                        return new JsonResult { Data = new { status = status } };
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
                    if (CreateMemory(model.Date, model.Note, memoryStruct.hasPhoto, memoryStruct.path, memoryStruct.fileSize) == 1)
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

            //If the user has not submitted a file
            else
            {
                //If the note already exists, update it
                //Put hasPhoto=0, photoLocation=null, fileSize=0 because there are no photos
                if (model.NoteID > 0)
                {
                    if (UpdateMemory(model.NoteID, model.Date, model.Note, 0, null, 0) == 1)
                    {
                        status = true;
                        return new JsonResult { Data = new { status = status } };
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
                    if (CreateMemory(model.Date, model.Note, 0, null, 0) == 1)
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

            return new JsonResult { Data = new { status = status } };
        }


        public ActionResult About()
        {
            ViewBag.Message = "More about Baby's First Calendar";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Contact us";

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]

        public ActionResult Contact(ContactModel model)
        {
            if (ModelState.IsValid)
            {
                //Enter logic to submit a contact request here
                return RedirectToAction("Index", "Home");
            }
            return View(model);
        }
    }

}