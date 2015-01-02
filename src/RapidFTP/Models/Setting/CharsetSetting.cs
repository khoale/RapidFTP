namespace RapidFTP.Models.Setting
{
    using System;
    using System.Text;

    public class CharsetSetting
    {
        public CharsetSetting(bool autoDetect, bool forceUTF8, string customCharset)
        {
            if (!autoDetect && !forceUTF8)
            {
                this.ValidateCharset(customCharset);
            }

            this.AutoDetect = autoDetect;
            this.ForceUTF8 = forceUTF8;
            this.CustomCharset = customCharset;
        }

        public Encoding Encoding
        {
            get
            {
                return this.GetEncoding();
            }
        }

        public bool AutoDetect { get; private set; }

        public bool ForceUTF8 { get; private set; }

        public string CustomCharset { get; private set; }

        public static CharsetSetting Auto()
        {
            return new CharsetSetting(true, false, string.Empty);
        }

        private Encoding GetEncoding()
        {
            Encoding encoding = null;

            if (!this.AutoDetect)
            {
                if (this.ForceUTF8)
                {
                    encoding = Encoding.UTF8;
                }
                else if (!string.IsNullOrEmpty(this.CustomCharset))
                {
                    encoding = Encoding.GetEncoding(this.CustomCharset);
                }
            }

            return encoding;
        }

        private void ValidateCharset(string charset)
        {
            if (charset == null)
            {
                throw new ArgumentNullException("charset");
            }

            var encoding = Encoding.GetEncoding(charset);
            if (encoding == null)
            {
                throw new ArgumentException(string.Format("{0} is invalid encoding", charset));
            }
        }
    }
}