using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TigerLineScores.Models;

namespace TigerLineScores.Controllers
{
    public class RegisterController : Controller

    {
        private TigerLineScoresEntities1 db = new TigerLineScoresEntities1();

        // GET: Register
        public ActionResult Index()
        {

            return View(new User());
        }


        [HttpPost]
        public ActionResult Register(User user)
        {
            if (ModelState.IsValid)
            {

                // Has email already been registered?
                var emailDuplicated = checkEmail(user.Email);
                if (emailDuplicated == true)
                {
                    TempData["Message"] = user.Email + Environment.NewLine + "This Email address has already been registered.";
                    return RedirectToAction ("Index");
                }

                db.Users.Add(user);
                db.SaveChanges();

                // Create a blank user profile for this new user
                var newprofile = new Profile();
                newprofile.UserID = user.UserID;
                newprofile.Photo = "/Images/Default Profile Photo.jpg";
                db.Profiles.Add(newprofile);
                db.SaveChanges();

                TempData["Register"] = "Thanks for registering to join Tiger Line Scores." + Environment.NewLine  +  "An email will be sent to you shortly with your password and information about where you should upload your competition score cards. Use your email address and the password to login into this site.";
                return RedirectToAction("Index");
            }
            return View("Index");

        }


        public bool checkEmail(string uemail)
        {
            var data = from u in db.Users
                       where u.Email == uemail
                       select u;

            bool EmailDuplicated = false;
            if (data.Count() > 0)
            {
                EmailDuplicated = true;
            }

            return EmailDuplicated;

        }


    }
}
