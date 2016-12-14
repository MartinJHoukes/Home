using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace TigerLineScores.Models
{
    public class PlayerView
    {
        public int compPlayerID { get; set; }
        [DisplayName("Player")]
        public string playerName { get; set; }

        [DisplayName("Total Points")]
        public int totalPoints { get; set; }

        public int totalNETPoints { get; set; }

        [DisplayName("Rounds Played")]
        public int rndsPlayed { get; set; }

        public int currentPosition { get; set; }

        [DisplayFormat(DataFormatString = "{0:0.00}", ApplyFormatInEditMode = true)]
        public double avgPointsPerRnd { get; set; }

        public string Photo { get; set; }
    }
}