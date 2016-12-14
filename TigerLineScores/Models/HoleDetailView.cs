using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace TigerLineScores.Models
{
    public class HoleDetailView
    {
        public int HoleNumber { get; set; }
        public int HoleYrds { get; set; }
        public int HolePar { get; set; }
        public int HoleSI { get; set; }
    }
}