namespace RapidFTP
{
    using RapidFTP.Models.Setting;

    public interface IFtpClientFactory
    {
        IFtpClient Create(FtpSetting setting);
    }
}