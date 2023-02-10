<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ucPartidas.ascx.cs" Inherits="NBAStats.Controles.ucPartidas" %>

<asp:GridView ID="dgPartidas" runat="server" AutoGenerateColumns="false">
    <Columns>
        <asp:TemplateField HeaderText="Data">
            <ItemTemplate>
                <asp:Label runat="server" ID="lblData" Text='<%# DataBinder.Eval(Container, "DataItem.Data") %>' />
            </ItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="Adversário">
            <ItemTemplate>
                <asp:Label runat="server" ID="lblAdversario" Text='<%# DataBinder.Eval(Container, "DataItem.Adversario") %>' />
            </ItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="Resultado">
            <ItemTemplate>
                <asp:Label runat="server" ID="lblResultado" Text='<%# DataBinder.Eval(Container, "DataItem.Resultado") %>' />
            </ItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="Minutos">
            <ItemTemplate>
                <asp:Label runat="server" ID="lblMinutos" Text='<%# DataBinder.Eval(Container, "DataItem.Minutos") %>' />
            </ItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="Field Goals Feitos">
            <ItemTemplate>
                <asp:Label runat="server" ID="lblFieldGoalsFeitos" Text='<%# DataBinder.Eval(Container, "DataItem.FieldGoalsFeitos") %>' />
            </ItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="Field Goals Tentados">
            <ItemTemplate>
                <asp:Label runat="server" ID="lblFieldGoalsTentados" Text='<%# DataBinder.Eval(Container, "DataItem.FieldGoalsTentados") %>' />
            </ItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="Field Goals %">
            <ItemTemplate>
                <asp:Label runat="server" ID="lblFieldGoalPercentual" Text='<%# DataBinder.Eval(Container, "DataItem.FieldGoalPercentual") + "%" %>' />
            </ItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="Cestas de 3 Feitas">
            <ItemTemplate>
                <asp:Label runat="server" ID="lblCestas3Feitas" Text='<%# DataBinder.Eval(Container, "DataItem.Cestas3Feitas") %>' />
            </ItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="Cestas de 3 Tentadas">
            <ItemTemplate>
                <asp:Label runat="server" ID="lblCestas3Tentadas" Text='<%# DataBinder.Eval(Container, "DataItem.Cestas3Tentadas") %>' />
            </ItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="Cestas de 3 %">
            <ItemTemplate>
                <asp:Label runat="server" ID="lblCestas3Percentual" Text='<%# DataBinder.Eval(Container, "DataItem.Cestas3Percentual") + "%" %>' />
            </ItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="Lances Livres Feitos">
            <ItemTemplate>
                <asp:Label runat="server" ID="lblLancesLivresFeitos" Text='<%# DataBinder.Eval(Container, "DataItem.LancesLivresFeitos") %>' />
            </ItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="Lances Livres Tentados">
            <ItemTemplate>
                <asp:Label runat="server" ID="lblLancesLivresTentados" Text='<%# DataBinder.Eval(Container, "DataItem.LancesLivresTentados") %>' />
            </ItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="Lances Livres %">
            <ItemTemplate>
                <asp:Label runat="server" ID="lblLancesLivresPercentual" Text='<%# DataBinder.Eval(Container, "DataItem.LancesLivresPercentual") + "%" %>' />
            </ItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="Rebotes">
            <ItemTemplate>
                <asp:Label runat="server" ID="lblRebotes" Text='<%# DataBinder.Eval(Container, "DataItem.Rebotes") %>' />
            </ItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="Assistências">
            <ItemTemplate>
                <asp:Label runat="server" ID="lblAssistencias" Text='<%# DataBinder.Eval(Container, "DataItem.Assistencias") %>' />
            </ItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="Bloqueios">
            <ItemTemplate>
                <asp:Label runat="server" ID="lblBloqueios" Text='<%# DataBinder.Eval(Container, "DataItem.Bloqueios") %>' />
            </ItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="Roubos">
            <ItemTemplate>
                <asp:Label runat="server" ID="lblRoubos" Text='<%# DataBinder.Eval(Container, "DataItem.Roubos") %>' />
            </ItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="Inversões de Posse">
            <ItemTemplate>
                <asp:Label runat="server" ID="lblInversoesPosse" Text='<%# DataBinder.Eval(Container, "DataItem.InversoesPosse") %>' />
            </ItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="Pontos">
            <ItemTemplate>
                <asp:Label runat="server" ID="lblPontos" Text='<%# DataBinder.Eval(Container, "DataItem.Pontos") %>' />
            </ItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="Pontos, Assistências e Rebotes">
            <ItemTemplate>
                <asp:Label runat="server" ID="lblPontosAssistenciasRebotes" Text='<%# DataBinder.Eval(Container, "DataItem.PontosAssistenciasRebotes") %>' />
            </ItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="Pontos e Assistências">
            <ItemTemplate>
                <asp:Label runat="server" ID="lblPontosAssistencias" Text='<%# DataBinder.Eval(Container, "DataItem.PontosAssistencias") %>' />
            </ItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="Pontos e Rebotes">
            <ItemTemplate>
                <asp:Label runat="server" ID="lblPontosRebotes" Text='<%# DataBinder.Eval(Container, "DataItem.PontosRebotes") %>' />
            </ItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="Assistências e Rebotes">
            <ItemTemplate>
                <asp:Label runat="server" ID="lblAssistenciasRebotes" Text='<%# DataBinder.Eval(Container, "DataItem.AssistenciasRebotes") %>' />
            </ItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="Roubos e Bloqueios">
            <ItemTemplate>
                <asp:Label runat="server" ID="lblRoubosBloqueios" Text='<%# DataBinder.Eval(Container, "DataItem.RoubosBloqueios") %>' />
            </ItemTemplate>
        </asp:TemplateField>
    </Columns>
</asp:GridView>

