<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPages/GeneralUser.Master" AutoEventWireup="true" CodeBehind="Confirm.aspx.cs" Inherits="QuestionnaireSystem.GeneralUserPages.Confirm" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <table width="700">
        <tr>
            <td align="right">
                <asp:Label ID="lblMsg" runat="server" Text="投票中" Style="font-size: 20px"></asp:Label><br />
                <asp:Label ID="lblDuring" runat="server"></asp:Label>
            </td>
        </tr>
        <tr>
            <td align="center">
                <asp:Label ID="lblCaption" runat="server" Text="問卷名稱" Style="font-size: 30px; font-weight: bold"></asp:Label><br /><br />
                <asp:Label ID="lblDescription" runat="server" Text="描述內容"></asp:Label><br /><br />
            </td>
        </tr>
        <tr>
            <td>
                <asp:Label ID="lblName" runat="server" Text="姓名"></asp:Label>&nbsp &nbsp &nbsp
                <asp:Label ID="lblNameValue" runat="server" Text="xxx"></asp:Label><br /><br />

                <asp:Label ID="lblPhone" runat="server" Text="手機"></asp:Label>&nbsp &nbsp &nbsp
                <asp:Label ID="lblPhoneValue" runat="server" Text="xxxx-xxx-xxx"></asp:Label><br /><br />

                <asp:Label ID="lblEmail" runat="server" Text="Email"></asp:Label>&nbsp &nbsp
                <asp:Label ID="lblEmailValue" runat="server" Text="xxxxxxx@xxxxx.xxx"></asp:Label><br /><br />

                <asp:Label ID="lblAge" runat="server" Text="年齡"></asp:Label>&nbsp &nbsp &nbsp
                <asp:Label ID="lblAgeValue" runat="server" Text="xx"></asp:Label><br /><br />
            </td>
        </tr>
    </table>
    <br /><br />

    <asp:Literal ID="ltlReply" runat="server"></asp:Literal>

    <br />
    <table width="100%">
        <tr>
            <td align="right">
                <br /><br />
                <asp:Button ID="btnEditC" CssClass="btn btn-outline-dark" runat="server" Text="修改" OnClick="btnEditC_Click"/>
                &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp 
                <asp:Button ID="btnSendC" CssClass="btn btn-success" runat="server" Text="送出" OnClick="btnSendC_Click"/>
            </td>
        </tr>
    </table>

    <br /><br /><br />

</asp:Content>
