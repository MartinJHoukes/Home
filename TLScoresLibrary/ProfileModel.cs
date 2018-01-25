using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TLScoresLibrary
{
    public class ProfileModel
    {

        public int ProfileID { get; set; }
        public int UserID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int HomeClubID { get; set; } = 0;
        public decimal Handicap { get; set; }
        public string Photo { get; set; }


    }
}
