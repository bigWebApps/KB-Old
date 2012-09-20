<%@ Page Title="Article Requests List" Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true"
    CodeFile="RequestList.aspx.cs" Inherits="BWA.Knowledgebase.RequestList" meta:resourcekey="PageResource1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="PageBody" runat="Server">
    <mits:CommonGridView ID="GridRequestList" runat="server" AllowPaging="True" AutoGenerateColumns="False"
        DataKeyNames="ArticleGuid" DataSourceID="ObjectDataSourceRequest" 
        meta:resourcekey="GridRequestListResource1" ColorScheme="TanGray">
        <Columns>
            <asp:TemplateField HeaderText="#" SortExpression="ArticleID" 
                meta:resourcekey="TemplateFieldResource1">
                <ItemTemplate>
                    <asp:HyperLink ID="HyperLink1" runat="server" Text='<%# Eval("ArticleID") %>' 
                        NavigateUrl='<%# "~/ArticleViewAdmin.aspx?id=" + ((Guid)Eval("ArticleGuid")).ToString("N") %>' 
                        meta:resourcekey="HyperLink1Resource1" />
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Subject" SortExpression="Subject" 
                meta:resourcekey="TemplateFieldResource2">
                <ItemTemplate>
                    <asp:Label ID="Label2" runat="server" Text='<%# Eval("Subject") %>' 
                        meta:resourcekey="Label2Resource1"></asp:Label>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Alternate #" SortExpression="AlternateId" 
                meta:resourcekey="TemplateFieldResource3">
                <ItemTemplate>
                    <asp:Label ID="Label2" runat="server" Text='<%# Eval("AlternateId") %>' 
                        meta:resourcekey="Label2Resource2"></asp:Label>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Applicant" SortExpression="UserName" 
                meta:resourcekey="TemplateFieldResource4">
                <ItemTemplate>
                    <asp:Label ID="Label2" runat="server" Text='<%# Eval("UserName") %>' 
                        meta:resourcekey="Label2Resource3"></asp:Label>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Date" SortExpression="CreatedTime" 
                meta:resourcekey="TemplateFieldResource5">
                <ItemTemplate>
                    <asp:Label ID="Label2" runat="server" Text='<%# Eval("CreatedTime") %>' 
                        meta:resourcekey="Label2Resource4"></asp:Label>
                </ItemTemplate>
            </asp:TemplateField>
        </Columns>
        <PagerStyle BackColor="#666666" ForeColor="White" HorizontalAlign="Center" />
    </mits:CommonGridView>
    <asp:ObjectDataSource ID="ObjectDataSourceRequest" runat="server" OldValuesParameterFormatString="original_{0}"
        SelectMethod="GetData" TypeName="MainDataSetTableAdapters.RequestListTableAdapter"
        OnSelecting="ObjectDataSourceRequest_Selecting">
        <SelectParameters>
            <asp:Parameter DbType="Guid" Name="DepartmentGuid" />
            <asp:Parameter Name="Type" Type="String" />
        </SelectParameters>
    </asp:ObjectDataSource>
</asp:Content>
