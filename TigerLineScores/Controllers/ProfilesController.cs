using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
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
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            // Get Club Names For Drop Down List
            var courseInfo = new CourseInfo();
            ViewBag.HomeClubID = courseInfo.GetCourseList();

            // Get the current logged in user
            User user = db.Users.Find(Session["userid"]);
            var currentuserid = user.UserID;

            // Get the ProfileID
            int profileid = 0;
            var getprofiles = from gp in db.Profiles
                              where gp.UserID == currentuserid
                              select gp;

            foreach (var pro in getprofiles)
            {
                profileid = pro.ProfileID;

            }
            Profile profile = db.Profiles.Find(profileid);

            if (profile == null)
            {
                return HttpNotFound();
            }
            return View(profile);
        }

        // POST: Profiles/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ProfileID,UserID,FirstName,LastName,HomeClubID,Handicap,Photo")] Profile profile)
        {
            if (ModelState.IsValid)
            {

                // Deal with Profile Photo ******************************
                string photofileName = "";
                HttpPostedFileBase photo = Request.Files["Profilephoto"];
                if (photo.FileName != "")
                {
                    photofileName = new FileInfo(photo.FileName).Name;
                    string path = Path.Combine(Server.MapPath("~/Images/"), photofileName);
                    photo.SaveAs(path);
                    profile.Photo = "/Images/" + photofileName;
                    
                }
                Session["UserPhoto"] = profile.Photo;
                // ******************************************************

                db.Entry(profile).State = EntityState.Modified;
                db.SaveChanges();
                TempData["EditMessage"] = "Profile successfully saved.";
                return RedirectToAction("Edit");
            }
            return View(profile);
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
