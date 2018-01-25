using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using TigerLineScores.Models;

namespace TigerLineScores.Controllers
{
    public class ProfilesController : Controller
    {
        private TigerLineScoresEntities1 db = new TigerLineScoresEntities1();

        // GET: Profiles
        public ActionResult Index()
        {
            return View(db.Profiles.ToList());
        }

        // GET: Profiles/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Profile profile = db.Profiles.Find(id);
            if (profile == null)
            {
                return HttpNotFound();
            }
            return View(profile);
        }

        // GET: Profiles/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Profiles/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ProfileID,UserID,FirstName,LastName,HomeClubID,Handicap,Photo")] Profile profile)
        {
            if (ModelState.IsValid)
            {
                db.Profiles.Add(profile);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(profile);
        }

        // GET: Profiles/Edit/5
        public ActionResult Edit()
        {
            if (Session["userid"] == null)
            {
                return RedirectToAction("Index", "Home");
            }

            // Get Club Names For Drop Down List
            var courseInfo = new CourseInfo();
            ViewBag.CourseList = courseInfo.GetCourseList();

            // Get the user record
            User user = db.Users.Find(Session["userid"]);
            var userID = user.UserID;

            // Get the Profile record
            int profileID = 0;
            var getprofiles = from gp in db.Profiles
                              where gp.UserID == userID
                              select gp;

            foreach (var pro in getprofiles)
            {
                profileID = pro.ProfileID;
            }
            Profile profile = db.Profiles.Find(profileID);

            // Populate the UserProfile Model
            UserProfile userProfile = new UserProfile();
            userProfile.UserID = userID;
            userProfile.UserName = user.UserName;
            userProfile.Email = user.Email;
            userProfile.ProfileID = profile.ProfileID;
            userProfile.HomeCourseID = Convert.ToInt32(profile.HomeClubID);
            userProfile.Handicap = Convert.ToDecimal(profile.Handicap);
            userProfile.Photo = profile.Photo;

            //Get User Name from the user table
            ViewBag.UserName = user.UserName;

            if (profile == null)
            {
                return HttpNotFound();
            }
            return View(userProfile);
        }

        // POST: Profiles/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ProfileID,UserID,HomeCourseID,Handicap,Photo,Email,UserName")] UserProfile userProfile)
        {
            if (ModelState.IsValid)
            {
                // Save Profile Record
                Profile profile = db.Profiles.Find(userProfile.ProfileID);
                profile.HomeClubID = userProfile.HomeCourseID;
                profile.Handicap = userProfile.Handicap;

                // Deal with Profile Photo ******************************
                string photofileName = "";
                HttpPostedFileBase photo = Request.Files["Profilephoto"];
                if (photo.FileName != "")
                {
                    //Reduce size of file if required
                    photofileName = new FileInfo(photo.FileName).Name;
                    WebImage img = new WebImage(photo.InputStream);
                    if (img.Height > 600)
                    {
                        img.Resize(600, 600, true);
                    }
                    string path = Path.Combine(Server.MapPath("~/Images/"), photofileName);
                    img.Save(path);
                    profile.Photo = "/Images/" + photofileName;
                    Session["UserPhoto"] = profile.Photo;
                }
                else
                {
                    Session["UserPhoto"] = userProfile.Photo;
                    profile.Photo = userProfile.Photo;
                }
                // ******************************************************

                db.Entry(profile).State = EntityState.Modified;
                db.SaveChanges();

                // Save User Record
                User user = db.Users.Find(userProfile.UserID);
                user.UserName = userProfile.UserName;
                db.Entry(user).State = EntityState.Modified;
                db.SaveChanges();


                TempData["EditMessage"] = "Profile successfully saved.";
                return RedirectToAction("Edit");
            }
            return View(userProfile);
        }

        // GET: Profiles/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Profile profile = db.Profiles.Find(id);
            if (profile == null)
            {
                return HttpNotFound();
            }
            return View(profile);
        }

        // POST: Profiles/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Profile profile = db.Profiles.Find(id);
            db.Profiles.Remove(profile);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
