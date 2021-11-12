<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPages/GeneralUser.Master" AutoEventWireup="true" CodeBehind="GStatic.aspx.cs" Inherits="QuestionnaireSystem.GeneralUserPages.GStatic" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <asp:Literal ID="ltlCaption" runat="server"></asp:Literal>
    <asp:Literal ID="ltlDescription" runat="server"></asp:Literal>
    <br /><br /><br />

    <asp:PlaceHolder ID="PlaceHolder1" runat="server"></asp:PlaceHolder>
    <br /><br /><br />

</asp:Content>
