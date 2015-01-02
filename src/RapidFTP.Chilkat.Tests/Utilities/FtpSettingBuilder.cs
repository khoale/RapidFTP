namespace RapidFTP.Chilkat.Tests.Utilities
{
    using RapidFTP.Models.Setting;

    public class FtpSettingBuilder
    {
        public FtpSettingBuilder()
        {
        }

        public static FtpSettingBuilder Default
        {
            get
            {
                return new FtpSettingBuilder();
            }
        }

        public FtpSetting Build()
        {
            var authentication = new FtpAuthentication("rapid", "rapid");
            var serverSetting = FtpServerSetting.SFtp("localhost");
            var proxySetting = ProxySetting.None();
            var charsetSetting = CharsetSetting.Auto();
            return new FtpSetting(authentication, serverSetting, proxySetting, charsetSetting);
        }
    }
}