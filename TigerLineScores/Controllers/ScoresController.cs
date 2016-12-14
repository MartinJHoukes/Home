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
    public class ScoresController : Controller
    {
      
        private TigerLineScoresEntities1 db = new TigerLineScoresEntities1();
        public RndSummary rndSum = new RndSummary();
        public PlayerStats pStats = new PlayerStats();
        public PointsScore pScore = new PointsScore();
        public CourseInfo cInfo = new CourseInfo();

        // GET: Scores
        public ActionResult Index(int compID, int compPlayerID)
        {
            CompMain cMain = db.CompMains.Find(compID);

            var playerRnds = from pr in db.CompScores
                             join cm in db.CourseMains on pr.CourseID equals cm.CourseID
                             where pr.CompID == compID && pr.CompPlayerID == compPlayerID
                             orderby pr.RndDate
                             select new RndSummary { CompScoreID = pr.CompScoreID, RndDate = pr.RndDate, ClubName = cm.ClubName, TeeColour = pr.TeeColour, RndPoints = pr.TotalPoints, CourseID = pr.CourseID, SSS = pr.SSS  };

            // Get and store the Total Score for each round
            List<object> playerRndsIncScore = new List<object>();
            foreach (var item in playerRnds)
            {
                 var RndIncScore = new RndSummary();
                RndIncScore.CompScoreID = item.CompScoreID;
                RndIncScore.RndDate = item.RndDate;
                RndIncScore.ClubName = item.ClubName;
                RndIncScore.TeeColour = item.TeeColour;
                RndIncScore.RndPoints = item.RndPoints;
                RndIncScore.RndScore = rndSum.GetRndScore(item.CompScoreID);
                RndIncScore.CourseID = item.CourseID;
                RndIncScore.SSS = item.SSS;
                RndIncScore.NETRndPoints = Convert.ToInt32(item.RndPoints - (cInfo.GetCoursePar(RndIncScore.CourseID, RndIncScore.TeeColour) - RndIncScore.SSS));
                playerRndsIncScore.Add(RndIncScore);
            }

            // Get Competition Name and Player Name
            ViewBag.PlayerName = pStats.GetPlayerName(compPlayerID);
            ViewBag.CompName = cMain.CompName;
            ViewBag.CompID = cMain.CompID;

            ViewBag.RndSummary = playerRndsIncScore;
            return View();
        }

        // GET: Scores/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CompScore compScore = db.CompScores.Find(id);
            if (compScore == null)
            {
                return HttpNotFound();
            }
            return View(compScore);
        }

        // GET: Scores/Create
        public ActionResult Create(int compID, int compPlayerID)
        {
            // Set default Values for the CompScore model initial view
            var scoresModel =  new CompScore();
            // default players home course
            var pStats = new PlayerStats();
            scoresModel.CourseID = pStats.HomeClubID(compPlayerID);

            // Get Competition Name
            CompMain compMain = db.CompMains.Find(compID);
            ViewBag.CompName = compMain.CompName;
            ViewBag.CompID = compID;

            // Get Player Name
            CompPlayer compPlayer = db.CompPlayers.Find(compPlayerID);
            int userID = compPlayer.UserID;
            User users = db.Users.Find(userID);
            ViewBag.PlayerName = users.UserName;   

            // Get Players Current Handicap
            ViewBag.Hcap = pStats.CurrentHcap(compPlayerID);
            ViewBag.PHcap = Math.Round(ViewBag.HCap,MidpointRounding.AwayFromZero);
            
            // Get Club Names For Drop Down List
            var courseInfo = new CourseInfo();
            ViewBag.CourseList = courseInfo.GetCourseList();

            return View(scoresModel);
        }

        // POST: Scores/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "CompScoreID,CompID,CompPlayerID,CourseID,TeeColour,SSS,Hole1,Hole2,Hole3,Hole4,Hole5,Hole6,Hole7,Hole8,Hole9,Hole10,Hole11,Hole12,Hole13,Hole14,Hole15,Hole16,Hole17,Hole18,PlayerHcap,RndDate")] CompScore compScore)
        {
            if (ModelState.IsValid)
            {
                // Create the points model for this rounds score
                CompPoints compPoints = pScore.GetCompPoints(compScore);
                ViewBag.RndPoints = compPoints;

                // Get the points total
                compScore.TotalPoints = compPoints.TotalPoints;
                
                db.CompScores.Add(compScore);
                db.SaveChanges();
                int compScoreID = compScore.CompScoreID;
                return RedirectToAction("Edit","Scores", new { compScoreID });
            }

            return Redirect(Request.UrlReferrer.PathAndQuery);
        }

        // GET: Scores/Edit/5
        public ActionResult Edit(int? CompScoreID)
        {
            if (CompScoreID == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CompScore compScore = db.CompScores.Find(CompScoreID);
            ViewBag.PHcap = compScore.PlayerHcap;
            ViewBag.PlayerName = pStats.GetPlayerName(compScore.CompPlayerID);

            CourseMain courseM = db.CourseMains.Find(compScore.CourseID);
            ViewBag.CourseName = courseM.ClubName;

            CompMain cMain = db.CompMains.Find(compScore.CompID);
            ViewBag.CompName = cMain.CompName;

            // Get the Course Par 
            ViewBag.CoursePar = cInfo.GetCoursePar(compScore.CourseID, compScore.TeeColour);

            // Get the NET Points (Gross Points - (CoursePar - SSS))
            ViewBag.NetPoints = compScore.TotalPoints - (ViewBag.CoursePar - compScore.SSS);
 
            if (compScore == null)
            {
                return HttpNotFound();
            }
            return View(compScore);
        }

        // POST: Scores/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "CompScoreID,CompID,CompPlayerID,CourseID,TeeColour,SSS,Hole1,Hole2,Hole3,Hole4,Hole5,Hole6,Hole7,Hole8,Hole9,Hole10,Hole11,Hole12,Hole13,Hole14,Hole15,Hole16,Hole17,Hole18,PlayerHcap,RndDate")] CompScore compScore)
        {
            if (ModelState.IsValid)
            {
                // Create the Points Score
                CompPoints compPoints = pScore.GetCompPoints(compScore);
                ViewBag.RndPoints = compPoints;

                // Get the points total
                compScore.TotalPoints = compPoints.TotalPoints;

                db.Entry(compScore).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index", new { compScore.CompID, compScore.CompPlayerID });
            }
            return View(compScore);
        }

        // GET: Scores/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CompScore compScore = db.CompScores.Find(id);
            if (compScore == null)
            {
                return HttpNotFound();
            }
            return View(compScore);
        }

        // POST: Scores/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            CompScore compScore = db.CompScores.Find(id);
            db.CompScores.Remove(compScore);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        // Get the Selected Course SSS and return via a Json call
        public ActionResult CourseSSS(int id, string teecolour)
        {
            CourseInfo cinfo = new CourseInfo();
            CompScore compscore = new CompScore();
            {
                compscore.CourseID = id;
                compscore.SSS = cinfo.GetCourseSSS(id, teecolour);
            };

            return Json(new { newcompScore = compscore }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ScoreCard(int? id, string teeColour, int? PHcap)
        {
            CourseInfo cInfo = new CourseInfo();

            // Get Course Details depending on course and tee colour selected
            ViewBag.scoreCard = cInfo.GetCourseDetails(Convert.ToInt32(id), teeColour);

            // Blank Scores
            var compScore = new CompScore();
            ViewBag.CurrentScore = compScore;
            // Blank Points
            CompPoints compPoints = pScore.GetCompPoints(compScore);
            ViewBag.RndPoints = compPoints;


            // Get Course Totals depending on course and tee colour - Store totals in View bag variables
            CourseTotals cTotals = cInfo.GetCourseTotals(Convert.ToInt32(id), teeColour);
            ViewBag.TeeColour = cTotals.teeColour;
            ViewBag.FrontYrds = cTotals.frontYrds;
            ViewBag.BackYrds = cTotals.backYrds;
            ViewBag.TotalYrds = cTotals.totalYrds;
            ViewBag.FrontPar = cTotals.frontPar;
            ViewBag.BackPar = cTotals.backPar;
            ViewBag.TotalPar = cTotals.totalPar;
           
            // Store the playing handicap in the ViewBag
            var playingHCap = 0;
            if (PHcap != null)
            {
                playingHCap = Convert.ToInt32(PHcap);
            }
            ViewBag.PHcap = playingHCap; 
        
            return PartialView("_ScoreCard");
        }

        public ActionResult ScoreCardEdit(int compScoreID)
        {
            CompScore compScore = db.CompScores.Find(compScoreID);
            if (compScore == null)
            {
                return HttpNotFound();
            }

            CourseInfo cInfo = new CourseInfo();

            // Get Course Details depending on course and tee colour selected
            int courseID = compScore.CourseID;
            string teeColour = compScore.TeeColour;
            ViewBag.scoreCard = cInfo.GetCourseDetails(Convert.ToInt32(courseID), teeColour);

            // Get Course Totals depending on course and tee colour - Store totals in View bag variables
            CourseTotals cTotals = cInfo.GetCourseTotals(Convert.ToInt32(courseID), teeColour);
            ViewBag.TeeColour = cTotals.teeColour;
            ViewBag.FrontYrds = cTotals.frontYrds;
            ViewBag.BackYrds = cTotals.backYrds;
            ViewBag.TotalYrds = cTotals.totalYrds;
            ViewBag.FrontPar = cTotals.frontPar;
            ViewBag.BackPar = cTotals.backPar;
            ViewBag.TotalPar = cTotals.totalPar;

            ViewBag.PHcap = compScore.PlayerHcap;

            // Store the current scores in a viewbag variable. 
            ViewBag.CurrentScore = compScore;

            // Store the current points in a viewbag variable
            CompPoints compPoints = pScore.GetCompPoints(compScore);
            ViewBag.RndPoints = compPoints;

            return PartialView("_ScoreCard");

        }

        // Calculate the Points Score
        public ActionResult PointsScore(int Hcap, int SI, int strokes, int par)
        {
            PointsScore pScore = new Models.PointsScore();
            {
                // Calculate points for this hole and send back using Json
                pScore.points = pScore.calculatePointsScore(Hcap, SI, strokes, par);
            }
            
            return Json(new { newPoints = pScore }, JsonRequestBehavior.AllowGet);

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
