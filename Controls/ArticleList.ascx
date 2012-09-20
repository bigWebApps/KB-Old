<%@ Control Language="C#" AutoEventWireup="true" CodeFile="ArticleList.ascx.cs" Inherits="BWA.Knowledgebase.ArticleListControl" %>
<div id="related">
    <asp:Repeater ID="DataListArticles" runat="server">
        <HeaderTemplate><ol></HeaderTemplate>
        <FooterTemplate></ol></FooterTemplate>
        <ItemTemplate><li><a href='<%# "Default.aspx?i=" + ((Guid)Eval("DepartmentGuid")).ToString("N") + "&t=" + ((Guid)Eval("ArticleGuid")).ToString("N") %>'><em><%# Eval("Subject") %></em></a>
                <span style="display: block;"><%#  Regex.Replace((string)Eval("Body"), @"<(.|\n)*?>", "")%></span> </li>
        </ItemTemplate>
    </asp:Repeater>
</div>
