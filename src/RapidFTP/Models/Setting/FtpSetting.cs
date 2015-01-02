namespace RapidFTP.Models.Setting
{
    using System.Text;

    public class FtpSetting
    {
        public FtpSetting(
            FtpAuthentication authentication, 
            FtpServerSetting serverSetting, 
            ProxySetting proxySetting, 
            CharsetSetting charsetSetting)
        {
            this.CharsetSetting = charsetSetting;
            this.ProxySetting = proxySetting;
            this.ServerSetting = serverSetting;
            this.Authentication = authentication;
        }

        public FtpAuthentication Authentication { get; private set; }

        public FtpServerSetting ServerSetting { get; private set; }

        public ProxySetting ProxySetting { get; private set; }

        public CharsetSetting CharsetSetting { get; private set; }

        public string Print()
        {
            var stringBuilder = new StringBuilder();
            
            if (this.ServerSetting.Protocol == FtpProtocol.FTP)
            {
                stringBuilder.AppendFormat(
                    "{3}-{2}: {0}:{1}\n", this.ServerSetting.Host, this.ServerSetting.Port, this.ServerSetting.Encryption, this.ServerSetting.Protocol);    
            }
            else
            {
                stringBuilder.AppendFormat(
                    "{2}: {0}:{1}\n", this.ServerSetting.Host, this.ServerSetting.Port, this.ServerSetting.Protocol);
            }

            return stringBuilder.ToString();
        }

        public override string ToString()
        {
            return this.Print();
        }
    }
}