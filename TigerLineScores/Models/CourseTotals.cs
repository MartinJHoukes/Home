using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TigerLineScores.Models
{
    public class CourseTotals
    {
        public string teeColour { get; set; }
        public int frontYrds { get; set; }
        public int backYrds { get; set; }
        public int totalYrds { get; set; }
        public int frontPar { get; set; }
        public int backPar { get; set; }
        public int totalPar { get; set; }
    }
}