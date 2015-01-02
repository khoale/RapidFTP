namespace RapidFTP
{
    using System;

    using RapidFTP.Models;
    using RapidFTP.Models.Setting;

    public class RapidFtpClient : IFtpClient
    {
        private readonly IFtpClientFactory factory;

        private IFtpClient innerFtpClient;

        public RapidFtpClient(IFtpClientFactory factory)
        {
            this.factory = factory;
        }

        public void Dispose()
        {
            this.ReleaseClient();
        }

        public void Connect(FtpSetting setting)
        {
            this.ReleaseClient();
            this.innerFtpClient = this.CreateAndConnect(setting);
        }

        public void Disconnect()
        {
            var ftpClient = this.innerFtpClient;
            if (ftpClient != null)
            {
                ftpClient.Disconnect();
            }
        }

        public bool ExistFile(string remoteFile)
        {
            this.CheckConnected();
            return this.innerFtpClient.ExistFile(remoteFile);
        }

        /// <summary>
        /// Check if remote directory is exist or not.
        /// </summary>
        /// <param name="remoteDirectory">
        /// The remote directory.
        /// </param>
        /// <returns>
        /// The boolean.
        /// </returns>
        public bool ExistDirectory(string remoteDirectory)
        {
            this.CheckConnected();
            return this.innerFtpClient.ExistDirectory(remoteDirectory);
        }

        /// <summary>
        /// Delete remote directory. Does not throw exception if directory does not exist.
        /// </summary>
        /// <param name="remoteDirectory">
        /// The remote directory.
        /// </param>
        public void DeleteDirectory(string remoteDirectory)
        {
            this.CheckConnected();
            this.innerFtpClient.DeleteDirectory(remoteDirectory);
        }

        public void DeleteFile(string remoteFile)
        {
            this.CheckConnected();
            this.innerFtpClient.DeleteFile(remoteFile);
        }

        public FtpItem[] ListItems(string remoteDir)
        {
            this.CheckConnected();
            return this.innerFtpClient.ListItems(remoteDir);
        }

        public FtpDirectoryItem[] ListDirectories(string remoteDir)
        {
            this.CheckConnected();
            return this.innerFtpClient.ListDirectories(remoteDir);
        }

        public FtpFileItem[] ListFiles(string remoteDir)
        {
            this.CheckConnected();
            return this.innerFtpClient.ListFiles(remoteDir);
        }

        public void CreateDirectory(string remoteDirectory)
        {
            this.CheckConnected();
            this.innerFtpClient.CreateDirectory(remoteDirectory);
        }
        
        public void UploadFile(string localFile, string remoteFile)
        {
            this.CheckConnected();
            this.innerFtpClient.UploadFile(localFile, remoteFile);
        }

        public void UploadDirectory(string localDirectory, string remoteDirectory)
        {
            this.CheckConnected();
            this.innerFtpClient.UploadDirectory(localDirectory, remoteDirectory);
        }

        public void DownloadFolder(string remoteDirectory, string localDirectory)
        {
            this.CheckConnected();
            this.innerFtpClient.DownloadFolder(remoteDirectory, localDirectory);
        }

        public void DownloadFile(string remoteFile, string localFile)
        {
            this.CheckConnected();
            this.innerFtpClient.DownloadFile(remoteFile, localFile);
        }

        private void ReleaseClient()
        {
            if (this.innerFtpClient != null)
            {
                this.innerFtpClient.Dispose();
                this.innerFtpClient = null;
            }
        }

        private void CheckConnected()
        {
            if (this.innerFtpClient == null)
            {
                throw new InvalidOperationException("Ftp client is not connect with server");
            }
        }

        private IFtpClient CreateAndConnect(FtpSetting setting)
        {
            IFtpClient ftpClient = this.factory.Create(setting);

            if (ftpClient != null)
            {
                ftpClient.Connect(setting);
            }
            else
            {
                throw new NotSupportedException(
                    string.Format("Does not support {0} protocol", setting.ServerSetting.Protocol));
            }

            return ftpClient;
        }
    }
}