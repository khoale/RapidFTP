namespace RapidFTP.Chilkat.Tests.Data
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;

    public class FtpSettingData : IEnumerable<object[]>
    {
        private readonly List<object[]> datas;

        public FtpSettingData()
        {
            // ReSharper disable once DoNotCallOverridableMethodsInConstructor
            this.datas = this.GetSettings();
        }

        public IEnumerator<object[]> GetEnumerator()
        {
            return this.datas.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        protected virtual List<object[]> GetSettings()
        {
            var settings = FtpServerTestProvider.Default.GetSettings();
            return settings.Select(setting => new object[] { setting }).ToList();
        }
    }
}