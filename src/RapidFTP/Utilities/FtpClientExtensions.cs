namespace RapidFTP.Utilities
{
    using System;

    using RapidFTP.Models;

    public static class FtpClientExtensions
    {
        public static void Delete(this IFtpClient ftpClient, FtpItem ftpItem)
        {
            if (ftpItem == null)
            {
                throw new ArgumentNullException("ftpItem");
            }

            // Doesn't care if fail to delete file or directory
            switch (ftpItem.Type)
            {
                case ItemType.File:
                    ftpClient.DeleteFile(ftpItem.RemotePath);
                    break;
                case ItemType.Directory:
                    ftpClient.DeleteDirectory(ftpItem.RemotePath);
                    break;
                default:
                    throw new NotSupportedException(ftpItem.Type.ToString());
            }
        }

        public static bool Exist(this IFtpClient ftpClient, FtpItem ftpItem)
        {
            if (ftpItem == null)
            {
                throw new ArgumentNullException("ftpItem");
            }

            bool isExist;
            switch (ftpItem.Type)
            {
                case ItemType.Directory:
                    isExist = ftpClient.ExistDirectory(ftpItem.RemotePath);
                    break;
                case ItemType.File:
                    isExist = ftpClient.ExistFile(ftpItem.RemotePath);
                    break;
                default:
                    throw new NotSupportedException(ftpItem.Type.ToString());
            }

            return isExist;
        }

        public static void Download(this IFtpClient ftpClient, FtpItem ftpItem, string localPath)
        {
            switch (ftpItem.Type)
            {
                case ItemType.Directory:
                    ftpClient.DownloadFolder(ftpItem.RemotePath, localPath);
                    break;
                case ItemType.File:
                    ftpClient.DownloadFile(ftpItem.RemotePath, localPath);
                    break;
            }
        }
    }
}