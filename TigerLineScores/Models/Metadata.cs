using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace TigerLineScores.Models
{
    public class CompMainMetadata
    {
        [Required(ErrorMessage = "Please enter the competition name")]
        [DisplayName("Competition Name")]
        public string CompName { get; set; }

        [Required(ErrorMessage = "Please enter the competition start date")]
        [DisplayName("Start Date")]
        [DisplayFormat(DataFormatString = "{0:dd MMMM yyyy}", ApplyFormatInEditMode = true)]
        public Nullable<System.DateTime> DateStart { get; set; }

        [Required(ErrorMessage = "Please enter the competition end date")]
        [DisplayName("End Date")]
        [DisplayFormat(DataFormatString = "{0:dd MMMM yyyy}", ApplyFormatInEditMode = true)]
        public Nullable<System.DateTime> DateEnd { get; set; }
        public int NumberOfPlayers { get; set; }
    }

    public class CourseDetailMetadata
    {
        [DisplayName("Hole")]
        public int HoleNumber { get; set; }

        [DisplayName("White Yards")]
        public Nullable<int> WhiteYrds { get; set; }

        [DisplayName("Yellow Yards")]
        public Nullable<int> YellowYrds { get; set; }

        [DisplayName("Mens Par")]
        public Nullable<int> MensPar { get; set; }

        [DisplayName("Mens SI")]
        public Nullable<int> MensSI { get; set; }

        [DisplayName("Red Yards")]
        public Nullable<int> RedYrds { get; set; }

        [DisplayName("Ladies Par")]
        public Nullable<int> LadiesPar { get; set; }

        [DisplayName("Ladies SI")]
        public Nullable<int> LadiesSI { get; set; }
    }

    public class CourseMainMetadata
    {
        [Required(ErrorMessage = "The club name is required")]
        [DisplayName("Club Name")]
        public string ClubName { get; set; }

        [DisplayName("White SSS")]
        public Nullable<int> WhiteSSS { get; set; }

        [DisplayName("Yellow SSS")]
        public Nullable<int> YellowSSS { get; set; }

        [DisplayName("Red SSS")]
        public Nullable<int> RedSSS { get; set; }
    }

    public class ProfileMetadata
    {
        [DisplayName("First Name")]
        public string FirstName { get; set; }

        [DisplayName("Last Name")]
        public string LastName { get; set; }

        [DisplayName("Home Club")]
        public Nullable<int> HomeClubID { get; set; }

        [Range(0, 36, ErrorMessage = "Your handicap must be between 0 and 36")]
        [DisplayFormat(DataFormatString = "{0:0.0}", ApplyFormatInEditMode = true)]
        public Nullable<decimal> Handicap { get; set; }
    }

    public class UserMetadata
    {
        [Required(ErrorMessage = "A user name is required")]
        [DisplayName("User Name")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "An email address is required")]
        [EmailAddress(ErrorMessage = "A valid email address is required")]
        public string Email { get; set; }
    }

    public class ScoreCardImageMetadata
    {

        [DisplayName("Round Date")]
        [DisplayFormat(DataFormatString = "{0:dd MMMM yyyy}", ApplyFormatInEditMode = true)]
        public Nullable<System.DateTime> RoundDate { get; set; }

        [DisplayName("Course")]
        public Nullable<int> CourseID { get; set; }

        [DisplayName("Tee Colour")]
        public string TeeColour { get; set; }

        [Required]
        [Range(60, 80, ErrorMessage = "Invalid SSS entered")]
        public Nullable<int> SSS { get; set; }


    }

    public class CompScoreMetadata
    {
        [DisplayName("Round Date")]
        [DisplayFormat(DataFormatString = "{0:dd MMMM yyyy}", ApplyFormatInEditMode = true)]
        public Nullable<System.DateTime> RndDate { get; set; }

        [DisplayName("Tee Colour")]
        public string TeeColour { get; set; }

        [DisplayName("Course")]
        public int CourseID { get; set; }

        [DisplayFormat(DataFormatString = "{0:N0}", ApplyFormatInEditMode = true)]
        public Nullable<decimal> PlayerHcap { get; set; }

    }

    public partial class UpcomingRndMetadata
    {
        [DisplayName("Course")]
        public int CourseID { get; set; }

        [DisplayName("Round Date")]
        [DisplayFormat(DataFormatString = "{0:dd MMMM yyyy}", ApplyFormatInEditMode = true)]
        public Nullable<System.DateTime> RndDate { get; set; }

        [DisplayName("Type")]
        public string RndType { get; set; }

        public virtual string CourseName { get; set; }
    }
}