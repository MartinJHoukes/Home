using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TigerLineScores.Models
{
    public class PlayerStats
    {

        private TigerLineScoresEntities1 db = new TigerLineScoresEntities1();
        private CourseInfo cInfo = new CourseInfo();

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
        public decimal CurrentHcap (int compPlayerID)
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

        // Get Total Points (Player / Comp)
        public int TotalNumberPoints(int CompID, int CompPlayerID)
        {
            int totalPoints = 0;
            var GetTotalPoints = from tp in db.CompScores
                                 where tp.CompID == CompID && tp.CompPlayerID == CompPlayerID
                                 select tp;

            foreach (var item in GetTotalPoints)
            {
                if (item.TotalPoints != null)
                {
                    totalPoints += Convert.ToInt32(item.TotalPoints);
                }
            }
            return totalPoints;
        }

        // Get Total Points (Player / Comp)
        public int TotalNETPoints(int CompID, int CompPlayerID)
        {
            int totalNETPoints = 0;
            var GetTotalNETPoints = from tp in db.CompScores
                                    where tp.CompID == CompID && tp.CompPlayerID == CompPlayerID
                                    select tp;

            foreach (var item in GetTotalNETPoints)
            {
                if (item.TotalPoints != null)
                {
                    totalNETPoints += Convert.ToInt32(item.TotalPoints) - (cInfo.GetCoursePar(item.CourseID, item.TeeColour) - Convert.ToInt32(item.SSS));
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


    }
}