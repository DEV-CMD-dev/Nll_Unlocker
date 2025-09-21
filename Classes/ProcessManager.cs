using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Management;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Nll_Unlocker.Classes
{
    public static class ProcessManager
    {
        [DllImport("ntdll.dll")]
        private static extern int NtQueryInformationProcess(
        IntPtr processHandle,
        int processInformationClass,
        ref int processInformation,
        int processInformationLength,
        ref int returnLength
    );

        // NtSetInformationProcess
        [DllImport("ntdll.dll")]
        private static extern int NtSetInformationProcess(
            IntPtr processHandle,
            int processInformationClass,
            ref int processInformation,
            int processInformationLength
        );

        private const int ProcessBreakOnTermination = 0x1D;

        public static bool IsCritical(Process process)
        {
            try
            {
                int breakOnTermination = 0;
                int retLength = 0;
                int status = NtQueryInformationProcess(process.Handle, ProcessBreakOnTermination, ref breakOnTermination, sizeof(int), ref retLength);
                return status == 0 && breakOnTermination != 0;
            }
            catch
            {
                return true;
            }
        }

        public static void SetCritical(Process process, bool critical)
        {
            try
            {
                int value = critical ? 1 : 0;
                int status = NtSetInformationProcess(process.Handle, ProcessBreakOnTermination, ref value, sizeof(int));
                if (status != 0)
                    MessageBox.Show($"Cannot change critical status. NtSetInformationProcess returned: {status}");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }

        public static async Task<string?> GetCommandLineAsync(Process process)
        {
            return await Task.Run(() =>
            {
                try
                {
                    using (var searcher = new ManagementObjectSearcher(
                               $"SELECT CommandLine FROM Win32_Process WHERE ProcessId = {process.Id}"))
                    {
                        foreach (ManagementObject obj in searcher.Get())
                        {
                            return obj["CommandLine"]?.ToString();
                        }
                    }
                }
                catch { }
                return null;
            });
        }

        public static async Task<int> EvaluateRiskAsync(Process process)
        {
            int risk = 0;

            try
            {
                string cmd = await GetCommandLineAsync(process) ?? "";
                string? path = process.MainModule?.FileName?.ToLower();

                //Sign
                string publisher = GetPublisher(process) ?? "";
                if (publisher == "Unknown") risk += 40;

                // Check path
                if (!string.IsNullOrEmpty(path) && !path.Contains(@"\windows\") && !path.Contains(@"\program files\")) risk += 20;

                // CmLine check
                if (cmd.Length > 100 || cmd.Contains("/runhidden") || cmd.Contains("/stealth"))
                    risk += 20;

            }
            catch { risk += 10; }

            return risk;
        }


        public static ImageSource? GetProcessIcon(Process process)
        {
            try
            {
                string? path = process.MainModule?.FileName;
                if (!string.IsNullOrEmpty(path))
                {
                    Icon? ico = Icon.ExtractAssociatedIcon(path);
                    if (ico != null)
                    {
                        using (MemoryStream ms = new MemoryStream())
                        {
                            ico.ToBitmap().Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                            ms.Seek(0, SeekOrigin.Begin);

                            BitmapImage bmp = new BitmapImage();
                            bmp.BeginInit();
                            bmp.StreamSource = ms;
                            bmp.CacheOption = BitmapCacheOption.OnLoad;
                            bmp.EndInit();
                            return bmp;
                        }
                    }
                }
            }
            catch
            {
            }
            return null;
        }

        public static string GetPublisher(Process process)
        {
            try
            {
                string? path = process.MainModule?.FileName;
                if (!string.IsNullOrEmpty(path) && File.Exists(path))
                {
                    FileVersionInfo info = FileVersionInfo.GetVersionInfo(path);
                    if (!string.IsNullOrEmpty(info.CompanyName))
                        return info.CompanyName;
                }
            }
            catch
            {
            }
            return "Unknown";
        }
    }
}
