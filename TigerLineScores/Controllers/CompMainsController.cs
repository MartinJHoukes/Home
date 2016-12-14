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
    [Authorize]
    public class CompMainsController : Controller
    {
        private TigerLineScoresEntities1 db = new TigerLineScoresEntities1();

        // GET: CompMains
       
        public ActionResult Index()
        {
            return View(db.CompMains.ToList());
        }

        // GET: CompMains/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CompMain compMain = db.CompMains.Find(id);
            if (compMain == null)
            {
                return HttpNotFound();
            }
            return View(compMain);
        }

        // GET: CompMains/Create
        public ActionResult Create()
        {

            // Create Format DDL Options
            var CompFormatList = new List<SelectListItem>
            {
                new SelectListItem{ Text = "Stroke Play"},
                new SelectListItem{ Text = "Stableford", Selected = true},
            };
           
            ViewBag.FormatList = CompFormatList; 

            return View();
        }

        // POST: CompMains/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "CompID,CompName,DateStart,DateEnd,Format")] CompMain compMain)
        {
            if (ModelState.IsValid)
            {
                db.CompMains.Add(compMain);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(compMain);
        }

        // GET: CompMains/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CompMain compMain = db.CompMains.Find(id);
            if (compMain == null)
            {
                return HttpNotFound();
            }

            // Create Format DDL Options
            var CompFormatList = new List<SelectListItem>
            {
                new SelectListItem{ Text = "Stroke Play"},
                new SelectListItem{ Text = "Stableford", Selected = true},
            };

            ViewBag.FormatList = CompFormatList;

            return View(compMain);
        }

        // POST: CompMains/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "CompID,CompName,DateStart,DateEnd,Format")] CompMain compMain)
        {
            if (ModelState.IsValid)
            {
                db.Entry(compMain).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(compMain);
        }

        // GET: CompMains/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CompMain compMain = db.CompMains.Find(id);
            if (compMain == null)
            {
                return HttpNotFound();
            }
            return View(compMain);
        }

        // POST: CompMains/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            CompMain compMain = db.CompMains.Find(id);
            db.CompMains.Remove(compMain);
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
