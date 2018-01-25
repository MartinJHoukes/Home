using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TigerLineScores.Models
{
    public class InterClubSummary
    {
        public int Position { get; set; }
        public string ClubName { get; set; }
        public int courseID { get; set; }
        public int NumberOfPlayers { get; set; }
        public int NumberOfRnds { get; set; }
        public int TotalPoints { get; set; }
        public decimal AveragePoints { get; set; }

    }
}