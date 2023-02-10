<%@ Page Title="NBA STATS" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="NBAStats._Default" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <div>
        <asp:Label ID="lblPlayerName" runat="server" Text="Nome Jogador: " />
        <asp:TextBox ID="txtPlayerName" runat="server" />
        <asp:Label ID="lblPlayerNameRequired" runat="server" Text="The player name is required!" ForeColor="Red" Visible="false" />
    </div>

    <div>
        <asp:Label ID="lblQuantJogos" runat="server" Text="Quantidade de Jogos: " />
        <asp:TextBox ID="txtQuantJogos" runat="server" Text="5" />
        <asp:Label ID="lblQuantJogosObrigatorio" runat="server" Text="Informe uma quantidade de jogos válida!" ForeColor="Red" Visible="false" />
    </div>

    <div>
        <asp:Label ID="lblMinMinutos" runat="server" Text="Mínimo de minutos: " />
        <asp:TextBox ID="txtMinMinutos" runat="server" />
        <asp:Label ID="lblMinMinutosValido" runat="server" Text="Informe um mínimo de minutos válido!" ForeColor="Red" Visible="false" />
    </div>

    <div>
        <asp:Button ID="butSearch" runat="server" Text="Search" OnClick="butSearch_Click" />
    </div>

    <div>
        <asp:DataGrid ID="dgGames" runat="server" AutoGenerateColumns="false">
            <Columns>
                <asp:TemplateColumn HeaderText="Data">
                    <ItemTemplate>
                        <asp:Label runat="server" ID="lblData" Text='<%# DataBinder.Eval(Container, "DataItem.Data") %>' />
                    </ItemTemplate>
                </asp:TemplateColumn>
            </Columns>
            <Columns>
                <asp:TemplateColumn HeaderText="Adversário">
                    <ItemTemplate>
                        <asp:Label runat="server" ID="lblAdversario" Text='<%# DataBinder.Eval(Container, "DataItem.Adversario") %>' />
                    </ItemTemplate>
                </asp:TemplateColumn>
            </Columns>
            <Columns>
                <asp:TemplateColumn HeaderText="Resultado">
                    <ItemTemplate>
                        <asp:Label runat="server" ID="lblResultado" Text='<%# DataBinder.Eval(Container, "DataItem.Resultado") %>' />
                    </ItemTemplate>
                </asp:TemplateColumn>
            </Columns>
            <Columns>
                <asp:TemplateColumn HeaderText="Minutos">
                    <ItemTemplate>
                        <asp:Label runat="server" ID="lblMinutos" Text='<%# DataBinder.Eval(Container, "DataItem.Minutos") %>' />
                    </ItemTemplate>
                </asp:TemplateColumn>
            </Columns>
            <Columns>
                <asp:TemplateColumn HeaderText="Field Goals Feitos">
                    <ItemTemplate>
                        <asp:Label runat="server" ID="lblFieldGoalsFeitos" Text='<%# DataBinder.Eval(Container, "DataItem.FieldGoalsFeitos") %>' />
                    </ItemTemplate>
                </asp:TemplateColumn>
            </Columns>
            <Columns>
                <asp:TemplateColumn HeaderText="Field Goals Tentados">
                    <ItemTemplate>
                        <asp:Label runat="server" ID="lblFieldGoalsTentados" Text='<%# DataBinder.Eval(Container, "DataItem.FieldGoalsTentados") %>' />
                    </ItemTemplate>
                </asp:TemplateColumn>
            </Columns>
            <Columns>
                <asp:TemplateColumn HeaderText="Field Goals %">
                    <ItemTemplate>
                        <asp:Label runat="server" ID="lblFieldGoalPercentual" Text='<%# DataBinder.Eval(Container, "DataItem.FieldGoalPercentual") + "%" %>' />
                    </ItemTemplate>
                </asp:TemplateColumn>
            </Columns>
            <Columns>
                <asp:TemplateColumn HeaderText="Cestas de 3 Feitas">
                    <ItemTemplate>
                        <asp:Label runat="server" ID="lblCestas3Feitas" Text='<%# DataBinder.Eval(Container, "DataItem.Cestas3Feitas") %>' />
                    </ItemTemplate>
                </asp:TemplateColumn>
            </Columns>
            <Columns>
                <asp:TemplateColumn HeaderText="Cestas de 3 Tentadas">
                    <ItemTemplate>
                        <asp:Label runat="server" ID="lblCestas3Tentadas" Text='<%# DataBinder.Eval(Container, "DataItem.Cestas3Tentadas") %>' />
                    </ItemTemplate>
                </asp:TemplateColumn>
            </Columns>
            <Columns>
                <asp:TemplateColumn HeaderText="Cestas de 3 %">
                    <ItemTemplate>
                        <asp:Label runat="server" ID="lblCestas3Percentual" Text='<%# DataBinder.Eval(Container, "DataItem.Cestas3Percentual") + "%" %>' />
                    </ItemTemplate>
                </asp:TemplateColumn>
            </Columns>
            <Columns>
                <asp:TemplateColumn HeaderText="Lances Livres Feitos">
                    <ItemTemplate>
                        <asp:Label runat="server" ID="lblLancesLivresFeitos" Text='<%# DataBinder.Eval(Container, "DataItem.LancesLivresFeitos") %>' />
                    </ItemTemplate>
                </asp:TemplateColumn>
            </Columns>
            <Columns>
                <asp:TemplateColumn HeaderText="Lances Livres Tentados">
                    <ItemTemplate>
                        <asp:Label runat="server" ID="lblLancesLivresTentados" Text='<%# DataBinder.Eval(Container, "DataItem.LancesLivresTentados") %>' />
                    </ItemTemplate>
                </asp:TemplateColumn>
            </Columns>
            <Columns>
                <asp:TemplateColumn HeaderText="Lances Livres %">
                    <ItemTemplate>
                        <asp:Label runat="server" ID="lblLancesLivresPercentual" Text='<%# DataBinder.Eval(Container, "DataItem.LancesLivresPercentual") + "%" %>' />
                    </ItemTemplate>
                </asp:TemplateColumn>
            </Columns>
            <Columns>
                <asp:TemplateColumn HeaderText="Rebotes">
                    <ItemTemplate>
                        <asp:Label runat="server" ID="lblRebotes" Text='<%# DataBinder.Eval(Container, "DataItem.Rebotes") %>' />
                    </ItemTemplate>
                </asp:TemplateColumn>
            </Columns>
            <Columns>
                <asp:TemplateColumn HeaderText="Assistências">
                    <ItemTemplate>
                        <asp:Label runat="server" ID="lblAssistencias" Text='<%# DataBinder.Eval(Container, "DataItem.Assistencias") %>' />
                    </ItemTemplate>
                </asp:TemplateColumn>
            </Columns>
            <Columns>
                <asp:TemplateColumn HeaderText="Bloqueios">
                    <ItemTemplate>
                        <asp:Label runat="server" ID="lblBloqueios" Text='<%# DataBinder.Eval(Container, "DataItem.Bloqueios") %>' />
                    </ItemTemplate>
                </asp:TemplateColumn>
            </Columns>
            <Columns>
                <asp:TemplateColumn HeaderText="Roubos">
                    <ItemTemplate>
                        <asp:Label runat="server" ID="lblRoubos" Text='<%# DataBinder.Eval(Container, "DataItem.Roubos") %>' />
                    </ItemTemplate>
                </asp:TemplateColumn>
            </Columns>
            <Columns>
                <asp:TemplateColumn HeaderText="Inversões de Posse">
                    <ItemTemplate>
                        <asp:Label runat="server" ID="lblInversoesPosse" Text='<%# DataBinder.Eval(Container, "DataItem.InversoesPosse") %>' />
                    </ItemTemplate>
                </asp:TemplateColumn>
            </Columns>
            <Columns>
                <asp:TemplateColumn HeaderText="Pontos">
                    <ItemTemplate>
                        <asp:Label runat="server" ID="lblPontos" Text='<%# DataBinder.Eval(Container, "DataItem.Pontos") %>' />
                    </ItemTemplate>
                </asp:TemplateColumn>
            </Columns>
            <Columns>
                <asp:TemplateColumn HeaderText="Pontos, Assistências e Rebotes">
                    <ItemTemplate>
                        <asp:Label runat="server" ID="lblPontosAssistenciasRebotes" Text='<%# DataBinder.Eval(Container, "DataItem.PontosAssistenciasRebotes") %>' />
                    </ItemTemplate>
                </asp:TemplateColumn>
            </Columns>
            <Columns>
                <asp:TemplateColumn HeaderText="Pontos e Assistências">
                    <ItemTemplate>
                        <asp:Label runat="server" ID="lblPontosAssistencias" Text='<%# DataBinder.Eval(Container, "DataItem.PontosAssistencias") %>' />
                    </ItemTemplate>
                </asp:TemplateColumn>
            </Columns>
            <Columns>
                <asp:TemplateColumn HeaderText="Pontos e Rebotes">
                    <ItemTemplate>
                        <asp:Label runat="server" ID="lblPontosRebotes" Text='<%# DataBinder.Eval(Container, "DataItem.PontosRebotes") %>' />
                    </ItemTemplate>
                </asp:TemplateColumn>
            </Columns>
            <Columns>
                <asp:TemplateColumn HeaderText="Assistências e Rebotes">
                    <ItemTemplate>
                        <asp:Label runat="server" ID="lblAssistenciasRebotes" Text='<%# DataBinder.Eval(Container, "DataItem.AssistenciasRebotes") %>' />
                    </ItemTemplate>
                </asp:TemplateColumn>
            </Columns>
            <Columns>
                <asp:TemplateColumn HeaderText="Roubos e Bloqueios">
                    <ItemTemplate>
                        <asp:Label runat="server" ID="lblRoubosBloqueios" Text='<%# DataBinder.Eval(Container, "DataItem.RoubosBloqueios") %>' />
                    </ItemTemplate>
                </asp:TemplateColumn>
            </Columns>
        </asp:DataGrid>
    </div>

    <div>
        <asp:DataGrid ID="dgAverages" runat="server" OnItemDataBound="dgAverages_ItemDataBound" AutoGenerateColumns="false">
            <Columns>
                <asp:TemplateColumn HeaderText="Cestas de 3">
                    <HeaderStyle HorizontalAlign="Center" VerticalAlign="Middle"/>
                    <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle"/>
                    <ItemTemplate>
                        <div>
                            <asp:Label runat="server" ID="lblAvgCestas3Feitas" Text='<%# DataBinder.Eval(Container, "DataItem.Cestas3Feitas") %>' />
                        </div>
                        <asp:Label runat="server" ID="coefAvgCestas3Feitas" Font-Size="XX-Small" />
                    </ItemTemplate>
                </asp:TemplateColumn>
            </Columns>
            <Columns>
                <asp:TemplateColumn HeaderText="Rebotes">
                    <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle"/>
                    <ItemTemplate>
                        <div>
                            <asp:Label runat="server" ID="lblAvgRebotes" Text='<%# DataBinder.Eval(Container, "DataItem.Rebotes") %>' />
                        </div>
                        <asp:Label runat="server" ID="coefAvgRebotes" Font-Size="XX-Small" />
                    </ItemTemplate>
                </asp:TemplateColumn>
            </Columns>
            <Columns>
                <asp:TemplateColumn HeaderText="Assistências">
                    <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle"/>
                    <ItemTemplate>
                        <div>
                            <asp:Label runat="server" ID="lblAvgAssistencias" Text='<%# DataBinder.Eval(Container, "DataItem.Assistencias") %>' />
                        </div>
                        <asp:Label runat="server" ID="coefAvgAssistencias" Font-Size="XX-Small" />
                    </ItemTemplate>
                </asp:TemplateColumn>
            </Columns>
            <Columns>
                <asp:TemplateColumn HeaderText="Bloqueios">
                    <ItemTemplate>
                        <div>
                            <asp:Label runat="server" ID="lblAvgBloqueios" Text='<%# DataBinder.Eval(Container, "DataItem.Bloqueios") %>' />
                        </div>
                        <asp:Label runat="server" ID="coefAvgBloqueios" Font-Size="XX-Small" />
                    </ItemTemplate>
                </asp:TemplateColumn>
            </Columns>
            <Columns>
                <asp:TemplateColumn HeaderText="Roubos">
                    <ItemTemplate>
                        <div>
                            <asp:Label runat="server" ID="lblAvgRoubos" Text='<%# DataBinder.Eval(Container, "DataItem.Roubos") %>' />
                        </div>
                        <asp:Label runat="server" ID="coefAvgRoubos" Font-Size="XX-Small" />
                    </ItemTemplate>
                </asp:TemplateColumn>
            </Columns>
            <Columns>
                <asp:TemplateColumn HeaderText="Inversões de Posse">
                    <ItemTemplate>
                        <div>
                            <asp:Label runat="server" ID="lblAvgInversoesPosse" Text='<%# DataBinder.Eval(Container, "DataItem.InversoesPosse") %>' />
                        </div>
                        <asp:Label runat="server" ID="coefAvgInversoesPosse" Font-Size="XX-Small" />
                    </ItemTemplate>
                </asp:TemplateColumn>
            </Columns>
            <Columns>
                <asp:TemplateColumn HeaderText="Pontos">
                    <ItemTemplate>
                        <div>
                            <asp:Label runat="server" ID="lblAvgPontos" Text='<%# DataBinder.Eval(Container, "DataItem.Pontos") %>' />
                        </div>
                        <asp:Label runat="server" ID="coefAvgPontos" Font-Size="XX-Small" />
                    </ItemTemplate>
                </asp:TemplateColumn>
            </Columns>
            <Columns>
                <asp:TemplateColumn HeaderText="Pontos, Assistências e Rebotes">
                    <ItemTemplate>
                        <div>
                            <asp:Label runat="server" ID="lblAvgPontosAssistenciasRebotes" Text='<%# DataBinder.Eval(Container, "DataItem.PontosAssistenciasRebotes") %>' />
                        </div>
                        <asp:Label runat="server" ID="coefAvgPontosAssistenciasRebotes" Font-Size="XX-Small" />
                    </ItemTemplate>
                </asp:TemplateColumn>
            </Columns>
            <Columns>
                <asp:TemplateColumn HeaderText="Pontos e Assistências">
                    <ItemTemplate>
                        <div>
                            <asp:Label runat="server" ID="lblAvgPontosAssistencias" Text='<%# DataBinder.Eval(Container, "DataItem.PontosAssistencias") %>' />
                        </div>
                        <asp:Label runat="server" ID="coefAvgPontosAssistencias" Font-Size="XX-Small" />
                    </ItemTemplate>
                </asp:TemplateColumn>
            </Columns>
            <Columns>
                <asp:TemplateColumn HeaderText="Pontos e Rebotes">
                    <ItemTemplate>
                        <div>
                            <asp:Label runat="server" ID="lblAvgPontosRebotes" Text='<%# DataBinder.Eval(Container, "DataItem.PontosRebotes") %>' />
                        </div>
                        <asp:Label runat="server" ID="coefAvgPontosRebotes" Font-Size="XX-Small" />
                    </ItemTemplate>
                </asp:TemplateColumn>
            </Columns>
            <Columns>
                <asp:TemplateColumn HeaderText="Assistências e Rebotes">
                    <ItemTemplate>
                        <div>
                            <asp:Label runat="server" ID="lblAvgAssistenciasRebotes" Text='<%# DataBinder.Eval(Container, "DataItem.AssistenciasRebotes") %>' />
                        </div>
                        <asp:Label runat="server" ID="coefAvgAssistenciasRebotes" Font-Size="XX-Small" />
                    </ItemTemplate>
                </asp:TemplateColumn>
            </Columns>
            <Columns>
                <asp:TemplateColumn HeaderText="Roubos e Bloqueios">
                    <ItemTemplate>
                        <div>
                            <asp:Label runat="server" ID="lblAvgRoubosBloqueios" Text='<%# DataBinder.Eval(Container, "DataItem.RoubosBloqueios") %>' />
                        </div>
                        <asp:Label runat="server" ID="coefAvgRoubosBloqueios" Font-Size="XX-Small" />
                    </ItemTemplate>
                </asp:TemplateColumn>
            </Columns>
        </asp:DataGrid>
    </div>

    <div>
        <asp:Label runat="server" ID="lblDoubleDouble" Text="Duplo Duplo" Visible="false" />
    </div>

    <div>
        <asp:Label runat="server" ID="lblTripleDouble" Text="Triplo Duplo" Visible="false" />
    </div>
</asp:Content>
