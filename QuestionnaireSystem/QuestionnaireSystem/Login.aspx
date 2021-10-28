<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPages/FrontMaster.Master" AutoEventWireup="true" CodeBehind="Login.aspx.cs" Inherits="QuestionnaireSystem.Login" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <script>
        document.body.oncopy = function () {event.returnValue = false}
    </script>

    <br /><br />
    <h1>問卷系統後台管理</h1>
    <br /><br />
    <div>
        <asp:Label CssClass="col-form-label-lg" ID="lblAccount" runat="server" Text="帳號："></asp:Label><br />
        <asp:TextBox CssClass="form-control" ID="txtAccount" runat="server" placeholder="在此輸入您的帳號"></asp:TextBox><br />
        <asp:Label CssClass="col-form-label-lg" ID="lblPWD" runat="server" Text="密碼："></asp:Label><br />
        <asp:TextBox CssClass="form-control" ID="txtPWD" runat="server" TextMode="Password" placeholder="在此輸入您的密碼"></asp:TextBox><br />
        <asp:Label CssClass="col-form-label-lg align-bottom" ID="lblVerification" runat="server" Text="驗證碼："></asp:Label>
        <img class="m-1" src="DrawingVerification.ashx" /><br />
        <asp:TextBox CssClass="form-control" ID="txtConfirmCode" runat="server" placeholder="在此輸入驗證碼"></asp:TextBox><br />
    </div>

    <div>
        <asp:Button CssClass="btn btn-outline-success btn-lg" ID="btnLogin" runat="server" Text="   登入系統   " OnClick="btnLogin_Click"/> &nbsp &nbsp &nbsp
        <asp:Button CssClass="btn btn-outline-danger" ID="btnForgetPWD" runat="server" Text="忘記密碼" OnClick="btnForgetPWD_Click"/> &nbsp &nbsp
        <asp:Label CssClass="col-form-label-lg" ID="lblMsg" runat="server" Text=""></asp:Label>
    </div>
    <br />
    <asp:Button CssClass="btn btn-outline-success" ID="btnDefault" runat="server" Text="返回預設頁" OnClick="btnDefault_Click"/> &nbsp &nbsp

</asp:Content>
