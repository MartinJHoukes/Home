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
    public class CourseMainsController : Controller
    {
        private TigerLineScoresEntities1 db = new TigerLineScoresEntities1();
        CourseInfo cInfo = new CourseInfo();

        // GET: CourseMains
        public ActionResult Index()
        {
            var AllCourses = from ac in db.CourseMains
                             where ac.ClubName != " < Select a Course >"
                             orderby ac.ClubName
                             select ac;

            return View(AllCourses.ToList());
        }


        // GET: CourseMains/Details/5
        public ActionResult Details(int id)
        {

            CourseMain courseMain = db.CourseMains.Find(id);
            if (courseMain == null)
            {
                return HttpNotFound();
            }

            // Get Course Details (Card of Course)
            var CourseDetail = from cd in db.CourseDetails
                               where cd.CourseID == id
                               select cd;

            ViewBag.ClubName = courseMain.ClubName;

            // Get 9 Holes Total and Store in ViewBag variables
            // Front 9
            ViewBag.FrontWhiteYrds = cInfo.GetCourseTotal(1, 9, id, "WhiteYrds");
            ViewBag.FrontYellowYrds = cInfo.GetCourseTotal(1, 9, id, "YellowYrds");
            ViewBag.FrontMensPar = cInfo.GetCourseTotal(1, 9, id, "MensPar");
            ViewBag.FrontRedYrds = cInfo.GetCourseTotal(1, 9, id, "RedYrds");
            ViewBag.FrontLadiesPar = cInfo.GetCourseTotal(1, 9, id, "LadiesPar");

            //Back 9 
            ViewBag.BackWhiteYrds = cInfo.GetCourseTotal(10, 18, id, "WhiteYrds");
            ViewBag.BackYellowYrds = cInfo.GetCourseTotal(10, 18, id, "YellowYrds");
            ViewBag.BackMensPar = cInfo.GetCourseTotal(10, 18, id, "MensPar");
            ViewBag.BackRedYrds = cInfo.GetCourseTotal(10, 18, id, "RedYrds");
            ViewBag.BackLadiesPar = cInfo.GetCourseTotal(10, 18, id, "LadiesPar");

            // Totals
            ViewBag.TotalWhiteYrds = Convert.ToInt32(ViewBag.FrontWhiteYrds + Convert.ToInt32(ViewBag.BackWhiteYrds));
            ViewBag.TotalYellowYrds = Convert.ToInt32(ViewBag.FrontYellowYrds + Convert.ToInt32(ViewBag.BackYellowYrds));
            ViewBag.TotalMensPar = Convert.ToInt32(ViewBag.FrontMensPar + Convert.ToInt32(ViewBag.BackMensPar));
            ViewBag.TotalRedYrds = Convert.ToInt32(ViewBag.FrontRedYrds + Convert.ToInt32(ViewBag.BackRedYrds));
            ViewBag.TotalLadiesPar = Convert.ToInt32(ViewBag.FrontLadiesPar + Convert.ToInt32(ViewBag.BackLadiesPar));

            return View(CourseDetail.ToList());
        }

        // GET: CourseMains/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: CourseMains/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "CourseID,ClubName,WhiteSSS,YellowSSS,RedSSS")] CourseMain courseMain)
        {
            if (ModelState.IsValid)
            {
                db.CourseMains.Add(courseMain);
                db.SaveChanges();

                // Create 18 Hole Details for Main Course Record
                for (int i = 1; i < 19; i++)
                {
                    var cDetail = new Models.CourseDetail();
                    cDetail.CourseID = courseMain.CourseID;
                    cDetail.HoleNumber = i;

                    db.CourseDetails.Add(cDetail);

                }
                db.SaveChanges();

                return RedirectToAction("Index");
            }

            return View(courseMain);
        }

        // GET: CourseMains/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CourseMain courseMain = db.CourseMains.Find(id);
            if (courseMain == null)
            {
                return HttpNotFound();
            }
            return View(courseMain);
        }

        // POST: CourseMains/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "CourseID,ClubName,WhiteSSS,YellowSSS,RedSSS")] CourseMain courseMain)
        {
            if (ModelState.IsValid)
            {
                db.Entry(courseMain).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(courseMain);
        }

        // GET: CourseMains/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CourseMain courseMain = db.CourseMains.Find(id);
            if (courseMain == null)
            {
                return HttpNotFound();
            }
            return View(courseMain);
        }

        // POST: CourseMains/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            CourseMain courseMain = db.CourseMains.Find(id);
            db.CourseMains.Remove(courseMain);
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

