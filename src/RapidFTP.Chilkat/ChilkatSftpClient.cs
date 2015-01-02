namespace RapidFTP.Chilkat
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    using global::Chilkat;

    using RapidFTP.Chilkat.Resources;
    using RapidFTP.Chilkat.Utilities;
    using RapidFTP.Models;
    using RapidFTP.Models.Setting;
    using RapidFTP.Utilities;

    public class ChilkatSftpClient : IFtpClient
    {
        private readonly object sync = new object();

        private SFtp sftp;

        private bool disposed;

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        public void Connect(FtpSetting setting)
        {
            lock (this.sync)
            {
                this.ReleaseClient();
                this.sftp = SFtpBuilder.Default.Configure(setting).Build();

                var success = this.sftp.Connect(setting.ServerSetting.Host, setting.ServerSetting.Port);
                if (!success)
                {
                    throw new InvalidOperationException(
                        string.Format("Fail to connect with ftp server.\nSetting:\n{0}", setting.Print()));
                }

                // Must reconfig sftp client after success connect
                SFtpBuilder.FromClient(this.sftp).Configure(setting);
            }
        }

        public void Disconnect()
        {
            lock (this.sync)
            {
                if (this.sftp != null)
                {
                    this.sftp.Disconnect();
                }
            }
        }

        public bool ExistFile(string remoteFile)
        {
            if (remoteFile == null)
            {
                throw new ArgumentNullException("remoteFile");
            }

            var fileSize = this.sftp.GetFileSize32(remoteFile, false, false);
            return fileSize > 0;
        }

        public bool ExistDirectory(string remoteDirectory)
        {
            if (remoteDirectory == null)
            {
                throw new ArgumentNullException("remoteDirectory");
            }

            string handle = null;

            try
            {
                handle = this.sftp.OpenDir(remoteDirectory);
                if (handle == null)
                {
                    return false;
                }

                return true;
            }
            finally
            {
                this.CloseHandle(handle);
            }
        }

        public void DeleteDirectory(string remoteDirectory)
        {
            if (remoteDirectory == null)
            {
                throw new ArgumentNullException("remoteDirectory");
            }

            if (!this.ExistDirectory(remoteDirectory))
            {
                return;
            }

            var ftpItems = this.ListItems(remoteDirectory);
            foreach (var ftpItem in ftpItems)
            {
                ftpItem.Delete();
            }

            var success = this.sftp.RemoveDir(remoteDirectory);
            if (!success)
            {
                throw new InvalidOperationException(
                    string.Format(ErrorTexts.FailToDeleteDirectory, remoteDirectory));
            }
        }

        public void DeleteFile(string remoteFile)
        {
            if (remoteFile == null)
            {
                throw new ArgumentNullException("remoteFile");
            }

            if (!this.ExistFile(remoteFile))
            {
                return;
            }
            
            var success = this.sftp.RemoveFile(remoteFile);
            if (!success)
            {
                throw new InvalidOperationException(
                    string.Format(ErrorTexts.FailToDeleteFile, remoteFile));
            }
        }

        public FtpItem[] ListItems(string remoteDir)
        {
            lock (this.sync)
            {
                var ftpItems = new List<FtpItem>();
                string handle = null;
                try
                {
                    handle = this.sftp.OpenDir(remoteDir);
                    if (handle == null)
                    {
                        throw new InvalidOperationException(
                            string.Format("Fail to open handle for directory {0}", remoteDir));
                    }

                    var sftpDir = this.sftp.ReadDir(handle);

                    for (var i = 0; i < sftpDir.NumFilesAndDirs; i++)
                    {
                        var sftpFile = sftpDir.GetFileObject(i);

                        var isDirectory = sftpFile.IsDirectory;
                        var remotePath = remoteDir + @"/" + sftpFile.Filename;
                        var name = sftpFile.Filename;
                        var size = sftpFile.Size32;
                        var createdTime = sftpFile.CreateTime;

                        FtpItem ftpItem;
                        if (isDirectory)
                        {
                            ftpItem = new FtpDirectoryItem(this, name, remoteDir, remotePath, size, createdTime);
                        }
                        else
                        {
                            ftpItem = new FtpFileItem(this, name, remoteDir, remotePath, size, createdTime);
                        }

                        ftpItems.Add(ftpItem);
                    }

                    return ftpItems.ToArray();
                }
                finally
                {
                    this.CloseHandle(handle);
                }
            }
        }

        public FtpDirectoryItem[] ListDirectories(string remoteDir)
        {
            lock (this.sync)
            {
                return
                    this.ListItems(remoteDir).Where(x => x.Type == ItemType.Directory).Cast<FtpDirectoryItem>().ToArray();
            }
        }

        public FtpFileItem[] ListFiles(string remoteDir)
        {
            lock (this.sync)
            {
                return
                    this.ListItems(remoteDir).Where(x => x.Type == ItemType.File).Cast<FtpFileItem>().ToArray();
            }
        }

        public void CreateDirectory(string remoteDirectory)
        {
            if (remoteDirectory == null)
            {
                throw new ArgumentNullException("remoteDirectory");
            }

            lock (this.sync)
            {
                var relatedRemoteDirectoies = UnixPath.GetRelatedDirectories(remoteDirectory);
                foreach (var relatedRemoteDirectoy in relatedRemoteDirectoies)
                {
                    if (!this.ExistDirectory(relatedRemoteDirectoy))
                    {
                        var success = this.sftp.CreateDir(relatedRemoteDirectoy);
                        if (!success)
                        {
                            throw new InvalidOperationException(
                                string.Format(ErrorTexts.FailCreateDirectory, relatedRemoteDirectoy));
                        }
                    }
                }
            }
        }

        public void UploadFile(string localFile, string remoteFile)
        {
            lock (this.sync)
            {
                // TODO: change to UploadFileByName
                // Open a file for writing on the SSH server.
                // If the file already exists, it is overwritten.
                // (Specify "createNew" instead of "createTruncate" to
                // prevent overwriting existing files.)
                string handle = null;

                try
                {
                    handle = this.sftp.OpenFile(remoteFile, "writeOnly", "createTruncate");
                    if (handle == null)
                    {
                        throw new InvalidOperationException("Fail to open file handle");
                    }

                    // Upload from the local file to the SSH server.
                    var success = this.sftp.UploadFile(handle, localFile);
                    if (success != true)
                    {
                        throw new InvalidOperationException(
                            string.Format(ErrorTexts.FailUploadFile, localFile, remoteFile));
                    }
                }
                finally
                {
                    if (handle != null)
                    {
                        this.sftp.CloseHandle(handle);        
                    }
                }
            }
        }

        public void UploadDirectory(string localDirectory, string remoteDirectory)
        {
            lock (this.sync)
            {
                var files = Directory.GetFiles(localDirectory);
                foreach (var file in files)
                {
                    var remoteFile = UnixPath.Combine(remoteDirectory, Path.GetFileName(file));
                    this.UploadFile(file, remoteFile);
                }
            }
        }

        public void DownloadFolder(string remoteDirectory, string localDirectory)
        {
            lock (this.sync)
            {
                var ftpFileItems = this.ListFiles(remoteDirectory);
                foreach (var fileItem in ftpFileItems)
                {
                    var localFileName = Path.Combine(localDirectory, fileItem.Name);
                    this.DownloadFile(fileItem.RemotePath, localFileName);
                }
            }
        }

        public void DownloadFile(string remoteFile, string localFile)
        {
            lock (this.sync)
            {
                // Open a file on the server:
                string handle = null;
                try
                {
                    handle = this.OpenReadFileHandle(remoteFile);

                    // Download the file:
                    var success = this.sftp.DownloadFile(handle, localFile);
                    if (success != true)
                    {
                        throw new InvalidOperationException(ErrorTexts.FailUploadFile);
                    }
                }
                finally
                {
                    this.CloseHandle(handle);
                }
            }
        }

        private void ReleaseClient()
        {
            if (this.sftp != null)
            {
                this.sftp.Dispose();
                this.sftp = null;
            }
        }

        private string OpenReadFileHandle(string remotePath)
        {
            string handle = this.sftp.OpenFile(remotePath, "readOnly", "openExisting");
            if (handle == null)
            {
                throw new InvalidOperationException(string.Format("Fail to open handle for file {0}", remotePath));
            }

            return handle;
        }

        private void CloseHandle(string handle)
        {
            if (handle == null)
            {
                return;
            }

            var success = this.sftp.CloseHandle(handle);
            if (!success)
            {
                throw new InvalidOperationException("Fail to close handle");
            }
        }

        private void Dispose(bool disposing)
        {
            if (this.disposed)
            {
                return;
            }

            if (disposing)
            {
                this.ReleaseClient();
                this.disposed = true;
            }
        }
    }
}