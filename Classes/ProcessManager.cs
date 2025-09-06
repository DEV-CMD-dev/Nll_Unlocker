using System.Diagnostics;
using System.Management;
using System.Runtime.InteropServices;
using System.Windows;

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

        public static string GetCommandLine(Process process)
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
        }
    }
}
