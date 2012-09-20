<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Unsubscribe.aspx.cs" Inherits="BWA.Knowledgebase.Unsubscribe" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Unsubscribe</title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <table width="100%" border="0" style="font-family: Arial; font-size: small">
            <tr>
                <td>
                    <div style="padding: 20px 0px 0px 20px">
                        <asp:Label runat="server" ID="Message" Font-Bold="true" />
                        <br />
                        <br />
                        <asp:Literal runat="server" ID="CanCloseLiteral" Text="You can close this browser window now." />
                    </div>
                </td>
            </tr>
        </table>
    </div>
    </form>
</body>
</html>
