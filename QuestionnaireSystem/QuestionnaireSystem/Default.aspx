<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPages/FrontMaster.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="QuestionnaireSystem.Default" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <br /><br />

    <table>
        <asp:Label ID="lblMarquee" runat="server" Style="font-size: 30px"></asp:Label>
    </table>
        
    <br /><br /><br />

    <asp:Button CssClass="btn btn-secondary btn-lg" ID="BtnGeneral" runat="server" Text="進入前台 - 問卷填寫" OnClick="BtnGeneral_Click" /><br /><br /><br /><br /><br />

    <asp:Button CssClass="btn btn-secondary btn-lg" ID="BtnSystem" runat="server" Text="登入後台 - 問卷管理" OnClick="BtnSystem_Click" />

    <br /><br /><br /><br /><br /><br /><br />

</asp:Content>
