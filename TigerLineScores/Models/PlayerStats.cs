using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace TigerLineScores.Models
{
    public class PlayerStats
    {

        private TigerLineScoresEntities1 db = new TigerLineScoresEntities1();
        private CourseInfo cInfo = new CourseInfo();
        private PointsScore pScore = new PointsScore();

        //Get Player Name
        public string GetPlayerName(int? CompPlayerID)
        {
            string pName = "";
            var pUsers = from pl in db.CompPlayers
                         join ur in db.Users on pl.UserID equals ur.UserID
                         where pl.CompPlayerID == CompPlayerID
                         select ur;

            foreach (var item in pUsers)
            {
                pName = item.UserName;
            }
            return pName;
        }

        // Get Player Photo
        public string GetPlayerPhoto(int compPlayerID)
        {
            string pPhoto = "";
            var pUsers = from pl in db.CompPlayers
                         join ur in db.Users on pl.UserID equals ur.UserID
                         join pr in db.Profiles on ur.UserID equals pr.UserID
                         where pl.CompPlayerID == compPlayerID
                         select pr;

            foreach (var item in pUsers)
            {
                if (item.Photo != null)
                {
                    pPhoto = item.Photo;
                }
            }
            return pPhoto;
        }

        // Get Players Current H'Cap
        public decimal CurrentHcap(int compPlayerID)
        {
            decimal Hcap = 0;
            CompPlayer cplayer = db.CompPlayers.Find(compPlayerID);
            int userID = cplayer.UserID;
            var pProfile = from pp in db.Profiles
                           where pp.UserID == userID
                           select pp;

            foreach (var item in pProfile)
            {
                Hcap = Convert.ToDecimal(item.Handicap);
            }
            return Hcap;
        }

        // Get compPlayerID from UserID and CompID
        public int GetcompPlayerID(int UserID, int CompID)
        {
            int compPlayerID = 0;
            var compPlayers = from cp in db.CompPlayers
                              where cp.UserID == UserID && cp.CompID == CompID
                              select cp;

            foreach (var item in compPlayers)
            {
                compPlayerID = item.CompPlayerID;
            }
            return compPlayerID;
        }


        // Get Players Home Club
        public int HomeClubID(int compPlayerID)
        {
            int ClubID = 0;
            CompPlayer cplayer = db.CompPlayers.Find(compPlayerID);
            int userID = cplayer.UserID;
            var pProfile = from pp in db.Profiles
                           where pp.UserID == userID
                           select pp;

            foreach (var item in pProfile)
            {
                ClubID = Convert.ToInt32(item.HomeClubID);
            }
            return ClubID;
        }

        // Get Number of Rounds Played (Player / Comp)
        public int NumberRndsPlayed(int CompID, int CompPlayerID)
        {
            var rndsPlayed = (from rp in db.CompScores
                              where rp.CompID == CompID && rp.CompPlayerID == CompPlayerID
                              select rp).Count();

            return rndsPlayed;
        }

        // Get Total Points (Player / Comp) **** BEST 10 Scores ****
        public int TotalNumberPoints(int CompID, int CompPlayerID)
        {
            int totalPoints = 0;
            var GetTotalPoints = from tp in db.CompScores
                                 where tp.CompID == CompID && tp.CompPlayerID == CompPlayerID
                                 orderby tp.TotalPoints descending 
                                 select tp;

            int ptnCount = 1;
            foreach (var item in GetTotalPoints)
            {
                if (item.TotalPoints != null && ptnCount <= 10)
                {
                    totalPoints += Convert.ToInt32(item.TotalPoints);
                    ptnCount += 1;
                }
            }
            return totalPoints;
        }

        // Get Total Points (Player / Comp) **** BEST 10 Scores ****
        public int TotalNETPoints(int CompID, int CompPlayerID)
        {
            int totalNETPoints = 0;
            var GetTotalNETPoints = from tp in db.CompScores
                                    where tp.CompID == CompID && tp.CompPlayerID == CompPlayerID
                                    orderby tp.TotalPoints descending 
                                    select tp;

            int ptnCount = 1;
            foreach (var item in GetTotalNETPoints)
            {
                if (item.TotalPoints != null && ptnCount <= 10)
                {
                    totalNETPoints += Convert.ToInt32(item.TotalPoints) - (cInfo.GetCoursePar(item.CourseID, item.TeeColour) - Convert.ToInt32(item.SSS));
                    ptnCount += 1;
                }
            }
            return totalNETPoints;
        }

        // Is Player currently in the Competition
        public bool PlayerInComp(int UserID, int CompID)
        {
            bool inComp = false;
            var CompPlayers = from cp in db.CompPlayers
                              where cp.UserID == UserID && cp.CompID == CompID
                              select cp;

            if (CompPlayers.Count() > 0)
            {
                inComp = true;
            }
            return inComp;
        }

        // Get players BEST score per hole for the Eclectic Competition
        public List<int> BestScoreList(int compPlayerID, int courseID)
        {
            // Loop through each score card to make list of net scores for each hole
            List<int> listScores = new List<int>();

            var compHoleScores = from cs in db.CompScores
                                 where cs.CompPlayerID == compPlayerID && cs.CourseID == courseID
                                 select cs;

            List<int> netScores1 = new List<int>();
            List<int> netScores2 = new List<int>();
            List<int> netScores3 = new List<int>();
            List<int> netScores4 = new List<int>();
            List<int> netScores5 = new List<int>();
            List<int> netScores6 = new List<int>();
            List<int> netScores7 = new List<int>();
            List<int> netScores8 = new List<int>();
            List<int> netScores9 = new List<int>();
            List<int> netScores10 = new List<int>();
            List<int> netScores11 = new List<int>();
            List<int> netScores12 = new List<int>();
            List<int> netScores13 = new List<int>();
            List<int> netScores14 = new List<int>();
            List<int> netScores15 = new List<int>();
            List<int> netScores16 = new List<int>();
            List<int> netScores17 = new List<int>();
            List<int> netScores18 = new List<int>();

            foreach (var item in compHoleScores)
            {
                decimal? Hcap = item.PlayerHcap;
                string teeColour = item.TeeColour;
                netScores1.Add(pScore.NetScore(courseID, 1, Hcap, teeColour, item.Hole1));
                netScores2.Add(pScore.NetScore(courseID, 2, Hcap, teeColour, item.Hole2));
                netScores3.Add(pScore.NetScore(courseID, 3, Hcap, teeColour, item.Hole3));
                netScores4.Add(pScore.NetScore(courseID, 4, Hcap, teeColour, item.Hole4));
                netScores5.Add(pScore.NetScore(courseID, 5, Hcap, teeColour, item.Hole5));
                netScores6.Add(pScore.NetScore(courseID, 6, Hcap, teeColour, item.Hole6));
                netScores7.Add(pScore.NetScore(courseID, 7, Hcap, teeColour, item.Hole7));
                netScores8.Add(pScore.NetScore(courseID, 8, Hcap, teeColour, item.Hole8));
                netScores9.Add(pScore.NetScore(courseID, 9, Hcap, teeColour, item.Hole9));
                netScores10.Add(pScore.NetScore(courseID, 10, Hcap, teeColour, item.Hole10));
                netScores11.Add(pScore.NetScore(courseID, 11, Hcap, teeColour, item.Hole11));
                netScores12.Add(pScore.NetScore(courseID, 12, Hcap, teeColour, item.Hole12));
                netScores13.Add(pScore.NetScore(courseID, 13, Hcap, teeColour, item.Hole13));
                netScores14.Add(pScore.NetScore(courseID, 14, Hcap, teeColour, item.Hole14));
                netScores15.Add(pScore.NetScore(courseID, 15, Hcap, teeColour, item.Hole15));
                netScores16.Add(pScore.NetScore(courseID, 16, Hcap, teeColour, item.Hole16));
                netScores17.Add(pScore.NetScore(courseID, 17, Hcap, teeColour, item.Hole17));
                netScores18.Add(pScore.NetScore(courseID, 18, Hcap, teeColour, item.Hole18));
            }

            // Get the best net score and add to list of scores
            listScores.Add(netScores1.Min());
            listScores.Add(netScores2.Min());
            listScores.Add(netScores3.Min());
            listScores.Add(netScores4.Min());
            listScores.Add(netScores5.Min());
            listScores.Add(netScores6.Min());
            listScores.Add(netScores7.Min());
            listScores.Add(netScores8.Min());
            listScores.Add(netScores9.Min());
            listScores.Add(netScores10.Min());
            listScores.Add(netScores11.Min());
            listScores.Add(netScores12.Min());
            listScores.Add(netScores13.Min());
            listScores.Add(netScores14.Min());
            listScores.Add(netScores15.Min());
            listScores.Add(netScores16.Min());
            listScores.Add(netScores17.Min());
            listScores.Add(netScores18.Min());

            return listScores;

        }

        // Get Current Saved Position
        public int GetCurrentPosition(int CompPlayerID)
        {
            int CurrentPos = 0;
            var CompPlayers = from cp in db.CompPlayers
                              where cp.CompPlayerID == CompPlayerID
                              select cp;

            foreach (var item in CompPlayers)
            {
                if (item.CurrentPos != null)
                {
                    CurrentPos = Convert.ToInt32(item.CurrentPos);
                }
            }
            return CurrentPos;
        }

        // Save Current Position
        public void SaveCurrentPos(int CompPlayerID, int pos)
        {

            CompPlayer cPlayer = db.CompPlayers.Find(CompPlayerID);
            cPlayer.CurrentPos = pos;
            db.Entry(cPlayer).State = EntityState.Modified;
            db.SaveChanges();

        }

        // Get Current Movement Icon
        public string GetMovementIcon(int CompPlayerID)
        {
            string movementIcon = "";
            var CompPlayers = from cp in db.CompPlayers
                              where cp.CompPlayerID == CompPlayerID
                              select cp;

            foreach (var item in CompPlayers)
            {
                if (item.MovementIcon != null)
                {
                    movementIcon = item.MovementIcon;
                }
            }
            return movementIcon;
        }

        // Save Movement Icon
        public void SaveMovementIcon(int CompPlayerID, string movementIcon)
        {
            CompPlayer cPlayer = db.CompPlayers.Find(CompPlayerID);
            cPlayer.MovementIcon  = movementIcon;
            db.Entry(cPlayer).State = EntityState.Modified;
            db.SaveChanges();
        }



    }
}