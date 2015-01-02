namespace RapidFTP.Models.Setting
{
    using System;

    public class FtpAuthentication
    {
        public FtpAuthentication(string username, string password)
        {
            this.Username = username;
            this.Password = password;
            this.LogonType = LogonType.Normal;
        }

        public FtpAuthentication(string username, string password, string account, LogonType logonType)
        {
            this.Username = username;
            this.Password = password;
            this.Account = account;
            this.LogonType = logonType;

            this.ValidateSetting();
        }

        public string Username { get; private set; }

        public string Password { get; private set; }

        public string Account { get; private set; }

        public LogonType LogonType { get; private set; }

        public static FtpAuthentication Normal(string username, string password)
        {
            return new FtpAuthentication(username, password);
        }

        private void ValidateSetting()
        {
            if (this.LogonType == LogonType.Normal || this.LogonType == LogonType.Account)
            {
                if (string.IsNullOrEmpty(this.Username))
                {
                    throw new ArgumentException("Username is require");
                }

                if (string.IsNullOrEmpty(this.Password))
                {
                    throw new ArgumentException("Password is require");
                }
            }

            if (this.LogonType == LogonType.Account)
            {
                throw new ArgumentException("Account is require");
            }
        }
    }
}