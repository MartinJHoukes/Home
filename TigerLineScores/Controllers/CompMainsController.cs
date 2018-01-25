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
            var CompFormatList = new List<SelectListItem>();
            {
               CompFormatList.Add(new SelectListItem() { Text = "Order of Merit : Stableford",  Selected = true });
               CompFormatList.Add(new SelectListItem() { Text = "Modified Knock-Out : Stableford" });
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
                DateTime CompStartDate =  Convert.ToDateTime(compMain.DateStart);
                compMain.LeaguePosUpdate = CompStartDate.AddDays(7);
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


        public ActionResult DeleteComp(int compID)
        {
            CompInfo compInfo = new CompInfo();
            // Remove Each Player from this Competition
            var AllPlayers = from ap in db.CompPlayers
                             where ap.CompID == compID
                             select ap;

            foreach (var player in AllPlayers)
            {
                int compPlayerID = player.CompPlayerID;
                compInfo.RemovePlayer(compPlayerID);
            }

            // Remove the Main Competition Record
            CompMain compMain = db.CompMains.Find(compID);
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
