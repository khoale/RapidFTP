namespace RapidFTP
{
    using RapidFTP.Models.Setting;

    public interface IFtpSettingProvider
    {
        FtpSetting GetSetting(string profile);
    }
}