namespace RapidFTP.Chilkat.Tests.Data
{
    using System.Collections.Generic;
    using System.Linq;

    using RapidFTP.Models.Setting;

    public class MinimalFtpSettingData : FtpSettingData
    {
        protected override List<object[]> GetSettings()
        {
            var filterd = new List<FtpSetting>();
            var settings = base.GetSettings();
            foreach (var arguments in settings)
            {
                var ftpSetting = (FtpSetting)arguments[0];
                if (filterd.All(x => 
                    x.ServerSetting.Protocol != ftpSetting.ServerSetting.Protocol))
                {
                    filterd.Add(ftpSetting);
                }
            }

            return filterd.Select(
                ftpSetting => new object[] { ftpSetting }).ToList();
        }
    }
}