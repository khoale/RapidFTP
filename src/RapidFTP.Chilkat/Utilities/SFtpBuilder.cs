namespace RapidFTP.Chilkat.Utilities
{
    using System;

    using global::Chilkat;

    using RapidFTP.Models.Setting;

    /// <summary>
    /// See example at http://www.example-code.com/csharp/sftp_readDir.asp
    /// </summary>
    public class SFtpBuilder
    {
        private static string licenseCode;

        private readonly SFtp sftp;

        public SFtpBuilder()
            : this(new SFtp())
        {
        }

        public SFtpBuilder(SFtp existSftp)
        {
            this.sftp = existSftp;
            var success = this.sftp.UnlockComponent(licenseCode);
            if (!success)
            {
                throw new InvalidOperationException("Fail to unlock SFtp component.");
            }
        }

        public static SFtpBuilder Default
        {
            get
            {
                return new SFtpBuilder();
            }
        }

        public static void SetLicense(string code)
        {
            licenseCode = code;
        }

        public static SFtpBuilder FromClient(SFtp existSftp)
        {
            return new SFtpBuilder(existSftp);
        }

        public SFtpBuilder Configure(FtpSetting setting)
        {
            this.ConfigureServerSetting(setting.ServerSetting);
            this.ConfigureAuthentication(setting.Authentication);
            this.ConfigureProxy(setting.ProxySetting);
            this.ConfigureCharset(setting.CharsetSetting);

            return this;
        }

        public SFtpBuilder ConfigureServerSetting(FtpServerSetting ftpServerSetting)
        {
            // Cannot set server setting here due to unsuport method
            // this.sftp.Connect(host, port)
            this.sftp.ConnectTimeoutMs = (int)ftpServerSetting.ConnectTimeout.TotalMilliseconds;
            return this;
        }

        public SFtpBuilder ConfigureAuthentication(FtpAuthentication authentication)
        {
            if (!this.sftp.IsConnected)
            {
                return this;
            }

            // Authenticate with the SSH server. Chilkat SFTP supports
            // both password-based authenication as well as public-key authentication.
            var success = this.sftp.AuthenticatePw(authentication.Username, authentication.Password);
            if (success != true)
            {
                throw new InvalidOperationException("Fail to authenticate with server");
            }

            // After authenticating, the SFTP subsystem must be initialized:
            success = this.sftp.InitializeSftp();
            if (success != true)
            {
                throw new InvalidOperationException("Fail to InitializeSftp");
            }

            return this;
        }

        public SFtpBuilder ConfigureProxy(ProxySetting proxySetting)
        {
            if (this.sftp.IsConnected)
            {
                return this;
            }

            switch (proxySetting.Type)
            {
                case ProxyType.None:
                    break;
                case ProxyType.Http:
                    this.sftp.HttpProxyHostname = proxySetting.Server;
                    this.sftp.HttpProxyPort = proxySetting.Port;
                    this.sftp.HttpProxyUsername = proxySetting.Username;
                    this.sftp.HttpProxyPassword = proxySetting.Password;
                    break;
                case ProxyType.Socks4:
                case ProxyType.Socks5:
                    this.sftp.SocksHostname = proxySetting.Server;
                    this.sftp.SocksPort = proxySetting.Port;
                    this.sftp.SocksUsername = proxySetting.Username;
                    this.sftp.SocksPassword = proxySetting.Password;
                    this.sftp.SocksVersion = proxySetting.GetSocksVersion();
                    break;
            }

            return this;
        }

        public SFtpBuilder ConfigureCharset(CharsetSetting charsetSetting)
        {
            if (this.sftp.IsConnected)
            {
                return this;
            }

            if (!charsetSetting.AutoDetect)
            {
                this.sftp.FilenameCharset = charsetSetting.Encoding.EncodingName;
            }

            return this;
        }

        public SFtp Build()
        {
            return this.sftp;
        }
    }
}