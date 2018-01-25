using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace TigerLineScores.Models
{
    public class CourseInfo
    {
        private TigerLineScoresEntities1 db = new TigerLineScoresEntities1();

        public SelectList GetCourseList()
        {
            // Get Club Names For Drop Down List

            var ClubNames = (from cn in db.CourseMains
                             orderby cn.ClubName
                             select cn).ToList();

            var courseList = new SelectList(ClubNames, "CourseID", "ClubName");
            return courseList;
        }

        public int GetCourseSSS(int courseID, string teeColour)
        {
            int sss = 0;
            var courses = from cr in db.CourseMains
                          where cr.CourseID == courseID
                          select cr;

            foreach (var item in courses)
            {
                switch (teeColour)
                {
                    case "White":
                        if (item.WhiteSSS != null)
                        {
                            sss = Convert.ToInt32(item.WhiteSSS);
                        }
                        break;
                    case "Yellow":
                        if (item.YellowSSS != null)
                        {
                            sss = Convert.ToInt32(item.YellowSSS);
                        }
                        break;
                    case "Red":
                        if (item.RedSSS != null)
                        {
                            sss = Convert.ToInt32(item.RedSSS);
                        }
                        break;
                }
            }
            return sss;
        }

        public int GetCoursePar(int courseID, string teeColour)
        {
            int coursePar = 0;
            for (int i = 1; i < 19; i++)
            {
                var holeDetail = from hd in db.CourseDetails
                                 where hd.CourseID == courseID && hd.HoleNumber == i
                                 select hd;

                foreach (var item in holeDetail)
                {
                    if (teeColour == "Red")
                    {
                        coursePar += Convert.ToInt32(item.LadiesPar);
                    }
                    else
                    {
                        coursePar += Convert.ToInt32(item.MensPar);
                    }
                }
            }
            return coursePar;
        }

        public int GetHoleYrds(int holeNumber, int courseID, string teeColour)
        {
            var HoleDetail = from hd in db.CourseDetails
                             where hd.CourseID == courseID && hd.HoleNumber == holeNumber
                             select hd;

            int Yrds = 0;
            foreach (var item in HoleDetail)
            {
                switch (teeColour)
                {
                    case "White":
                        if (item.WhiteYrds != null)
                        {
                            Yrds = Convert.ToInt32(item.WhiteYrds);
                        }
                        break;
                    case "Yellow":
                        if (item.YellowYrds != null)
                        {
                            Yrds = Convert.ToInt32(item.YellowYrds);
                        }
                        break;
                    case "Red":
                        if (item.RedYrds != null)
                        {
                            Yrds = Convert.ToInt32(item.RedYrds);
                        }
                        break;
                }
            }
            return Yrds;
        }

        public int GetHolePar(int holeNumber, int courseID, string teeColour)
        {
            var HoleDetail = from hd in db.CourseDetails
                             where hd.CourseID == courseID && hd.HoleNumber == holeNumber
                             select hd;

            int Par = 0;
            foreach (var item in HoleDetail)
            {
                switch (teeColour)
                {
                    case "White":
                    case ("Yellow"):
                        if (item.MensPar != null)
                        {
                            Par = Convert.ToInt32(item.MensPar);
                        }
                        break;
                    case "Red":
                        if (item.LadiesPar != null)
                        {
                            Par = Convert.ToInt32(item.LadiesPar);
                        }
                        break;
                }
            }
            return Par;
        }

        public int GetHoleSI(int holeNumber, int courseID, string teeColour)
        {
            var HoleDetail = from hd in db.CourseDetails
                             where hd.CourseID == courseID && hd.HoleNumber == holeNumber
                             select hd;

            int SI = 0;
            foreach (var item in HoleDetail)
            {
                switch (teeColour)
                {
                    case "White":
                    case ("Yellow"):
                        if (item.MensSI != null)
                        {
                            SI = Convert.ToInt32(item.MensSI);
                        }
                        break;
                    case "Red":
                        if (item.LadiesSI != null)
                        {
                            SI = Convert.ToInt32(item.LadiesSI);
                        }
                        break;
                }
            }
            return SI;
        }

        public List<object> GetCourseDetails(int courseID, string teeColour)
        {
            // Get hole information depending on course and tee colour selected
            List<object> holeList = new List<object>();

            for (int i = 1; i < 19; i++)
            {
                HoleDetailView hDetail = new HoleDetailView();
                hDetail.HoleNumber = i;
                hDetail.HoleYrds = GetHoleYrds(i, Convert.ToInt32(courseID), teeColour);
                hDetail.HolePar = GetHolePar(i, Convert.ToInt32(courseID), teeColour);
                hDetail.HoleSI = GetHoleSI(i, Convert.ToInt32(courseID), teeColour);

                holeList.Add(hDetail);
            }
            return holeList;
        }

        public CourseTotals GetCourseTotals(int courseID, string teeColour)
        {

            CourseTotals cTotals = new CourseTotals();
            // Tee Colour Selected and Course In, Out and Total Yardages and Pars
            switch (teeColour)
            {
                case "Yellow":
                    cTotals.teeColour = "#f7de54";
                    cTotals.frontYrds = GetCourseTotal(1, 9, Convert.ToInt32(courseID), "YellowYrds");
                    cTotals.backYrds = GetCourseTotal(10, 18, Convert.ToInt32(courseID), "YellowYrds");
                    cTotals.totalYrds = GetCourseTotal(1, 18, Convert.ToInt32(courseID), "YellowYrds");
                    cTotals.frontPar = GetCourseTotal(1, 9, Convert.ToInt32(courseID), "MensPar");
                    cTotals.backPar = GetCourseTotal(10, 18, Convert.ToInt32(courseID), "MensPar");
                    cTotals.totalPar = GetCourseTotal(1, 18, Convert.ToInt32(courseID), "MensPar");
                    break;
                case "Red":
                    cTotals.teeColour = "#f66868";
                    cTotals.frontYrds = GetCourseTotal(1, 9, Convert.ToInt32(courseID), "RedYrds");
                    cTotals.backYrds = GetCourseTotal(10, 18, Convert.ToInt32(courseID), "RedYrds");
                    cTotals.totalYrds = GetCourseTotal(1, 18, Convert.ToInt32(courseID), "RedYrds");
                    cTotals.frontPar = GetCourseTotal(1, 9, Convert.ToInt32(courseID), "LadiesPar");
                    cTotals.backPar = GetCourseTotal(10, 18, Convert.ToInt32(courseID), "LadiesPar");
                    cTotals.totalPar = GetCourseTotal(1, 18, Convert.ToInt32(courseID), "LadiesPar");
                    break;
                default:
                    cTotals.teeColour = "White";
                    cTotals.frontYrds = GetCourseTotal(1, 9, Convert.ToInt32(courseID), "WhiteYrds");
                    cTotals.backYrds = GetCourseTotal(10, 18, Convert.ToInt32(courseID), "WhiteYrds");
                    cTotals.totalYrds = GetCourseTotal(1, 18, Convert.ToInt32(courseID), "WhiteYrds");
                    cTotals.frontPar = GetCourseTotal(1, 9, Convert.ToInt32(courseID), "MensPar");
                    cTotals.backPar = GetCourseTotal(10, 18, Convert.ToInt32(courseID), "MensPar");
                    cTotals.totalPar = GetCourseTotal(1, 18, Convert.ToInt32(courseID), "MensPar");
                    break;
            }
            return cTotals;
        }


        // Course Yards Total
        public int GetCourseTotal(int StartHole, int EndHole, int id, string field)
        {
            int CourseTotal = 0;

            var CDetail = from cd in db.CourseDetails
                          where cd.CourseID == id && cd.HoleNumber >= StartHole && cd.HoleNumber <= EndHole
                          select cd;

            foreach (var item in CDetail)
            {
                switch (field)
                {
                    case "WhiteYrds":
                        if (item.WhiteYrds.HasValue)
                        {
                            CourseTotal += Convert.ToInt32(item.WhiteYrds);
                        }
                        break;

                    case "YellowYrds":
                        if (item.YellowYrds.HasValue)
                        {
                            CourseTotal += Convert.ToInt32(item.YellowYrds);
                        }
                        break;

                    case "MensPar":
                        if (item.MensPar.HasValue)
                        {
                            CourseTotal += Convert.ToInt32(item.MensPar);
                        }
                        break;

                    case "RedYrds":
                        if (item.RedYrds.HasValue)
                        {
                            CourseTotal += Convert.ToInt32(item.RedYrds);
                        }
                        break;

                    case "LadiesPar":
                        if (item.LadiesPar.HasValue)
                        {
                            CourseTotal += Convert.ToInt32(item.LadiesPar);
                        }
                        break;

                    default:
                        break;
                }

            }
            return CourseTotal;
        }

        public string GetCourseName(int courseID)
        {
            string courseName = "";
            var Allcourses = from ac in db.CourseMains
                             where ac.CourseID == courseID
                             select ac;

            foreach (var item in Allcourses)
            {
                courseName = item.ClubName;
            }
            return courseName;     
        }


    }
}