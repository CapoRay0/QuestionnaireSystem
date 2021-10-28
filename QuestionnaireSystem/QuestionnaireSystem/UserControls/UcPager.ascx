<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="UcPager.ascx.cs" Inherits="QuestionnaireSystem.UserControls.UcPager" %>

<div align="center">
    <asp:Literal ID="ltpager" runat="server"></asp:Literal>
    <a runat="server" id="aLinkFirst" href="#"><<</a>
    <a runat="server" id="aLink1" href="#">1</a>
    <a runat="server" id="aLink2" href="#">2</a>
    <asp:Literal runat="server" ID="ltlCurrentPage"></asp:Literal>
    <a runat="server" id="aLink4" href="#">4</a>
    <a runat="server" id="aLink5" href="#">5</a>
    <a runat="server" id="aLinkLast" href="#">>></a>
</div>