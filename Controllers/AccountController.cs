using BabysFirstCalendar.Models;
using System;
using static BabysFirstCalendar.DatabaseBusinessLogic.AccountProcessor;
using static BabysFirstCalendar.DatabaseBusinessLogic.LayoutProcessor;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using System.Dynamic;

namespace BabysFirstCalendar.Controllers
{
    public class AccountController : Controller
    { 
        public ActionResult Login()
        {
            ViewBag.Message = "Login to your account.";

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LoginModel model)
        {
            if (ModelState.IsValid)
            {
                if (ComparePassword(model.Password, model.Email))
                {
                    FormsAuthentication.SetAuthCookie(model.Email, model.RememberMe);
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ModelState.AddModelError("", "Email or password is incorrect");
                }
            }
            return View(model);
        }
        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Index", "Home");
        }

        public ActionResult SignUp()
        {
            ViewBag.Message = "Account signup.";
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SignUp(AccountModel model)
        {
            if (ModelState.IsValid)
            {
                //Cast enum Notification to an int before passing it
                int notificationInt = Convert.ToInt32(model.NotificationSchedule);

                if (CreateAccount(model.FirstName, model.LastName, model.Email,
                    notificationInt, model.Password) == 1)
                {
                    FormsAuthentication.SetAuthCookie(model.Email, false);
                    return RedirectToAction("Child", "Child");
                }

                else
                    ModelState.AddModelError("", "Error creating an account");
            }

            return View();
        }
        
        [Authorize]
        public ActionResult Edit()
        {
            EditAccountModel data = new EditAccountModel
            {
                FirstName = RetrieveName(),
                LastName = RetrieveLastName(),
                Email = RetrieveEmail(),
                Phone = RetrievePhone(),
                NotificationSchedule = (Notification)RetrieveNotification()
            };

            return View(data);
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(EditAccountModel model)
        {
            if (ModelState.IsValid)
            {
                //Cast enum Notification to an int before passing it
                int notificationInt = Convert.ToInt32(model.NotificationSchedule);
                string currentEmail = RetrieveEmail();

                if (UpdateAccount(model.FirstName, model.LastName, model.Phone, currentEmail, model.Email,
                    notificationInt) == 1)
                    return RedirectToAction("Index", "Home");
                else
                    ModelState.AddModelError("", "Error updating account");
            }
            return View(model);
        }

        [Authorize]
        public ActionResult EditPassword()
        {
            return View();
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditPassword(EditPasswordModel model)
        {
            if (ModelState.IsValid)
            {
                string CurrentEmail = RetrieveEmail();
                if (UpdatePassword(CurrentEmail, model.CurrentPassword, model.NewPassword) == 1)
                    return RedirectToAction("Index", "Home");
                else
                    ModelState.AddModelError("", "Error updating password");
            }

            return View(model);
        }

    }
}
