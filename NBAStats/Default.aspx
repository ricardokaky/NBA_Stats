<%@ Page Title="NBA STATS" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="NBAStats._Default" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <asp:Table runat="server">
        <asp:TableRow>
            <asp:TableCell>
                <asp:Button ID="butGerarPlanilha" runat="server" Text="Gerar Planilha" OnClick="butGerarPlanilha_Click" />
            </asp:TableCell>
        </asp:TableRow>
        <asp:TableRow>
            <asp:TableCell>
                <asp:FileUpload ID="FileUploadControl" runat="server" class="multi" AllowMultiple="false" />
            </asp:TableCell>
            <asp:TableCell>
                <asp:Button ID="butUpload" runat="server" Text="Carregar Planilha" onclick="butUpload_Click" />
            </asp:TableCell>
        </asp:TableRow>
    </asp:Table>
</asp:Content>