<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPages/GeneralUser.Master" AutoEventWireup="true" CodeBehind="GList.aspx.cs" Inherits="QuestionnaireSystem.GeneralUserPages.GList" %>

<%@ Register Src="~/UserControls/UcPager.ascx" TagPrefix="uc1" TagName="UcPager" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <table cellpadding="10" border="1" width="600">
        <tr>
            <td>
                <asp:Label ID="lblTitle" runat="server" Text="問卷標題"></asp:Label> &nbsp &nbsp
                <asp:TextBox ID="txtTitle" runat="server" style="width:350px"></asp:TextBox>
            </td>
        </tr>
        <tr>
            <td>
                <asp:Label ID="lblDate" runat="server" Text="開始 / 結束"></asp:Label> &nbsp &nbsp
                <asp:TextBox ID="txtDateStart" runat="server" TextMode="Date"></asp:TextBox>
                <asp:TextBox ID="txtDateEnd" runat="server" TextMode="Date"></asp:TextBox> &nbsp &nbsp
                <asp:Button ID="btnSearch" runat="server" Text="搜尋" OnClick="btnSearch_Click" />
            </td>
        </tr>
    </table>
    <br /><br />
    
    <asp:Literal ID="ltlMsg" runat="server"></asp:Literal>
    <asp:GridView class="table table-condensed" ID="gvGList" runat="server" AutoGenerateColumns="False" OnRowDataBound="gvGList_RowDataBound">
        <Columns>
            <asp:BoundField DataField="QuesID" HeaderText="#" />
            <asp:TemplateField HeaderText="問卷">
                    <ItemTemplate>
                        <asp:Label ID="lblCaption" runat="server"></asp:Label>
                    </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="狀態">
                    <ItemTemplate>
                        <asp:Label ID="lblState" runat="server"></asp:Label>
                    </ItemTemplate>
            </asp:TemplateField>
            <asp:BoundField DataField="StartDate" HeaderText="開始時間" DataFormatString="{0:d}" />
            <asp:BoundField DataField="EndDate" HeaderText="結束時間" DataFormatString="{0:d}" />
            <asp:TemplateField HeaderText="觀看統計">
                <ItemTemplate>
                    <a href="GStastic.aspx?ID=<%# Eval("QuesID") %>">前往</a>
                </ItemTemplate>
            </asp:TemplateField>
        </Columns>
    </asp:GridView>

    <uc1:UcPager runat="server" ID="UcPager" PageSize="10" CurrentPage="1" Url="/GeneralUserPages/GList.aspx" />

    <br /><br /><br />

</asp:Content>
