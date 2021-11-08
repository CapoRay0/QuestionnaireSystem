<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPages/SystemAdmin.Master" AutoEventWireup="true" CodeBehind="SList.aspx.cs" Inherits="QuestionnaireSystem.SystemAdminPages.SList" %>

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
    <br />

    <asp:ImageButton ID="btnDelete" runat="server" ImageUrl="../Images/trash.png" width="30" height="30" OnClick="btnDelete_Click" /> &nbsp
    <asp:ImageButton ID="btnNewForm" runat="server" ImageUrl="../Images/plus.png" width="30" height="30" OnClick="btnNewForm_Click" />

    <asp:Literal ID="ltlMsg" runat="server"></asp:Literal>
    <asp:GridView class="table table-condensed" ID="gvSList" AlternatingRowStyle-Wrap="false" runat="server" AutoGenerateColumns="False" OnRowDataBound="gvSlist_RowDataBound">
        <Columns>
            <asp:TemplateField>
                <ItemTemplate>
                    <asp:CheckBox ID="ckbDelete" runat="server" />
                </ItemTemplate>
            </asp:TemplateField>
            <asp:BoundField DataField="QuesID" HeaderText="#" />
            <asp:HyperLinkField DataNavigateUrlFields="QuesGuid" DataNavigateUrlFormatString="Detail.aspx?ID={0}" DataTextField="Caption" HeaderText="問卷" />
            <asp:TemplateField HeaderText="狀態">
                <ItemTemplate>
                    <asp:Label ID="lblState" runat="server"></asp:Label>
                </ItemTemplate>
            </asp:TemplateField>

            <asp:TemplateField HeaderText="開始時間">
                <ItemTemplate>
                    <asp:Label ID="lblStartDate" runat="server"></asp:Label>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="結束時間">
                <ItemTemplate>
                    <asp:Label ID="lblEndDate" runat="server"></asp:Label>
                </ItemTemplate>
            </asp:TemplateField>

            <asp:TemplateField HeaderText="觀看統計">
                <ItemTemplate>
                    <a href="SStastic.aspx?ID=<%# Eval("QuesGuid") %>">前往</a>
                </ItemTemplate>
            </asp:TemplateField>
        </Columns>
    </asp:GridView>

    <uc1:UcPager runat="server" id="UcPager" PageSize="10" CurrentPage="1" Url="/SystemAdminPages/SList.aspx" />

    <br /><br />

</asp:Content>
