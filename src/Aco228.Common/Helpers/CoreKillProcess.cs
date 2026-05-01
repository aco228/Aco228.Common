using System.Diagnostics;
using System.Management;

namespace Aco228.Common.Helpers;

public static class CoreKillProcess
{
    public static void TryKill(string processName)
    {
        Process[] process = Process.GetProcessesByName(processName);
        if (process.Length == 0)
            return;
        
        Console.WriteLine($"Killing {process.Length} chrome instances");

        try
        {
            foreach (var proc in process)
                KillProcessAndChildren(proc.Id);
        }
        catch
        {
            int a = 0;
        }
    }
    
    public static void KillProcessAndChildren(int pid)
    {
        ManagementObjectSearcher searcher = new ManagementObjectSearcher("Select * From Win32_Process Where ParentProcessID=" + pid);
        ManagementObjectCollection moc = searcher.Get();
        foreach (ManagementObject mo in moc)
        {
            KillProcessAndChildren(Convert.ToInt32(mo["ProcessID"]));
        }
        try
        {
            Process proc = Process.GetProcessById(pid);
            proc.Kill();
        }
        catch (ArgumentException)
        {
            // Process already exited.
        }
    }
}