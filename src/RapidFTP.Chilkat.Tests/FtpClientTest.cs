namespace RapidFTP.Chilkat.Tests
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;

    using RapidFTP.Chilkat;
    using RapidFTP.Chilkat.Tests.Data;
    using RapidFTP.Chilkat.Tests.Fixture;
    using RapidFTP.Models;
    using RapidFTP.Models.Setting;

    using Xunit;
    using Xunit.Extensions;

    public class FtpClientTest : 
        IUseFixture<UnlockComponentFixture>, 
        IUseFixture<XlightFtpServer>,
        IDisposable
    {
        private readonly RapidFtpClient ftpClient;

        public FtpClientTest()
        {
            this.ftpClient = new RapidFtpClient(new ChilkatFtpClientFactory());
        }

        [ClassData(typeof(FtpSettingData))]
        [Theory]
        public void Connect_ShouldSuccess(FtpSetting ftpSetting)
        {
            this.ftpClient.Connect(ftpSetting);
        }

        [ClassData(typeof(FtpSettingData))]
        [Theory]
        public void Connect_ConnectToRequireEncryptionServer_ShouldThrowExecption(FtpSetting ftpSetting)
        {
            var serverSetting = ftpSetting.ServerSetting;
            if (serverSetting.Protocol != FtpProtocol.FTP || 
                serverSetting.Encryption == FtpEncryption.PlainFTP)
            {
                Trace.WriteLine("Ignore test");
                return;
            }
            
            var modifiedServerSetting = FtpServerSetting.Ftp(
                serverSetting.Host,
                serverSetting.Port,
                FtpEncryption.PlainFTP,
                serverSetting.TransferMode,
                serverSetting.AdjustTimeOffset,
                serverSetting.VerifySslCert,
                serverSetting.ConnectTimeout);

            var modifiedFtpSetting = new FtpSetting(
                ftpSetting.Authentication,
                modifiedServerSetting,
                ftpSetting.ProxySetting,
                ftpSetting.CharsetSetting);

            var stopwatch = Stopwatch.StartNew();
            try
            {
                Assert.Throws<InvalidOperationException>(() => 
                    this.ftpClient.Connect(modifiedFtpSetting));
            }
            finally
            {
                Trace.WriteLine("Elasped for " + stopwatch.Elapsed);
            }
        }

        [ClassData(typeof(MinimalFtpSettingData))]
        [Theory]
        public void UploadFile_TextFile_ShouldSuccess(FtpSetting ftpSetting)
        {
            this.ftpClient.Connect(ftpSetting);
            this.ftpClient.UploadFile("Samples/TextFile.txt", "TextFile.txt");
            this.ftpClient.DeleteFile("TextFile.txt");
        }

        [ClassData(typeof(MinimalFtpSettingData))]
        [Theory]
        public void UploadFile_TextFileWithPath_ShouldSuccess(FtpSetting ftpSetting)
        {
            this.ftpClient.Connect(ftpSetting);

            this.ftpClient.CreateDirectory("/Sample");
            this.ftpClient.UploadFile("Samples/TextFile.txt", "/Sample/TextFile.txt");
            this.ftpClient.DeleteDirectory("/Sample");
        }

        [ClassData(typeof(MinimalFtpSettingData))]
        [Theory]
        public void UploadFile_MultiFile_ShouldSuccess(FtpSetting ftpSetting)
        {
            this.ftpClient.Connect(ftpSetting);

            var workingDirectory = "/Sample" + Guid.NewGuid();
            this.ftpClient.CreateDirectory(workingDirectory);
            this.ftpClient.UploadFile(
                "Samples/TextFile.txt", string.Format("{0}/TextFile_{1}.txt", workingDirectory, Guid.NewGuid()));
            this.ftpClient.UploadFile(
                "Samples/TextFile.txt", string.Format("{0}/TextFile_{1}.txt", workingDirectory, Guid.NewGuid()));

            this.ftpClient.DeleteDirectory("/Sample");
        }

        [ClassData(typeof(MinimalFtpSettingData))]
        [Theory]
        public void UploadDirectory_SampleDirectory_ShouldSuccess(FtpSetting ftpSetting)
        {
            this.ftpClient.Connect(ftpSetting);

            this.ftpClient.DeleteDirectory("/Sample");

            this.ftpClient.CreateDirectory("/Sample");
            this.ftpClient.UploadDirectory("Samples", "/Sample");

            var existSampleFile = this.ftpClient.ExistFile("/Sample/TextFile.txt");

            Assert.True(existSampleFile);
            this.ftpClient.DeleteDirectory("/Sample");
        }

        [ClassData(typeof(MinimalFtpSettingData))]
        [Theory]
        public void ExistFile_FileNotExist_ShouldReturnFalse(FtpSetting ftpSetting)
        {
            this.ftpClient.Connect(ftpSetting);

            var remoteFile = string.Format("/Sample/NotExistFile_{0}.txt", Guid.NewGuid());
            var isExist = this.ftpClient.ExistFile(remoteFile);

            Assert.False(isExist, "File should not exist");
        }

        [ClassData(typeof(MinimalFtpSettingData))]
        [Theory]
        public void ExistFile_TextFileInsideFolder_ShouldSuccess(FtpSetting ftpSetting)
        {
            this.ftpClient.Connect(ftpSetting);

            if (!this.ftpClient.ExistDirectory("/Sample"))
            {
                this.ftpClient.CreateDirectory("/Sample");
            }

            var remoteFile = string.Format("/Sample/TextFile{0}.txt", Guid.NewGuid());
            this.ftpClient.UploadFile("Samples/TextFile.txt", remoteFile);
            var isExist = this.ftpClient.ExistFile(remoteFile);

            Assert.True(isExist);
            this.ftpClient.DeleteDirectory("/Sample");
        }

        [ClassData(typeof(MinimalFtpSettingData))]
        [Theory]
        public void ExistFile_TextFileInsideFolderLv2_ShouldSuccess(FtpSetting ftpSetting)
        {
            this.ftpClient.Connect(ftpSetting);

            if (!this.ftpClient.ExistDirectory("/Sample"))
            {
                this.ftpClient.CreateDirectory("/Sample");
            }

            if (!this.ftpClient.ExistDirectory("/Sample/Sample2"))
            {
                this.ftpClient.CreateDirectory("/Sample/Sample2");
            }

            var remoteFile = string.Format("/Sample/Sample2/TextFile{0}.txt", Guid.NewGuid());
            this.ftpClient.UploadFile("Samples/TextFile.txt", remoteFile);
            var isExist = this.ftpClient.ExistFile(remoteFile);

            Assert.True(isExist);
            this.ftpClient.DeleteDirectory("/Sample");
        }

        [ClassData(typeof(MinimalFtpSettingData))]
        [Theory]
        public void ListItem_FileAndFolder_ShouldReturnFileAndFolder(FtpSetting ftpSetting)
        {
            this.ftpClient.Connect(ftpSetting);

            this.ftpClient.DeleteDirectory("/Sample");
            this.ftpClient.CreateDirectory("/Sample");
            this.ftpClient.CreateDirectory("/Sample/Sample2");
            this.ftpClient.CreateDirectory("/Sample/Sample3");

            var remoteFile1 = string.Format("/Sample/TextFile{0}.txt", Guid.NewGuid());
            this.ftpClient.UploadFile("Samples/TextFile.txt", remoteFile1);

            var remoteFile2 = string.Format("/Sample/TextFile{0}.txt", Guid.NewGuid());
            this.ftpClient.UploadFile("Samples/TextFile.txt", remoteFile2);

            var sample2FtpItems = this.ftpClient.ListItems("/Sample/Sample2");
            var sampleFtpItems = this.ftpClient.ListItems("/Sample");

            Assert.True(
                sampleFtpItems.Any(
                    x => x.Name == "Sample2" && x.RemotePath == "/Sample/Sample2" && x.Type == ItemType.Directory));
            Assert.True(
                sampleFtpItems.Any(
                    x => x.Name == "Sample3" && x.RemotePath == "/Sample/Sample3" && x.Type == ItemType.Directory));
            Assert.True(
                sampleFtpItems.Any(
                    x =>
                    x.Name == Path.GetFileName(remoteFile1) && x.RemotePath == remoteFile1 && x.Type == ItemType.File));
            Assert.True(
                sampleFtpItems.Any(
                    x =>
                    x.Name == Path.GetFileName(remoteFile2) && x.RemotePath == remoteFile2 && x.Type == ItemType.File));

            Assert.Equal(4, sampleFtpItems.Length);
            Assert.Equal(0, sample2FtpItems.Length);

            this.ftpClient.DeleteDirectory("/Sample");
        }

        [ClassData(typeof(MinimalFtpSettingData))]
        [Theory]
        public void DeleteDirectory_Level1Directory_ShouldSuccess(FtpSetting ftpSetting)
        {
            this.ftpClient.Connect(ftpSetting);

            if (!this.ftpClient.ExistDirectory("/Sample"))
            {
                this.ftpClient.CreateDirectory("/Sample");
            }

            this.ftpClient.DeleteDirectory("/Sample");
        }

        [ClassData(typeof(MinimalFtpSettingData))]
        [Theory]
        public void DeleteDirectory_Level2Directory_ShouldSuccessDeleteParentDirectory(FtpSetting ftpSetting)
        {
            this.ftpClient.Connect(ftpSetting);

            if (!this.ftpClient.ExistDirectory("/Sample"))
            {
                this.ftpClient.CreateDirectory("/Sample");
            }

            if (!this.ftpClient.ExistDirectory("/Sample/Sample2"))
            {
                this.ftpClient.CreateDirectory("/Sample/Sample2");
            }

            this.ftpClient.DeleteDirectory("/Sample");
        }

        [ClassData(typeof(MinimalFtpSettingData))]
        [Theory]
        public void DeleteDirectory_Level2Directory_ShouldSuccessDeleteChildDirectory(FtpSetting ftpSetting)
        {
            this.ftpClient.Connect(ftpSetting);

            if (!this.ftpClient.ExistDirectory("/Sample"))
            {
                this.ftpClient.CreateDirectory("/Sample");
            }

            if (!this.ftpClient.ExistDirectory("/Sample/Sample2"))
            {
                this.ftpClient.CreateDirectory("/Sample/Sample2");
            }

            this.ftpClient.DeleteDirectory("/Sample/Sample2");
        }

        [ClassData(typeof(MinimalFtpSettingData))]
        [Theory]
        public void ExistDirectory_DirectoryNotExist_ShouldReturnFalse(FtpSetting ftpSetting)
        {
            this.ftpClient.Connect(ftpSetting);
            var notExistDirectory = string.Format("/NotExistDirectory_{0}", Guid.NewGuid());
            var isExist = this.ftpClient.ExistDirectory(notExistDirectory);

            Assert.False(isExist);
        }

        [ClassData(typeof(MinimalFtpSettingData))]
        [Theory]
        public void ExistDirectory_Lv1Directory_ShouldReturnTrue(FtpSetting ftpSetting)
        {
            this.ftpClient.Connect(ftpSetting);

            var remoteDirectory = string.Format("/Sample_{0}", Guid.NewGuid());
            this.ftpClient.CreateDirectory(remoteDirectory);

            var isExist = this.ftpClient.ExistDirectory(remoteDirectory);

            Assert.True(isExist);
            this.ftpClient.DeleteDirectory(remoteDirectory);
        }

        [ClassData(typeof(MinimalFtpSettingData))]
        [Theory]
        public void ExistDirectory_Lv2Directory_ShouldReturnTrue(FtpSetting ftpSetting)
        {
            this.ftpClient.Connect(ftpSetting);

            var remoteDirectory = string.Format("/Sample_{0}/Lv2", Guid.NewGuid());
            this.ftpClient.CreateDirectory(remoteDirectory);

            var isExist = this.ftpClient.ExistDirectory(remoteDirectory);

            Assert.True(isExist);
            this.ftpClient.DeleteDirectory(remoteDirectory);
        }

        public void SetFixture(UnlockComponentFixture data)
        {
        }

        public void SetFixture(XlightFtpServer data)
        {
        }

        public void Dispose()
        {
            this.ftpClient.Dispose();
        }
    }
}