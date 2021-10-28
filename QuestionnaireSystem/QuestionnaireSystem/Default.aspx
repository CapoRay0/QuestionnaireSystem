<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPages/FrontMaster.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="QuestionnaireSystem.Default" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <br /><br /><br />
    <div class="d-grid gap-2 d-md-flex justify-content-md-center">
        <asp:Label CssClass="col-form-label-lg" ID="Label1" runat="server" Text="Label"></asp:Label>
    </div>
    <br /><br /><br />
    <asp:Button CssClass="btn btn-outline-primary btn-lg" ID="BtnGeneral" runat="server" Text="進入前台 - 問卷填寫" OnClick="BtnGeneral_Click" /><br /><br /><br />

    <asp:Button CssClass="btn btn-outline-primary btn-lg" ID="BtnSystem" runat="server" Text="登入後台 - 問卷管理" OnClick="BtnSystem_Click" />

</asp:Content>
