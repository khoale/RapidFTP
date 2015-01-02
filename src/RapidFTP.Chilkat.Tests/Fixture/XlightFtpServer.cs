namespace RapidFTP.Chilkat.Tests.Fixture
{
    using System;
    using System.Diagnostics;
    using System.IO;

    public class XlightFtpServer : IDisposable
    {
        private readonly Process process;

        private readonly string rootDirectory;

        private readonly string tempDirectory;

        public XlightFtpServer()
        {
            this.rootDirectory = Directory.GetCurrentDirectory();
            this.tempDirectory = Path.Combine(this.rootDirectory, "temp");

            this.KillXlight();
            try
            {
                this.UpdateConfigFile();
                var exeFile = Path.Combine(this.rootDirectory, "tools/xlight/StartXlight.exe");
                var startInfo = new ProcessStartInfo(exeFile);

                this.process = new Process { StartInfo = startInfo };
                this.process.Start();
            }
            catch (Exception)
            {
                if (Debugger.IsAttached)
                {
                    Debugger.Break();
                }

                throw;
            }
        }

        public void Dispose()
        {
            this.process.Dispose();
            this.KillXlight();

            if (Directory.Exists(this.tempDirectory))
            {
                Directory.Delete(this.tempDirectory, true);
            }
        }

        private void UpdateConfigFile()
        {
            var hostsFile = Path.Combine(this.rootDirectory, "tools/xlight/ftpd.hosts");
            var hosts = File.ReadAllText(hostsFile);

            hosts = hosts.Replace("%ROOTDIR%", this.tempDirectory);
            File.WriteAllText(hostsFile, hosts);
        }

        private void KillXlight()
        {
            var xlightProcesses = Process.GetProcessesByName("xlight");
            foreach (var xlightProcess in xlightProcesses)
            {
                try
                {
                    xlightProcess.Kill();
                    xlightProcess.WaitForExit();
                }
                finally
                {
                    xlightProcess.Dispose();    
                }
            }
        }
    }
}