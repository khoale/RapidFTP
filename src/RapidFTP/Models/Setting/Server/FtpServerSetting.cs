namespace RapidFTP.Models.Setting
{
    using System;

    public class FtpServerSetting
    {
        public const int DefaultFtpPort = 21;

        public const int DefaultSFtpPort = 22;

        public FtpServerSetting(
            string host, 
            int port, 
            FtpProtocol protocol, 
            FtpEncryption encryption, 
            TransferMode transferMode, 
            TimeSpan adjustTimeOffset,
            bool verifySslCert)
        {
            this.Host = host;
            this.Port = port;
            this.Protocol = protocol;
            this.Encryption = encryption;
            this.TransferMode = transferMode;
            this.AdjustTimeOffset = adjustTimeOffset;
            this.VerifySslCert = verifySslCert;
            this.ConnectTimeout = TimeSpan.FromSeconds(60);
        }

        public FtpServerSetting(
            string host, 
            int port, 
            FtpProtocol protocol, 
            FtpEncryption encryption, 
            TransferMode transferMode)
            : this(host, port, protocol, encryption, transferMode, new TimeSpan(), false)
        {
        }

        private FtpServerSetting()
        {
        }

        public string Host { get; private set; }

        public int Port { get; private set; }

        public FtpProtocol Protocol { get; private set; }

        public FtpEncryption Encryption { get; private set; }

        public TransferMode TransferMode { get; private set; }

        public TimeSpan AdjustTimeOffset { get; private set; }

        public bool VerifySslCert { get; private set; }

        public TimeSpan ConnectTimeout { get; private set; }

        public static FtpServerSetting SFtp(
            string host, int port = DefaultSFtpPort)
        {
            return new FtpServerSetting
                       {
                           Host = host, 
                           Port = port, 
                           Protocol = FtpProtocol.SFTP
                       };
        }

        public static FtpServerSetting Ftp(
            string host, int port = DefaultFtpPort, FtpEncryption encryption = FtpEncryption.PlainFTP, TransferMode transferMode = TransferMode.Default, TimeSpan adjustTimeOffset = default(TimeSpan), bool verifySslCert = false, TimeSpan connectTimeout = default(TimeSpan))
        {
            if (connectTimeout == default(TimeSpan))
            {
                connectTimeout = TimeSpan.FromSeconds(60);
            }

            var severSetting = new FtpServerSetting(
                host,
                port,
                FtpProtocol.FTP,
                encryption,
                transferMode,
                adjustTimeOffset,
                verifySslCert);
            
            severSetting.ConnectTimeout = connectTimeout;
            return severSetting;
        }
    }
}