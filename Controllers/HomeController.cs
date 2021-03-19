﻿using BabysFirstCalendar.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using static BabysFirstCalendar.DatabaseBusinessLogic.AccountProcessor;
using static BabysFirstCalendar.EmailManagement.SMTPManager;
using BabysFirstCalendar.DatabaseModels;

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

    }

}