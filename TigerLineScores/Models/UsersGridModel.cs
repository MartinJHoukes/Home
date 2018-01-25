using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;
using Jq.Grid;


namespace TigerLineScores.Models
{
    public class UsersGridModel
    {
        public UsersGridModel()
        {
             UsersGrid = new JQGrid
            {
                Columns = new List<JQGridColumn>()
                {
                    new JQGridColumn { DataField = "UserID",
                                       PrimaryKey = true,
                                       Editable = false,
                                       Visible = false,
                                       DataType = typeof(int)
                                     },
                    new JQGridColumn { DataField = "UserName",
                                       HeaderText = "Name",
                                       Width = 175,
                                       DataType = typeof(string)
                                     },
                    new JQGridColumn { DataField = "HomeCourse",
                                       HeaderText = "Home Club",
                                       Width = 175,
                                       DataType = typeof(string)
                                     },
                    new JQGridColumn { DataField = "Handicap",
                                       HeaderText = "Handicap",
                                       Width = 50,
                                       DataType = typeof(decimal)
                                     },
                    new JQGridColumn { DataField = "Admin",
                                       HeaderText = "Admin",
                                       Width = 40,
                                       DataType = typeof(bool)
                                     },
                },
                Height = Unit.Percentage(100),
                Width = Unit.Percentage(100)
            };

            UsersGrid.ToolBarSettings.ShowRefreshButton = true;
        }

        public JQGrid UsersGrid { get; set; }
    }
}