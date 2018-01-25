using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TigerLineScores.Models
{
    public class CompInfo
    {
        private TigerLineScoresEntities1 db = new TigerLineScoresEntities1();

        public int GetNumberOfPlayers(int compID)
        {
            var CompPlayersCount = (from pc in db.CompPlayers
                                    where pc.CompID == compID
                                    select pc).Count();

            return CompPlayersCount;
        }

        public string GetCompetitonName(int compID)
        {
            string compName = "";
            var comps = from co in db.CompMains
                        where co.CompID == compID
                        select co;

            foreach (var item in comps)
            {
                compName = item.CompName;
            }
            return compName;
        }

        public Boolean IsUserinComp(int compID, int UserID)
        {
            Boolean InComp = false;
            var compPlayers = from cp in db.CompPlayers
                              where cp.CompID == compID
                              select cp;

            foreach (var item in compPlayers)
            {
                if (item.UserID == UserID)
                {
                    InComp = true;
                }
            }
            return InComp;
        }

        public string GetScoreCardImage(int compScoreID)
        {
            string scardImage = "";
            CompScore compScore = db.CompScores.Find(compScoreID);
            int ImageID = Convert.ToInt32(compScore.ImageID);
            var scardImages = from ci in db.ScoreCardImages
                              where ci.ImageID == ImageID
                              select ci;

            foreach (var item in scardImages)
            {
                if (item.CardImage != null)
                {
                    scardImage = item.CardImage;
                }
            }
            return scardImage;
        }

        public void RemovePlayer(int compPlayerID)
        {

            CompPlayer compPlayer = db.CompPlayers.Find(compPlayerID);
            int compID = compPlayer.CompID;
            int userID = compPlayer.UserID;

            // Remove any CompScore Records
            var PlayerScores = from ps in db.CompScores
                               where ps.CompPlayerID == compPlayerID
                               select ps;

            foreach (var Score in PlayerScores)
            {
                db.CompScores.Remove(Score);
            }

            // Remove any Score Card Image Records
            var cardImages = from si in db.ScoreCardImages
                             where si.CompID == compID && si.UserID == userID
                             select si;

            foreach (var image in cardImages)
            {
                db.ScoreCardImages.Remove(image);
            }

            // Remove any Upcomping Rounds
            var comingRnds = from cr in db.UpcomingRnds
                             where cr.CompPlayerID == compPlayerID
                             select cr;

            foreach (var rnd in comingRnds)
            {
                db.UpcomingRnds.Remove(rnd);
                // Remove Comp Player
                db.CompPlayers.Remove(compPlayer);

            }

            // Remove Player
            db.CompPlayers.Remove(compPlayer);
            db.SaveChanges();

        }
    }
}