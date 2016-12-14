using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TigerLineScores.Models
{
    public class PointsScore
    {

        private TigerLineScoresEntities1 db = new TigerLineScoresEntities1();
        private CourseInfo cInfo = new CourseInfo();
        public int points { get; set; }
      

        public int calculatePointsScore(int Hcap, int SI, int strokes, int par)
        {
            // Calculate the points for this hole depending on Player handicap, Hole SI, Strokes and Hole Par
            int pointsScore = 0;

            if (strokes > 0 )
            {
                // Calculate the net strokes for this hole
                if (SI <= Hcap)
                {
                    strokes = strokes - 1;
                }
                if (Hcap > 18 && SI <= (Hcap - 18))
                {
                    strokes = strokes - 1;
                }

                // Calculate the points for this hole
                int netScore = strokes - par;
                switch (netScore)
                {
                    case 0:
                        pointsScore = 2;
                        break;
                    case -1:
                        pointsScore = 3;
                        break;
                    case -2:
                        pointsScore = 4;
                        break;
                    case -3:
                        pointsScore = 5;
                        break;
                    case -4:
                        pointsScore = 6;
                        break;
                    case 1:
                        pointsScore = 1;
                        break;
                    default:
                        pointsScore = 0;
                        break;
                }
            }
            return pointsScore;
        }

        public CompPoints GetCompPoints(CompScore compScr)
        {
            CompPoints cPoints = new CompPoints();
            int Hcap = Convert.ToInt32(compScr.PlayerHcap);
            string teeColour = compScr.TeeColour;
            int courseID = compScr.CourseID;
            cPoints.CompScoreID = compScr.CompScoreID;
            // Calculate the Points for each Hole
            cPoints.Hole1 = calculatePointsScore(Hcap, cInfo.GetHoleSI(1, courseID, teeColour), Convert.ToInt32(compScr.Hole1), cInfo.GetHolePar(1, courseID, teeColour));
            cPoints.Hole2 = calculatePointsScore(Hcap, cInfo.GetHoleSI(2, courseID, teeColour), Convert.ToInt32(compScr.Hole2), cInfo.GetHolePar(2, courseID, teeColour));
            cPoints.Hole3 = calculatePointsScore(Hcap, cInfo.GetHoleSI(3, courseID, teeColour), Convert.ToInt32(compScr.Hole3), cInfo.GetHolePar(3, courseID, teeColour));
            cPoints.Hole4 = calculatePointsScore(Hcap, cInfo.GetHoleSI(4, courseID, teeColour), Convert.ToInt32(compScr.Hole4), cInfo.GetHolePar(4, courseID, teeColour));
            cPoints.Hole5 = calculatePointsScore(Hcap, cInfo.GetHoleSI(5, courseID, teeColour), Convert.ToInt32(compScr.Hole5), cInfo.GetHolePar(5, courseID, teeColour));
            cPoints.Hole6 = calculatePointsScore(Hcap, cInfo.GetHoleSI(6, courseID, teeColour), Convert.ToInt32(compScr.Hole6), cInfo.GetHolePar(6, courseID, teeColour));
            cPoints.Hole7 = calculatePointsScore(Hcap, cInfo.GetHoleSI(7, courseID, teeColour), Convert.ToInt32(compScr.Hole7), cInfo.GetHolePar(7, courseID, teeColour));
            cPoints.Hole8 = calculatePointsScore(Hcap, cInfo.GetHoleSI(8, courseID, teeColour), Convert.ToInt32(compScr.Hole8), cInfo.GetHolePar(8, courseID, teeColour));
            cPoints.Hole9 = calculatePointsScore(Hcap, cInfo.GetHoleSI(9, courseID, teeColour), Convert.ToInt32(compScr.Hole9), cInfo.GetHolePar(9, courseID, teeColour));
            cPoints.Hole10 = calculatePointsScore(Hcap, cInfo.GetHoleSI(10, courseID, teeColour), Convert.ToInt32(compScr.Hole10), cInfo.GetHolePar(10, courseID, teeColour));
            cPoints.Hole11 = calculatePointsScore(Hcap, cInfo.GetHoleSI(11, courseID, teeColour), Convert.ToInt32(compScr.Hole11), cInfo.GetHolePar(11, courseID, teeColour));
            cPoints.Hole12 = calculatePointsScore(Hcap, cInfo.GetHoleSI(12, courseID, teeColour), Convert.ToInt32(compScr.Hole12), cInfo.GetHolePar(12, courseID, teeColour));
            cPoints.Hole13 = calculatePointsScore(Hcap, cInfo.GetHoleSI(13, courseID, teeColour), Convert.ToInt32(compScr.Hole13), cInfo.GetHolePar(13, courseID, teeColour));
            cPoints.Hole14 = calculatePointsScore(Hcap, cInfo.GetHoleSI(14, courseID, teeColour), Convert.ToInt32(compScr.Hole14), cInfo.GetHolePar(14, courseID, teeColour));
            cPoints.Hole15 = calculatePointsScore(Hcap, cInfo.GetHoleSI(15, courseID, teeColour), Convert.ToInt32(compScr.Hole15), cInfo.GetHolePar(15, courseID, teeColour));
            cPoints.Hole16 = calculatePointsScore(Hcap, cInfo.GetHoleSI(16, courseID, teeColour), Convert.ToInt32(compScr.Hole16), cInfo.GetHolePar(16, courseID, teeColour));
            cPoints.Hole17 = calculatePointsScore(Hcap, cInfo.GetHoleSI(17, courseID, teeColour), Convert.ToInt32(compScr.Hole17), cInfo.GetHolePar(17, courseID, teeColour));
            cPoints.Hole18 = calculatePointsScore(Hcap, cInfo.GetHoleSI(18, courseID, teeColour), Convert.ToInt32(compScr.Hole18), cInfo.GetHolePar(18, courseID, teeColour));
            cPoints.OutPoints = cPoints.Hole1 + cPoints.Hole2 + cPoints.Hole3 + cPoints.Hole4 + cPoints.Hole5 + cPoints.Hole6 + cPoints.Hole7 + cPoints.Hole8 + cPoints.Hole9;
            cPoints.InPoints = cPoints.Hole10 + cPoints.Hole11 + cPoints.Hole12 + cPoints.Hole13 + cPoints.Hole14 + cPoints.Hole15 + cPoints.Hole16 + cPoints.Hole17 + cPoints.Hole18;
            cPoints.TotalPoints = cPoints.OutPoints + cPoints.InPoints;

            return cPoints;
        }

    }
}