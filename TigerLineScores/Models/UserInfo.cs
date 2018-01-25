using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TigerLineScores.Models
{
    public class UserInfo
    {
        private TigerLineScoresEntities1 db = new TigerLineScoresEntities1();

        public string CreatePassword(int passwordLength)
        {
            string allowedChars = "";
            allowedChars = "a,b,c,d,e,f,g,h,i,j,k,m,n,p,q,r,s,t,u,v,w,x,y,z,";
            allowedChars += "A,B,C,D,E,F,G,H,J,K,L,M,N,P,Q,R,S,T,U,V,W,X,Y,Z,";
            allowedChars += "1,2,3,4,5,6,7,8,9,0,@,#,&";
            char[] sep = { ',' };
            string[] arr = allowedChars.Split(sep);
            string passwordString = "";
            string temp = "";
            Random rand = new Random();
            for (int i = 0; i < Convert.ToInt32(passwordLength); i++)
            {
                temp = arr[rand.Next(0, arr.Length)];
                passwordString += temp;
            }

            return passwordString;
        }

        public string GetUserName(int userID)
        {
            string userName = "";
            var GetUsers = from gu in db.Users
                           where gu.UserID == userID
                           select gu;

            foreach (var item in GetUsers)
            {
                if (item.UserName != null)
                {
                    userName = item.UserName;
                }
            }
            return userName;
        }

        public IEnumerable<TigerLineScores.Models.User> GetAllAdmin()
        {
            var AllAdmin = from aa in db.Users
                           where aa.Admin == true
                           select aa;

            return AllAdmin;
        }
    }

    
}