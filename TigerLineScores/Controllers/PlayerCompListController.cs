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
using Dapper;

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

        private string posUp = "/Images/posUp.png";
        private string posDown = "/Images/posDown.png";
        private string posNomove = "/Images/posNomove.png";

        // GET: PlayerCompList
        public ActionResult Index()
        {
            var CompetitionMains = from cm in db.CompMains
                                   orderby cm.DateStart
                                   select cm;

            foreach (var item in CompetitionMains)
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
                    int rnds = getPlayers[i].rndsPlayed;
                    if (getPlayers[i].rndsPlayed > 10)
                    {
                        rnds = 10;
                    }
                    
                    getPlayers[i].avgPointsPerRnd = Math.Round((Double)getPlayers[i].totalNETPoints / rnds, 2);
                }

                // Get current Movement Icon
                getPlayers[i].movementIcon = pStats.GetMovementIcon(getPlayers[i].compPlayerID);
            };

            // Sort list into Points Descending and apply current Position

            var sortedPlayers = (from sp in getPlayers
                                 orderby sp.avgPointsPerRnd descending, sp.playerName
                                 select sp).ToList();

            for (int i = 0; i < sortedPlayers.Count(); i++)
            {
                sortedPlayers[i].currentPosition = i + 1;
            }

            // See if the weekly league position movement needs to be carried out
            if (DateTime.Now.Date > competition.LeaguePosUpdate)
            {
                // Calculate Week league position movement for each Player
                for (int i = 0; i < sortedPlayers.Count(); i++)
                {
                    int compPlayerID = sortedPlayers[i].compPlayerID;
                    CompPlayer cPlayer = db.CompPlayers.Find(compPlayerID);
                    int CurrentPos = sortedPlayers[i].currentPosition;
                    int SavedWeekPos = CurrentPos;
                    string updatedMoveIcon = "";
                    if (cPlayer.WeekPos != null)
                    {
                        SavedWeekPos = Convert.ToInt32(cPlayer.WeekPos);
                    }
                    int posMovement = SavedWeekPos - CurrentPos;
                    sortedPlayers[i].posMove = posMovement;

                    if (posMovement > 0)
                    {
                        sortedPlayers[i].movementIcon = posUp;
                        updatedMoveIcon = posUp;
                    }
                    else if (posMovement < 0)
                    {
                        sortedPlayers[i].movementIcon = posDown;
                        updatedMoveIcon = posDown;
                    }
                    else
                    {
                        sortedPlayers[i].movementIcon = posNomove;
                        updatedMoveIcon = posNomove;
                    }
                    cPlayer.WeekPos = CurrentPos;
                    cPlayer.CurrentPos = CurrentPos;
                    cPlayer.MovementIcon = updatedMoveIcon;
                    db.Entry(cPlayer).State = EntityState.Modified;
                }
                // Update the compMain record with new League Postion Update date + 7 Days from Now

                competition.LeaguePosUpdate = DateTime.Now.AddDays(7);
                db.Entry(competition).State = EntityState.Modified;
                db.SaveChanges();
            }

            return View(sortedPlayers);
        }

        // GET: Scores
        public ActionResult PlayerRnds(int compID, int compPlayerID, string sortBy)
        {
            CompMain cMain = db.CompMains.Find(compID);

            var playerRnds = from pr in db.CompScores
                             join cm in db.CourseMains on pr.CourseID equals cm.CourseID
                             where pr.CompID == compID && pr.CompPlayerID == compPlayerID
                             orderby pr.RndDate
                             select new RndSummary
                             {
                                 CompScoreID = pr.CompScoreID,
                                 RndDate = pr.RndDate,
                                 ClubName = cm.ClubName,
                                 TeeColour = pr.TeeColour,
                                 RndPoints = pr.TotalPoints,
                                 CourseID = pr.CourseID,
                                 SSS = pr.SSS,
                                 Handicap = pr.PlayerHcap
                             };

            // Get and store the Total Score for each round
            List<RndSummary> playerRndsIncScore = new List<RndSummary>();
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

            // if Rnd Count > 10 then sort by Rnd Points descending and discard the rnds after the 10th
            if (RndNumber > 10)
            {
                // *** Sort rnds into Points Order ***
                List<RndSummary> checksortedRnds = playerRndsIncScore.OrderByDescending(o => o.NETRndPoints).ToList();
                int c = 0;
                foreach (var item in checksortedRnds)
                {
                    c += 1;
                    if (c > 10)
                    {
                        item.Discard = true;
                    }
                }
            }

            List<RndSummary> sortedRnds;
            if (sortBy == "Points")
            {
               // *** Sort rnds into Points Order ***
               sortedRnds = playerRndsIncScore.OrderByDescending(o => o.NETRndPoints).ToList();
            }
            else
            {
              // *** Sort rnds into Date Order ***
              sortedRnds = playerRndsIncScore.OrderBy(o => o.RndDate).ToList();
            }
                      

            // Get Competition Name, Player Name, Current Handicap and Player Photo
            ViewBag.PlayerName = pStats.GetPlayerName(compPlayerID);
            ViewBag.PlayerPhoto = pStats.GetPlayerPhoto(compPlayerID);
            ViewBag.CurrentHCap = pStats.CurrentHcap(compPlayerID);
            ViewBag.PlayerHomeID = pStats.HomeClubID(compPlayerID);
            ViewBag.CompName = cMain.CompName;
            ViewBag.CompID = cMain.CompID;
            ViewBag.CompPlayerID = compPlayerID;
            ViewBag.SortBy = sortBy;

            ViewBag.RndSummary = sortedRnds;
            return View();
        }


        // GET: Scores
        public ActionResult ClubPlayerRnds(int compID, int courseID)
        {
            CompMain cMain = db.CompMains.Find(compID);

            var playerRnds = (from cs in db.CompScores
                              join cp in db.CompPlayers on cs.CompPlayerID equals cp.CompPlayerID
                              join pr in db.Profiles on cp.UserID equals pr.UserID
                              join ur in db.Users on pr.UserID equals ur.UserID
                              where cs.CompID == compID && cs.CourseID == courseID && pr.HomeClubID == courseID
                              orderby cs.RndDate
                              select new RndSummary
                              {
                                  CompScoreID = cs.CompScoreID,
                                  RndDate = cs.RndDate,
                                  TeeColour = cs.TeeColour,
                                  RndPoints = cs.TotalPoints,
                                  SSS = cs.SSS,
                                  Handicap = cs.PlayerHcap,
                                  Player = ur.UserName
                              }).ToList();

            // Get and store the Total Gross Score and Net Points for each round
            foreach (var item in playerRnds)
            {
                item.NETRndPoints = Convert.ToInt32(item.RndPoints - (cInfo.GetCoursePar(courseID, item.TeeColour) - item.SSS));
                item.RndScore = rndSum.GetRndScore(item.CompScoreID);
            }

            // Sort Club Player Rounds into NET Points Order
            var sortedRnds = from sr in playerRnds
                             orderby sr.NETRndPoints descending
                             select sr;

            // Get Competition Name, Player Name, Current Handicap and Player Photo
            ViewBag.CompName = cMain.CompName;
            ViewBag.CompID = cMain.CompID;
            ViewBag.ClubName = cInfo.GetCourseName(courseID);

            ViewBag.RndSummary = sortedRnds;
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

            // Get Course Logo using a stored procedure
            using (IDbConnection connection = new System.Data.SqlClient.SqlConnection(Helper.CnnValue("TigerLineScoresDB")))
            {
                List<String> courseLogo = connection.Query<string>("dbo.spGetLogoImagePath @CourseID", new { CourseID = compScore.CourseID }).ToList();
                ViewBag.CourseLogo = courseLogo[0];
            }
            
            ViewBag.PHcap = compScore.PlayerHcap;
            ViewBag.PlayerName = pStats.GetPlayerName(compScore.CompPlayerID);
            ViewBag.PlayerPhoto = pStats.GetPlayerPhoto(compScore.CompPlayerID);
            ViewBag.PlayerHomeID = pStats.HomeClubID(compScore.CompPlayerID);
            ViewBag.RndDate = compScore.RndDate;
            ViewBag.RndNumber = rndNumber;

            // Score Card Image
            ViewBag.CardImage = compInfo.GetScoreCardImage(compScore.CompScoreID);

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

            return PartialView("_ScoreCard", compScore);
        }

        public ActionResult EclecticLeagueTable(int courseID, int compID)
        {

            // Get Players in this competition with matching Home Course and populate the BestScoreView Model
            var getPlayers = (from cp in db.CompPlayers
                              join ur in db.Users on cp.UserID equals ur.UserID
                              join pr in db.Profiles on ur.UserID equals pr.UserID
                              where cp.CompID == compID && pr.HomeClubID == courseID
                              select new BestScoreView
                              {
                                  playerName = ur.UserName,
                                  compPlayerID = cp.CompPlayerID

                              }).ToList();

            // Loop through each Player and get their BEST List of Scores
            int loopcount = getPlayers.Count();
            for (int i = 0; i < loopcount; i++)
            {
                int compPlayerID = getPlayers[i].compPlayerID;

                // Check player has posted a round
                var playersRndsCount = (from pr in db.CompScores
                                        where pr.CompPlayerID == compPlayerID && pr.CompID == compID && pr.CourseID == courseID
                                        select pr).Count();

                if (playersRndsCount > 0)
                {
                    List<int> bestScores = pStats.BestScoreList(compPlayerID, courseID);
                    getPlayers[i].Hole1 = bestScores[0];
                    getPlayers[i].Hole2 = bestScores[1];
                    getPlayers[i].Hole3 = bestScores[2];
                    getPlayers[i].Hole4 = bestScores[3];
                    getPlayers[i].Hole5 = bestScores[4];
                    getPlayers[i].Hole6 = bestScores[5];
                    getPlayers[i].Hole7 = bestScores[6];
                    getPlayers[i].Hole8 = bestScores[7];
                    getPlayers[i].Hole9 = bestScores[8];
                    getPlayers[i].Hole10 = bestScores[9];
                    getPlayers[i].Hole11 = bestScores[10];
                    getPlayers[i].Hole12 = bestScores[11];
                    getPlayers[i].Hole13 = bestScores[12];
                    getPlayers[i].Hole14 = bestScores[13];
                    getPlayers[i].Hole15 = bestScores[14];
                    getPlayers[i].Hole16 = bestScores[15];
                    getPlayers[i].Hole17 = bestScores[16];
                    getPlayers[i].Hole18 = bestScores[17];

                    // Number of Rounds
                    getPlayers[i].NumberOfRnds = playersRndsCount;

                    // Get Total Score
                    getPlayers[i].TotalScore = bestScores.Sum();

                    // Get Par Score
                    int parScore = getPlayers[i].TotalScore - cInfo.GetCoursePar(courseID, "Yellow");
                    if (parScore == 0)
                    {
                        getPlayers[i].ParScore = "E";
                    }
                    else if (parScore > 0)
                    {
                        getPlayers[i].ParScore = "+" + parScore.ToString();
                    }
                    else
                    {
                        getPlayers[i].ParScore = parScore.ToString();
                    }
                }
            };

            // Sort List for table
            // Sort list into Points Descending and apply current Position
            var sortedPlayers = (from sp in getPlayers
                                 where sp.TotalScore > 0
                                 orderby sp.TotalScore, sp.playerName
                                 select sp).ToList();

            for (int i = 0; i < sortedPlayers.Count(); i++)
            {
                sortedPlayers[i].currentPosition = i + 1;
            }


            // Get list of hole Pars for this course
            List<int> parList = new List<int>();
            for (int i = 1; i < 19; i++)
            {
                parList.Add(cInfo.GetHolePar(i, courseID, "Yellow"));
            }
            ViewBag.ParList = parList;

            // Get course Name for title
            ViewBag.Course = cInfo.GetCourseName(courseID);
            ViewBag.CompID = compID;
            ViewBag.CourseID = courseID;

            // Get Club Names For Drop Down List
            var courseInfo = new CourseInfo();
            ViewBag.CourseList = courseInfo.GetCourseList();

            // Return List to View
            return PartialView("_EclecticLeagueTable", sortedPlayers);
        }

        public ActionResult EclecticTable(int compID, int courseID)
        {
            // Get Club Names For Drop Down List
            var courseInfo = new CourseInfo();
            ViewBag.CourseList = courseInfo.GetCourseList();

            ViewBag.CompID = compID;
            ViewBag.CourseID = courseID;

            // Return List to View
            return View();
        }

        public ActionResult EclecticPlayerRnds(int compID, int compPlayerID, int courseID)
        {
            PointsScore pScore = new PointsScore();
            var eclecPlayerRnds = (from cs in db.CompScores
                                   join cp in db.CompPlayers on cs.CompPlayerID equals cp.CompPlayerID
                                   join ur in db.Users on cp.UserID equals ur.UserID
                                   join pr in db.Profiles on ur.UserID equals pr.UserID
                                   where cp.CompID == compID && cs.CompPlayerID == compPlayerID && cs.CourseID == courseID
                                   orderby cs.RndDate
                                   select new NetScoreView
                                   {
                                       RndDate = cs.RndDate,
                                       HCap = cs.PlayerHcap,
                                       TeeColour = cs.TeeColour,
                                       CompScoreID = cs.CompScoreID
                                   }).ToList();

            // Loop through each Round to calculate and store Net score Par Score and Total Score
            foreach (var item in eclecPlayerRnds)
            {
                int compScoreID = item.CompScoreID;
                var compScores = from cs in db.CompScores
                                 where cs.CompScoreID == compScoreID
                                 select cs;

                foreach (var gScore in compScores)
                {
                    item.Hole1 = pScore.NetScore(courseID, 1, Convert.ToInt32(item.HCap), item.TeeColour, gScore.Hole1);
                    item.Hole2 = pScore.NetScore(courseID, 2, Convert.ToInt32(item.HCap), item.TeeColour, gScore.Hole2);
                    item.Hole3 = pScore.NetScore(courseID, 3, Convert.ToInt32(item.HCap), item.TeeColour, gScore.Hole3);
                    item.Hole4 = pScore.NetScore(courseID, 4, Convert.ToInt32(item.HCap), item.TeeColour, gScore.Hole4);
                    item.Hole5 = pScore.NetScore(courseID, 5, Convert.ToInt32(item.HCap), item.TeeColour, gScore.Hole5);
                    item.Hole6 = pScore.NetScore(courseID, 6, Convert.ToInt32(item.HCap), item.TeeColour, gScore.Hole6);
                    item.Hole7 = pScore.NetScore(courseID, 7, Convert.ToInt32(item.HCap), item.TeeColour, gScore.Hole7);
                    item.Hole8 = pScore.NetScore(courseID, 8, Convert.ToInt32(item.HCap), item.TeeColour, gScore.Hole8);
                    item.Hole9 = pScore.NetScore(courseID, 9, Convert.ToInt32(item.HCap), item.TeeColour, gScore.Hole9);
                    item.Hole10 = pScore.NetScore(courseID, 10, Convert.ToInt32(item.HCap), item.TeeColour, gScore.Hole10);
                    item.Hole11 = pScore.NetScore(courseID, 11, Convert.ToInt32(item.HCap), item.TeeColour, gScore.Hole11);
                    item.Hole12 = pScore.NetScore(courseID, 12, Convert.ToInt32(item.HCap), item.TeeColour, gScore.Hole12);
                    item.Hole13 = pScore.NetScore(courseID, 13, Convert.ToInt32(item.HCap), item.TeeColour, gScore.Hole13);
                    item.Hole14 = pScore.NetScore(courseID, 14, Convert.ToInt32(item.HCap), item.TeeColour, gScore.Hole14);
                    item.Hole15 = pScore.NetScore(courseID, 15, Convert.ToInt32(item.HCap), item.TeeColour, gScore.Hole15);
                    item.Hole16 = pScore.NetScore(courseID, 16, Convert.ToInt32(item.HCap), item.TeeColour, gScore.Hole16);
                    item.Hole17 = pScore.NetScore(courseID, 17, Convert.ToInt32(item.HCap), item.TeeColour, gScore.Hole17);
                    item.Hole18 = pScore.NetScore(courseID, 18, Convert.ToInt32(item.HCap), item.TeeColour, gScore.Hole18);

                }

                item.TotalScore = item.Hole1 + item.Hole2 + item.Hole3 + item.Hole4 + item.Hole5 + item.Hole6 + item.Hole7 + item.Hole8 + item.Hole9 +
                                  item.Hole10 + item.Hole11 + item.Hole12 + item.Hole13 + item.Hole14 + item.Hole15 + item.Hole16 + item.Hole17 + item.Hole18;

                // Get Par Score
                int parScore = item.TotalScore - cInfo.GetCoursePar(courseID, "Yellow");
                if (parScore == 0)
                {
                    item.ParScore = "E";
                }
                else if (parScore > 0)
                {
                    item.ParScore = "+" + parScore.ToString();
                }
                else
                {
                    item.ParScore = parScore.ToString();
                }

            }

            // CourseID & CompID
            ViewBag.CourseID = courseID;
            ViewBag.CompID = compID;

            // Course Name
            ViewBag.Course = cInfo.GetCourseName(courseID);

            // Player Name
            ViewBag.PlayerName = pStats.GetPlayerName(compPlayerID);

            // Photo
            ViewBag.Photo = pStats.GetPlayerPhoto(compPlayerID);

            // Current Handicap
            ViewBag.CurrentHcap = pStats.CurrentHcap(compPlayerID);

            // Get list of hole Pars for this course
            List<int> parList = new List<int>();
            for (int i = 1; i < 19; i++)
            {
                parList.Add(cInfo.GetHolePar(i, courseID, "Yellow"));
            }
            ViewBag.ParList = parList;

            return View(eclecPlayerRnds);

        }

        public ActionResult InterClubTable(int compID)
        {

            CompMain competition = db.CompMains.Find(compID);
            if (competition == null)
            {
                return HttpNotFound();
            }

            //Gather info for InterClubSummary model
            InterClubInfo inclubInfo = new InterClubInfo();
            var courseM = (from cm in db.CourseMains
                           orderby cm.ClubName
                           select new InterClubSummary
                           {
                               ClubName = cm.ClubName,
                               courseID = cm.CourseID
                           }).ToList();

            // Loop through clubs to calulate and store No of Players, No of Rounds, Total Points and Average Points in InterClub Summary 
            foreach (var item in courseM)
            {
                // Number of Players
                item.NumberOfPlayers = inclubInfo.GetNumberOfPlayers(item.courseID, compID);

                // Number of Rounds
                item.NumberOfRnds = inclubInfo.GetNumberOfRnds(item.courseID, compID);

                // Total Points
                item.TotalPoints = inclubInfo.GetTotalPoints(item.courseID, compID);

                // Average Points
                if (item.NumberOfRnds > 0 && item.TotalPoints > 0)
                {
                    item.AveragePoints = Math.Round((decimal)item.TotalPoints / item.NumberOfRnds, 2);
                }
            }

            //Sort into Average Points descending
            var sortedClubTable = (from st in courseM
                                   orderby st.AveragePoints descending
                                   select st).ToList();
            int p = 0;
            foreach (var item in sortedClubTable)
            {
                p += 1;
                item.Position = p;
            }

            ViewBag.CompName = competition.CompName;
            ViewBag.CompID = compID;

            return View(sortedClubTable);
        }


        //public ActionResult Test(CompMain cm)
        //{

        //    return RedirectToAction("Index");
        //}

    }
}