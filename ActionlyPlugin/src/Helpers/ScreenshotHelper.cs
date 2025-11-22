

namespace Loupedeck.ActionlyPlugin
{
    using System;
    using System.Drawing;
    using System.Drawing.Imaging;
    using System.IO;
    using System.Runtime.InteropServices;


    public static class ScreenshotHelper
    {
        [DllImport("User32.dll")]
        private static extern int GetSystemMetrics(int index);

        private const int SM_CXSCREEN = 0;
        private const int SM_CYSCREEN = 1;

        private static readonly string Folder =
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                         "Logi", "LogiPluginService", "PluginData", "Actionly");

        private static readonly string FileName = "screenshot.png";

        public static string ScreenshotPath => Path.Combine(Folder, FileName);

        public static void TakeScreenshot()
        {
            Directory.CreateDirectory(Folder);

            int width = GetSystemMetrics(SM_CXSCREEN);
            int height = GetSystemMetrics(SM_CYSCREEN);

            using var bmp = new Bitmap(width, height);
            using var g = Graphics.FromImage(bmp);
            g.CopyFromScreen(0, 0, 0, 0, new Size(width, height));

            bmp.Save(ScreenshotPath, ImageFormat.Png);
        }
    }
}
