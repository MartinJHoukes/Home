using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace TigerLineScores.Models
{
    public class UserView
    {
        public int UserID { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public Boolean Admin { get; set; }
        public string Photo { get; set; }

        [Range(0, 36, ErrorMessage = "Your handicap must be between 0 and 36")]
        public Nullable<decimal> Handicap { get; set; }

    }
}