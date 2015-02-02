namespace RapidFTP.Chilkat
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    using global::Chilkat;

    using RapidFTP.Chilkat.Resources;
    using RapidFTP.Chilkat.Utilities;
    using RapidFTP.Models;
    using RapidFTP.Models.Setting;
    using RapidFTP.Utilities;

    /// <summary>
    /// The ftp client.
    /// </summary>
    public class ChilkatFtpClient : IFtpClient
    {
        private readonly object sync;

        private bool disposed;

        private FtpSetting currentSetting;

        private Ftp2 ftp2;

        public ChilkatFtpClient()
        {
            this.sync = new object();
        }

        public void Connect(FtpSetting setting)
        {
            lock (this.sync)
            {
                this.ReleaseClient();
                var success = false;

                var ftpClient = Ftp2Builder.Default.Configure(setting).Build();
                var task = Task.Factory.StartNew(() => ftpClient.Connect());
                var waitHandle = new ManualResetEventSlim();
                task.ContinueWith(t =>
                    {
                        // Make sure success value can change
                        waitHandle.Wait();

                        Thread.MemoryBarrier();
                        // ReSharper disable once AccessToModifiedClosure
                        if (!success)
                        {
                            if (ftpClient != null)
                            {
                                // If not succcess then dispose ftpClient right away
                                ftpClient.Dispose();
                            }
                        }
                    });

                // NOTE: this piece of code can cause problem if running on UI thread
                // http://stackoverflow.com/questions/17490339/task-wait-and-continuewith
                success = task.Wait(setting.ServerSetting.ConnectTimeout);
                waitHandle.Set();
                
                // FtpClient can connect success but will disconnect right after
                // so we must recheck the connection status right after that
                if (!success || !ftpClient.IsConnected)
                {
                    throw new InvalidOperationException(
                        string.Format("Fail to connect with ftp server.\nSetting:\n{0}", setting.Print()));
                }

                this.currentSetting = setting;
                this.ftp2 = ftpClient;
            }
        }

        public FtpItem[] ListItems(string remoteDir)
        {
            if (remoteDir == null)
            {
                throw new ArgumentNullException("remoteDir");
            }

            remoteDir = UnixPath.CorrectDirectorySeperator(remoteDir);

            lock (this.sync)
            {
                this.ChangeRemoteDirectory(remoteDir);
                var ftpItems = new List<FtpItem>();

                for (var i = 0; i < this.ftp2.NumFilesAndDirs; i++)
                {
                    var isDirectory = this.ftp2.GetIsDirectory(i);
                    var name = this.ftp2.GetFilename(i);
                    var remotePath = UnixPath.Combine(remoteDir, name);
                    var size = this.ftp2.GetSize(i);
                    var createdTime =
                        this.ftp2.GetCreateTime(i) + this.currentSetting.ServerSetting.AdjustTimeOffset;

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
        }

        public FtpDirectoryItem[] ListDirectories(string remoteDir)
        {
            lock (this.sync)
            {
                return
                    this.ListItems(remoteDir)
                        .Where(x => x.Type == ItemType.Directory)
                        .Cast<FtpDirectoryItem>()
                        .ToArray();
            }
        }

        public FtpFileItem[] ListFiles(string remoteDir)
        {
            lock (this.sync)
            {
                return
                    this.ListItems(remoteDir)
                        .Where(x => x.Type == ItemType.File)
                        .Cast<FtpFileItem>()
                        .ToArray();
            }
        }

        public void CreateDirectory(string remoteDirectory)
        {
            if (string.IsNullOrEmpty(remoteDirectory))
            {
                throw new ArgumentException("remoteDirectory");
            }

            remoteDirectory = UnixPath.CorrectDirectorySeperator(remoteDirectory);

            lock (this.sync)
            {
                var relatedRemoteDirectoies = UnixPath.GetRelatedDirectories(remoteDirectory);
                foreach (var relatedRemoteDirectoy in relatedRemoteDirectoies)
                {
                    var success = this.ftp2.CreateRemoteDir(relatedRemoteDirectoy);
                    if (!success)
                    {
                        if (this.ExistDirectory(relatedRemoteDirectoy))
                        {
                            continue;
                        }

                        throw new InvalidOperationException(
                            string.Format(ErrorTexts.FailCreateDirectory, relatedRemoteDirectoy));
                    }
                }
            }
        }

        public void DeleteFile(string remoteFile)
        {
            if (remoteFile == null)
            {
                throw new ArgumentNullException("remoteFile");
            }

            remoteFile = UnixPath.CorrectDirectorySeperator(remoteFile);

            lock (this.sync)
            {
                var success = this.ftp2.DeleteRemoteFile(remoteFile);
                if (!success)
                {
                    throw new InvalidOperationException(
                        string.Format(ErrorTexts.FailToDeleteFile, remoteFile));
                }
            }
        }

        public void DeleteDirectory(string remoteDirectory)
        {
            if (remoteDirectory == null)
            {
                throw new ArgumentNullException("remoteDirectory");
            }

            remoteDirectory = UnixPath.CorrectDirectorySeperator(remoteDirectory);

            if (!this.ExistDirectory(remoteDirectory))
            {
                return;
            }

            lock (this.sync)
            {
                this.ChangeRemoteDirectory(remoteDirectory);
                this.ftp2.DeleteTree();

                // Must change back to root directory before delete remoteDirectory
                this.ftp2.ChangeRemoteDir("/");

                var success = this.ftp2.RemoveRemoteDir(remoteDirectory);
                if (!success)
                {
                    throw new InvalidOperationException(
                        string.Format(ErrorTexts.FailToDeleteDirectory, remoteDirectory));
                }
            }
        }

        public bool ExistFile(string remoteFile)
        {
            if (remoteFile == null)
            {
                throw new ArgumentNullException("remoteFile");
            }

            remoteFile = UnixPath.CorrectDirectorySeperator(remoteFile);

            lock (this.sync)
            {
                var remoteDirectory = UnixPath.GetDirectoryName(remoteFile);
                var fileName = UnixPath.GetFileName(remoteFile);

                if (string.IsNullOrEmpty(fileName))
                {
                    throw new InvalidOperationException(
                        string.Format("File name {0} is invalid. Remote file is {1}.", fileName, remoteFile));
                }

                if (!this.ExistDirectory(remoteDirectory))
                {
                    return false;
                }

                this.ChangeRemoteDirectory(remoteDirectory);

                // On xlight ftp server set ListPattern = fileName approach is not working.
                // That why we must loop for each file and check for filename.
                this.ftp2.ListPattern = "*";
                var numFileAndDir = this.ftp2.NumFilesAndDirs;
                var isExist = false;
                for (int i = 0; i < numFileAndDir; i++)
                {
                    var ftpFileName = this.ftp2.GetFilename(i);
                    if (string.Compare(ftpFileName, fileName, StringComparison.InvariantCultureIgnoreCase) == 0)
                    {
                        var isDirectory = this.ftp2.GetIsDirectory(i);
                        if (!isDirectory)
                        {
                            isExist = true;
                        }

                        break;
                    }
                }

                return isExist;
            }
        }

        public bool ExistDirectory(string remoteDirectory)
        {
            if (remoteDirectory == null)
            {
                throw new ArgumentNullException("remoteDirectory");
            }

            remoteDirectory = UnixPath.CorrectDirectorySeperator(remoteDirectory);

            lock (this.sync)
            {
                return this.ftp2.ChangeRemoteDir(remoteDirectory);
            }
        }

        public void UploadFile(string localFile, string remoteFile)
        {
            if (string.IsNullOrEmpty(localFile))
            {
                throw new ArgumentException("localFile");
            }

            if (string.IsNullOrEmpty(remoteFile))
            {
                throw new ArgumentException("remoteFile");
            }

            remoteFile = UnixPath.CorrectDirectorySeperator(remoteFile);

            if (!File.Exists(localFile))
            {
                throw new FileNotFoundException(
                    string.Format(ErrorTexts.FileNotFound, localFile));
            }

            lock (this.sync)
            {
                var remoteDirectory = UnixPath.GetDirectoryName(remoteFile);

                if (!string.IsNullOrEmpty(remoteDirectory))
                {
                    this.ChangeRemoteDirectory(remoteDirectory);
                }

                var fileName = UnixPath.GetFileName(remoteFile);
                bool success = this.ftp2.PutFile(localFile, fileName);
                if (!success)
                {
                    throw new InvalidOperationException(
                        string.Format(ErrorTexts.FailUploadFile, localFile, remoteFile));
                }
            }
        }

        public void UploadDirectory(string localDirectory, string remoteDirectory)
        {
            if (string.IsNullOrEmpty(localDirectory))
            {
                throw new ArgumentException("localDirectory");
            }

            if (string.IsNullOrEmpty(remoteDirectory))
            {
                throw new ArgumentException("remoteDirectory");
            }

            remoteDirectory = UnixPath.CorrectDirectorySeperator(remoteDirectory);

            if (!Directory.Exists(localDirectory))
            {
                throw new DirectoryNotFoundException(
                    string.Format(ErrorTexts.DirectoryNotFound, localDirectory));
            }

            lock (this.sync)
            {
                this.ChangeRemoteDirectory(remoteDirectory);
                var success = this.ftp2.PutTree(localDirectory);
                if (!success)
                {
                    throw new InvalidOperationException(
                        string.Format(ErrorTexts.FailUploadDirectory, localDirectory, remoteDirectory));
                }
            }
        }

        /// <summary>
        /// Download all file under remote directory to local directory.
        /// </summary>
        /// <param name="remoteDirectory">The remote directory</param>
        /// <param name="localDirectory">The local directory</param>
        public void DownloadFolder(string remoteDirectory, string localDirectory)
        {
            if (remoteDirectory == null)
            {
                throw new ArgumentNullException("remoteDirectory");
            }

            if (localDirectory == null)
            {
                throw new ArgumentNullException("localDirectory");
            }

            remoteDirectory = UnixPath.CorrectDirectorySeperator(remoteDirectory);

            lock (this.sync)
            {
                this.ChangeRemoteDirectory(remoteDirectory);

                var success = this.ftp2.DownloadTree(localDirectory);
                if (!success)
                {
                    throw new InvalidOperationException(
                        string.Format(ErrorTexts.FailDownloadDirectory, remoteDirectory, localDirectory));
                }
            }
        }

        public void DownloadFile(string remoteFile, string localFile)
        {
            if (remoteFile == null)
            {
                throw new ArgumentNullException("remoteFile");
            }

            if (localFile == null)
            {
                throw new ArgumentNullException("localFile");
            }

            remoteFile = UnixPath.CorrectDirectorySeperator(remoteFile);

            lock (this.sync)
            {
                var success = this.ftp2.GetFile(remoteFile, localFile);
                if (!success)
                {
                    throw new InvalidOperationException(
                        string.Format(ErrorTexts.FailDownloadFile, localFile, remoteFile));
                }
            }
        }

        public void Disconnect()
        {
            lock (this.sync)
            {
                if (this.ftp2 != null)
                {
                    // Doesn't care if fail to disconnect
                    this.ftp2.Disconnect();
                }
            }
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
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

        private void ReleaseClient()
        {
            lock (this.sync)
            {
                if (this.ftp2 != null)
                {
                    this.ftp2.Dispose();
                    this.ftp2 = null;
                    this.currentSetting = null;
                }
            }
        }

        private void ChangeRemoteDirectory(string remoteDir)
        {
            if (this.IsCurrentRemoteDirectory(remoteDir))
            {
                return;
            }

            var success = this.ftp2.ChangeRemoteDir(remoteDir);
            if (!success)
            {
                throw new InvalidOperationException(string.Format(ErrorTexts.FailToChangeRemoteDir, remoteDir));
            }
        }

        private bool IsCurrentRemoteDirectory(string remoteDir)
        {
            if (remoteDir == null)
            {
                return false;    
            }

            var currentRemoteDir = this.ftp2.GetCurrentRemoteDir();
            if (currentRemoteDir == null)
            {
                return false;
            }

            return UnixPath.IsEqual(currentRemoteDir, remoteDir);
        }
    }
}