<%@ Page Language="C#" Title="View Comments" AutoEventWireup="true" CodeFile="Comments.aspx.cs"
    Inherits="BWA.Knowledgebase.Comments" MasterPageFile="~/MasterPage.master"
    meta:resourcekey="PageResource1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="PageBody" runat="Server">
    <asp:ObjectDataSource ID="ObjectDataSourceUnreadComments" runat="server" OldValuesParameterFormatString="original_{0}"
        OnSelecting="ObjectDataSourceUnreadComments_Selecting" SelectMethod="GetData"
        TypeName="MainDataSetTableAdapters.UnreadCommentsTableAdapter">
        <SelectParameters>
            <asp:Parameter DbType="Guid" Name="DepartmentGuid" />
        </SelectParameters>
    </asp:ObjectDataSource>
    <mits:CommonGridView ID="GridUnreadComments" runat="server" AllowPaging="True" AutoGenerateColumns="False"
        ColorScheme="TanGray" DataKeyNames="ArticleGuid" DataSourceID="ObjectDataSourceUnreadComments"
        Width="60%" meta:resourcekey="GridUnreadCommentsResource1" PageSize="100">
        <Columns>
            <asp:HyperLinkField DataNavigateUrlFields="ArticleGuid" DataNavigateUrlFormatString="~/ArticleViewAdmin.aspx?id={0:N}&UC=#comments"
                DataTextField="Subject" SortExpression="Subject" HeaderText="Subject" >
            </asp:HyperLinkField>
            <asp:BoundField DataField="UnreadCount" HeaderText="Unread" ReadOnly="True" SortExpression="UnreadCount"
                meta:resourcekey="BoundFieldResource2">
                <ItemStyle HorizontalAlign="Center" Font-Bold="true" Font-Size="Small" />
            </asp:BoundField>
            <asp:BoundField DataField="CommentsCount" HeaderText="Comments" ReadOnly="True" SortExpression="CommentsCount">
                <ItemStyle HorizontalAlign="Center" />
            </asp:BoundField>
            <asp:BoundField DataField="LastPost" HeaderText="Last Post" HtmlEncode="false" ReadOnly="True" SortExpression="LastPost">
                <ItemStyle HorizontalAlign="Center" />
            </asp:BoundField>
        </Columns>
    </mits:CommonGridView>
</asp:Content>
