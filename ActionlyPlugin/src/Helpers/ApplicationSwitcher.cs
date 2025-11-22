namespace Loupedeck.ActionlyPlugin
{
    using System;
    using System.Diagnostics;
    using System.Runtime.InteropServices;

    public static class ApplicationSwitcher
    {
        [DllImport("user32.dll")]
        private static extern bool SetForegroundWindow(IntPtr hWnd);

        [DllImport("user32.dll")]
        private static extern bool IsIconic(IntPtr hWnd);

        [DllImport("user32.dll")]
        private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        private const int SW_MINIMIZE = 6;  // Minimize window
        private const int SW_RESTORE = 9;   // Restore window (from minimized)
        private const int SW_MAXIMIZE = 3;  // Maximize window

        public static bool SwitchToProcess(string processName)
        {
            // Get processes by name (without file extension)
            var processes = Process.GetProcessesByName(processName);
            if (processes.Length == 0)
                return false;

            var process = processes[0];
            IntPtr handle = process.MainWindowHandle;

            if (handle == IntPtr.Zero)
                return false;

            // Minimize the window if it's already open and not minimized
            if (!IsIconic(handle))
            {
                ShowWindow(handle, SW_MINIMIZE);
            }

            // Restore the window and bring it to the foreground
            ShowWindow(handle, SW_RESTORE);
            ShowWindow(handle, SW_MAXIMIZE);  // Optionally maximize it to ensure it gets focus
            return SetForegroundWindow(handle);
        }

        public static List<string> GetOpenProcessNames()
        {
            var openApps = new HashSet<string>(); // HashSet automatically avoids duplicates

            foreach (var process in Process.GetProcesses())
            {
                // Check if the process has a main window (GUI application)
                if (process.MainWindowHandle != IntPtr.Zero)
                {
                    // Add the process name to the HashSet (no duplicates)
                    openApps.Add(process.ProcessName);
                }
            }

            return new List<string>(openApps); // Convert HashSet to List and return
        }
    }
}