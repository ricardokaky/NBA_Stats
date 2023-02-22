using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace NBAStats.Controles
{
    public partial class ucMedias : UserControl
    {
        public List<Media> Medias { get; set; }
        public List<Partida> Partidas { get; set; }

        protected void Page_Load(object sender, EventArgs e)
        {

        }

        public void Prepara(List<Partida> partidas, List<Media> medias)
        {
            Partidas = partidas;
            Medias = medias;

            dgMedias.DataSource = DeListParaDT();
            dgMedias.DataBind();

            Visible = true;
        }

        private DataTable DeListParaDT()
        {
            DataTable dt = new DataTable();

            foreach (var media in Medias)
            {
                dt.Columns.Add(media.Nome);
            }

            DataRow dr = dt.NewRow();

            for (int i = 0; i < Medias.Count(); i++)
            {
                Medias[i].Valor = Math.Round(Partidas.Select(y => (int)y.GetType().GetProperty(Medias[i].Nome).GetValue(y)).Average(), 1);
                ProcessarCoeficienteVariacao(Medias[i]);

                dr[i] = Medias[i].Valor;
            }

            dt.Rows.Add(dr);

            return dt;
        }

        private void ProcessarCoeficienteVariacao(Media media)
        {
            double variancia = 0;

            foreach (var value in Partidas.Select(x => x.GetType().GetProperty(media.Nome).GetValue(x)))
            {
                variancia += Math.Pow((Convert.ToDouble(value) - media.Valor), 2);
            }

            double desvioPadrao = Math.Sqrt(variancia / (Partidas.Count() - 1));

            media.CoeficienteVariacao = Math.Round((desvioPadrao / media.Valor) * 100, 2);
        }

        protected void dgMedias_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                foreach (Media media in Medias)
                {
                    int coefRuim;
                    int coefBom;

                    Label lblAverage = (Label)e.Row.FindControl("lblAvg" + media.Nome);

                    if (media.Valor > 15)
                    {
                        coefRuim = 30;
                        coefBom = 15;
                    }
                    else if (media.Valor > 5 && media.Valor <= 15)
                    {
                        coefRuim = 35;
                        coefBom = 20;
                    }
                    else
                    {
                        coefRuim = 40;
                        coefBom = 25;
                    }

                    if (media.Valor < 1 || media.CoeficienteVariacao > coefRuim)
                    {
                        lblAverage.ForeColor = System.Drawing.Color.Red;
                    }
                    else if (media.CoeficienteVariacao > coefBom && media.CoeficienteVariacao <= coefRuim)
                    {
                        lblAverage.ForeColor = System.Drawing.Color.Blue;
                    }
                    else
                    {
                        lblAverage.ForeColor = System.Drawing.Color.LightGreen;
                    }

                    Label coefMedia = (Label)e.Row.FindControl("coefAvg" + media.Nome);
                    coefMedia.Text = $"({media.CoeficienteVariacao})";
                }
            }
        }
    }
}