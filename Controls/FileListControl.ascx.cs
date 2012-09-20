using System;
using System.Data;
using Micajah.Common.Security;
using Micajah.FileService.WebControls;

namespace BWA.Knowledgebase
{
    public enum AttachType
    {
        None = 0,
        Hidden,
        Visible
    }

    public partial class FileListControl : System.Web.UI.UserControl
    {
        #region Public Properties

        public bool EnableDeleting
        {
            get { return FileList.EnableDeleting; }
            set { FileList.EnableDeleting = value; }
        }

        public Guid ArticleGuid
        {
            get
            {
                Guid res = Guid.Empty;
                try { res = new Guid(FileList.LocalObjectId); }
                catch { }
                return res;
            }
            set
            {
                if (!value.Equals(Guid.Empty))
                    FileList.LocalObjectId = value.ToString("N");
            }
        }

        public string Title { get { return TitleLabel.Text; } set { TitleLabel.Text = value; } }

        public AttachType AttachedFileType
        {
            get
            {
                AttachType at = AttachType.None;
                if (FileList.NegateFileExtensionsFilter)
                    at = AttachType.Visible;
                else
                    at = AttachType.Hidden;
                return at;
            }
            set
            {
                if(value != AttachType.Visible)
                    TitleLabel.Text = value.ToString() + " Attachments";
                FileList.NegateFileExtensionsFilter = (value == AttachType.Visible);
            }
        }
        #endregion

        #region Handle Methods
        public override void DataBind()
        {
            if (FileList.LocalObjectId == null)
                FileList.LocalObjectId = string.Empty;
            FileList.OrganizationId = UserContext.Current.SelectedOrganization.OrganizationId;
            FileList.OrganizationName = UserContext.Current.SelectedOrganization.Name;
            FileList.DepartmentName = UserContext.Current.SelectedInstance.Name;
            FileList.DepartmentId = UserContext.Current.SelectedInstance.InstanceId;
            FileList.DataBind();
            this.Visible = FileList.FilesCount > 0;
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
                this.DataBind();
        }
        #endregion
    }
}