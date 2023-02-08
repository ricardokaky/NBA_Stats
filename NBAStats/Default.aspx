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
        <asp:Button ID="butSearch" runat="server" Text="Search" OnClick="butSearch_Click" />
    </div>

    <div>
        <asp:DataGrid ID="dgGames" runat="server" AutoGenerateColumns="false">
            <Columns>
                <asp:TemplateColumn HeaderText="Data">
                    <ItemTemplate>
                        <asp:Label runat="server" ID="Date" Text='<%# DataBinder.Eval(Container, "DataItem.Date") %>'>
                        </asp:Label>
                    </ItemTemplate>
                </asp:TemplateColumn>
            </Columns>
            <Columns>
                <asp:TemplateColumn HeaderText="Adversário">
                    <ItemTemplate>
                        <asp:Label runat="server" ID="Opponent" Text='<%# DataBinder.Eval(Container, "DataItem.Opponent") %>'>
                        </asp:Label>
                    </ItemTemplate>
                </asp:TemplateColumn>
            </Columns>
            <Columns>
                <asp:TemplateColumn HeaderText="Resultado">
                    <ItemTemplate>
                        <asp:Label runat="server" ID="Result" Text='<%# DataBinder.Eval(Container, "DataItem.Result") %>'>
                        </asp:Label>
                    </ItemTemplate>
                </asp:TemplateColumn>
            </Columns>
            <Columns>
                <asp:TemplateColumn HeaderText="Minutos">
                    <ItemTemplate>
                        <asp:Label runat="server" ID="Minutes" Text='<%# DataBinder.Eval(Container, "DataItem.Minutes") %>'>
                        </asp:Label>
                    </ItemTemplate>
                </asp:TemplateColumn>
            </Columns>
            <Columns>
                <asp:TemplateColumn HeaderText="Field Goals Feitos">
                    <ItemTemplate>
                        <asp:Label runat="server" ID="FieldGoalsMade" Text='<%# DataBinder.Eval(Container, "DataItem.FieldGoalsMade") %>'>
                        </asp:Label>
                    </ItemTemplate>
                </asp:TemplateColumn>
            </Columns>
            <Columns>
                <asp:TemplateColumn HeaderText="Field Goals Tentados">
                    <ItemTemplate>
                        <asp:Label runat="server" ID="FieldGoalsAttempted" Text='<%# DataBinder.Eval(Container, "DataItem.FieldGoalsAttempted") %>'>
                        </asp:Label>
                    </ItemTemplate>
                </asp:TemplateColumn>
            </Columns>
            <Columns>
                <asp:TemplateColumn HeaderText="Field Goals %">
                    <ItemTemplate>
                        <asp:Label runat="server" ID="FieldGoalPercentage" Text='<%# DataBinder.Eval(Container, "DataItem.FieldGoalPercentage") + "%" %>'>
                        </asp:Label>
                    </ItemTemplate>
                </asp:TemplateColumn>
            </Columns>
            <Columns>
                <asp:TemplateColumn HeaderText="Cestas de 3 Feitas">
                    <ItemTemplate>
                        <asp:Label runat="server" ID="TreePointFieldGoalsMade" Text='<%# DataBinder.Eval(Container, "DataItem.TreePointFieldGoalsMade") %>'>
                        </asp:Label>
                    </ItemTemplate>
                </asp:TemplateColumn>
            </Columns>
            <Columns>
                <asp:TemplateColumn HeaderText="Cestas de 3 Tentadas">
                    <ItemTemplate>
                        <asp:Label runat="server" ID="TreePointFieldGoalsAttempted" Text='<%# DataBinder.Eval(Container, "DataItem.TreePointFieldGoalsAttempted") %>'>
                        </asp:Label>
                    </ItemTemplate>
                </asp:TemplateColumn>
            </Columns>
            <Columns>
                <asp:TemplateColumn HeaderText="Cestas de 3 %">
                    <ItemTemplate>
                        <asp:Label runat="server" ID="TreePointFieldGoalPercentage" Text='<%# DataBinder.Eval(Container, "DataItem.TreePointFieldGoalPercentage") + "%" %>'>
                        </asp:Label>
                    </ItemTemplate>
                </asp:TemplateColumn>
            </Columns>
            <Columns>
                <asp:TemplateColumn HeaderText="Lances Livres Feitos">
                    <ItemTemplate>
                        <asp:Label runat="server" ID="FreeThrowsMade" Text='<%# DataBinder.Eval(Container, "DataItem.FreeThrowsMade") %>'>
                        </asp:Label>
                    </ItemTemplate>
                </asp:TemplateColumn>
            </Columns>
            <Columns>
                <asp:TemplateColumn HeaderText="Lances Livres Tentados">
                    <ItemTemplate>
                        <asp:Label runat="server" ID="FreeThrowsAttempted" Text='<%# DataBinder.Eval(Container, "DataItem.FreeThrowsAttempted") %>'>
                        </asp:Label>
                    </ItemTemplate>
                </asp:TemplateColumn>
            </Columns>
            <Columns>
                <asp:TemplateColumn HeaderText="Lances Livres %">
                    <ItemTemplate>
                        <asp:Label runat="server" ID="FreeThrowPercentage" Text='<%# DataBinder.Eval(Container, "DataItem.FreeThrowPercentage") + "%" %>'>
                        </asp:Label>
                    </ItemTemplate>
                </asp:TemplateColumn>
            </Columns>
            <Columns>
                <asp:TemplateColumn HeaderText="Rebotes">
                    <ItemTemplate>
                        <asp:Label runat="server" ID="Rebounds" Text='<%# DataBinder.Eval(Container, "DataItem.Rebounds") %>'>
                        </asp:Label>
                    </ItemTemplate>
                </asp:TemplateColumn>
            </Columns>
            <Columns>
                <asp:TemplateColumn HeaderText="Assistências">
                    <ItemTemplate>
                        <asp:Label runat="server" ID="Assists" Text='<%# DataBinder.Eval(Container, "DataItem.Assists") %>'>
                        </asp:Label>
                    </ItemTemplate>
                </asp:TemplateColumn>
            </Columns>
            <Columns>
                <asp:TemplateColumn HeaderText="Bloqueios">
                    <ItemTemplate>
                        <asp:Label runat="server" ID="Blocks" Text='<%# DataBinder.Eval(Container, "DataItem.Blocks") %>'>
                        </asp:Label>
                    </ItemTemplate>
                </asp:TemplateColumn>
            </Columns>
            <Columns>
                <asp:TemplateColumn HeaderText="Roubos">
                    <ItemTemplate>
                        <asp:Label runat="server" ID="Steals" Text='<%# DataBinder.Eval(Container, "DataItem.Steals") %>'>
                        </asp:Label>
                    </ItemTemplate>
                </asp:TemplateColumn>
            </Columns>
            <Columns>
                <asp:TemplateColumn HeaderText="Inversões de Posse">
                    <ItemTemplate>
                        <asp:Label runat="server" ID="Turnovers" Text='<%# DataBinder.Eval(Container, "DataItem.Turnovers") %>'>
                        </asp:Label>
                    </ItemTemplate>
                </asp:TemplateColumn>
            </Columns>
            <Columns>
                <asp:TemplateColumn HeaderText="Pontos">
                    <ItemTemplate>
                        <asp:Label runat="server" ID="Points" Text='<%# DataBinder.Eval(Container, "DataItem.Points") %>'>
                        </asp:Label>
                    </ItemTemplate>
                </asp:TemplateColumn>
            </Columns>
            <Columns>
                <asp:TemplateColumn HeaderText="Pontos, Assistências e Rebotes">
                    <ItemTemplate>
                        <asp:Label runat="server" ID="PointsAssistsRebounds" Text='<%# DataBinder.Eval(Container, "DataItem.PointsAssistsRebounds") %>'>
                        </asp:Label>
                    </ItemTemplate>
                </asp:TemplateColumn>
            </Columns>
            <Columns>
                <asp:TemplateColumn HeaderText="Pontos e Assistências">
                    <ItemTemplate>
                        <asp:Label runat="server" ID="PointsAssists" Text='<%# DataBinder.Eval(Container, "DataItem.PointsAssists") %>'>
                        </asp:Label>
                    </ItemTemplate>
                </asp:TemplateColumn>
            </Columns>
            <Columns>
                <asp:TemplateColumn HeaderText="Pontos e Rebotes">
                    <ItemTemplate>
                        <asp:Label runat="server" ID="PointsRebounds" Text='<%# DataBinder.Eval(Container, "DataItem.PointsRebounds") %>'>
                        </asp:Label>
                    </ItemTemplate>
                </asp:TemplateColumn>
            </Columns>
            <Columns>
                <asp:TemplateColumn HeaderText="Assistências e Rebotes">
                    <ItemTemplate>
                        <asp:Label runat="server" ID="AssistsRebounds" Text='<%# DataBinder.Eval(Container, "DataItem.AssistsRebounds") %>'>
                        </asp:Label>
                    </ItemTemplate>
                </asp:TemplateColumn>
            </Columns>
            <Columns>
                <asp:TemplateColumn HeaderText="Roubos e Bloqueios">
                    <ItemTemplate>
                        <asp:Label runat="server" ID="StealsBlocks" Text='<%# DataBinder.Eval(Container, "DataItem.StealsBlocks") %>'>
                        </asp:Label>
                    </ItemTemplate>
                </asp:TemplateColumn>
            </Columns>
        </asp:DataGrid>
    </div>

    <div>
        <asp:DataGrid ID="dgAverages" runat="server" OnItemDataBound="dgAverages_ItemDataBound" AutoGenerateColumns="false">
            <Columns>
                <asp:TemplateColumn HeaderText="Cestas de 3">
                    <ItemTemplate>
                        <asp:Label runat="server" ID="AvgTreePointFieldGoalsMade" Text='<%# DataBinder.Eval(Container, "DataItem.AvgTreePointFieldGoalsMade") %>'>
                        </asp:Label>
                    </ItemTemplate>
                </asp:TemplateColumn>
            </Columns>
            <Columns>
                <asp:TemplateColumn HeaderText="Rebotes">
                    <ItemTemplate>
                        <asp:Label runat="server" ID="AvgRebounds" Text='<%# DataBinder.Eval(Container, "DataItem.AvgRebounds") %>'>
                        </asp:Label>
                    </ItemTemplate>
                </asp:TemplateColumn>
            </Columns>
            <Columns>
                <asp:TemplateColumn HeaderText="Assistências">
                    <ItemTemplate>
                        <asp:Label runat="server" ID="AvgAssists" Text='<%# DataBinder.Eval(Container, "DataItem.AvgAssists") %>'>
                        </asp:Label>
                    </ItemTemplate>
                </asp:TemplateColumn>
            </Columns>
            <Columns>
                <asp:TemplateColumn HeaderText="Bloqueios">
                    <ItemTemplate>
                        <asp:Label runat="server" ID="AvgBlocks" Text='<%# DataBinder.Eval(Container, "DataItem.AvgBlocks") %>'>
                        </asp:Label>
                    </ItemTemplate>
                </asp:TemplateColumn>
            </Columns>
            <Columns>
                <asp:TemplateColumn HeaderText="Roubos">
                    <ItemTemplate>
                        <asp:Label runat="server" ID="AvgSteals" Text='<%# DataBinder.Eval(Container, "DataItem.AvgSteals") %>'>
                        </asp:Label>
                    </ItemTemplate>
                </asp:TemplateColumn>
            </Columns>
            <Columns>
                <asp:TemplateColumn HeaderText="Inversões de Posse">
                    <ItemTemplate>
                        <asp:Label runat="server" ID="AvgTurnovers" Text='<%# DataBinder.Eval(Container, "DataItem.AvgTurnovers") %>'>
                        </asp:Label>
                    </ItemTemplate>
                </asp:TemplateColumn>
            </Columns>
            <Columns>
                <asp:TemplateColumn HeaderText="Pontos">
                    <ItemTemplate>
                        <asp:Label runat="server" ID="AvgPoints" Text='<%# DataBinder.Eval(Container, "DataItem.AvgPoints") %>'>
                        </asp:Label>
                    </ItemTemplate>
                </asp:TemplateColumn>
            </Columns>
            <Columns>
                <asp:TemplateColumn HeaderText="Pontos, Assistências e Rebotes">
                    <ItemTemplate>
                        <asp:Label runat="server" ID="AvgPointsAssistsRebounds" Text='<%# DataBinder.Eval(Container, "DataItem.AvgPointsAssistsRebounds") %>'>
                        </asp:Label>
                    </ItemTemplate>
                </asp:TemplateColumn>
            </Columns>
            <Columns>
                <asp:TemplateColumn HeaderText="Pontos e Assistências">
                    <ItemTemplate>
                        <asp:Label runat="server" ID="AvgPointsAssists" Text='<%# DataBinder.Eval(Container, "DataItem.AvgPointsAssists") %>'>
                        </asp:Label>
                    </ItemTemplate>
                </asp:TemplateColumn>
            </Columns>
            <Columns>
                <asp:TemplateColumn HeaderText="Pontos e Rebotes">
                    <ItemTemplate>
                        <asp:Label runat="server" ID="AvgPointsRebounds" Text='<%# DataBinder.Eval(Container, "DataItem.AvgPointsRebounds") %>'>
                        </asp:Label>
                    </ItemTemplate>
                </asp:TemplateColumn>
            </Columns>
            <Columns>
                <asp:TemplateColumn HeaderText="Assistências e Rebotes">
                    <ItemTemplate>
                        <asp:Label runat="server" ID="AvgAssistsRebounds" Text='<%# DataBinder.Eval(Container, "DataItem.AvgAssistsRebounds") %>'>
                        </asp:Label>
                    </ItemTemplate>
                </asp:TemplateColumn>
            </Columns>
            <Columns>
                <asp:TemplateColumn HeaderText="Roubos e Bloqueios">
                    <ItemTemplate>
                        <asp:Label runat="server" ID="AvgStealsBlocks" Text='<%# DataBinder.Eval(Container, "DataItem.AvgStealsBlocks") %>'>
                        </asp:Label>
                    </ItemTemplate>
                </asp:TemplateColumn>
            </Columns>
        </asp:DataGrid>
    </div>

    <div>
        <asp:Label runat="server" ID="lblDoubleDouble" Text="Possible Double-Double" Visible="false" />
        <asp:Label runat="server" ID="lblTripleDouble" Text="Possible Triple-Double" Visible="false" />
    </div>
</asp:Content>
