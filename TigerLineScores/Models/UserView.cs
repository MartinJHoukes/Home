using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace TigerLineScores.Models
{
    public class UserView
    {
        public int UserID { get; set; }

        [DisplayName("User Name")]
        public string UserName { get; set; }

        public string Email { get; set; }
        public string Password { get; set; }
        public Nullable<Boolean> Admin { get; set; }
        public string Photo { get; set; }


        public string HomeCourse { get; set; }

        [DisplayName("Home Course")]
        public int? HomeClubID { get; set; }

        [Required]
        [RegularExpression(@"^\d+(.\d{1,2})?$",ErrorMessage = "Invalid handicap entered")]
        [Range(0, 36, ErrorMessage = "Your handicap must be between 0 and 36")]
        public Nullable<decimal> Handicap { get; set; }

    }
}