using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TigerLineScores.Models;
using System.Web.Security;

namespace TigerLineScores.Controllers
{
    public class RegisterController : Controller

    {
        private TigerLineScoresEntities1 db = new TigerLineScoresEntities1();

        // GET: Register
        public ActionResult Index()
        {

            // Get Club Names For Drop Down List
            var courseInfo = new CourseInfo();
            ViewBag.CourseList = courseInfo.GetCourseList();


            return View(new UserProfile());
        }


        [HttpPost]
        public ActionResult Register(UserProfile userProfile)
        {
            if (ModelState.IsValid)
            {

                // Has email already been registered?
                var emailDuplicated = checkEmail(userProfile.Email);
                if (emailDuplicated == true)
                {
                    TempData["Message"] = userProfile.Email + Environment.NewLine + "This Email address has already been registered.";
                    return RedirectToAction ("Index");
                }

                // Create the User Record
                User user = new Models.User();
                user.UserName = userProfile.UserName;
                user.Email = userProfile.Email;
                user.Admin = false;

                // Create a random password for this user
                UserInfo uInfo = new UserInfo();
                string password = uInfo.CreatePassword(6);
                user.Password = password;

                db.Users.Add(user);
                db.SaveChanges();   

                // Create the Profile Record
                var newprofile = new Profile();
                newprofile.UserID = user.UserID;
                newprofile.HomeClubID = userProfile.HomeCourseID;
                newprofile.Handicap = userProfile.Handicap;
                newprofile.Photo = "/Images/Default Profile Photo.jpg";
                db.Profiles.Add(newprofile);
                db.SaveChanges(); 

                // Create New Message to Inform Admin of NEW user
                MessageInfo messageInfo = new MessageInfo();

                var AllAdmin = uInfo.GetAllAdmin();

                string AdminEmail = "";
                string Subject = "";
                string Body = "";

                foreach (var item in AllAdmin)
                {
                    messageInfo.CreateMessage(newprofile.UserID, item.UserID, DateTime.Now, user.UserName + " has registered to join Tigerline Scores.");
                    AdminEmail = item.Email;
                    // Send Email to Admin
                    Subject = "NEW USER HAS REGISTERED";
                    Body = "<span style='font-family: Calibri; font-size: 24px; font-weight: bold; color: green'>TIGERLINE SCORES</span><br/><br/>";
                    Body += "<span style='font-family: Calibri'>New user " + user.UserName + " has registered to use Tigerline Scores..<br/><br/>";
                    Body +=  "<a href='www.tigerlinescores.co.uk'>Tigerline Scores</a>";
                    messageInfo.SendEmail(AdminEmail, Subject, Body, null);
                }

                // Send email to new user with NEW password (created above) and instructions on how to use Tigerline Scores
                Body = "<span style='font-family: Calibri; font-size: 24px; font-weight: bold; color: green'>TIGERLINE SCORES</span><br/><br/>";
                Body += "<div style='font-family: Calibri; font-size: 14px'>" + user.UserName + "<br/><br/>Thanks for registering and welcome to Tigerline Scores.<br/><br/>";
                Body += "Your login details are : <br/><br/>";
                Body += "<span style='font-weight: bold'>" + user.Email + "</span><br/>";
                Body += "Password  <span style='font-weight: bold'>" + password  + "</span><br/><br/>";
                Body += "Attached to this email are instructions on how to find your way around the site, how to register an upcoming round and how to upload your score cards.<br/><br/>";
                Body += "I hope you enjoy using Tigerline Scores. Please feel free to send me any comments you have about the site as I am still developing and improving it.<br/>";
                Body += "Send your emails to admin@tigerlinescores.co.uk<br/><br/>";
                Body += "Thanks again and good luck!<br/><br/>Martin<br/><br/>";
                Body += "<a href='www.tigerlinescores.co.uk'>Tigerline Scores</a>";
                Body += "</div>";

                string userEmail = user.Email;
                Subject = "Welcome to Tigerline Scores";
                string attachment = Server.MapPath("~/Images/Tigerline_Scores_Instructions.pdf");
                messageInfo.SendEmail(userEmail, Subject, Body, attachment);


                TempData["Register"] = "Thanks for registering to join Tigerline Scores." + Environment.NewLine + "An email will be sent to you shortly with your password and information about how to use Tigerline Scores. Use your email address and the password to login into this site.";
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
