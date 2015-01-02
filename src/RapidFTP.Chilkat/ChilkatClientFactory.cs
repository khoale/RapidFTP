namespace RapidFTP.Chilkat
{
    using RapidFTP.Models.Setting;

    public class ChilkatFtpClientFactory : IFtpClientFactory
    {
        public IFtpClient Create(FtpSetting setting)
        {
            IFtpClient ftpClient = null;
            switch (setting.ServerSetting.Protocol)
            {
                case FtpProtocol.FTP:
                    ftpClient = new ChilkatFtpClient();
                    break;
                case FtpProtocol.SFTP:
                    ftpClient = new ChilkatSftpClient();
                    break;
            }

            return ftpClient;
        }
    }
}