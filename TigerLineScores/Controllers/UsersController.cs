using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using TigerLineScores.Models;

namespace TigerLineScores.Controllers
{
    public class UsersController : Controller
    {
        private TigerLineScoresEntities1 db = new TigerLineScoresEntities1();

        // GET: Users
        [Authorize]
        public ActionResult Index()
        {

             var AllUsers = from au in db.Users
                            join pro in db.Profiles on au.UserID equals pro.UserID
                            orderby au.UserName 
                            select new UserView {UserID = au.UserID, UserName = au.UserName, Email = au.Email, Password = au.Password, Admin = au.Admin, Photo = pro.Photo, Handicap= pro.Handicap };

            //return View(AllUsers);
            ViewBag.AllUsers = AllUsers.ToList();

            return View(db.Users.ToList());

        }

        // GET: Users/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
           
            User user = db.Users.Find(id);
          
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }

        // GET: Users/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Users/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "UserID,UserName,Email,Password,Admin")] User user)
        {
            if (ModelState.IsValid)
            {
                db.Users.Add(user);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(user);
        }

        // GET: Users/Edit/5
        [Authorize]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var UserProfile = from ur in db.Users
                              join pr in db.Profiles on ur.UserID equals pr.UserID
                              where ur.UserID == id
                              select new UserView { UserID = ur.UserID, UserName = ur.UserName, Email = ur.Email, Password = ur.Password, Admin = ur.Admin,
                                                    Photo = pr.Photo, Handicap = pr.Handicap };

            UserView uView = new UserView();
            foreach (var item in UserProfile)
            {
                 uView = item;
            }
            
            if (UserProfile == null)
            {
                return HttpNotFound();
            }
            return View(uView);
        }

        // POST: Users/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "UserID,UserName,Email,Password,Handicap,Admin")] UserView userView)
        {
            if (ModelState.IsValid)
            {
                // Encrypt the Password
                //user.Password = SHA256.Encode(user.Password);
                //db.Entry(User).State = EntityState.Modified;
                //db.Entry(Profile).State = EntityState.Modified;

                //Update User Record
                User user = db.Users.Find(userView.UserID);
                user.UserName = userView.UserName;
                user.Email = userView.Email;
                user.Password = userView.Password;
                user.Admin = userView.Admin;

                //Update Profile Record
                var UserProfile = from up in db.Profiles
                                  where up.UserID == user.UserID
                                  select up;

                foreach (var item in UserProfile)
                {
                    item.Handicap = userView.Handicap;
                }

                db.SaveChanges();

                return RedirectToAction("Index");
            }
            return View(userView);
        }

        // GET: Users/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            User user = db.Users.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }

            ViewBag.EditUserName = user.UserName;
            return View(user);
        }

        // POST: Users/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            User user = db.Users.Find(id);
            db.Users.Remove(user);

            // Remove Associated User Profile
            var getprofiles = from gp in db.Profiles
                              where gp.UserID == id
                              select gp;

            foreach (var pro in getprofiles)
            {
                db.Profiles.Remove(pro);
            }

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
