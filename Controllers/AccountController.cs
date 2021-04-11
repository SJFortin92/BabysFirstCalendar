using BabysFirstCalendar.Models;
using System;
using static BabysFirstCalendar.DatabaseBusinessLogic.AccountProcessor;
using static BabysFirstCalendar.DatabaseBusinessLogic.RetrievalProcessor;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using System.Dynamic;

//Controller to process Account CRUD, Login and Logout

namespace BabysFirstCalendar.Controllers
{
    public class AccountController : Controller
    { 
        //View for the Login page
        public ActionResult Login()
        {
            ViewBag.Message = "Login to your account.";

            return View();
        }


        //If the user tries to log in from the normal view page
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LoginModel model)
        {
            if (ModelState.IsValid)
            {
                //Calls ComparePassword from the AccountProcessor class in DatabaseBusinessLogic folder
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

        //Logs users out on click
        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Index", "Home");
        }

        //View for the SignUp page
        public ActionResult SignUp()
        {
            ViewBag.Message = "Account signup.";
            return View();
        }

        //Processes SignUp after user posts data
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SignUp(AccountModel model)
        {
            if (ModelState.IsValid)
            {
                //Cast enum Notification to an int before passing it
                int notificationInt = Convert.ToInt32(model.NotificationSchedule);

                //Calls CreateAccount from the AccountProcessor class in DatabaseBusinessLogic folder
                if (CreateAccount(model.FirstName, model.LastName, model.Email,
                    notificationInt, model.Password) == 1)
                {
                    //Logs in the user
                    FormsAuthentication.SetAuthCookie(model.Email, false);

                    //Redirects user to add a child to their account
                    return RedirectToAction("Child", "Child");
                }

                else
                    ModelState.AddModelError("", "Error creating an account");
            }

            return View();
        }
        
        //The Edit view
        //Will only allow users to see it if they are logged in
        [Authorize]
        public ActionResult Edit()
        {
            //Automatically pulls user's existing data. If null, the textbox is blank
            //This calls functions from the RetrievalProcessor class in the DatabaseBusinessLogic folder
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

        //Processes if the user posts data
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(EditAccountModel model)
        {
            if (ModelState.IsValid)
            {
                //Cast enum Notification to an int before passing it
                int notificationInt = Convert.ToInt32(model.NotificationSchedule);

                //Users may choose to update their email, but we need the current one for
                //pulling the account.
                string currentEmail = RetrieveEmail();

                //Calls UpdateAccount from the AccountProcessor class in DatabaseBusinessLogic folder
                if (UpdateAccount(model.FirstName, model.LastName, model.Phone, currentEmail, model.Email,
                    notificationInt) == 1)
                    return RedirectToAction("Index", "Home");
                else
                    ModelState.AddModelError("", "Error updating account");
            }
            return View(model);
        }

        //Returns the EditPassword view
        //Will only allow users to see it if they are logged in
        [Authorize]
        public ActionResult EditPassword()
        {
            return View();
        }

        //Submits data if user posts data
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditPassword(EditPasswordModel model)
        {
            if (ModelState.IsValid)
            {
                //We need the user's current email to retrieve the account
                string CurrentEmail = RetrieveEmail();

                //Calls UpdatePassword from the AccountProcessor class in the DatabaseBusinessLogic folder
                if (UpdatePassword(CurrentEmail, model.CurrentPassword, model.NewPassword) == 1)
                    return RedirectToAction("Index", "Home");
                else
                    ModelState.AddModelError("", "Error updating password");
            }

            return View(model);
        }

        //The DeleteAccount view
        //Will only allow users to see it if they are logged in
        [Authorize]
        public ActionResult DeleteAccount()
        {
            return View();
        }

        //Processes if the user posts data
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteAccount(DeleteAccountModel model)
        {
            if (ModelState.IsValid)
            {
                //We need to pull the user's current email to access the account
                string CurrentEmail = RetrieveEmail();

                //Calls AccountDelete from the AccountProcessor class in the DatabaseBusinessLogic folder
                if (AccountDelete(CurrentEmail, model.Password) == 1)
                {
                    FormsAuthentication.SignOut();
                    return RedirectToAction("Index", "Home");
                }
                else
                    ModelState.AddModelError("", "Error deleting account");
            }

            return View(model);
        }

    }
}
