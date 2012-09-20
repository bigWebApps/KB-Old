<%@ Control Language="C#" AutoEventWireup="true" CodeFile="FileListControl.ascx.cs"
    Inherits="BWA.Knowledgebase.FileListControl" %>
<div class="pageInfo">
    <table border="0" cellpadding="0" cellspacing="0">
        <tr>
            <td>
                <p class="SubHeader">
                    <asp:Label runat="server" ID="TitleLabel" meta:resourcekey="TitleLabel" Text="Attachments" />
                </p>
                <fs:FileList ID="FileList" runat="server" LocalObjectType="Article" RenderingMode="GridView" 
                    ShowIcons="true" EnableDeleting="true" EnableDeletingConfirmation="true" ShowViewAllAtOnceLink="false"
                    DateTimeHoursOffset="0" Visible="true" NegateFileExtensionsFilter="true" 
                    FileExtensionsFilter=".bmp,.gif,.jif,.jiff,.jng,.jpc,.jpe,.jpeg,.jpg,.tif,.tiff,.png,.swf" />
            </td>
        </tr>
    </table>
</div>
