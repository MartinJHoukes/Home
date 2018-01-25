using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using TigerLineScores.Models;

namespace TigerLineScores.Views
{
    public class UpcomingRndsController : Controller
    {
        private TigerLineScoresEntities1 db = new TigerLineScoresEntities1();

        // GET: UpcomingRnds
        public ActionResult Index(int compPlayerID, int compID)
        {

            //Check User is in this Competition
            CompInfo compInfo = new CompInfo();
            int userID = Convert.ToInt32(Session["userid"]);
            Boolean IsInComp = compInfo.IsUserinComp(compID, userID);
            if (IsInComp == false)
            {
                TempData["FailUpload"] = "You need to be in a competition to register an upcoming round ..";
                TempData["SuccessUpload"] = null;

                return RedirectToAction("Index", "PLayerCompList");
            }

            // Get Players Upcoming Rounds
            CourseInfo cInfo = new CourseInfo();
            var Rnds = from rd in db.UpcomingRnds
                       where rd.CompPlayerID == compPlayerID && rd.CompID == compID
                       orderby rd.RndDate
                       select rd;

            foreach (var item in Rnds)
            {
                item.CourseName = cInfo.GetCourseName(item.CourseID); 
            }

            // Get Competition Name, Player Name, Current Handicap and Player Photo
            PlayerStats pStats = new PlayerStats();
         
            CompMain cMain = db.CompMains.Find(compID);
            ViewBag.PlayerName = pStats.GetPlayerName(compPlayerID);
            ViewBag.PlayerPhoto = pStats.GetPlayerPhoto(compPlayerID);
            ViewBag.CurrentHCap = pStats.CurrentHcap(compPlayerID);
            ViewBag.CompName = cMain.CompName;
            ViewBag.CompPlayerID = compPlayerID;
            ViewBag.CompID = compID;

            return View(Rnds.ToList());
        }

        // GET: UpcomingRnds/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            UpcomingRnd upcomingRnd = db.UpcomingRnds.Find(id);
            if (upcomingRnd == null)
            {
                return HttpNotFound();
            }
            return View(upcomingRnd);
        }

        // GET: UpcomingRnds/Create
        public ActionResult Create(int compPlayerID, int compID)
        {
            // Get Competition Name, Player Name, Current Handicap and Player Photo
            PlayerStats pStats = new PlayerStats();
            CompMain cMain = db.CompMains.Find(compID);
            ViewBag.PlayerName = pStats.GetPlayerName(compPlayerID);
            ViewBag.PlayerPhoto = pStats.GetPlayerPhoto(compPlayerID);
            ViewBag.CurrentHCap = pStats.CurrentHcap(compPlayerID);
            ViewBag.CompName = cMain.CompName;

            // Get Club Names For Drop Down List
            var courseInfo = new CourseInfo();
            ViewBag.CourseList = courseInfo.GetCourseList();

            // default players home course
            var upcomingRnd = new UpcomingRnd();
            upcomingRnd.CourseID = pStats.HomeClubID(compPlayerID);
            upcomingRnd.CompPlayerID = compPlayerID;


            // Options for Round Type - Competition or Practice
            List<SelectListItem> rndType = new List<SelectListItem>();
            rndType.Add(new SelectListItem() { Text = "Competiton", Value = "Competition" });
            rndType.Add(new SelectListItem() { Text = "Practice", Value = "Practice" });
            ViewBag.RndType = rndType;

            return View(upcomingRnd);
        }

        // POST: UpcomingRnds/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "RndID,CompPlayerID,CompID,RndDate,RndType,CourseID,Note")] UpcomingRnd upcomingRnd)
        {
            if (ModelState.IsValid)
            {
                db.UpcomingRnds.Add(upcomingRnd);
                db.SaveChanges();
                return RedirectToAction("Index", new { upcomingRnd.CompPlayerID, upcomingRnd.CompID });
            }

            return View(upcomingRnd);
        }

        // GET: UpcomingRnds/Edit/5
        public ActionResult Edit(int? RndID)
        {
            if (RndID == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            UpcomingRnd upcomingRnd = db.UpcomingRnds.Find(RndID);

            // Get Competition Name, Player Name, Current Handicap and Player Photo
            PlayerStats pStats = new PlayerStats();
            CompMain cMain = db.CompMains.Find(upcomingRnd.CompID);
            ViewBag.PlayerName = pStats.GetPlayerName(upcomingRnd.CompPlayerID);
            ViewBag.PlayerPhoto = pStats.GetPlayerPhoto(upcomingRnd.CompPlayerID);
            ViewBag.CurrentHCap = pStats.CurrentHcap(upcomingRnd.CompPlayerID);
            ViewBag.CompName = cMain.CompName;

            // Get Club Names For Drop Down List
            var courseInfo = new CourseInfo();
            ViewBag.CourseList = courseInfo.GetCourseList();

            // Options for Round Type - Competition or Practice
            List<SelectListItem> rndType = new List<SelectListItem>();
            rndType.Add(new SelectListItem() { Text = "Competiton", Value = "Competition" });
            rndType.Add(new SelectListItem() { Text = "Practice", Value = "Practice" });
            ViewBag.RndType = rndType;

            return View(upcomingRnd);
        }

        // POST: UpcomingRnds/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "RndID,CompPlayerID,CompID,RndDate,RndType,CourseID,Note")] UpcomingRnd upcomingRnd)
        {
            if (ModelState.IsValid)
            {
                db.Entry(upcomingRnd).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index", new { upcomingRnd.CompPlayerID, upcomingRnd.CompID });
            }
            return View(upcomingRnd);
        }

        public ActionResult DeleteRnd(int RndID)
        {
            UpcomingRnd upcomingRnd = db.UpcomingRnds.Find(RndID);
            db.UpcomingRnds.Remove(upcomingRnd);
            db.SaveChanges();
            return RedirectToAction("Index", new { upcomingRnd.CompPlayerID, upcomingRnd.CompID });
        }








        // GET: UpcomingRnds/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            UpcomingRnd upcomingRnd = db.UpcomingRnds.Find(id);
            if (upcomingRnd == null)
            {
                return HttpNotFound();
            }
            return View(upcomingRnd);
        }

        // POST: UpcomingRnds/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            UpcomingRnd upcomingRnd = db.UpcomingRnds.Find(id);
            db.UpcomingRnds.Remove(upcomingRnd);
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
