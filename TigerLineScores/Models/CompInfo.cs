using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TigerLineScores.Models
{
    public class CompInfo
    {
        private TigerLineScoresEntities1 db = new TigerLineScoresEntities1();

        public int GetNumberOfPlayers(int compID)
        {
            var CompPlayersCount = (from pc in db.CompPlayers
                                    where pc.CompID == compID
                                    select pc).Count();

            return CompPlayersCount;
        }
    }
}