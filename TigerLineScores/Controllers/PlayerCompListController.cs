using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using TigerLineScores.Models;

namespace TigerLineScores.Controllers
{
    public class PlayerCompListController : Controller
    {

        private TigerLineScoresEntities1 db = new TigerLineScoresEntities1();
        private CompInfo compInfo = new CompInfo();
        public RndSummary rndSum = new RndSummary();
        public PlayerStats pStats = new PlayerStats();
        public PointsScore pScore = new PointsScore();
        public CourseInfo cInfo = new CourseInfo();

        // GET: PlayerCompList
        public ActionResult Index()
        {
            var CompetitionMains = from cm in db.CompMains
                                   orderby cm.DateStart
                                   select cm;

            foreach (var item in CompetitionMains )
            {
                item.NumberOfPlayers = compInfo.GetNumberOfPlayers(item.CompID);
            }

            return View(CompetitionMains);
        }

        // GET: List of Players
        public ActionResult LeagueTable(int compID)
        {
            CompMain competition = db.CompMains.Find(compID);
            if (competition == null)
            {
                return HttpNotFound();
            }
            // Competition Name & CompID
            ViewBag.CompName = competition.CompName;
            ViewBag.CompID = competition.CompID;

            var pStats = new PlayerStats();
            // Get Player Info for this competition and populate the PlayerView Model
            var getPlayers = (from cp in db.CompPlayers
                              join ur in db.Users on cp.UserID equals ur.UserID
                              join pr in db.Profiles on ur.UserID equals pr.UserID
                              where cp.CompID == compID
                              select new PlayerView
                              {
                                  playerName = ur.UserName,
                                  compPlayerID = cp.CompPlayerID,
                                  Photo = pr.Photo
                                  
                              }).ToList();

            // Loop through each Player and get their Number of Rounds Played, Gross Points, NET Points and average Points per Round
            for (int i = 0; i < getPlayers.Count(); i++)
            {
                getPlayers[i].rndsPlayed = pStats.NumberRndsPlayed(Convert.ToInt32(compID), getPlayers[i].compPlayerID);
                getPlayers[i].totalPoints = pStats.TotalNumberPoints(Convert.ToInt32(compID), getPlayers[i].compPlayerID);
                getPlayers[i].totalNETPoints = pStats.TotalNETPoints(Convert.ToInt32(compID), getPlayers[i].compPlayerID);
                getPlayers[i].avgPointsPerRnd = 0;
                if (getPlayers[i].rndsPlayed > 0)
                {
                    getPlayers[i].avgPointsPerRnd = Math.Round((Double)getPlayers[i].totalNETPoints / getPlayers[i].rndsPlayed, 2);
                }
            };

            // Sort list into Points Descending and apply current Position
            var sortedPlayers = (from sp in getPlayers
                                 orderby sp.avgPointsPerRnd descending, sp.playerName
                                 select sp).ToList();

            for (int i = 0; i < sortedPlayers.Count(); i++)
            {
                sortedPlayers[i].currentPosition = i + 1;
            }

            return View(sortedPlayers);
        }

        // GET: Scores
        public ActionResult PlayerRnds(int compID, int compPlayerID)
        {
            CompMain cMain = db.CompMains.Find(compID);

            var playerRnds = from pr in db.CompScores
                             join cm in db.CourseMains on pr.CourseID equals cm.CourseID
                             where pr.CompID == compID && pr.CompPlayerID == compPlayerID
                             orderby pr.RndDate
                             select new RndSummary { CompScoreID = pr.CompScoreID, RndDate = pr.RndDate, ClubName = cm.ClubName, TeeColour = pr.TeeColour, RndPoints = pr.TotalPoints,
                                                     CourseID = pr.CourseID, SSS = pr.SSS, Handicap = pr.PlayerHcap };

            // Get and store the Total Score for each round
            List<object> playerRndsIncScore = new List<object>();
            int RndNumber = 1;
            foreach (var item in playerRnds)
            {
                var RndIncScore = new RndSummary();
                RndIncScore.CompScoreID = item.CompScoreID;
                RndIncScore.RndDate = item.RndDate;
                RndIncScore.ClubName = item.ClubName;
                RndIncScore.TeeColour = item.TeeColour;
                RndIncScore.Handicap = item.Handicap;
                RndIncScore.RndPoints = item.RndPoints;
                RndIncScore.RndScore = rndSum.GetRndScore(item.CompScoreID);
                RndIncScore.CourseID = item.CourseID;
                RndIncScore.SSS = item.SSS;
                RndIncScore.NETRndPoints = Convert.ToInt32(item.RndPoints - (cInfo.GetCoursePar(RndIncScore.CourseID, RndIncScore.TeeColour) - RndIncScore.SSS));
                RndIncScore.RndNumber = RndNumber;
                RndNumber += 1;
                playerRndsIncScore.Add(RndIncScore);
            }

            // Get Competition Name, Player Name, Current Handicap and Player Photo
            ViewBag.PlayerName = pStats.GetPlayerName(compPlayerID);
            ViewBag.PlayerPhoto = pStats.GetPlayerPhoto(compPlayerID);
            ViewBag.CurrentHCap = pStats.CurrentHcap(compPlayerID);
            ViewBag.CompName = cMain.CompName;
            ViewBag.CompID = cMain.CompID;

            ViewBag.RndSummary = playerRndsIncScore;
            return View();
        }

        public ActionResult PlayerScoreCard(int? compScoreID, int rndNumber)
        {
            ScoreInfo scoreInfo = new ScoreInfo();
            if (compScoreID == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CompScore compScore = db.CompScores.Find(compScoreID);

            int NumberofRnds = pStats.NumberRndsPlayed(Convert.ToInt32(compScore.CompID), compScore.CompPlayerID);
            if (NumberofRnds < rndNumber)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            ViewBag.PHcap = compScore.PlayerHcap;
            ViewBag.PlayerName = pStats.GetPlayerName(compScore.CompPlayerID);
            ViewBag.PlayerPhoto = pStats.GetPlayerPhoto(compScore.CompPlayerID);
            ViewBag.RndDate = compScore.RndDate;
            ViewBag.RndNumber = rndNumber;

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

            // Get Course Details depending on course and tee colour selected
            int courseID = compScore.CourseID;
            string teeColour = compScore.TeeColour;
            decimal? PHcap = compScore.PlayerHcap; 

            ViewBag.scoreCard = cInfo.GetCourseDetails(Convert.ToInt32(courseID), teeColour);

            // Get Hole Scores
            ViewBag.CurrentScore = compScore;
            // Front 9 Total Score
            ViewBag.FrontScore = scoreInfo.FrontScore(compScore.CompScoreID);
            // Back 9 Total Score 
            ViewBag.BackScore = scoreInfo.BackScore(compScore.CompScoreID);
            // Total Score
            ViewBag.TotalScore = ViewBag.FrontScore + ViewBag.BackScore;

            // Blank Points
            CompPoints compPoints = pScore.GetCompPoints(compScore);
            ViewBag.RndPoints = compPoints;

            // Get Course Totals depending on course and tee colour - Store totals in View bag variables
            CourseTotals cTotals = cInfo.GetCourseTotals(Convert.ToInt32(courseID), teeColour);
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

            return View(compScore);

        }

    }
}