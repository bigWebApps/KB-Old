<%@ Control Language="C#" AutoEventWireup="true" CodeFile="SearchArticleControl.ascx.cs"
    Inherits="BWA.Knowledgebase.SearchArticleControl" %>
<table border="0" cellpadding="2" cellspacing="0" id="tableSearch" runat="server"
    width="700px">
    <tr align="center">
        <td style="width: 99%; padding-right: 7px;" class="textbox">
            <mits:TextBox Required="false" ID="TextBoxSearch" runat="server" MaxLength="255" Width="100%" meta:resourcekey="TextBoxSearchResource1" />
        </td>
        <td style="width: 1%; padding-top: 7px" valign="top">
            <asp:Button ID="ButtonSearch" runat="server" Text=" Search " meta:resourcekey="ButtonSearchResource1"
                Font-Size="11pt" OnClick="ButtonSearch_Click" />
        </td>
        <td style="width: 1%; padding-top: 7px" valign="top">
            <asp:Button Text=" Bing Search " Font-Size="11pt" ID="BingSearch" runat="server"
                OnClick="BingSearch_Click" />
        </td>
        <td style="width: 1%; padding-top: 7px" valign="top">
            <asp:Button Text=" Google Search " Font-Size="11pt" ID="GoogleSearch" runat="server"
                OnClick="GoogleSearch_Click" />
        </td>
        <%--        <td align="center"  style="width:1%">
            &nbsp;<asp:Label ID="LabelOR" runat="server" Text="OR" 
                meta:resourcekey="LabelORResource1"></asp:Label>&nbsp;
        </td>
        <td style="width:1%">
            <asp:Button ID="ButtonBrowse" runat="server" Text=" Browse " 
                meta:resourcekey="ButtonBrowseResource1" />
        </td>
--%>
    </tr>
</table>

<script language="javascript" type="text/javascript">
    function preventEnter(e) {
        var k = (e.keyCode ? e.keyCode : (e.which ? e.which : null));
        if (k == 13)
            <%= this.Page.ClientScript.GetPostBackEventReference(this.ButtonSearch, null)%>
    }
</script>

