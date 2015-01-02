namespace RapidFTP.Models
{
    using System;

    public class FtpFileItem : FtpItem
    {
        public FtpFileItem(
            IFtpClient client, 
            string name, 
            string parentDir, 
            string remotePath, 
            int size, 
            DateTime createdTime)
            : base(client, name, parentDir, remotePath, size, createdTime, ItemType.File)
        {
        }
    }
}