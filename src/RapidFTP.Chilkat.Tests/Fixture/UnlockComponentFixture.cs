namespace RapidFTP.Chilkat.Tests.Fixture
{
    using System.Diagnostics;
    using System.IO;

    using global::Chilkat;

    using RapidFTP.Chilkat.Utilities;
    using RapidFTP.Utilities;

    using Xunit;

    public class UnlockComponentFixture
    {
        private readonly string iniFileName;

        private readonly IniFile iniFile;

        private readonly string ftpLicense;

        private readonly string sftpLicense;

        public UnlockComponentFixture()
        {
            var rootDirectory = Directory.GetCurrentDirectory();
            this.iniFileName = Path.Combine(rootDirectory, "ChilkatLicense.ini");

            this.iniFile = IniFile.ReadFile(this.iniFileName);
            this.ftpLicense = this.iniFile.Read("ftp", "Chilkat");
            this.sftpLicense = this.iniFile.Read("sftp", "Chilkat");

            Ftp2Builder.SetLicense(this.ftpLicense);
            SFtpBuilder.SetLicense(this.sftpLicense);
        }

        [Fact]
        public void Read()
        {
            // Sample content
            //// [Chilkat]
            //// ftp={Your license key}
            //// sftp={Your license key}
            Assert.True(
                File.Exists(this.iniFileName), 
                string.Format("You must create license file under '{0}'", this.iniFile));

            Assert.NotEmpty(this.ftpLicense);
            Assert.NotEmpty(this.sftpLicense);

            Trace.WriteLine(this.ftpLicense);
            Trace.WriteLine(this.sftpLicense);

            var ftp2 = new Ftp2();
            Assert.True(ftp2.UnlockComponent(this.ftpLicense));

            var sftp = new SFtp();
            Assert.True(sftp.UnlockComponent(this.sftpLicense));
        }
    }
}