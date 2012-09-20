<%@ Page Language="C#" AutoEventWireup="true" CodeFile="OrganizationHome.aspx.cs"
    Inherits="BWA.Knowledgebase.OrganizationHome" ValidateRequest="false" %>

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>Organization Home</title>
    <link href="StyleSheet.css" rel="Stylesheet" rev="Stylesheet" type="text/css" />
</head>
<body>
    <form id="form1" runat="server">
    <table width="100%" border="0" cellpadding="5">
        <tr>
            <td style="font-family: Arial; font-size: 14pt; font-weight: bold; text-align: center; padding-bottom: 20px">
                Select Instance
            </td>
        </tr>
        <asp:Repeater runat="server" ID="InstanceRepeater">
            <ItemTemplate>
                <tr style='visibility:<%# DataBinder.Eval(Container.DataItem, "InstanceId").ToString().Length == 0 ? "hidden" : "inherit" %>'>
                    <td style="font-family: Arial; font-size: 12pt; font-weight: bold; text-align: center">
                        <asp:HyperLink ForeColor="Black" runat="server" ID="InstanceLinkButton" Text='<%# DataBinder.Eval(Container.DataItem, "Name") %>'
                            NavigateUrl='<%# "http://"+DataBinder.Eval(Container.DataItem, "Description") %>' />
                    </td>
                </tr>
            </ItemTemplate>
        </asp:Repeater>
    </table>
    </form>
</body>
</html>
