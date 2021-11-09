<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPages/GeneralUser.Master" AutoEventWireup="true" CodeBehind="Form.aspx.cs" Inherits="QuestionnaireSystem.GeneralUserPages.Form" %>

<%@ Register Src="~/UserControls/UcAddControl.ascx" TagPrefix="uc1" TagName="UcAddControl" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <%--<uc1:UcAddControl runat="server" id="UcAddControl" />--%>

    <script>
        // 通過JS獲取Request.QueryString() 參考網頁：https://www.itread01.com/article/1474950736.html
        function getArgs(strParame) {
            var args = new Object();
            var query = location.search.substring(1); // Get query string
            var pairs = query.split("&"); // Break at ampersand
            for (var i = 0; i < pairs.length; i++) {
                var pos = pairs[i].indexOf('='); // Look for "name=value"
                if (pos == -1) continue; // If not found, skip
                var argname = pairs[i].substring(0, pos); // Extract the name
                var value = pairs[i].substring(pos + 1); // Extract the value
                //var value = pairs[i].substring(pos + 1, pairs[i].length); // Extract the value
                value = decodeURIComponent(value); // Decode it, if needed
                args[argname] = value; // Store as a property
            }
            return args[strParame]; // Return the object
        }

        // 動態新增控制項：泛型處理常式 >> AJAX，透過 QuesGuid 取得問題資料表
        $(function () {

            // 獲取 Request.QueryString() 上的 QuesGuid
            var id = getArgs("ID");

            $.ajax({
                url: "/Handlers/ProblemHandler.ashx?QuesGuid=" + id,
                type: "GET",
                data: {},
                success: function (result) {
                    for (var i = 0; i < result.length; i++) {
                        var obj = result[i];
                        var htmlText = `<br /><p>${i + 1}. ${obj.Text}`; // 問卷題目

                        if (obj.IsMust) // 是否必填
                            htmlText += ` (必填)</p>`;
                        else
                            htmlText += `</p>`;

                        switch (obj.SelectionType) // 問題種類
                        {
                            case 0: // 單選方塊
                                var optionStr = obj.Selection.split(';'); // 字串分割
                                for (var j = 0; j < optionStr.length; j++) // 題目選項
                                    htmlText += `&nbsp&nbsp&nbsp&nbsp<input type="radio" id="${i}" name="${obj.ProbGuid}" value=${optionStr[j]} /> ${optionStr[j]}<br />`;
                                // 相同題目中選項的 name 必須相同
                                break;
                            case 1: // 複選方塊 (回答之間會自動以逗號分割)
                                var optionStr = obj.Selection.split(';');
                                for (var j = 0; j < optionStr.length; j++)
                                    htmlText += `&nbsp&nbsp&nbsp&nbsp<input type="checkbox" id="${optionStr[j]}" name="${obj.ProbGuid}" value=${optionStr[j]} /> ${optionStr[j]}<br />`;
                                // 將選項放入 id 以便做回填
                                break;
                            case 2: // 文字
                                htmlText += `&nbsp&nbsp&nbsp&nbsp<input type="text" id="${i}" name="${obj.ProbGuid}" /><br />`;
                                break;
                            case 3: // 文字(數字)
                                htmlText += `&nbsp&nbsp&nbsp&nbsp<input type="number" id="${i}" name="${obj.ProbGuid}" /><br />`;
                                break;
                            case 4: // 文字(Email)
                                htmlText += `&nbsp&nbsp&nbsp&nbsp<input type="email" id="${i}" name="${obj.ProbGuid}" /><br />`;
                                break;
                            case 5: // 文字(日期)
                                htmlText += `&nbsp&nbsp&nbsp&nbsp<input type="date" id="${i}" name="${obj.ProbGuid}" /><br />`;
                                break;
                        }
                        $("#Question").append(htmlText); // 將這一題新增進區塊內，並於下方 div 中顯示
                    }
                }
            });
        });

        // 從確認頁返回時將 Session["Reply"] 值回填：頁面全部跑完最後才執行此段 (ready)
        $(document).ready(function () {

            var id = getArgs("ID");
            var backReply = "<%=Session["Reply"]%>"; // 抓出 Session 中的回答內容
            if (backReply != "" && backReply != null) { // 有值才回填

                var BackReplys = backReply.split(";"); // 將 Session 中的答案字串做分割

                $.ajax({
                    url: "/Handlers/ProblemHandler.ashx?QuesGuid=" + id,
                    type: "GET",
                    data: {},
                    success: function (result) {
                        for (var i = 0; i < result.length; i++) {
                            var obj = result[i];
                            switch (obj.SelectionType) // 問題種類
                            {
                                // 參考網頁：https://www.itread01.com/p/1316819.html // 參考網頁2：https://cythilya.github.io/2017/09/10/jquery-attr-vs-prop/
                                
                                case 0: // 單選方塊
                                    if (BackReplys[i] != " ") // 有填答案
                                        $("input[name=" + obj.ProbGuid + "][value=" + BackReplys[i] + "]").prop("checked", true);
                                    // 透過 name 找出 radio 控制項，若 value 值符合則打勾 >> prop() 方法設置或返回被選元素的屬性和值。
                                    break;
                                case 1: // 複選方塊 (回答之間會自動以逗號分割)
                                    if (BackReplys[i] != " ") {
                                        var Options = BackReplys[i].split(','); // 複數答案分割

                                        for (var j = 0; j < Options.length; j++)
                                            $("#" + Options[j]).prop("checked", true);
                                        // 透過 id 找出 checkbox 控制項的選項 (動態新增時有先將答案選項放入 id )
                                    }
                                    break;
                                case 2: // 文字
                                case 3: // 文字(數字)
                                case 4: // 文字(Email)
                                case 5: // 文字(日期)
                                    $("input[name=" + obj.ProbGuid + "]").val(BackReplys[i]); // 透過 name 找到文字方塊並將內容填入
                                    break;
                            }
                        }
                    }
                });
            }
        });

    </script>

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
                <asp:Label ID="lblName" runat="server" Text="姓名"></asp:Label>&nbsp &nbsp &nbsp &nbsp &nbsp
                <asp:TextBox ID="txtName" runat="server" Style="width: 250px"></asp:TextBox><br /><br />

                <asp:Label ID="lblPhone" runat="server" Text="手機"></asp:Label>&nbsp &nbsp &nbsp &nbsp &nbsp
                <asp:TextBox ID="txtPhone" runat="server" Style="width: 250px" TextMode="Phone"></asp:TextBox><br /><br />

                <asp:Label ID="lblEmail" runat="server" Text="Email"></asp:Label>&nbsp &nbsp &nbsp &nbsp
                <asp:TextBox ID="txtEmail" runat="server" Style="width: 250px" TextMode="Email"></asp:TextBox><br /><br />

                <asp:Label ID="lblAge" runat="server" Text="年齡"></asp:Label>&nbsp &nbsp &nbsp &nbsp &nbsp
                <asp:TextBox ID="txtAge" runat="server" Style="width: 250px" TextMode="Number"></asp:TextBox><br /><br />
            </td>
        </tr>
    </table>

    <div id="Question"></div>

    <br />
    <table width="100%">
        <tr>
            <td align="right">
                <asp:Label ID="lblCount" runat="server" Text="共 0 個問題" Style="font-size: 20px"></asp:Label><br /><br /><br />
                <asp:Button ID="btnCancelF" CssClass="btn btn-outline-dark" runat="server" Text="取消" OnClick="btnCancelF_Click" />
                &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp 
                <asp:Button ID="btnSendF" CssClass="btn btn-success" runat="server" Text="送出" OnClick="btnSendF_Click" />
            </td>
        </tr>
    </table>

    <%--<asp:Literal ID="Literal1" runat="server"></asp:Literal>--%>

    <br /><br /><br />

</asp:Content>
