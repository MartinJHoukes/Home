//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace TigerLineScores.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class Profile
    {
        public int ProfileID { get; set; }
        public int UserID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public Nullable<decimal> Handicap { get; set; }
        public string Photo { get; set; }
        public Nullable<int> HomeClubID { get; set; }
    }
}
