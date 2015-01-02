namespace RapidFTP.Chilkat.Utilities
{
    using System;

    using global::Chilkat;

    using RapidFTP.Models.Setting;

    public class Ftp2Builder
    {
        private static string licenseCode;

        private readonly Ftp2 ftp2;

        public Ftp2Builder()
        {
            this.ftp2 = new Ftp2();
            var success = this.ftp2.UnlockComponent(licenseCode);
            if (!success)
            {
                throw new InvalidOperationException("Fail to unlock Ftp2 component.");
            }
        }

        public static Ftp2Builder Default
        {
            get
            {
                return new Ftp2Builder();
            }
        }

        public static void SetLicense(string code)
        {
            licenseCode = code;
        }

        public Ftp2Builder Configure(FtpSetting ftpSetting)
        {
            this.ConfigureServerSetting(ftpSetting.ServerSetting);
            this.ConfigureAuthentication(ftpSetting.Authentication);
            this.ConfigureProxy(ftpSetting.ProxySetting);
            this.ConfigureCharset(ftpSetting.CharsetSetting);

            return this;
        }

        public Ftp2Builder ConfigureServerSetting(FtpServerSetting serverSetting)
        {
            this.ftp2.Hostname = serverSetting.Host;
            this.ftp2.Port = serverSetting.Port == default(int) ? 22 : serverSetting.Port;

            // Different between implicit and explicit FTP
            // http://blogs.iis.net/robert_mcmurray/archive/2008/11/10/ftp-clients-part-2-explicit-ftps-versus-implicit-ftps.aspx
            if (serverSetting.Encryption == FtpEncryption.Explicit)
            {
                this.UseExplicitFTPS();
            }
            else if (serverSetting.Encryption == FtpEncryption.Implicit)
            {
                this.UseImplicitFTPS();
            }

            // This timeout setting seem not working
            this.ftp2.ConnectTimeout = (int)serverSetting.ConnectTimeout.TotalSeconds;
            this.ftp2.ReadTimeout = (int)serverSetting.ConnectTimeout.TotalSeconds;
            this.ftp2.RequireSslCertVerify = serverSetting.VerifySslCert;

            switch (serverSetting.TransferMode)
            {
                case TransferMode.Passive:
                    this.ftp2.Passive = true;
                    break;
                case TransferMode.Default:
                case TransferMode.Active:
                    this.ftp2.Passive = false;
                    break;
            }

            return this;
        }

        public Ftp2Builder UseExplicitFTPS()
        {
            // http://www.example-code.com/csharp/ftp_authTLS.asp
            this.ftp2.Ssl = false;
            this.ftp2.AuthTls = true;
            this.ftp2.AuthSsl = false;
            return this;
        }

        public Ftp2Builder UseImplicitFTPS()
        {
            // http://www.chilkatforum.com/questions/441/ftp-implicit-or-explicit-mode
            // this.ftp2.AutoFix = false;
            this.ftp2.Ssl = true;
            this.ftp2.AuthTls = false;
            this.ftp2.AuthSsl = false;
            return this;
        }

        public Ftp2Builder ConfigureAuthentication(FtpAuthentication authentication)
        {
            switch (authentication.LogonType)
            {
                case LogonType.Anonymous:
                    this.ftp2.Username = "Anonymous";
                    this.ftp2.Password = string.Empty;
                    break;
                case LogonType.Normal:
                case LogonType.Account:
                    this.ftp2.Username = authentication.Username;
                    this.ftp2.Password = authentication.Password;

                    if (authentication.LogonType == LogonType.Account)
                    {
                        this.ftp2.Account = authentication.Account;
                    }

                    break;
            }

            return this;
        }

        public Ftp2Builder ConfigureProxy(ProxySetting proxySetting)
        {
            switch (proxySetting.Type)
            {
                case ProxyType.None:
                    break;
                case ProxyType.Http:
                    this.ftp2.HttpProxyHostname = proxySetting.Server;
                    this.ftp2.HttpProxyPort = proxySetting.Port;
                    this.ftp2.HttpProxyUsername = proxySetting.Username;
                    this.ftp2.HttpProxyPassword = proxySetting.Password;
                    this.ftp2.ProxyMethod = this.ftp2.DetermineProxyMethod();
                    break;
                case ProxyType.Socks4:
                case ProxyType.Socks5:
                    this.ftp2.SocksHostname = proxySetting.Server;
                    this.ftp2.SocksPort = proxySetting.Port;
                    this.ftp2.SocksUsername = proxySetting.Username;
                    this.ftp2.SocksPassword = proxySetting.Password;
                    this.ftp2.SocksVersion = proxySetting.GetSocksVersion();
                    this.ftp2.ProxyMethod = this.ftp2.DetermineProxyMethod();
                    break;
            }

            return this;
        }

        public Ftp2Builder ConfigureCharset(CharsetSetting charsetSetting)
        {
            if (!charsetSetting.AutoDetect)
            {
                this.ftp2.DirListingCharset = charsetSetting.Encoding.EncodingName;
                this.ftp2.CommandCharset = charsetSetting.Encoding.EncodingName;
            }

            return this;
        }

        public Ftp2 Build()
        {
            return this.ftp2;
        }
    }
}