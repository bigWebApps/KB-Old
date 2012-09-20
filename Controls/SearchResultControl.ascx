<%@ Control Language="C#" AutoEventWireup="true" CodeFile="SearchResultControl.ascx.cs"
    Inherits="BWA.Knowledgebase.SearchResultControl" %>
<%@ Register Src="~/Controls/SearchArticleControl.ascx" TagName="SearchArticleControl"
    TagPrefix="kbc" %>
<kbc:SearchArticleControl ID="SearchArticleCtrl" runat="server" />
<br />
<asp:ObjectDataSource ID="ObjectDataSourceSearch" runat="server" OldValuesParameterFormatString="original_{0}"
    OnSelecting="ObjectDataSourceSearch_Selecting" SelectMethod="GetData" TypeName="MainDataSetTableAdapters.SearchArticlesTableAdapter">
    <SelectParameters>
        <asp:Parameter DbType="Guid" Name="DepartmentGuid" />
        <asp:Parameter Name="SearchingText" Type="String" />
    </SelectParameters>
</asp:ObjectDataSource>
<mits:CommonGridView ID="GridSearchResult" runat="server" AllowPaging="True" AutoGenerateColumns="False"
    ColorScheme="TanGray" Width="90%" DataKeyNames="ArticleGuid"
    DataSourceID="ObjectDataSourceSearch"
    meta:resourcekey="GridSearchResultResource1">
    <Columns>
        <asp:HyperLinkField DataNavigateUrlFields="ArticleGuid" DataNavigateUrlFormatString="~/ArticleViewAdmin.aspx?id={0:N}"
            DataTextField="ArticleID" DataTextFormatString="KB{0}" HeaderText="Article#"
            SortExpression="ArticleID" meta:resourcekey="HyperLinkFieldResource1">
            <ItemStyle HorizontalAlign="Center" Width="100px" />
        </asp:HyperLinkField>
        <asp:BoundField DataField="Subject" HeaderText="Subject" 
            SortExpression="Subject" meta:resourcekey="BoundFieldResource1" />
    </Columns>
</mits:CommonGridView>
