namespace RapidFTP.Chilkat.Tests
{
    using System;
    using System.Collections.Generic;

    using RapidFTP.Models.Setting;

    public class FtpServerTestProvider
    {
        private const string DefaultUsername = "rapid";

        private const string DefaultPassword = "rapid";

        private const string DefaultHost = "127.0.0.1";

        private const int DefaultTimeout = 3;

        public static FtpServerTestProvider Default
        {
            get
            {
                return new FtpServerTestProvider();
            }
        }

        public FtpSetting[] GetSettings()
        {
            var ftpSettings = new List<FtpSetting>();
            //// ftpSettings.Add(this.GetFileZillaFtpSettings());
            ftpSettings.Add(this.GetXlightFtpSettings());
            ftpSettings.Add(this.GetXlightFtpSettingsActive());
            ftpSettings.Add(this.GetXlightFtpSettingsPassive());
            ftpSettings.Add(this.GetXlightFtpImplicitSetting());
            ftpSettings.Add(this.GetXlightFtpExplicitSetting());
            ftpSettings.Add(this.GetXlightSFtpSettings());

            return ftpSettings.ToArray();
        }

        private FtpSetting GetFileZillaFtpSettings()
        {
            var authentication = FtpAuthentication.Normal(DefaultUsername, DefaultPassword);
            var ftpServerSetting = FtpServerSetting.Ftp(DefaultHost, connectTimeout: TimeSpan.FromSeconds(DefaultTimeout));
            var proxySetting = ProxySetting.None();
            var charsetSetting = CharsetSetting.Auto();

            return new FtpSetting(authentication, ftpServerSetting, proxySetting, charsetSetting);
        }

        private FtpSetting GetXlightFtpSettings()
        {
            var authentication = FtpAuthentication.Normal(DefaultUsername, DefaultPassword);
            var ftpServerSetting = FtpServerSetting.Ftp(
                DefaultHost, 
                port: 40000, 
                connectTimeout: TimeSpan.FromSeconds(DefaultTimeout));
            var proxySetting = ProxySetting.None();
            var charsetSetting = CharsetSetting.Auto();

            return new FtpSetting(authentication, ftpServerSetting, proxySetting, charsetSetting);
        }

        private FtpSetting GetXlightFtpSettingsPassive()
        {
            var authentication = FtpAuthentication.Normal(DefaultUsername, DefaultPassword);
            var ftpServerSetting = FtpServerSetting.Ftp(
                DefaultHost, 
                port: 40000,
                connectTimeout: TimeSpan.FromSeconds(DefaultTimeout),
                transferMode: TransferMode.Passive);
            var proxySetting = ProxySetting.None();
            var charsetSetting = CharsetSetting.Auto();

            return new FtpSetting(authentication, ftpServerSetting, proxySetting, charsetSetting);
        }

        private FtpSetting GetXlightFtpSettingsActive()
        {
            var authentication = FtpAuthentication.Normal(DefaultUsername, DefaultPassword);
            var ftpServerSetting = FtpServerSetting.Ftp(
                DefaultHost, 
                port: 40000,
                transferMode: TransferMode.Active,
                connectTimeout: TimeSpan.FromSeconds(DefaultTimeout));
            var proxySetting = ProxySetting.None();
            var charsetSetting = CharsetSetting.Auto();

            return new FtpSetting(authentication, ftpServerSetting, proxySetting, charsetSetting);
        }

        private FtpSetting GetXlightFtpImplicitSetting()
        {
            var authentication = FtpAuthentication.Normal(DefaultUsername, DefaultPassword);
            var ftpServerSetting = FtpServerSetting.Ftp(DefaultHost, port: 40001, encryption: FtpEncryption.Implicit, connectTimeout: TimeSpan.FromSeconds(DefaultTimeout));
            var proxySetting = ProxySetting.None();
            var charsetSetting = CharsetSetting.Auto();

            return new FtpSetting(authentication, ftpServerSetting, proxySetting, charsetSetting);
        }

        private FtpSetting GetXlightFtpExplicitSetting()
        {
            var authentication = FtpAuthentication.Normal(DefaultUsername, DefaultPassword);
            var ftpServerSetting = FtpServerSetting.Ftp(
                DefaultHost, 
                port: 40002, 
                encryption: FtpEncryption.Explicit, 
                connectTimeout: TimeSpan.FromSeconds(DefaultTimeout + 5));

            var proxySetting = ProxySetting.None();
            var charsetSetting = CharsetSetting.Auto();

            return new FtpSetting(authentication, ftpServerSetting, proxySetting, charsetSetting);
        }

        private FtpSetting GetXlightSFtpSettings()
        {
            var authentication = FtpAuthentication.Normal(DefaultUsername, DefaultPassword);
            var ftpServerSetting = FtpServerSetting.SFtp(DefaultHost, port: 40003);
            var proxySetting = ProxySetting.None();
            var charsetSetting = CharsetSetting.Auto();

            return new FtpSetting(authentication, ftpServerSetting, proxySetting, charsetSetting);
        }
    }
}