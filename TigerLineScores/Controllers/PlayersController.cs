using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TigerLineScores.Models;

namespace TigerLineScores.Controllers
{
    public class PlayersController : Controller
    {

        private TigerLineScoresEntities1 db = new TigerLineScoresEntities1();
        private CourseInfo cInfo = new CourseInfo();
        private string posUp = "/Images/posUp.png";
        private string posDown = "/Images/posDown.png";
        private string posNomove = "/Images/posNomove.png";

        // GET: Players
        public ActionResult Index(int? CompID)
        {
            CompMain competition = db.CompMains.Find(CompID);
            if (competition == null)
            {
                return HttpNotFound();
            }
            // Competition Name & CompID
            ViewBag.CompName = competition.CompName;
            ViewBag.CompID = competition.CompID;

            var pStats = new PlayerStats();

            // List of Players NOT already in this Competition to show in check box list when adding players.
            var playerList = (from pl in db.Users
                              orderby pl.UserName
                              select pl).ToList();

            for (int i = playerList.Count - 1; i > -1; i--)
            {
                if (pStats.PlayerInComp(playerList[i].UserID, Convert.ToInt32(CompID)))
                {
                    playerList.RemoveAt(i);
                }
            }
            ViewBag.PlayerList = playerList;


            // Get Player Info for this competition and populate the PlayerView Model
            var getPlayers = (from cp in db.CompPlayers
                              join ur in db.Users on cp.UserID equals ur.UserID
                              where cp.CompID == CompID
                              select new PlayerView
                              {
                                  playerName = ur.UserName,
                                  compPlayerID = cp.CompPlayerID
                              }).ToList();

            // Loop through each Player and get their Number of Rounds Played, Gross Points, NET Points and average Points per Round
            for (int i = 0; i < getPlayers.Count(); i++)
            {
                getPlayers[i].rndsPlayed = pStats.NumberRndsPlayed(Convert.ToInt32(CompID), getPlayers[i].compPlayerID);
                getPlayers[i].totalPoints = pStats.TotalNumberPoints(Convert.ToInt32(CompID), getPlayers[i].compPlayerID);
                getPlayers[i].totalNETPoints = pStats.TotalNETPoints(Convert.ToInt32(CompID), getPlayers[i].compPlayerID);
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
            };

            // Sort list into Points Descending and apply current Position
            var sortedPlayers = (from sp in getPlayers
                                 orderby sp.avgPointsPerRnd descending, sp.playerName
                                 select sp).ToList();

            for (int i = 0; i < sortedPlayers.Count(); i++)
            {
                sortedPlayers[i].currentPosition = i + 1;

                // Calc Positional Movement Current Position - Saved Current Position
                int savedPos = pStats.GetCurrentPosition(sortedPlayers[i].compPlayerID);
                if (savedPos > 0)
                {
                    int posMovement = savedPos - (i + 1);
                    sortedPlayers[i].posMove = posMovement;

                    if (posMovement > 0)
                    {
                        sortedPlayers[i].movementIcon = posUp;
                        pStats.SaveMovementIcon(sortedPlayers[i].compPlayerID, posUp);
                    }
                    else if (posMovement < 0)
                    {
                        sortedPlayers[i].movementIcon = posDown;
                        pStats.SaveMovementIcon(sortedPlayers[i].compPlayerID, posDown);
                    }
                    else
                    {
                        sortedPlayers[i].movementIcon = pStats.GetMovementIcon(sortedPlayers[i].compPlayerID);
                    }
                }

                //Update Current Player Position in Comp Player Table
                pStats.SaveCurrentPos(sortedPlayers[i].compPlayerID, i + 1);
            }

            return View(sortedPlayers);
        }

        [HttpPost]
        public ActionResult Index(int[] SelectedPlayers, int CompID)
        {
            if (SelectedPlayers != null)
            {
                // Add the New Player(s) to this competition
                for (int i = 0; i < SelectedPlayers.Length; i++)
                {
                    var cPlayer = new Models.CompPlayer();
                    cPlayer.CompID = CompID;
                    cPlayer.UserID = SelectedPlayers[i];
                    cPlayer.MovementIcon = posNomove;
                    db.CompPlayers.Add(cPlayer);
                }
                db.SaveChanges();
            }

            return RedirectToAction("Index", new { compID = CompID });
        }

        public ActionResult DeletePlayer(int compPlayerID, int compID)
        {
            //Remove ALL records relating to this Comp Player
            CompInfo compInfo = new CompInfo();
            compInfo.RemovePlayer(compPlayerID);
            return RedirectToAction("Index", new { compID });
        }
    }


}