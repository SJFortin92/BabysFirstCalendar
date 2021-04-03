using BabysFirstCalendar.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using System.Web.Script.Serialization;
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
            //Declare the path, file size and photo bit so we can use it later
            string path;
            int fileSize;
            int hasPhoto;
            var status = false;

            //If there are files to save...
            if (Request.Files.Count > 0)
            {
                //Get the file
                HttpFileCollectionBase files = Request.Files;
                HttpPostedFileBase photoUpload = files[0];

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

            return new JsonResult { Data = new { status = status } };
        }


        //Add Memory Delete function here
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