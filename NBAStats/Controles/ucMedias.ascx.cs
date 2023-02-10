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
        public Jogador Jogador { get; set; }

        protected void Page_Load(object sender, EventArgs e)
        {

        }

        public void Prepara(Jogador jogador)
        {
            Jogador = jogador;

            dgMedias.DataSource = DeListParaDT();
            dgMedias.DataBind();

            Visible = true;
        }

        private DataTable DeListParaDT()
        {
            DataTable dt = new DataTable();

            foreach (var media in Jogador.Medias)
            {
                dt.Columns.Add(media.Nome);
            }

            DataRow dr = dt.NewRow();

            for (int i = 0; i < Jogador.Medias.Count(); i++)
            {
                Jogador.Medias[i].Valor = Math.Round(Jogador.Partidas.Select(y => (int)y.GetType().GetProperty(Jogador.Medias[i].Nome).GetValue(y)).Average(), 1);
                ProcessarCoeficienteVariacao(Jogador.Medias[i]);

                dr[i] = Jogador.Medias[i].Valor;
            }

            dt.Rows.Add(dr);

            return dt;
        }

        private void ProcessarCoeficienteVariacao(Media media)
        {
            double variancia = 0;

            foreach (var value in Jogador.Partidas.Select(x => x.GetType().GetProperty(media.Nome).GetValue(x)))
            {
                variancia += Math.Pow((Convert.ToDouble(value) - media.Valor), 2);
            }

            double desvioPadrao = Math.Sqrt(variancia / (Jogador.Partidas.Count() - 1));

            media.CoeficienteVariacao = Math.Round((desvioPadrao / media.Valor) * 100, 2);
        }

        protected void dgMedias_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                foreach (Media media in Jogador.Medias)
                {
                    Label lblAverage = (Label)e.Row.FindControl("lblAvg" + media.Nome);

                    if (media.Valor < 1 || media.CoeficienteVariacao > 30)
                    {
                        lblAverage.ForeColor = System.Drawing.Color.Red;
                    }
                    else if (media.CoeficienteVariacao > 15 && media.CoeficienteVariacao <= 30)
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