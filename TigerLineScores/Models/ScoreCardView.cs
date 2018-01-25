using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace TigerLineScores.Models
{
    public class ScoreCardView
    {
        public string playerName { get; set; }
        public string courseName { get; set; }

        [DisplayName("Tee Colour")]
        public string teeColour { get; set; }

        public int? SSS { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd MMMM yyyy}", ApplyFormatInEditMode = true)]
        public Nullable<System.DateTime> rndDate { get; set; }

        public string Note { get; set; }
        public string cardImage { get; set; }
        public int? UserID { get; set; }

        [DisplayName("Course")]
        public int? CourseID { get; set; }

        public int ImageID { get; set; }

    }
}