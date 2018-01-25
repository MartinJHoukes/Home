using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace TigerLineScores.Models
{
    public class UserProfile
    {
        public int UserID { get; set; }
        public int ProfileID { get; set; }

        [DisplayName("User Name")]
        [Required(ErrorMessage = "A user name is required")]
        public string UserName { get; set; }

        [DisplayName("Email Address")]
        [Required(ErrorMessage = "An email address is required")]
        [EmailAddress(ErrorMessage = "A valid email address is required")]
        public string Email { get; set; }

        [DisplayName("Home Course")]
        [Required(ErrorMessage = "Please select your home course")]
        public int HomeCourseID { get; set; }

        [Range(0, 36, ErrorMessage = "Your handicap must be between 0 and 36")]
        [DisplayName("Exact Handicap")]
        [DisplayFormat(DataFormatString = "{0:0.0}", ApplyFormatInEditMode = true)]
        public decimal Handicap { get; set; }

        public string Photo { get; set; }

    }
}