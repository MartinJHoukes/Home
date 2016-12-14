using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using TigerLineScores.Models;

namespace TigerLineScores.Controllers
{
    public class LoginController : Controller
    {
        private TigerLineScoresEntities1 db = new TigerLineScoresEntities1();

        // GET: Login
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Index(User user)
        {
            int id = (IsValid(user.Email, user.Password));
            if (id > 0)
            {
                // SESSION VARIABLE FOR USERID
                Session["userid"] = id;

                // SESSION VARIABLE FOR ADMIN
                Session["Admin"] = IsUserAdmin(id);

                // SESSION VARIABLE FOR USER NAME
                Session["UserName"] = GetUserName(id);

                //Session Variable for User Photo from Profile table
                Session["UserPhoto"] = GetUserPhoto(id);

                // Allow access to Edit User page to Admin User only
                if (IsUserAdmin(id))
                {
                    FormsAuthentication.SetAuthCookie(GetUserName(id), false);
                }

                return RedirectToAction("Index", "Home", new { id = id });
            }
            else
            {
                ModelState.AddModelError("", "The username and/or passord is incorrect, please try again");
            }

            return View(user);
        }

        public int IsValid(string Email, string Password)
        {
            string passwordHash = Password;
            var data = from u in db.Users
                       where u.Email == Email && u.Password == passwordHash
                       select new { u.UserID, u.Email, u.Password };

            int id = 0;
            foreach (var check in data)
            {
                id = check.UserID;
            }

            return id;
        }
        public bool IsUserAdmin(int uid)
        {
            var data = from u in db.Users
                       where u.UserID == uid
                       select u;

            bool isAd = false;
            foreach (var check in data)
            {
                if (check.Admin == true)
                {
                    isAd = true;
                }

            }

            return isAd;
        }

        public string GetUserName(int uid)
        {
            var data = from u in db.Users
                       where u.UserID == uid
                       select u;

            string uname = "";
            foreach (var check in data)
            {
                uname = check.UserName;
            }

            return uname;
        }

        public string GetUserPhoto(int uid)
        {
            var data = from u in db.Profiles
                       where u.UserID == uid
                       select u;

            string uPhoto = "";
            foreach (var check in data)
            {
                uPhoto = check.Photo;
            }

            return uPhoto;
        }

    }
}