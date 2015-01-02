namespace RapidFTP.Models
{
    using System;

    public class FtpDirectoryItem : FtpItem
    {
        public FtpDirectoryItem(
            IFtpClient client, 
            string name, 
            string parentDir, 
            string remotePath, 
            int size, 
            DateTime createdTime)
            : base(client, name, parentDir, remotePath, size, createdTime, ItemType.Directory)
        {
        }
    }
}