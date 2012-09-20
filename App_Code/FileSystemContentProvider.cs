using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Web;
using Micajah.FileService;
using Micajah.FileService.Client;
using Micajah.FileService.Client.Dal;
using Telerik.Web.UI.Widgets;

namespace Micajah.FileService.Providers
{
    public class DBContentProvider : FileBrowserContentProvider
    {
        private MainDataSetTableAdapters.ArticleTableAdapter _articleTableAdapter = null;

        private Micajah.FileService.Client.Dal.MetaDataSetTableAdapters.FileTableAdapter _fileTableAdapter = null;

        protected string FileType { get; set; }

        protected MainDataSetTableAdapters.ArticleTableAdapter ArticleTableAdapter
        {
            get
            {
                if (_articleTableAdapter == null) _articleTableAdapter = new MainDataSetTableAdapters.ArticleTableAdapter();
                return _articleTableAdapter;
            }
        }

        protected Micajah.FileService.Client.Dal.MetaDataSetTableAdapters.FileTableAdapter FileTableAdapter
        {
            get
            {
                if (_fileTableAdapter == null) _fileTableAdapter = new Micajah.FileService.Client.Dal.MetaDataSetTableAdapters.FileTableAdapter();
                return _fileTableAdapter;
            }
        }

        //        private PathPermissions fullPermissions = PathPermissions.Read | PathPermissions.Delete | PathPermissions.Upload;
        protected PathPermissions fullPermissions = PathPermissions.Read;

        public DBContentProvider(HttpContext context, string[] searchPatterns, string[] viewPaths, string[] uploadPaths, string[] deletePaths, string selectedUrl, string selectedItemTag) :
            base(context, searchPatterns, viewPaths, uploadPaths, deletePaths, selectedUrl, selectedItemTag)
        {
            if (SelectedItemTag != null && SelectedItemTag != string.Empty)
            {
                SelectedItemTag = RemoveProtocolNameAndServerName(SelectedItemTag);
            }
            this.FileType = "Article";
        }

        protected bool IsChildOf(Guid? parentId, Guid childId)
        {
            return (bool)this.ArticleTableAdapter.IsChildOf(childId, parentId);
        }

        protected DirectoryItem[] GetChildDirectories(MainDataSet.ArticleRow articleRow)
        {
            List<DirectoryItem> directories = new List<DirectoryItem>();

            MainDataSet.ArticleDataTable dtArticles = this.ArticleTableAdapter.GetDataByParent(articleRow.ArticleGuid);
            foreach (MainDataSet.ArticleRow row in dtArticles)
            {
                //string itemFullPath = string.Format("/{0}/{1}/", articleRow.Subject, row.Subject);
                string itemFullPath = string.Format("~/ArticleViewAdmin.aspx?id={0}", row.ArticleGuid.ToString("N"));
                directories.Add(new DirectoryItem(
                    row.Subject,
                    string.Empty,
                    itemFullPath,
                    string.Empty,
                    fullPermissions,
                    GetChildFiles(row),
                    GetChildDirectories(row)));
            }
            return directories.ToArray();
        }

        protected FileItem[] GetChildFiles(MainDataSet.ArticleRow articleRow)
        {
            if (articleRow != null && articleRow.DepartmentGuid != Guid.Empty)
            {                
                MetaDataSet.FileDataTable dtFiles = (new Micajah.FileService.Client.Dal.MetaDataSetTableAdapters.FileTableAdapter()).GetFiles(
                    Micajah.Common.Bll.Providers.InstanceProvider.GetInstance(articleRow.DepartmentGuid).OrganizationId,
                    articleRow.DepartmentGuid,
                    "Article",
                    articleRow.ArticleGuid.ToString("N"), false);
                ArrayList files = new ArrayList();
                foreach (MetaDataSet.FileRow frow in dtFiles)
                {
                    string path = string.Empty;
                    string ext = Path.GetExtension(frow.Name);
                    string mimeType = MimeType.GetMimeType(ext);
                    switch (FileType)
                    {
                        case "Image":
                            path = Access.GetThumbnailUrl(frow.FileUniqueId, frow.OrganizationId, frow.DepartmentId, 640, 0, 0);
                            if (MimeType.IsImageType(mimeType))
                            {
                                files.Add(
                                new FileItem(
                                    frow.Name,
                                    ext,
                                    frow.SizeInBytes,
                                    string.Empty,
                                    path,
                                    string.Format("{0}/{1}/", articleRow.Subject, frow.Name),
                                    fullPermissions));
                            }
                            break;
                        case "Video":
                            path = Access.GetFileUrl(frow.FileUniqueId, frow.OrganizationId, frow.DepartmentId);
                            if (MimeType.IsVideoType(mimeType))
                                files.Add(new FileItem(
                                    frow.Name,
                                    ext,
                                    frow.SizeInBytes,
                                    string.Empty,
                                    path,
                                    string.Format("{0}/{1}/", articleRow.Subject, frow.Name),
                                    fullPermissions));
                            break;
                        case "Flash":
                            path = Access.GetFlashUrl(frow.FileUniqueId, frow.OrganizationId, frow.DepartmentId);
                            if (MimeType.IsFlash(mimeType))
                                files.Add(new FileItem(
                                    frow.Name,
                                    ext,
                                    frow.SizeInBytes,
                                    string.Empty,
                                    path,
                                    string.Format("{0}/{1}/", articleRow.Subject, frow.Name),
                                    fullPermissions));
                            break;
                        default:
                            path = Access.GetFileUrl(frow.FileUniqueId, frow.OrganizationId, frow.DepartmentId);
                            files.Add(new FileItem(
                                frow.Name,
                                ext,
                                frow.SizeInBytes,
                                string.Empty,
                                path,
                                string.Format("{0}/{1}/", articleRow.Subject, frow.Name),
                                fullPermissions));
                            break;
                    }
                }
                return (FileItem[])files.ToArray(typeof(FileItem));
            }
            return new FileItem[] { };
        }

        protected DirectoryItem GetDefaultDirectoryItem()
        {
            string fullname = VirtualPathUtility.GetDirectory("~/EditorsImages");
            return new DirectoryItem("EditorsImages", fullname, fullname, string.Empty, fullPermissions, new FileItem[] { }, new DirectoryItem[] { });
        }

        protected bool IsExtensionAllowed(string extension)
        {
            return Array.IndexOf(SearchPatterns, "*.*") >= 0 || Array.IndexOf(SearchPatterns, "*" + extension.ToLower()) >= 0;
        }

        public override string GetFileName(string url)
        {
            return url.Substring(url.LastIndexOf('/') + 1);
        }

        public override string GetPath(string url)
        {
            //return GetDirectoryPath(ExtractPath(RemoveProtocolNameAndServerName(url)));
            return url.Substring(0, url.LastIndexOf('/'));
        }

        public override Stream GetFile(string url)
        {
            byte[] content = null;
            if (Access.StringIsFileUniqueId(url))
                content = Access.GetFile(url);
            //byte[] content = DataServer.GetContent(ExtractPath(RemoveProtocolNameAndServerName(url)));
            if (!Object.Equals(content, null))
            {
                return new MemoryStream(content);
            }
            return null;
        }

        public override string StoreBitmap(Bitmap bitmap, string url, ImageFormat format)
        {

            //string newItemPath = ExtractPath(RemoveProtocolNameAndServerName(url));
            //string name = GetName(newItemPath);
            //string _path = GetPath(newItemPath);
            //string tempFilePath = System.IO.Path.GetTempFileName();
            //bitmap.Save(tempFilePath);
            //byte[] content;
            //using (FileStream inputStream = File.OpenRead(tempFilePath))
            //{
            //    long size = inputStream.Length;
            //    content = new byte[size];
            //    inputStream.Read(content, 0, Convert.ToInt32(size));
            //}

            //if (File.Exists(tempFilePath))
            //{
            //    File.Delete(tempFilePath);
            //}

            //DataServer.CreateItem(name, _path, "image/gif", false, content.Length, content);
            return string.Empty;
        }

        public override string StoreFile(Telerik.Web.UI.UploadedFile file, string path, string name, params string[] arguments)
        {
            //int fileLength = Convert.ToInt32(file.InputStream.Length);
            //byte[] content = new byte[fileLength];
            //file.InputStream.Read(content, 0, fileLength);
            //string fullPath = CombinePath(path, name);
            //if (!Object.Equals(DataServer.GetItemRow(fullPath), null))
            //{
            //    DataServer.ReplaceItemContent(fullPath, content);
            //}
            //else
            //{
            //    DataServer.CreateItem(name, path, file.ContentType, false, fileLength, content);
            //}
            return string.Empty;
        }

        public override string DeleteFile(string path)
        {
            //DataServer.DeleteItem(path);
            return string.Empty;
        }

        public override string DeleteDirectory(string path)
        {
            //DataServer.DeleteItem(path);
            return string.Empty;
        }

        public override string CreateDirectory(string path, string name)
        {
            //DataServer.CreateItem(name, path, string.Empty, true, 0, new byte[0]);
            return string.Empty;
        }

        public override bool CanCreateDirectory
        {
            get
            {
                return false;
            }
        }

        public override DirectoryItem ResolveRootDirectoryAsTree(string path)
        {
            MainDataSet.ArticleRow articleRow = null;
            DirectoryItem returnValue = null;
            Guid articleGuid = Guid.Empty;
            try { articleGuid = new Guid(path); }
            catch { }
            if (articleGuid != Guid.Empty)
            {
                MainDataSet.ArticleDataTable dtArticle = this.ArticleTableAdapter.GetDataByArticleGuid(articleGuid);
                if (dtArticle.Count > 0)
                {
                    articleRow = dtArticle[0];
                    string fullName = this.ArticleTableAdapter.GetAlternateId(articleGuid);
                    if (string.IsNullOrEmpty(fullName))
                        fullName = articleRow.ArticleGuid.ToString("N");
                    returnValue = new DirectoryItem(
                        articleRow.Subject, articleRow.Subject + "/", fullName, string.Empty, fullPermissions, GetChildFiles(articleRow),
                        GetChildDirectories(articleRow));
                }
            }
            if (returnValue == null) returnValue = this.GetDefaultDirectoryItem();
            return returnValue;
        }

        public override DirectoryItem[] ResolveRootDirectoryAsList(string path)
        {
            List<DirectoryItem> directories = new List<DirectoryItem>();
            MainDataSet.ArticleRow articleRow = null;
            Guid articleGuid = Guid.Empty;
            try { articleGuid = new Guid(path); }
            catch { }
            if (articleGuid != Guid.Empty)
            {
                MainDataSet.ArticleDataTable dtArticle = this.ArticleTableAdapter.GetDataByArticleGuid(articleGuid);
                if (dtArticle.Count > 0)
                {
                    articleRow = dtArticle[0];
                    directories.Add(new DirectoryItem(
                        articleRow.Subject,
                        articleRow.Subject + "/",
                        string.Empty,
                        string.Empty,
                        fullPermissions,
                        GetChildFiles(articleRow),
                        new DirectoryItem[] { }));
                    MainDataSet.ArticleDataTable dtChildArticles = this.ArticleTableAdapter.GetDataRecursiveByArticleGuid(articleGuid);
                    foreach (MainDataSet.ArticleRow row in dtChildArticles)
                    {
                        directories.Add(new DirectoryItem(
                            row.Subject,
                            row.Subject + "/",
                            string.Empty,
                            string.Empty,
                            fullPermissions,
                            GetChildFiles(row),
                            new DirectoryItem[] { }));
                    }
                }
            }
            return directories.ToArray();
        }

        public override DirectoryItem ResolveDirectory(string path)
        {
            DirectoryItem[] directories;
            DirectoryItem returnValue = null;
            MainDataSet.ArticleRow articleRow = null;
            MainDataSet.ArticleDataTable dtArticle = null;

            Guid articleGuid = Guid.Empty;
            try { articleGuid = new Guid(path); }
            catch { }
            if (articleGuid != Guid.Empty)
                dtArticle = this.ArticleTableAdapter.GetDataByArticleGuid(articleGuid);
            else
                dtArticle = this.ArticleTableAdapter.GetDataByAlternateId(path);
            if (dtArticle.Count > 0) articleRow = dtArticle[0];

            //if (DisplayMode == FileBrowserDisplayMode.List)
                directories = new DirectoryItem[] { };
            //else
            //{
            //    if (articleRow != null)
            //        directories = GetChildDirectories(articleRow);
            //    else directories = new DirectoryItem[] { };
            //}
            if (articleRow != null)
                returnValue = new DirectoryItem(articleRow.Subject, articleRow + "/", articleRow + "/", articleRow + "/", fullPermissions, GetChildFiles(articleRow),
                    directories);
            else
                returnValue = this.GetDefaultDirectoryItem();
            return returnValue;
        }

    }
    public class ImageDBContentProvider : DBContentProvider
    {
        public ImageDBContentProvider(HttpContext context, string[] searchPatterns, string[] viewPaths, string[] uploadPaths, string[] deletePaths, string selectedUrl, string selectedItemTag) :
            base(context, searchPatterns, viewPaths, uploadPaths, deletePaths, selectedUrl, selectedItemTag)
        {
            if (SelectedItemTag != null && SelectedItemTag != string.Empty)
            {
                SelectedItemTag = RemoveProtocolNameAndServerName(SelectedItemTag);
            }
            this.FileType = "Image";
        }
    }

    public class VideoDBContentProvider : DBContentProvider
    {
        public VideoDBContentProvider(HttpContext context, string[] searchPatterns, string[] viewPaths, string[] uploadPaths, string[] deletePaths, string selectedUrl, string selectedItemTag) :
            base(context, searchPatterns, viewPaths, uploadPaths, deletePaths, selectedUrl, selectedItemTag)
        {
            if (SelectedItemTag != null && SelectedItemTag != string.Empty)
            {
                SelectedItemTag = RemoveProtocolNameAndServerName(SelectedItemTag);
            }
            this.FileType = "Video";
        }
    }

    public class FlashDBContentProvider : DBContentProvider
    {
        public FlashDBContentProvider(HttpContext context, string[] searchPatterns, string[] viewPaths, string[] uploadPaths, string[] deletePaths, string selectedUrl, string selectedItemTag) :
            base(context, searchPatterns, viewPaths, uploadPaths, deletePaths, selectedUrl, selectedItemTag)
        {
            if (SelectedItemTag != null && SelectedItemTag != string.Empty)
            {
                SelectedItemTag = RemoveProtocolNameAndServerName(SelectedItemTag);
            }
            this.FileType = "Flash";
        }
    }

    public class FileDBContentProvider : DBContentProvider
    {
        public FileDBContentProvider(HttpContext context, string[] searchPatterns, string[] viewPaths, string[] uploadPaths, string[] deletePaths, string selectedUrl, string selectedItemTag) :
            base(context, searchPatterns, viewPaths, uploadPaths, deletePaths, selectedUrl, selectedItemTag)
        {
            if (SelectedItemTag != null && SelectedItemTag != string.Empty)
            {
                SelectedItemTag = RemoveProtocolNameAndServerName(SelectedItemTag);
            }
            this.FileType = "File";
        }
    }
}