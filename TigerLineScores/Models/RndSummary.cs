using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TigerLineScores.Models
{
    public class RndSummary
    {
        private TigerLineScoresEntities1 db = new TigerLineScoresEntities1();

        public int CompScoreID { get; set; }
        public int CourseID { get; set; }
        public int? SSS { get; set; }
        public DateTime? RndDate { get; set; }
        public string ClubName { get; set; }
        public string TeeColour { get; set; }
        public Decimal? Handicap { get; set; }
        public int? RndScore { get; set; }
        public int? RndPoints { get; set; }
        public int NETRndPoints { get; set; }
        public int RndNumber { get; set; }
        public String Player { get; set; }
        public bool Discard { get; set; }

        // Calc and get the Round Score
        public int GetRndScore(int CompScoreID)
        {
            ScoreInfo scoreInfo = new ScoreInfo();
            int frontScore = scoreInfo.FrontScore(CompScoreID);
            int backScore = scoreInfo.BackScore(CompScoreID);
            int roundScore = frontScore + backScore;
            return roundScore;
        } 
    }
}