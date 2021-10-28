﻿<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPages/SystemAdmin.Master" AutoEventWireup="true" CodeBehind="Detail.aspx.cs" Inherits="QuestionnaireSystem.SystemAdminPages.Detail" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
  <link rel="stylesheet" href="//code.jquery.com/ui/1.13.0/themes/base/jquery-ui.css">
  <link rel="stylesheet" href="/resources/demos/style.css">
  <script src="https://code.jquery.com/jquery-3.6.0.js"></script>
  <script src="https://code.jquery.com/ui/1.13.0/jquery-ui.js"></script>
    <script>
        $(function () {
            $("#tabs").tabs();
        });
    </script>

    <div id="tabs">
        <ul>
            <li><a href="#tabs1">問卷</a></li>
            <li><a href="#tabs2">問題</a></li>
            <li><a href="#tabs3">填寫資料</a></li>
            <li><a href="#tabs4">統計</a></li>
        </ul>

        <div id="tabs1">
            <table cellpadding="5">
                <tr>
                    <td><asp:Label ID="lblCaption" runat="server" Text="問卷名稱"></asp:Label></td>
                    <td><asp:TextBox ID="txtCaption" runat="server" style="width:350px"></asp:TextBox></td>
                </tr>
                <tr valign="top">
                    <td><asp:Label ID="lblDescription" runat="server" Text="描述內容"></asp:Label></td>
                    <td><asp:TextBox ID="txtDescription" runat="server" style="width:400px" TextMode="MultiLine" placeholder="描述內容為非必填"></asp:TextBox></td>
                </tr>
                <tr>
                    <td><asp:Label ID="lblStartDate" runat="server" Text="開始時間"></asp:Label></td>
                    <td><asp:TextBox ID="txtStartDate" runat="server" style="width:350px" TextMode="Date"></asp:TextBox></td>
                </tr>
                <tr>
                    <td><asp:Label ID="lblEndDate" runat="server" Text="結束時間"></asp:Label></td>
                    <td><asp:TextBox ID="txtEndDate" runat="server" style="width:350px" TextMode="Date"></asp:TextBox></td>
                </tr>
                <tr>
                    <td></td>
                    <td>
                        <asp:CheckBox ID="chkStatic" runat="server" />
                        <asp:Label ID="lblStatic" runat="server" Text="啟用"></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td colspan="2" align="right">
                        <asp:Button ID="btnCancel" runat="server" Text="取消" OnClick="btnCancel_Click" />
                        <asp:Button ID="btnSend" runat="server" Text="送出" OnClick="btnSend_Click" />
                    </td>
                </tr>
            </table>
        </div>

        <div id="tabs2">
            <asp:Label ID="ltlSelectionType" runat="server" Text="種類"></asp:Label> &nbsp
            <asp:DropDownList ID="ddlCommon" runat="server" style="width:150px"></asp:DropDownList>
            <br /><br />

            <asp:Label ID="lblText" runat="server" Text="問題"></asp:Label> &nbsp
            <asp:TextBox ID="txtText" runat="server" style="width:200px"></asp:TextBox> &nbsp
            <asp:DropDownList ID="ddlSelectionType" runat="server" style="width:100px">
                <asp:ListItem Value="0">單選方塊</asp:ListItem>
                <asp:ListItem Value="1">複選方塊</asp:ListItem>
                <asp:ListItem Value="2">文字</asp:ListItem>
                <asp:ListItem Value="3">文字(數字)</asp:ListItem>
                <asp:ListItem Value="4">文字(Email)</asp:ListItem>
                <asp:ListItem Value="5">文字(日期)</asp:ListItem>
            </asp:DropDownList> &nbsp
            <asp:CheckBox ID="ckbIsMust" runat="server" />
            <asp:Label ID="lblIsMust" runat="server" Text="必填"></asp:Label>
            <br /><br />
            <asp:Label ID="lblSelection" runat="server" Text="回答"></asp:Label> &nbsp
            <asp:TextBox ID="txtSelection" runat="server" style="width:200px"></asp:TextBox> &nbsp
            <asp:Label ID="lbltip" runat="server" Text="(多個答案以 ; 分隔)"></asp:Label> &nbsp
            <asp:Button ID="btnAddSelection" runat="server" Text="加入" OnClick="btnAddSelection_Click"/>
            <br /><br />

            <asp:ImageButton ID="btnDelete" runat="server" ImageUrl="../Images/trash.png" width="30" height="30" /> 

            <asp:Literal ID="ltlMsg" runat="server"></asp:Literal>
            <asp:GridView class="table table-condensed" ID="gvProb" runat="server" AutoGenerateColumns="False" OnRowDataBound="gvProb_RowDataBound" OnRowCancelingEdit="gvProb_RowCancelingEdit" OnRowCommand="gvProb_RowCommand" OnRowDeleting="gvProb_RowDeleting">
                <Columns>
                    <asp:TemplateField>
                        <ItemTemplate>
                            <asp:CheckBox ID="ckbDelete" runat="server" />
                        </ItemTemplate>
                    </asp:TemplateField>

                    <asp:TemplateField HeaderText="#">
                         <ItemTemplate>
                            <%#Container.DataItemIndex + 1%>
                         </ItemTemplate>
                    </asp:TemplateField>

                    <asp:BoundField DataField="Text" HeaderText="問題" />
                    <asp:TemplateField HeaderText="種類">
                            <ItemTemplate>
                                <asp:Label ID="lblSelectionType" runat="server"></asp:Label>
                            </ItemTemplate>
                    </asp:TemplateField>
                    <asp:CheckBoxField DataField="IsMust" HeaderText="必填" />
<%--                    <asp:TemplateField>
                        <ItemTemplate>
                            <asp:Button ID="btnEdit" runat="server" Text="編輯" CommandName="ProbEdit" CommandArgument='<%# Eval("ProbID") %>'/>
                        </ItemTemplate>
                    </asp:TemplateField>--%>
                </Columns>
            </asp:GridView>
            <%--<table>
                <tr>
                    <td align="right">
                        <asp:Button ID="btnCancelP" runat="server" Text="Button" />
                        <asp:Button ID="btnSendP" runat="server" Text="Button" />
                    </td>
                </tr>
            </table>--%>
        </div>

        <div id="tabs3">

        </div>

        <div id="tabs4">
            
        </div>


    </div>

</asp:Content>
