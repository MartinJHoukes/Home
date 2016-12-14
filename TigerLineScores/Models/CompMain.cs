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
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;

    [MetadataType(typeof(CompMainMd))]
    public partial class CompMain
    {
        public int CompID { get; set; }
        public string CompName { get; set; }
        public Nullable<System.DateTime> DateStart { get; set; }
        public Nullable<System.DateTime> DateEnd { get; set; }
        public string Format { get; set; }
        public int NumberOfPlayers { get; set; }
    }

    public class CompMainMd
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

    }

}

