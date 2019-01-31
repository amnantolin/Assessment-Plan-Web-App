using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DROP.Models;
using System.IO;
using System.Data.Entity;

namespace DROP.Controllers
{
    public class HomeController : Controller
    {
        //User Homepage View
        public ActionResult MainPage()
        {
            ViewBag.Title = "Main Page";
            return View();
        }

        //Admin Homepage View
        public ActionResult AdminPage()
        {
            ViewBag.Title = "Admin Page";
            return View();
        }
        
        //Add User Page View 
        public ActionResult AddUser(string returnUrl)
        {
            HomeViewModel home = new HomeViewModel();
            ViewBag.ReturnUrl = returnUrl;
            ViewBag.Title = "Add Account";
            return View(home);
        }

        //Add New User Post-Process
        [HttpPost]
        public ActionResult AddData(HomeViewModel model)
        {
            try
            {
                projectEntities db = new projectEntities();
                user User = new user();

                //Get all inputted information and save to db
                User.type_id = model.SelectedType;
                User.FName = model.FName;
                User.MName = model.MName;
                User.LName = model.LName;
                User.username = model.username;
                User.password = model.password;

                db.users.Add(User);
                db.SaveChanges();

                string filepath = Server.MapPath("~/UserFiles/" + User.acc_id);

                if (Directory.Exists(filepath)){ }
                else
                {
                    //Add file folder for new user
                    DirectoryInfo di = Directory.CreateDirectory(filepath);
                }
            }
            catch(Exception ex)
            {
                throw ex;
            }

            //for message prompt purposes
            TempData["flag"] = 1;
            TempData["MessageTitle"] = "Create Account";
            TempData["MessagePrompt"] = "Account has been created";
            return RedirectToAction("AdminPage");
        }

        //return bool value of isAvailable
        [HttpPost]
        public JsonResult checkusername(string username)
        {
            return Json(isAvailable(username));
        }

        //check identical usernames, for add user 
        public bool isAvailable(string uname)
        {
            projectEntities db = new projectEntities();
            List<HomeViewModel> usernameList = db.users.Select(x => new HomeViewModel { username = x.username }).ToList();

            var userinput = (from x in usernameList where x.username == uname select new { uname }).FirstOrDefault();

            bool status;
            if (userinput != null)
            {
                status = false;
            }
            else
            {
                status = true;
            }

            return status;
        }

        //DeleteUser page
        //return listUser that stores all users
        public ActionResult DeleteUser(string searching)
        {
            projectEntities db = new projectEntities();

            //for searching capabilities in delete page
            List<HomeViewModel> listUser = db.users.Where(x => x.FName.StartsWith(searching) && x.type_id == 1 ||
                                                               x.MName.StartsWith(searching) && x.type_id == 1 ||
                                                               x.LName.StartsWith(searching) && x.type_id == 1 ||
                                                               x.username.StartsWith(searching) && x.type_id == 1 ||
                                                               searching == null && x.type_id == 1).Select(x => new HomeViewModel { FName = x.FName, MName = x.MName, LName = x.LName, username = x.username, acc_id = x.acc_id }).ToList();
            ViewBag.UserList = listUser;
            ViewBag.Title = "Delete Account";
            return View();
        }

        //Delete user in db as well as send json to signal js to remove row
        public JsonResult DeleteData(int acc_id)
        {
            projectEntities db = new projectEntities();

            bool result = false;
            user data = db.users.SingleOrDefault(x => x.acc_id == acc_id);
            if(data != null)
            {
                db.users.Remove(data);
                db.SaveChanges();
                result = true;

                string filepath = Server.MapPath("~/UserFiles/" + acc_id);
                if (Directory.Exists(filepath))
                {
                    DirectoryInfo di = new DirectoryInfo(filepath);
                    di.Delete();
                }
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }
        
        //EditData Page View
        public ActionResult EditData()
        {
            ViewBag.Title = "Edit Data";
            ViewBag.soList = new SelectList(ListSO(), "so_id", "so_desc");
            ViewBag.coList = new SelectList(ListCourse(), "course_id", "coursename");
            ViewBag.atList = new SelectList(ListAT(), "at_id", "at_desc");

            return View();
        }

        //Update COSOPIATT relationship
        [HttpPost]
        public ActionResult UpdateData(HomeViewModel dbmodel)
        {
            try
            {
                projectEntities db = new projectEntities();
                copiatt data = db.copiatts.SingleOrDefault(m => m.pi_id == dbmodel.pi_id);

                if (data != null)
                {
                    data.at_id = dbmodel.at_id;
                    data.course_id = dbmodel.course_id;
                    data.target = float.Parse(dbmodel.target);
                    db.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            //for message prompt
            TempData["flag"] = 1;
            TempData["MessageTitle"] = "Edit Data";
            TempData["MessagePrompt"] = "Assessment Plan has been successfully updated";
            return RedirectToAction("AdminPage", "Home");
        }

        //Store all SO values
        public List<so> ListSO()
        {
            projectEntities db = new projectEntities();
            List<so> SO = db.soes.ToList();
            return SO;
        }

        //Store PI depending on SO selected
        //Uses PartialView
        public ActionResult ListPI(int so_id)
        {
            projectEntities db = new projectEntities();
            List<pi> PI = db.pis.Where(x => x.so_id == so_id).ToList();

            ViewBag.PIOptions = new SelectList(PI, "pi_id", "pi_desc");

            return PartialView("PIOptionsPartial");
        }

        //Store all courses available
        public List<course> ListCourse()
        {
            projectEntities db = new projectEntities();
            List<course> CO = db.courses.ToList();
            return CO;
        }

        //Store all AT available
        public List<at> ListAT()
        {
            projectEntities db = new projectEntities();
            List<at> AT = db.ats.ToList();
            return AT;
        }

    }
}

