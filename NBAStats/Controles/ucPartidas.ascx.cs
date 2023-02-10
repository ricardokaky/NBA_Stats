using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace NBAStats.Controles
{
    public partial class ucPartidas : UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        public void Prepara(List<Partida> partidas)
        {
            Visible = true;

            dgPartidas.DataSource = partidas;
            dgPartidas.DataBind();
        }
    }
}