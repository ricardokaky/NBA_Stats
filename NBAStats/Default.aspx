<%@ Page Title="NBA STATS" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="NBAStats._Default" %>

<%@ Register TagPrefix="uc" TagName="Partidas" Src="~/Controles/ucPartidas.ascx" %>
<%@ Register TagPrefix="uc" TagName="Medias" Src="~/Controles/ucMedias.ascx" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <asp:Table runat="server">
        <asp:TableRow>
            <asp:TableCell>
                <asp:Label ID="lblQuantJogos" runat="server" Text="Quantidade de Jogos: " />
            </asp:TableCell>
            <asp:TableCell>
                <asp:TextBox ID="txtQuantJogos" runat="server" Text="5" />
            </asp:TableCell>
            <asp:TableCell>
                <asp:Label ID="lblQuantJogosObrigatorio" runat="server" Text="Informe uma quantidade de jogos válida!" ForeColor="Red" Visible="false" />
            </asp:TableCell>
        </asp:TableRow>
        <asp:TableRow>
            <asp:TableCell>
                <asp:Label ID="lblMinMinutos" runat="server" Text="Mínimo de Minutos: " />
            </asp:TableCell>
            <asp:TableCell>
                <asp:TextBox ID="txtMinMinutos" runat="server" />
            </asp:TableCell>
            <asp:TableCell>
                <asp:Label ID="lblMinMinutosValido" runat="server" Text="Informe um mínimo de minutos válido!" ForeColor="Red" Visible="false" />
            </asp:TableCell>
        </asp:TableRow>
        <asp:TableRow>
            <asp:TableCell>
                <asp:Label ID="lblNomeJogador" runat="server" Text="Nome Jogador:" />
            </asp:TableCell>
            <asp:TableCell>
                <asp:TextBox ID="txtNomeJogador" runat="server" />
            </asp:TableCell>
            <asp:TableCell>
                <asp:Label ID="lblNomeJogadorObrigatorio" runat="server" Text="O nome do jogador é obrigatório!" ForeColor="Red" Visible="false" />
            </asp:TableCell>
        </asp:TableRow>
        <asp:TableRow>
            <asp:TableCell>
                <asp:Label ID="lblAdversario" runat="server" Text="Adversário: " />
            </asp:TableCell>
            <asp:TableCell>
                <asp:DropDownList runat="server" ID="ddlAdversario">
                    <asp:ListItem Selected="True" />
                    <asp:ListItem Text="Atlanta Hawks" Value="ATL" />
                    <asp:ListItem Text="Boston Celtics" Value="BOS" />
                    <asp:ListItem Text="Brooklyn Nets" Value="BRK" />
                    <asp:ListItem Text="Charlotte Hornets" Value="CHO" />
                    <asp:ListItem Text="Chicago Bulls" Value="CHI" />
                    <asp:ListItem Text="Cleveland Cavaliers" Value="CLE" />
                    <asp:ListItem Text="Dallas Mavericks" Value="DAL" />
                    <asp:ListItem Text="Denver Nuggets" Value="DEN" />
                    <asp:ListItem Text="Detroit Pistons" Value="DET" />
                    <asp:ListItem Text="Golden State Warriors" Value="GSW" />
                    <asp:ListItem Text="Houston Rockets" Value="HOU" />
                    <asp:ListItem Text="Indiana Pacers" Value="IND" />
                    <asp:ListItem Text="Los Angeles Clippers" Value="LAC" />
                    <asp:ListItem Text="Los Angeles Lakers" Value="LAL" />
                    <asp:ListItem Text="Memphis Grizzlies" Value="MEM" />
                    <asp:ListItem Text="Miami Heat" Value="MIA" />
                    <asp:ListItem Text="Milwaukee Bucks" Value="MIL" />
                    <asp:ListItem Text="Minnesota Timberwolves" Value="MIN" />
                    <asp:ListItem Text="New Orleans Pelicans" Value="NOP" />
                    <asp:ListItem Text="New York Knicks" Value="NYK" />
                    <asp:ListItem Text="Oklahoma City Thunder" Value="OKC" />
                    <asp:ListItem Text="Orlando Magic" Value="ORL" />
                    <asp:ListItem Text="Philadelphia 76ers" Value="PHI" />
                    <asp:ListItem Text="Phoenix Suns" Value="PHO" />
                    <asp:ListItem Text="Portland Trail Blazers" Value="POR" />
                    <asp:ListItem Text="Sacramento Kings" Value="SAC" />
                    <asp:ListItem Text="San Antonio Spurs" Value="SAS" />
                    <asp:ListItem Text="Toronto Raptors" Value="TOR" />
                    <asp:ListItem Text="Utah Jazz" Value="UTA" />
                    <asp:ListItem Text="Washington Wizards" Value="WAS" />
                </asp:DropDownList>
            </asp:TableCell>
        </asp:TableRow>
    </asp:Table>

    <div>
        <asp:Button ID="butProcurar" runat="server" Text="Procurar" OnClick="butProcurar_Click" />
    </div>

    <div>
        <asp:Label runat="server" ID="lblTrocaTime" Text="O JOGADOR TROCOU DE TIME RECENTEMENTE!" ForeColor="Red" Visible="false" />
    </div>

    <asp:Label runat="server" ID="lblPartidasRecentes" Text="Partidas Recentes" Visible="false"/>

    <uc:Partidas runat="server" ID="ucPartidas" Visible="false" />

    <asp:Label runat="server" ID="lblMediasPartidasRecentes" Text="Médias Partidas Recentes" Visible="false"/>

    <uc:Medias runat="server" ID="ucMedias" Visible="false" />

    <asp:Label runat="server" ID="lblPartidasContraAdv" Visible="false" />

    <uc:Partidas runat="server" ID="ucPartidasContraAdv" Visible="false" />

    <asp:Label runat="server" ID="lblMediasContraAdv" Visible="false" />

    <uc:Medias runat="server" ID="ucMediasContraAdv" Visible="false"/>

    <div>
        <asp:Label runat="server" ID="lblDoubleDouble" Text="Duplo Duplo" Visible="false" />
    </div>

    <div>
        <asp:Label runat="server" ID="lblTripleDouble" Text="Triplo Duplo" Visible="false" />
    </div>
</asp:Content>