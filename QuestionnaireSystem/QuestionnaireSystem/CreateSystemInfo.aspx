<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPages/FrontMaster.Master" AutoEventWireup="true" CodeBehind="CreateSystemInfo.aspx.cs" Inherits="QuestionnaireSystem.CreateSystemInfo" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <br />
    <div>
    <asp:Label CssClass="col-form-label-lg" ID="lblName" runat="server" Text="請輸入姓名"></asp:Label>
    <asp:TextBox CssClass="form-control" ID="txtName" runat="server" placeholder="您的姓名"></asp:TextBox><br />

    <asp:Label CssClass="col-form-label-lg" ID="lblPhone" runat="server" Text="請輸入手機號碼"></asp:Label>
    <asp:TextBox CssClass="form-control" ID="txtPhone" runat="server" placeholder="您的手機號碼" TextMode="Phone"></asp:TextBox><br />

    <asp:Label CssClass="col-form-label-lg" ID="lblEmail" runat="server" Text="">請輸入Email</asp:Label>
    <asp:TextBox CssClass="form-control" ID="txtEmail" runat="server" placeholder="您的Email" TextMode="Email"></asp:TextBox><br />

    <asp:Label CssClass="col-form-label-lg" ID="lblAccount" runat="server" Text="請輸入帳號"></asp:Label>
    <asp:TextBox CssClass="form-control" ID="txtAccount" runat="server" placeholder="帳號"></asp:TextBox><br />

    <asp:Label CssClass="col-form-label-lg" ID="lblPwd" runat="server" Text="請輸入密碼"></asp:Label>
    <asp:TextBox CssClass="form-control" ID="txtPwd" runat="server" placeholder="密碼" TextMode="Password"></asp:TextBox><br />

    <asp:Label CssClass="col-form-label-lg" ID="lblPwdConf" runat="server" Text="請再次輸入密碼"></asp:Label>
    <asp:TextBox CssClass="form-control" ID="txtPwdConf" runat="server" placeholder="確認密碼" TextMode="Password"></asp:TextBox><br />

    <div class="d-grid gap-2 d-md-flex justify-content-md-end">
    <asp:Label CssClass="col-form-label-lg" ID="lblMsg" runat="server" Text=""></asp:Label> &nbsp &nbsp
    <asp:Button class="btn btn-outline-secondary btn-lg" ID="btnBackToLogin" runat="server" Text="返回" OnClick="btnBackToLogin_Click" />
    <asp:Button CssClass="btn btn-outline-success btn-lg" ID="btnConfirm" runat="server" Text="確認送出" OnClick="btnConfirm_Click" /></div>
    </div>
    <br /><br />
</asp:Content>
