<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPages/SystemAdmin.Master" AutoEventWireup="true" CodeBehind="Common.aspx.cs" Inherits="QuestionnaireSystem.SystemAdminPages.Common" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <link rel="stylesheet" href="//code.jquery.com/ui/1.13.0/themes/base/jquery-ui.css">
    <script src="https://code.jquery.com/ui/1.13.0/jquery-ui.js"></script>



    <script>
        $(function () {
            $("#tabs").tabs();
        });
    </script>
    
    <div id="tabs">
        <ul>
            <li><a href="#tabs1">常用問題</a></li>
        </ul>

        <div id="tabs1">
            <table width="500px"></table>
            <asp:Label ID="lblSelectionType" runat="server" Text="名稱"></asp:Label> &nbsp
            <asp:TextBox ID="txtName" runat="server" style="width:200px"></asp:TextBox>

            <br /><br />

            <asp:Label ID="lblText" runat="server" Text="問題"></asp:Label> &nbsp
            <asp:TextBox ID="txtQuestion" runat="server" style="width:200px"></asp:TextBox> &nbsp

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
            <asp:Button ID="btnAdd" runat="server" Text="加入" OnClick="btnAdd_Click"/>
            <br /><br />

            <asp:ImageButton ID="btnDelete" runat="server" ImageUrl="../Images/trash.png" width="30" height="30" OnClick="btnDelete_Click" /> 

            <asp:Literal ID="ltlMsg" runat="server"></asp:Literal>
            <asp:GridView class="table table-condensed" ID="gvComm" runat="server" AutoGenerateColumns="False" OnRowDataBound="gvComm_RowDataBound" OnRowCancelingEdit="gvComm_RowCancelingEdit" OnRowCommand="gvComm_RowCommand" OnRowDeleting="gvComm_RowDeleting">
                <Columns>

                    <asp:TemplateField>
                        <ItemTemplate>
                            <asp:CheckBox ID="ckbDelete" runat="server" />
                        </ItemTemplate>
                    </asp:TemplateField>

                    <asp:BoundField DataField="Count" HeaderText="#" />

                    <asp:BoundField DataField="Text" HeaderText="問題" />

                    <asp:TemplateField HeaderText="種類">
                            <ItemTemplate>
                                <asp:Label ID="lblSelectionType" runat="server"></asp:Label>
                            </ItemTemplate>
                    </asp:TemplateField>

                    <asp:CheckBoxField DataField="IsMust" HeaderText="必填" />

                    <asp:TemplateField>
                        <ItemTemplate>
                            <asp:Button ID="btnEdit" runat="server" Text="編輯" CommandName="CommEdit" CommandArgument='<%# Eval("CommID") %>'/>
                        </ItemTemplate>
                    </asp:TemplateField>

                </Columns>
            </asp:GridView>

            <table width="100%">
                <tr>
                    <td align="right">
                        <asp:Button ID="btnCancelP" CssClass="btn btn-outline-dark" runat="server" Text="取消" OnClick="btnCancelP_Click" />
                        &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp 
                        <asp:Button ID="btnSendP" CssClass="btn btn-success" runat="server" Text="送出" OnClick="btnSendP_Click" />
                    </td>
                </tr>
            </table>
        </div>
    </div>

    <br /><br /><br />

</asp:Content>
