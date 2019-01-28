using System;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Collections.Generic;
using DROP.Models;

namespace DROP.Controllers
{

    public class AccountController : Controller
    {
        //Login Page View
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            ViewBag.Title = "Login";
            return View();
        }

        //About Page View
        [AllowAnonymous]
        public ActionResult About()
        {
            ViewBag.Title = "About";
            return View();
        }

        //Contact Page View
        [AllowAnonymous]
        public ActionResult Contact()
        {
            ViewBag.Title = "Contact";
            return View();
        }

        //Login Authentication Post-Process
        [HttpPost]
        public ActionResult Authenticate(AccountViewModel userModel)
        {
            ViewBag.Title = "Login Attempt";
            using (projectEntities db = new projectEntities())
            {
                //Check db if username and password both exists
                var userDetails = db.users.Where(x => x.username == userModel.username && x.password == userModel.password).FirstOrDefault();

                if (userDetails == null)
                {
                    userModel.LogErrorMsg = "Incorrect Username and/or Password.";
                    return View("Login", userModel);
                }

                else
                {
                    //Save all the user details in the current session
                    Session["accID"] = userDetails.acc_id;
                    Session["user"] = userDetails.username;
                    Session["pw"] = userDetails.password;
                    Session["type"] = userDetails.type_id;
                    Session["fn"] = userDetails.FName;
                    Session["mn"] = userDetails.MName;
                    Session["ln"] = userDetails.LName;

                    //Check Redirection page (0=admin, 1=user)
                    if (userDetails.type_id == 0)
                    { 
                        return RedirectToAction("AdminPage", "Home");
                    }

                    else
                    {
                        Session["test"] = 1;
                        return RedirectToAction("MainPage", "Home");
                    }
                    
                }

            }
        }

        //End Session
        public ActionResult Logout()
        {
            if (Session["accID"] != null)
            {
                int accid = (int)Session["accID"];
                Session.Abandon();
            }
            return RedirectToAction("Login", "Account");
        }

        //Change Password Page View
        public ActionResult ChangePass()
        {
            return View(new AccountViewModel());
        }

        //Change Password Post-Process
        [HttpPost]
        public ActionResult UpdatePass(AccountViewModel userModel)
        {
            try
            {
                projectEntities db = new projectEntities();

                //Check if session still exists
                if (Session["accID"] == null)
                {
                    return RedirectToAction("Login", "Account");
                }

                int id = (int)Session["accID"];
                string pw = (string)Session["pw"];

                //Check for the current session account id in db
                user data = db.users.SingleOrDefault(m => m.acc_id == id);

                if (data != null)
                {
                    if (pw != userModel.password)
                    {
                        userModel.PassErrorMsg = "Incorrect Password, Try Again.";
                        return View("ChangePass", userModel);
                    }
                    else if (pw == userModel.newpassword)
                    {
                        userModel.PassErrorMsg = "Password is the same, Try Again.";
                        return View("ChangePass", userModel);
                    }
                    else
                    {
                        //Update user password in db and current session
                        data.password = userModel.newpassword;
                        db.SaveChanges();
                        Session["pw"] = userModel.newpassword;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            //For message prompt purposes
            TempData["flag"] = 1;
            TempData["MessageTitle"] = "Change Password";
            TempData["MessagePrompt"] = "Password has been successfully changed";

            //Redirection Check
            if ((int)Session["type"] != 0)
            {
                return RedirectToAction("MainPage", "Home");
            }
            return RedirectToAction("AdminPage", "Home");
        }
       
    }
}