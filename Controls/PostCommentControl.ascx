<%@ Control Language="C#" AutoEventWireup="true" CodeFile="PostCommentControl.ascx.cs"
    Inherits="BWA.Knowledgebase.PostCommentControl" %>
<div id="divMain" runat="server" class="pageInfo">
    <table border="0" cellpadding="2" cellspacing="2" id="tablePostComment" runat="server"
        style="margin-top: 10px" width="500px">
        <tr>
            <td style="width: 50%; white-space: nowrap">
                &nbsp;&nbsp;<asp:Label ID="LabelName" runat="server" AssociatedControlID="TextBoxName"
                    meta:resourcekey="LabelNameResource1" />
            </td>
            <td style="width: 50%; white-space: nowrap">
                &nbsp;&nbsp;<asp:Label ID="LabelEmail" runat="server" AssociatedControlID="TextBoxEmail"
                    meta:resourcekey="LabelEmailResource1" />
            </td>
        </tr>
        <tr>
            <td style="width: 50%" class="textbox" valign="top">
                <mits:TextBox ID="TextBoxName" Width="100%" runat="server" meta:resourcekey="TextBoxNameResource1"
                    Required="true" ValidationGroup="ValidationGroupPostComment" />
                <%--CssClass="width100"--%>
            </td>
            <td style="width: 50%" class="textbox" valign="top">
                <mits:TextBox ID="TextBoxEmail" runat="server" Width="100%" meta:resourcekey="TextBoxEmailResource1"
                    ValidationGroup="ValidationGroupPostComment" ValidationType="RegularExpression"
                    ValidationExpression="\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*" />
            </td>
        </tr>
        <tr class="Sm_PostComment">
            <td colspan="2">
                <mits:TextBox ID="TextBoxPhone" runat="server" meta:resourcekey="TextBoxPhoneResource1"
                    ValidationGroup="ValidationGroupPostComment" />
            </td>
        </tr>
        <tr>
            <td colspan="2" class="textbox">
                <mits:TextBox ID="TextBoxComment" runat="server" Width="100%" MaxLength="1000" Rows="6"
                    TextMode="MultiLine" Required="true" meta:resourcekey="TextBoxCommentResource1"
                    ValidationGroup="ValidationGroupPostComment" />
            </td>
        </tr>
        <tr>
            <td colspan="2" align="left">
                <asp:Button ID="ButtonPostComment" runat="server" meta:resourcekey="ButtonPostCommentResource1"
                    ValidationGroup="ValidationGroupPostComment" OnClick="ButtonPostComment_Click"
                    Font-Size="11pt" />
            </td>
        </tr>
    </table>
</div>
