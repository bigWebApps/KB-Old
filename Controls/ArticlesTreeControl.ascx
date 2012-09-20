<%@ Control Language="C#" AutoEventWireup="true" CodeFile="ArticlesTreeControl.ascx.cs"
    Inherits="BWA.Knowledgebase.ArticlesTreeControl" %>
<%--<div id="divRelatedArticles" runat="server" class="pageInfo">--%>
<div class="pageInfo">
    <table border="0" cellpadding="0" cellspacing="0">
        <tr>
            <td>
                <p class="SubHeader">
                    <asp:Label runat="server" ID="TitleLabel" meta:resourcekey="TitleLabel" Text="Related Articles" />
                </p>
            </td>
            <td>
                <asp:LinkButton ID="LinkButtonShowInactive" runat="server" Text="Show Inactive" 
                    OnClick="LinkButtonShowInactive_Click" Visible="false"
                    meta:resourcekey="LinkButtonShowInactiveResource1"></asp:LinkButton>
            </td>
        </tr>
        <tr>
            <td colspan="2">
                <asp:DataList ID="DataListArticles" runat="server" ShowHeader="False" CellPadding="1"
                    Width="500px" meta:resourcekey="DataListArticlesResource1">
                    <ItemTemplate>
                        <table border="0" cellpadding="1" cellspacing="1">
                            <tr>
                                <td width='<%# Convert.ToInt32(Eval("Level"))*30 %>px' align="right">
                                    &nbsp;
                                </td>
                                <td style="white-space: nowrap; font-weight: bold;" valign="top">
                                    <%# (bool)Eval("Deleted") ? "<img alt='X' width='16' height='16' src='Images/X.png' />" : string.Empty %> <a style="vertical-align:top" href='<%# this.IsAdmin ? ("ArticleViewAdmin.aspx?id=" + ((Guid)Eval("ArticleGuid")).ToString("N")) : ("Default.aspx?i=" + ((Guid)Eval("DepartmentGuid")).ToString("N") + "&t=" + ((Guid)Eval("ArticleGuid")).ToString("N")) %>'>
                                        <%# Eval("Subject") %></a>
                                </td>
                            </tr>
                        </table>
                    </ItemTemplate>
                </asp:DataList>
            </td>
        </tr>
    </table>
</div>
