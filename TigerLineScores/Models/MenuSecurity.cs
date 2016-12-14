using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TigerLineScores.Models;

namespace TigerLineScores.Models
{
    public class MenuSecurity
    {
        private TigerLineScoresEntities1 db = new TigerLineScoresEntities1();

        public bool isAdmin(int? userID)
        {
            {
                bool isAdmin = false;
                var data = from u in db.Users
                           where u.UserID == userID
                           select u;

                foreach (var check in data)
                {
                    if (check.Admin == true)
                    {
                        isAdmin = true;
                    }
                 
                }

                return isAdmin;
            }
        }
    }
}