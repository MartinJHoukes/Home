using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace TigerLineScores.Models
{
    public class PointsScore
    {

        private TigerLineScoresEntities1 db = new TigerLineScoresEntities1();
        private CourseInfo cInfo = new CourseInfo();
        public int points { get; set; }


        public int calculatePointsScore(int Hcap, int SI, int strokes, int par, int compScoreID, int holeNumber)
        {
            // Calculate the points for this hole depending on Player handicap, Hole SI, Strokes and Hole Par
            int pointsScore = 0;

            if (strokes == 0)
            {
                // Calculate and save stokes to produce zero points for this hole
                strokes = par + 2;
                if (SI <= Hcap)
                {
                    strokes = strokes + 1;
                }
                if (Hcap > 18 && SI <= (Hcap - 18))
                {
                    strokes = strokes + 1;
                }
                // Save calculated strokes
                if (compScoreID != 0)
                {
                    CompScore cmpScore = db.CompScores.Find(compScoreID);
                    switch (holeNumber)
                    {
                        case 1:
                            cmpScore.Hole1 = strokes;
                            break;
                        case 2:
                            cmpScore.Hole2 = strokes;
                            break;
                        case 3:
                            cmpScore.Hole3 = strokes;
                            break;
                        case 4:
                            cmpScore.Hole4 = strokes;
                            break;
                        case 5:
                            cmpScore.Hole5 = strokes;
                            break;
                        case 6:
                            cmpScore.Hole6 = strokes;
                            break;
                        case 7:
                            cmpScore.Hole7 = strokes;
                            break;
                        case 8:
                            cmpScore.Hole8 = strokes;
                            break;
                        case 9:
                            cmpScore.Hole9 = strokes;
                            break;
                        case 10:
                            cmpScore.Hole10 = strokes;
                            break;
                        case 11:
                            cmpScore.Hole11 = strokes;
                            break;
                        case 12:
                            cmpScore.Hole12 = strokes;
                            break;
                        case 13:
                            cmpScore.Hole13 = strokes;
                            break;
                        case 14:
                            cmpScore.Hole14 = strokes;
                            break;
                        case 15:
                            cmpScore.Hole15 = strokes;
                            break;
                        case 16:
                            cmpScore.Hole16 = strokes;
                            break;
                        case 17:
                            cmpScore.Hole17 = strokes;
                            break;
                        case 18:
                            cmpScore.Hole18 = strokes;
                            break;
                    }
                    db.Entry(cmpScore).State = EntityState.Modified;
                    db.SaveChanges();
                }

            }

            if (strokes > 0)
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
            int compScoreID = compScr.CompScoreID;
            cPoints.CompScoreID = compScoreID;
            // Calculate the Points for each Hole
            cPoints.Hole1 = calculatePointsScore(Hcap, cInfo.GetHoleSI(1, courseID, teeColour), Convert.ToInt32(compScr.Hole1), cInfo.GetHolePar(1, courseID, teeColour), compScoreID, 1);
            cPoints.Hole2 = calculatePointsScore(Hcap, cInfo.GetHoleSI(2, courseID, teeColour), Convert.ToInt32(compScr.Hole2), cInfo.GetHolePar(2, courseID, teeColour), compScoreID, 2);
            cPoints.Hole3 = calculatePointsScore(Hcap, cInfo.GetHoleSI(3, courseID, teeColour), Convert.ToInt32(compScr.Hole3), cInfo.GetHolePar(3, courseID, teeColour), compScoreID, 3);
            cPoints.Hole4 = calculatePointsScore(Hcap, cInfo.GetHoleSI(4, courseID, teeColour), Convert.ToInt32(compScr.Hole4), cInfo.GetHolePar(4, courseID, teeColour), compScoreID, 4);
            cPoints.Hole5 = calculatePointsScore(Hcap, cInfo.GetHoleSI(5, courseID, teeColour), Convert.ToInt32(compScr.Hole5), cInfo.GetHolePar(5, courseID, teeColour), compScoreID, 5);
            cPoints.Hole6 = calculatePointsScore(Hcap, cInfo.GetHoleSI(6, courseID, teeColour), Convert.ToInt32(compScr.Hole6), cInfo.GetHolePar(6, courseID, teeColour), compScoreID, 6);
            cPoints.Hole7 = calculatePointsScore(Hcap, cInfo.GetHoleSI(7, courseID, teeColour), Convert.ToInt32(compScr.Hole7), cInfo.GetHolePar(7, courseID, teeColour), compScoreID, 7);
            cPoints.Hole8 = calculatePointsScore(Hcap, cInfo.GetHoleSI(8, courseID, teeColour), Convert.ToInt32(compScr.Hole8), cInfo.GetHolePar(8, courseID, teeColour), compScoreID, 8);
            cPoints.Hole9 = calculatePointsScore(Hcap, cInfo.GetHoleSI(9, courseID, teeColour), Convert.ToInt32(compScr.Hole9), cInfo.GetHolePar(9, courseID, teeColour), compScoreID, 9);
            cPoints.Hole10 = calculatePointsScore(Hcap, cInfo.GetHoleSI(10, courseID, teeColour), Convert.ToInt32(compScr.Hole10), cInfo.GetHolePar(10, courseID, teeColour), compScoreID, 10);
            cPoints.Hole11 = calculatePointsScore(Hcap, cInfo.GetHoleSI(11, courseID, teeColour), Convert.ToInt32(compScr.Hole11), cInfo.GetHolePar(11, courseID, teeColour), compScoreID, 11);
            cPoints.Hole12 = calculatePointsScore(Hcap, cInfo.GetHoleSI(12, courseID, teeColour), Convert.ToInt32(compScr.Hole12), cInfo.GetHolePar(12, courseID, teeColour), compScoreID, 12);
            cPoints.Hole13 = calculatePointsScore(Hcap, cInfo.GetHoleSI(13, courseID, teeColour), Convert.ToInt32(compScr.Hole13), cInfo.GetHolePar(13, courseID, teeColour), compScoreID, 13);
            cPoints.Hole14 = calculatePointsScore(Hcap, cInfo.GetHoleSI(14, courseID, teeColour), Convert.ToInt32(compScr.Hole14), cInfo.GetHolePar(14, courseID, teeColour), compScoreID, 14);
            cPoints.Hole15 = calculatePointsScore(Hcap, cInfo.GetHoleSI(15, courseID, teeColour), Convert.ToInt32(compScr.Hole15), cInfo.GetHolePar(15, courseID, teeColour), compScoreID, 15);
            cPoints.Hole16 = calculatePointsScore(Hcap, cInfo.GetHoleSI(16, courseID, teeColour), Convert.ToInt32(compScr.Hole16), cInfo.GetHolePar(16, courseID, teeColour), compScoreID, 16);
            cPoints.Hole17 = calculatePointsScore(Hcap, cInfo.GetHoleSI(17, courseID, teeColour), Convert.ToInt32(compScr.Hole17), cInfo.GetHolePar(17, courseID, teeColour), compScoreID, 17);
            cPoints.Hole18 = calculatePointsScore(Hcap, cInfo.GetHoleSI(18, courseID, teeColour), Convert.ToInt32(compScr.Hole18), cInfo.GetHolePar(18, courseID, teeColour), compScoreID, 18);
            cPoints.OutPoints = cPoints.Hole1 + cPoints.Hole2 + cPoints.Hole3 + cPoints.Hole4 + cPoints.Hole5 + cPoints.Hole6 + cPoints.Hole7 + cPoints.Hole8 + cPoints.Hole9;
            cPoints.InPoints = cPoints.Hole10 + cPoints.Hole11 + cPoints.Hole12 + cPoints.Hole13 + cPoints.Hole14 + cPoints.Hole15 + cPoints.Hole16 + cPoints.Hole17 + cPoints.Hole18;
            cPoints.TotalPoints = cPoints.OutPoints + cPoints.InPoints;

            return cPoints;
        }


        // Get Hole Net Score
        public int NetScore(int courseID, int holeNumber, decimal? hCap, string teeColor, int? grossScore)
        {
            // Get Course Hole SI
            int? holeSI = 0;
            var Courses = from co in db.CourseDetails
                          where co.CourseID == courseID && co.HoleNumber == holeNumber
                          select co;

            foreach (var item in Courses)
            {
                if (teeColor == "White" || teeColor == "Yellow")
                {
                    holeSI = item.MensSI;
                }
                else
                {
                    holeSI = item.LadiesSI;
                }
            }

            int netScore = Convert.ToInt32(grossScore);
            if (grossScore > 0)
            {
                // Calculate the net strokes for this hole
                if (holeSI <= hCap)
                {
                    netScore = netScore - 1;
                }

                if (hCap > 18 && holeSI <= (hCap - 18))
                {
                    netScore = netScore - 1;
                }
            }
            else
            {
                // No Score Recorded for this hole so display Hole Par + 3
                int holePar = cInfo.GetHolePar(holeNumber, courseID, "Yellow");
                netScore = holePar + 3;
            }

            return netScore;

        }





    }
}