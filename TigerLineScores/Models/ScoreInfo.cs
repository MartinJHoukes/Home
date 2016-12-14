using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TigerLineScores.Models
{
    public class ScoreInfo
    {
        private TigerLineScoresEntities1 db = new TigerLineScoresEntities1();
        private CourseInfo courseInfo = new CourseInfo();

        public int FrontScore(int compScoreID)
        {

            CompScore cScore = db.CompScores.Find(compScoreID);
            int courseID = cScore.CourseID;
            string teeColour = cScore.TeeColour;

            // check for zero score entered on each hole
            int? hole1 = cScore.Hole1;
            if (cScore.Hole1 == 0)
            {
                hole1 = courseInfo.GetHolePar(1, courseID, teeColour) + 3;
            }
            int? hole2 = cScore.Hole2;
            if (cScore.Hole2 == 0)
            {
                hole2 = courseInfo.GetHolePar(2, courseID, teeColour) + 3;
            }
            int? hole3 = cScore.Hole3;
            if (cScore.Hole3 == 0)
            {
                hole3 = courseInfo.GetHolePar(3, courseID, teeColour) + 3;
            }
            int? hole4 = cScore.Hole4;
            if (cScore.Hole4 == 0)
            {
                hole4 = courseInfo.GetHolePar(4, courseID, teeColour) + 3;
            }
            int? hole5 = cScore.Hole5;
            if (cScore.Hole5 == 0)
            {
                hole5 = courseInfo.GetHolePar(5, courseID, teeColour) + 3;
            }
            int? hole6 = cScore.Hole6;
            if (cScore.Hole6 == 0)
            {
                hole6 = courseInfo.GetHolePar(6, courseID, teeColour) + 3;
            }
            int? hole7 = cScore.Hole7;
            if (cScore.Hole7 == 0)
            {
                hole7 = courseInfo.GetHolePar(7, courseID, teeColour) + 3;
            }
            int? hole8 = cScore.Hole8;
            if (cScore.Hole8 == 0)
            {
                hole8 = courseInfo.GetHolePar(8, courseID, teeColour) + 3;
            }
            int? hole9 = cScore.Hole9;
            if (cScore.Hole9 == 0)
            {
                hole9 = courseInfo.GetHolePar(9, courseID, teeColour) + 3;
            }

            int frontScoreTotal = Convert.ToInt32(hole1 + hole2 + hole3 + hole4 + hole5 + hole6 + hole7 + hole8 + hole9);
            return frontScoreTotal; 

        }

        public int BackScore(int compScoreID)
        {

            CompScore cScore = db.CompScores.Find(compScoreID);
            int courseID = cScore.CourseID;
            string teeColour = cScore.TeeColour;

            // check for zero score entered on each hole
            int? hole10 = cScore.Hole10;
            if (cScore.Hole10 == 0)
            {
                hole10 = courseInfo.GetHolePar(10, courseID, teeColour) + 3;
            }
            int? hole11 = cScore.Hole11;
            if (cScore.Hole11 == 0)
            {
                hole11 = courseInfo.GetHolePar(11, courseID, teeColour) + 3;
            }
            int? hole12 = cScore.Hole12;
            if (cScore.Hole12 == 0)
            {
                hole12 = courseInfo.GetHolePar(12, courseID, teeColour) + 3;
            }
            int? hole13 = cScore.Hole13;
            if (cScore.Hole13 == 0)
            {
                hole13 = courseInfo.GetHolePar(13, courseID, teeColour) + 3;
            }
            int? hole14 = cScore.Hole14;
            if (cScore.Hole14 == 0)
            {
                hole14 = courseInfo.GetHolePar(14, courseID, teeColour) + 3;
            }
            int? hole15 = cScore.Hole15;
            if (cScore.Hole15 == 0)
            {
                hole15 = courseInfo.GetHolePar(15, courseID, teeColour) + 3;
            }
            int? hole16 = cScore.Hole16;
            if (cScore.Hole16 == 0)
            {
                hole16 = courseInfo.GetHolePar(16, courseID, teeColour) + 3;
            }
            int? hole17 = cScore.Hole17;
            if (cScore.Hole17 == 0)
            {
                hole17 = courseInfo.GetHolePar(17, courseID, teeColour) + 3;
            }
            int? hole18 = cScore.Hole18;
            if (cScore.Hole18 == 0)
            {
                hole18 = courseInfo.GetHolePar(18, courseID, teeColour) + 3;
            }

            int backScoreTotal = Convert.ToInt32(hole10 + hole11 + hole12 + hole13 + hole14 + hole15 + hole16 + hole17 + hole18);
            return backScoreTotal;

        }


    }
}