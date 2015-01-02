namespace RapidFTP
{
    using System;

    using RapidFTP.Models;
    using RapidFTP.Models.Setting;

    public interface IFtpClient : IDisposable
    {
        void Connect(FtpSetting setting);

        FtpItem[] ListItems(string remoteDir);

        FtpDirectoryItem[] ListDirectories(string remoteDir);

        FtpFileItem[] ListFiles(string remoteDir);

        void CreateDirectory(string remoteDirectory);


        void DeleteFile(string remoteFile);

        void DeleteDirectory(string remoteDirectory);
        
        bool ExistFile(string remoteFile);

        bool ExistDirectory(string remoteDirectory);

        void UploadFile(string localFile, string remoteFile);

        void UploadDirectory(string localDirectory, string remoteDirectory);

        /// <summary>
        /// Download all file under remote directory to local directory.
        /// </summary>
        /// <param name="remoteDirectory">The remote directory</param>
        /// <param name="localDirectory">The local directory</param>
        void DownloadFolder(string remoteDirectory, string localDirectory);

        void DownloadFile(string remoteFile, string localFile);

        void Disconnect();
    }
}