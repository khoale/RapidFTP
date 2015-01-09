namespace RapidFTP.Chilkat.Tests.Fixture
{
    using System.Configuration;
    using System.Diagnostics;

    using global::Chilkat;

    using RapidFTP.Chilkat.Utilities;

    using Xunit;

    public class UnlockComponentFixture
    {
        private readonly string ftpLicense;

        private readonly string sftpLicense;

        public UnlockComponentFixture()
        {
            this.ftpLicense = ConfigurationManager.AppSettings["ChilkatFTP"];
            this.sftpLicense = ConfigurationManager.AppSettings["ChilkatSFTP"];

            Ftp2Builder.SetLicense(this.ftpLicense);
            SFtpBuilder.SetLicense(this.sftpLicense);
        }

        [Fact]
        public void Read()
        {
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