namespace RapidFTP.Models.Setting
{
    using System;

    public class ProxySetting
    {
        public ProxySetting(ProxyType proxyType, string username, string password, string server, int port)
        {
            this.Type = proxyType;
            this.Username = username;
            this.Password = password;
            this.Server = server;
            this.Port = port;
        }

        public ProxySetting()
        {
            this.Type = ProxyType.None;
        }

        public string Username { get; private set; }

        public string Password { get; private set; }

        public string Server { get; private set; }

        public int Port { get; private set; }

        public ProxyType Type { get; private set; }

        public static ProxySetting None()
        {
            return new ProxySetting();
        }

        public int GetSocksVersion()
        {
            switch (this.Type)
            {
                case ProxyType.Socks4:
                    return 4;
                case ProxyType.Socks5:
                    return 5;
            }

            throw new InvalidOperationException("Proxy type should be socks");
        }
    }
}