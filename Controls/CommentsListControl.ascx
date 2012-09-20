<%@ Control Language="C#" AutoEventWireup="true" CodeFile="CommentsListControl.ascx.cs"
    Inherits="BWA.Knowledgebase.CommentsListControl" %>
<div class="pageInfo">
    <%--id="divCommentsList" runat="server" --%>
    <p class="SubHeader">
        <a name='comments'></a>
        <asp:Label ID="LabelCommentCount" runat="server" Text="{0} comment(s) so far" meta:resourcekey="LabelCommentCountResource1" />
    </p>
    <asp:DataList ID="DataListComments" runat="server" ShowHeader="False" CellPadding="0"
        RepeatLayout="Flow" Width="500px" DataSourceID="ObjectDataSourceComments" meta:resourcekey="DataListCommentsResource1"
        OnDeleteCommand="DataListComments_DeleteCommand">
        <ItemTemplate>
            <table border="0" cellpadding="0" cellspacing="0">
                <tr>
                    <td rowspan="2" valign="top" style="padding-right: 5px;">
                        <asp:Button ID="ButtonDeleteComment" runat="server" Text="Delete" CommandArgument='<%# Eval("CommentId") %>'
                            CommandName="Delete" Visible='<%# this.IsPublic %>' OnClientClick="return confirm('Do you sure to delete this comment?');" />
                    </td>
                    <td>
                        <span class="author" style="padding-right: 10px"><a name='<%# Eval("CommentId") %>'></a>
                            <%# Eval("UserName") %>
                        </span><span class="date" style="padding-right: 10px">
                            <%# this.IsPublic ? (((string)Eval("UserEmail")) == string.Empty) ? string.Empty : string.Format("<a href='mailto:{0}' rel='nofollow'>{1}</a>", (string)Eval("UserEmail"), (string)Eval("UserEmail")) : string.Empty %>
                        </span>
                        <span class="emailstatus">                        
                            <%# Eval("EmailStatus").ToString() == "2" ? "Global Do Not Email List" : Eval("EmailStatus").ToString() == "1" ? "Not Receiving Article Emails" : String.Empty%>
                        </span>
                        <%--<span class="date"><%# ((DateTime)Eval("CreatedTime")).ToString("d", System.Globalization.CultureInfo.CurrentCulture) %></span>--%>
                    </td>
                </tr>
                <tr>
                    <td>
                        <div style="display: block; font-size: 14px; padding-top: 5px">
                            <%# ((string)Eval("Body")).Replace("\r\n", "<br>") %>
                        </div>
                    </td>
                </tr>
            </table>
        </ItemTemplate>
    </asp:DataList>
</div>
<asp:ObjectDataSource ID="ObjectDataSourceComments" runat="server" OldValuesParameterFormatString="original_{0}"
    OnSelecting="ObjectDataSourceComments_Selecting" SelectMethod="GetDataByArticleGuid"
    EnableCaching="false" TypeName="MainDataSetTableAdapters.CommentTableAdapter"
    OnSelected="ObjectDataSourceComments_Selected">
    <SelectParameters>
        <asp:Parameter DbType="Guid" Name="ArticleGuid" />
    </SelectParameters>
</asp:ObjectDataSource>
