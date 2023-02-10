<%@ Page Title="NBA STATS" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="NBAStats._Default" %>

<%@ Register TagPrefix="uc" TagName="Partidas" Src="~/Controles/ucPartidas.ascx" %>
<%@ Register TagPrefix="uc" TagName="Medias" Src="~/Controles/ucMedias.ascx" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <asp:Table runat="server">
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
    </asp:Table>

    <div>
        <asp:Button ID="butProcurar" runat="server" Text="Procurar" OnClick="butProcurar_Click" />
    </div>

    <uc:Partidas runat="server" ID="ucPartidas" Visible="false" />

    <uc:Medias runat="server" ID="ucMedias" Visible="false" />

    <div>
        <asp:Label runat="server" ID="lblDoubleDouble" Text="Duplo Duplo" Visible="false" />
    </div>

    <div>
        <asp:Label runat="server" ID="lblTripleDouble" Text="Triplo Duplo" Visible="false" />
    </div>
</asp:Content>
