namespace RapidFTP.Models
{
    using System;

    using RapidFTP.Utilities;

    public abstract class FtpItem : IEquatable<FtpItem>
    {
        private readonly IFtpClient client;

        private int? cacheHashCode;

        protected FtpItem(
            IFtpClient client, 
            string name, 
            string parentDirectory, 
            string remotePath, 
            int size, 
            DateTime createdTime, 
            ItemType itemType)
        {
            this.client = client;
            this.Name = name;
            this.RemotePath = remotePath;
            this.ParentDirectory = parentDirectory;
            this.Size = size;
            this.CreatedTime = createdTime;
            this.Type = itemType;
        }

        protected FtpItem()
        {
        }

        public string Name { get; private set; }

        public string RemotePath { get; private set; }

        public string ParentDirectory { get; private set; }

        public int Size { get; private set; }

        public DateTime CreatedTime { get; private set; }

        public ItemType Type { get; private set; }

        public virtual bool Exist()
        {
            return this.client.Exist(this);
        }

        public virtual void Delete()
        {
            this.client.Delete(this);
        }

        public virtual void Download(string localPath)
        {
            this.client.Download(this, localPath);
        }

        public bool Equals(FtpItem other)
        {
            if (ReferenceEquals(this, other))
            {
                return true;
            }

            return 
                this.Name == other.Name && 
                this.RemotePath == other.RemotePath && 
                this.CreatedTime == other.CreatedTime && 
                this.Size == other.Size && 
                this.client == other.client;
        }

        public override bool Equals(object obj)
        {
            var ftpItem = obj as FtpItem;
            if (ftpItem != null)
            {
                return this.Equals(ftpItem);
            }

            return ReferenceEquals(this, obj);
        }

        public override int GetHashCode()
        {
            // ReSharper disable NonReadonlyFieldInGetHashCode
            lock (this)
            {
                if (this.cacheHashCode == null)
                {
                    this.cacheHashCode =
                        string.Format("{0}.{1}.{2}", this.Name, this.RemotePath, this.CreatedTime).GetHashCode();
                }

                return this.cacheHashCode.Value;   
            }
            // ReSharper restore NonReadonlyFieldInGetHashCode
        }
    }
}