using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TigerLineScores.Models
{
    public class InterClubInfo
    {
        private TigerLineScoresEntities1 db = new TigerLineScoresEntities1();

        public int GetNumberOfPlayers(int courseID, int compID)
        {
            var CompScores = (from cs in db.CompScores
                              join cp in db.CompPlayers on cs.CompPlayerID equals cp.CompPlayerID
                              join ur in db.Users on cp.UserID equals ur.UserID
                              join pr in db.Profiles on ur.UserID equals pr.UserID
                              where cp.CompID == compID && cs.CourseID == courseID && pr.HomeClubID == courseID
                              select cp).Distinct();

            return CompScores.Count();

        }

        public int GetNumberOfRnds(int courseID, int compID)
        {
            var CompScores =  from cs in db.CompScores
                              join cp in db.CompPlayers on cs.CompPlayerID equals cp.CompPlayerID
                              join ur in db.Users on cp.UserID equals ur.UserID
                              join pr in db.Profiles on ur.UserID equals pr.UserID
                              where cp.CompID == compID && cs.CourseID == courseID && pr.HomeClubID == courseID
                              select cs;

            return CompScores.Count();

        }

        public int GetTotalPoints(int courseID, int compID)
        {
            CourseInfo cInfo = new CourseInfo();
            var CompScores = from cs in db.CompScores
                             join cp in db.CompPlayers on cs.CompPlayerID equals cp.CompPlayerID
                             join pr in db.Profiles on cp.UserID equals pr.UserID
                             join ur in db.Users on pr.UserID equals ur.UserID
                             where cs.CompID == compID && cs.CourseID == courseID && pr.HomeClubID == courseID
                             select new { cs.TotalPoints, cs.TeeColour, cs.SSS, cp.CompPlayerID } ;

            int totalPoints = 0;
            foreach (var item in CompScores)
            {
                totalPoints += Convert.ToInt32(item.TotalPoints) - (cInfo.GetCoursePar(courseID, item.TeeColour) - Convert.ToInt32(item.SSS));
            }

            return totalPoints;
        }


    }
}