<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ucMedias.ascx.cs" Inherits="NBAStats.Controles.ucMedias" %>

<asp:GridView ID="dgMedias" runat="server" AutoGenerateColumns="false" OnRowDataBound="dgMedias_RowDataBound">
    <Columns>
        <asp:TemplateField HeaderText="Cestas de 3">
            <ItemTemplate>
                <div>
                    <asp:Label runat="server" ID="lblAvgCestas3Feitas" Text='<%# DataBinder.Eval(Container, "DataItem.Cestas3Feitas") %>' />
                </div>
                <asp:Label runat="server" ID="coefAvgCestas3Feitas" Font-Size="XX-Small" />
            </ItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="Rebotes">
            <ItemTemplate>
                <div>
                    <asp:Label runat="server" ID="lblAvgRebotes" Text='<%# DataBinder.Eval(Container, "DataItem.Rebotes") %>' />
                </div>
                <asp:Label runat="server" ID="coefAvgRebotes" Font-Size="XX-Small" />
            </ItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="Assistências">
            <ItemTemplate>
                <div>
                    <asp:Label runat="server" ID="lblAvgAssistencias" Text='<%# DataBinder.Eval(Container, "DataItem.Assistencias") %>' />
                </div>
                <asp:Label runat="server" ID="coefAvgAssistencias" Font-Size="XX-Small" />
            </ItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="Bloqueios">
            <ItemTemplate>
                <div>
                    <asp:Label runat="server" ID="lblAvgBloqueios" Text='<%# DataBinder.Eval(Container, "DataItem.Bloqueios") %>' />
                </div>
                <asp:Label runat="server" ID="coefAvgBloqueios" Font-Size="XX-Small" />
            </ItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="Roubos">
            <ItemTemplate>
                <div>
                    <asp:Label runat="server" ID="lblAvgRoubos" Text='<%# DataBinder.Eval(Container, "DataItem.Roubos") %>' />
                </div>
                <asp:Label runat="server" ID="coefAvgRoubos" Font-Size="XX-Small" />
            </ItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="Inversões de Posse">
            <ItemTemplate>
                <div>
                    <asp:Label runat="server" ID="lblAvgInversoesPosse" Text='<%# DataBinder.Eval(Container, "DataItem.InversoesPosse") %>' />
                </div>
                <asp:Label runat="server" ID="coefAvgInversoesPosse" Font-Size="XX-Small" />
            </ItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="Pontos">
            <ItemTemplate>
                <div>
                    <asp:Label runat="server" ID="lblAvgPontos" Text='<%# DataBinder.Eval(Container, "DataItem.Pontos") %>' />
                </div>
                <asp:Label runat="server" ID="coefAvgPontos" Font-Size="XX-Small" />
            </ItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="Pontos, Assistências e Rebotes">
            <ItemTemplate>
                <div>
                    <asp:Label runat="server" ID="lblAvgPontosAssistenciasRebotes" Text='<%# DataBinder.Eval(Container, "DataItem.PontosAssistenciasRebotes") %>' />
                </div>
                <asp:Label runat="server" ID="coefAvgPontosAssistenciasRebotes" Font-Size="XX-Small" />
            </ItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="Pontos e Assistências">
            <ItemTemplate>
                <div>
                    <asp:Label runat="server" ID="lblAvgPontosAssistencias" Text='<%# DataBinder.Eval(Container, "DataItem.PontosAssistencias") %>' />
                </div>
                <asp:Label runat="server" ID="coefAvgPontosAssistencias" Font-Size="XX-Small" />
            </ItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="Pontos e Rebotes">
            <ItemTemplate>
                <div>
                    <asp:Label runat="server" ID="lblAvgPontosRebotes" Text='<%# DataBinder.Eval(Container, "DataItem.PontosRebotes") %>' />
                </div>
                <asp:Label runat="server" ID="coefAvgPontosRebotes" Font-Size="XX-Small" />
            </ItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="Assistências e Rebotes">
            <ItemTemplate>
                <div>
                    <asp:Label runat="server" ID="lblAvgAssistenciasRebotes" Text='<%# DataBinder.Eval(Container, "DataItem.AssistenciasRebotes") %>' />
                </div>
                <asp:Label runat="server" ID="coefAvgAssistenciasRebotes" Font-Size="XX-Small" />
            </ItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="Roubos e Bloqueios">
            <ItemTemplate>
                <div>
                    <asp:Label runat="server" ID="lblAvgRoubosBloqueios" Text='<%# DataBinder.Eval(Container, "DataItem.RoubosBloqueios") %>' />
                </div>
                <asp:Label runat="server" ID="coefAvgRoubosBloqueios" Font-Size="XX-Small" />
            </ItemTemplate>
        </asp:TemplateField>
    </Columns>
</asp:GridView>

