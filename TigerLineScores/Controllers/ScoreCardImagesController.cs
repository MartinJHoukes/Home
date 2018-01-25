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
    public class ScoreCardImagesController : Controller
    {
        private TigerLineScoresEntities1 db = new TigerLineScoresEntities1();
        PlayerStats pstats = new PlayerStats();

        // GET: ScoreCardImages
        public ActionResult Index(int compID)
        {
            var pendingscoreCards = from sc in db.ScoreCardImages
                                    join ur in db.Users on sc.UserID equals ur.UserID
                                    join cr in db.CourseMains on sc.CourseID equals cr.CourseID
                                    where sc.Processed == false && sc.CompID == compID
                                    select new ScoreCardView
                                    {
                                        playerName = ur.UserName,
                                        courseName = cr.ClubName,
                                        teeColour = sc.TeeColour,
                                        SSS = sc.SSS,
                                        rndDate = sc.RoundDate,
                                        Note = sc.Note,
                                        cardImage = sc.CardImage,
                                        UserID = sc.UserID,
                                        CourseID = sc.CourseID,
                                        ImageID = sc.ImageID
                                    };

            ViewBag.CompID = compID;
            return View(pendingscoreCards);
        }

        // GET: USER ScoreCard PENDING
        public ActionResult PendingScoreCards(int userID, int compID)
        {
            //Check User is in this Competition
            CompInfo cInfo = new CompInfo();
            Boolean IsInComp = cInfo.IsUserinComp(compID, userID);
            if (IsInComp == false)
            {
                TempData["FailUpload"] = "You need to be in a competition to view your pending score cards ..";
                TempData["SuccessUpload"] = null;

                return RedirectToAction("Index", "PLayerCompList");
            }

            var pendingscoreCards = from sc in db.ScoreCardImages
                                    join ur in db.Users on sc.UserID equals ur.UserID
                                    join cr in db.CourseMains on sc.CourseID equals cr.CourseID
                                    where sc.Processed == false && sc.UserID == userID
                                    orderby sc.RoundDate
                                    select new ScoreCardView
                                    {
                                        playerName = ur.UserName,
                                        courseName = cr.ClubName,
                                        teeColour = sc.TeeColour,
                                        SSS = sc.SSS,
                                        rndDate = sc.RoundDate,
                                        Note = sc.Note,
                                        cardImage = sc.CardImage,
                                        UserID = sc.UserID,
                                        CourseID = sc.CourseID,
                                        ImageID = sc.ImageID
                                    };

            // Get Competition Name, Player Name, Current Handicap and Player Photo
            PlayerStats pStats = new PlayerStats();
            var compPlayers = from cp in db.CompPlayers
                              where cp.UserID == userID && cp.CompID == compID
                              select cp;

            int compPlayerID = 0;
            foreach (var item in compPlayers)
            {
                compPlayerID = item.CompPlayerID;
            }

            CompMain cMain = db.CompMains.Find(compID);
            ViewBag.PlayerName = pStats.GetPlayerName(compPlayerID);
            ViewBag.PlayerPhoto = pStats.GetPlayerPhoto(compPlayerID);
            ViewBag.CurrentHCap = pStats.CurrentHcap(compPlayerID);
            ViewBag.CompName = cMain.CompName;

            return View(pendingscoreCards);
        }



        // GET: ScoreCardImages/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ScoreCardImage scoreCardImage = db.ScoreCardImages.Find(id);
            if (scoreCardImage == null)
            {
                return HttpNotFound();
            }
            return View(scoreCardImage);
        }

        // GET: ScoreCardImages/Create
        public ActionResult Create(int CompID)
        {
            if ((Session["userid"]) != null)
            {
                var cardImageModel = new ScoreCardImage();

                //Check User is in this Competition
                CompInfo cInfo = new CompInfo();
                int userID = Convert.ToInt32(Session["userid"]);
                Boolean IsInComp = cInfo.IsUserinComp(CompID, userID);
                if (IsInComp == false)
                {
                    TempData["FailUpload"] = "You need to be in this competition to upload a score card ..";
                    TempData["SuccessUpload"] = null;

                    return RedirectToAction("Index", "PLayerCompList");
                }

                // default players home course
                var pStats = new PlayerStats();

                cardImageModel.UserID = userID;

                // competition name
                var compInfo = new CompInfo();
                ViewBag.CompName = compInfo.GetCompetitonName(CompID);
                cardImageModel.CompID = CompID;

                // User Name
                ViewBag.UserName = Session["UserName"];

                //CompPlayerID
                ViewBag.CompPlayerID = Session["CompPlayerID"];

                // Get Club Names For Drop Down List
                var courseInfo = new CourseInfo();
                ViewBag.CourseList = courseInfo.GetCourseList();

                // Tee Colour List
                List<SelectListItem> teecolor = new List<SelectListItem>();
                teecolor.Add(new SelectListItem() { Text = "White", Value = "White" });
                teecolor.Add(new SelectListItem() { Text = "Yellow", Value = "Yellow" });
                teecolor.Add(new SelectListItem() { Text = "Red", Value = "Red" });
                ViewBag.TeeColour = teecolor;

                // List of Upcoming Rounds
                int compPlayerID = Convert.ToInt32(Session["CompPlayerID"]);
                var Rnds = from rd in db.UpcomingRnds
                           where rd.CompPlayerID == compPlayerID && rd.CompID == CompID
                           orderby rd.RndDate
                           select rd;

                int rndCount = 1;
                int upcomingCourseID = 0;
                DateTime upcomingRndDate = new DateTime();
                string upcomingTeeColor = "Yellow";
                string upcomingNote = "";
                foreach (var item in Rnds)
                {
                    item.CourseName = courseInfo.GetCourseName(item.CourseID);
                    if (rndCount == 1)
                    {
                        upcomingCourseID = item.CourseID;
                        upcomingRndDate = Convert.ToDateTime(item.RndDate);
                        upcomingNote = item.Note;
                        if (item.RndType == "Competition")
                        {
                            upcomingTeeColor = "White";
                        }
                    }
                    rndCount += 1;
                }
                ViewBag.UpcomingRnds = Rnds.ToList();

                // Pre-Populate the Model Items with the FIRST Upcoming Rnd
                cardImageModel.CourseID = upcomingCourseID;
                cardImageModel.RoundDate = upcomingRndDate;
                cardImageModel.TeeColour = upcomingTeeColor;
                cardImageModel.Note = upcomingNote;

                return View(cardImageModel);
            }

            return RedirectToAction("Index", "Home");
        }

        // POST: ScoreCardImages/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ImageID,CompID,CardImage,Processed,Note,UserID,RoundDate,CourseID,TeeColour,SSS")] ScoreCardImage scoreCardImage)
        {
            if (ModelState.IsValid)
            {
                // Deal with uploaded Score Card Photo PENDING ******************************
                string scorecardfileName = "";
                HttpPostedFileBase scorecard = Request.Files["UploadScoreCard"];
                if (scorecard.FileName != "" && scoreCardImage.UserID != null)
                {
                    scorecardfileName = scoreCardImage.UserID + new FileInfo(scorecard.FileName).Name;
                    string path = Path.Combine(Server.MapPath("~/ScoreCards/Pending"), scorecardfileName);

                    //Reduce size of file if required and save in large folder
                    WebImage img = new WebImage(scorecard.InputStream);
                    if (img.Height > 1200)
                    {
                        img.Resize(1200, 1200, true);
                    }
                    img.Save(path);

                    TempData["FailUpload"] = null;
                    TempData["SuccessUpload"] = "Score Card Image successfully Uploaded. You can edit this pending score card by selecting 'My Pending Score Cards' on the main menu ..";

                    // Save the Score Card Image
                    scoreCardImage.CardImage = "/ScoreCards/Pending/" + scorecardfileName;
                    scoreCardImage.Processed = false;
                    db.ScoreCardImages.Add(scoreCardImage);
                    db.SaveChanges();

                    // Remove the FIRST Upcoming Round Record
                    // List of Upcoming Rounds
                    int CompID = scoreCardImage.CompID;
                    int compPlayerID = Convert.ToInt32(Session["CompPlayerID"]);
                    var FirstRnd = (from rd in db.UpcomingRnds
                                    where rd.CompPlayerID == compPlayerID && rd.CompID == CompID
                                    orderby rd.RndDate
                                    select rd).First();

                    db.UpcomingRnds.Remove(FirstRnd);
                    db.SaveChanges();

                    // Send email to Admin to inform of new scorecard upload
                    MessageInfo messageInfo = new MessageInfo();
                    UserInfo userInfo = new UserInfo();

                    var AllAdmin = userInfo.GetAllAdmin();

                    string AdminEmail = "";
                    string Subject = "";
                    string Body = "";
                    string userName = userInfo.GetUserName(Convert.ToInt32(scoreCardImage.UserID));

                    foreach (var item in AllAdmin)
                    {
                       // messageInfo.CreateMessage(newprofile.UserID, item.UserID, DateTime.Now, user.UserName + " has registered to join Tigerline Scores.");
                        AdminEmail = item.Email;
                        Subject = "PLAYER SCORECARD IMAGE UPLOADED";
                        Body = "<span style='font-family: Calibri; font-size: 24px; font-weight: bold; color: green'>TIGERLINE SCORES</span><br/><br/>";
                        Body += "<span style='font-family: Calibri'>Player " + userName + " has uploaded a new scorecard image ..<br/><br/>";
                        Body += "<a href='www.tigerlinescores.co.uk'>Tigerline Scores</a>";
                        messageInfo.SendEmail(AdminEmail, Subject, Body, null);
                    }

                        return RedirectToAction("Index", "PLayerCompList");
                }
                else
                {
                    TempData["FailUpload"] = "Unable to upload the score card image ..";
                    TempData["SuccessUpload"] = null;
                }
            }
            return RedirectToAction("Index", "PLayerCompList");
        }

        // GET: ScoreCardImages/Edit/5
        public ActionResult Edit(int? imageID)
        {
            if (imageID == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ScoreCardImage scoreCardImage = db.ScoreCardImages.Find(imageID);
            if (scoreCardImage == null)
            {
                return HttpNotFound();
            }


            // competition name
            var compInfo = new CompInfo();
            ViewBag.CompName = compInfo.GetCompetitonName(scoreCardImage.CompID);

            // User Name
            ViewBag.UserName = Session["UserName"];

            // Get Club Names For Drop Down List
            var courseInfo = new CourseInfo();
            ViewBag.CourseList = courseInfo.GetCourseList();

            // Tee Colour List
            List<SelectListItem> teecolor = new List<SelectListItem>();
            teecolor.Add(new SelectListItem() { Text = "White", Value = "White" });
            teecolor.Add(new SelectListItem() { Text = "Yellow", Value = "Yellow" });
            teecolor.Add(new SelectListItem() { Text = "Red", Value = "Red" });
            ViewBag.TeeColour = teecolor;

            return View(scoreCardImage);
        }

        // POST: ScoreCardImages/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ImageID,CompID,CardImage,Processed,Note,UserID,RoundDate,CourseID,TeeColour,SSS")] ScoreCardImage scoreCardImage)
        {
            if (ModelState.IsValid)
            {
                string scorecardfileName = "";
                HttpPostedFileBase scorecard = Request.Files["UploadScoreCard"];
                if (scorecard.FileName != "" && scoreCardImage.UserID != null)  //Deal with New Score Card Image
                {
                    scorecardfileName = scoreCardImage.UserID + new FileInfo(scorecard.FileName).Name;
                    string path = Path.Combine(Server.MapPath("~/ScoreCards/Pending"), scorecardfileName);

                    //Reduce size of file if required and save in large folder
                    WebImage img = new WebImage(scorecard.InputStream);
                    if (img.Height > 1200)
                    {
                        img.Resize(1200, 1200, true);
                    }
                    img.Save(path);

                    TempData["FailUpload"] = null;
                    TempData["SuccessUpload"] = "Your NEW score card image has been successfully uploaded";

                    // Save the Score Card Image
                    scoreCardImage.CardImage = "/ScoreCards/Pending/" + scorecardfileName;
                }
                else
                {
                    TempData["SuccessUpload"] = "Your pending score card has been successfully edited";
                }


                db.Entry(scoreCardImage).State = EntityState.Modified;
                db.SaveChanges();

                // Send email to Admin to inform of edited score card image.
                MessageInfo messageInfo = new MessageInfo();
                UserInfo userInfo = new UserInfo();

                var AllAdmin = userInfo.GetAllAdmin();

                string AdminEmail = "";
                string Subject = "";
                string Body = "";
                string userName = userInfo.GetUserName(Convert.ToInt32(scoreCardImage.UserID));

                foreach (var item in AllAdmin)
                {
                    // messageInfo.CreateMessage(newprofile.UserID, item.UserID, DateTime.Now, user.UserName + " has registered to join Tigerline Scores.");
                    AdminEmail = item.Email;
                    Subject = "PLAYER PENDING SCORECARD EDITED";
                    Body = "<span style='font-family: Calibri; font-size: 24px; font-weight: bold; color: green'>TIGERLINE SCORES</span><br/><br/>";
                    Body += "<span style='font-family: Calibri'>Player " + userName + " has edited their pending scorecard ..<br/><br/>";
                    Body += "<a href='www.tigerlinescores.co.uk'>Tigerline Scores</a>";
                    messageInfo.SendEmail(AdminEmail, Subject, Body, null);
                }

                // Get Club Names For Drop Down List
                var courseInfo = new CourseInfo();
                ViewBag.CourseList = courseInfo.GetCourseList();

                // Tee Colour List
                List<SelectListItem> teecolor = new List<SelectListItem>();
                teecolor.Add(new SelectListItem() { Text = "White", Value = "White" });
                teecolor.Add(new SelectListItem() { Text = "Yellow", Value = "Yellow" });
                teecolor.Add(new SelectListItem() { Text = "Red", Value = "Red" });
                ViewBag.TeeColour = teecolor;

            }
            return View(scoreCardImage);
        }

        public ActionResult Delete(int imageID, int userID, int compID)
        {
            try
            {
                ScoreCardImage scard = db.ScoreCardImages.Find(imageID);
                db.ScoreCardImages.Remove(scard);
                db.SaveChanges();

                // Inform Admin that the pending scorecard has been removed
                MessageInfo messageInfo = new MessageInfo();
                UserInfo userInfo = new UserInfo();
                var AllAdmin = userInfo.GetAllAdmin();

                string AdminEmail = "";
                string Subject = "";
                string Body = "";
                string userName = userInfo.GetUserName(Convert.ToInt32(userID));
                foreach (var item in AllAdmin)
                {
                    // messageInfo.CreateMessage(newprofile.UserID, item.UserID, DateTime.Now, user.UserName + " has registered to join Tigerline Scores.");
                    AdminEmail = item.Email;
                    Subject = "PLAYER PENDING SCORECARD REMOVED";
                    Body = "<span style='font-family: Calibri; font-size: 24px; font-weight: bold; color: green'>TIGERLINE SCORES</span><br/><br/>";
                    Body += "<span style='font-family: Calibri'>Player " + userName + " has removed their pending scorecard ..<br/><br/>";
                    Body += "<a href='www.tigerlinescores.co.uk'>Tigerline Scores</a>";
                    messageInfo.SendEmail(AdminEmail, Subject, Body, null);
                }

                return RedirectToAction("PendingScoreCards", new { userID = userID, compID = compID });
            }
            catch
            {//TODO: Log error				
                return Content(Boolean.FalseString);
            }
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
