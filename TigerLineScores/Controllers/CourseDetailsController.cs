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
    public class CourseDetailsController : Controller
    {
        private TigerLineScoresEntities1 db = new TigerLineScoresEntities1();

        // GET: CourseDetails
        public ActionResult Index()
        {
            return View(db.CourseDetails.ToList());
        }

        // GET: CourseDetails/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CourseDetail courseDetail = db.CourseDetails.Find(id);
            if (courseDetail == null)
            {
                return HttpNotFound();
            }
            return View(courseDetail);
        }

        // GET: CourseDetails/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: CourseDetails/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "CourseDetailID,CourseID,HoleNumber,WhiteYrds,YellowYrds,MensPar,MensSI,RedYrds,LadiesPar,LadiesSI")] CourseDetail courseDetail)
        {
            if (ModelState.IsValid)
            {
                db.CourseDetails.Add(courseDetail);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(courseDetail);
        }

        // GET: CourseDetails/Edit/5

        public ActionResult Edit(int? courseID, int? holeNumber)
        {
            if (courseID == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            // Get the CourseDetailID from CourseID and Hole Number
            var HoleDetails = from hd in db.CourseDetails
                              where hd.CourseID == courseID && hd.HoleNumber == holeNumber
                              select hd;
            int courseDetailID = 0;
            foreach (var item in HoleDetails)
            {
                courseDetailID = item.CourseDetailID;
            }

            CourseDetail courseDetail = db.CourseDetails.Find(courseDetailID);
            if (courseDetail == null)
            {
                return HttpNotFound();
            }
            return View(courseDetail);
        }


        // POST: CourseDetails/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "CourseDetailID,CourseID,HoleNumber,WhiteYrds,YellowYrds,MensPar,MensSI,RedYrds,LadiesPar,LadiesSI")] CourseDetail courseDetail)
        {
            if (ModelState.IsValid)
            {
                db.Entry(courseDetail).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Details", "CourseMains", new { id = courseDetail.CourseID });
            }
            return View(courseDetail);
        }

        // GET: CourseDetails/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CourseDetail courseDetail = db.CourseDetails.Find(id);
            if (courseDetail == null)
            {
                return HttpNotFound();
            }
            return View(courseDetail);
        }

        // POST: CourseDetails/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            CourseDetail courseDetail = db.CourseDetails.Find(id);
            db.CourseDetails.Remove(courseDetail);
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


        // Get Hole Details and display View
        public ActionResult HoleDetail(int courseID, int holeNumber)
        {
            // Get the Club Name and store in Session variable
            var Course = from co in db.CourseMains
                         where co.CourseID == courseID
                         select co;

            foreach (var item in Course)
            {
                Session["EditHoleClubName"] = item.ClubName; 
            }

            return RedirectToAction("Edit", "CourseDetails", new { courseID = courseID, holeNumber = holeNumber });

        }

    }
}
