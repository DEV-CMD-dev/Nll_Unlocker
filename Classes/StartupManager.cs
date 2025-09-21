using System.Diagnostics;
using System.IO;

namespace Nll_Unlocker.Classes
{
    public static class StartupManager
    {
        public static void EnsureRandomizedStartup(string[] args)
        {
            if (args.Contains("/cloned"))
                return;

            try
            {
                string temp = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName() + ".exe");

                File.Copy(Process.GetCurrentProcess().MainModule.FileName, temp, true);

                Process.Start(temp, "/cloned");

                Environment.Exit(0);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("StartupManager error: " + ex.Message);
            }
        }
    }
}
