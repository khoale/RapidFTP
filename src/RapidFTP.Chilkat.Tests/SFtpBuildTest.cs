namespace RapidFTP.Chilkat.Tests
{
    using RapidFTP.Chilkat.Tests.Fixture;
    using RapidFTP.Chilkat.Tests.Utilities;
    using RapidFTP.Chilkat.Utilities;

    using Xunit;

    public class SFtpBuildTest : IUseFixture<OneTimeFixture<UnlockComponentFixture>>
    {
        [Fact]
        public void Configure()
        {
            var sftpBuilder = new SFtpBuilder();
            var result = sftpBuilder.Configure(FtpSettingBuilder.Default.Build());
            Assert.NotNull(result);
        }

        public void SetFixture(OneTimeFixture<UnlockComponentFixture> data)
        {
        }
    }
}