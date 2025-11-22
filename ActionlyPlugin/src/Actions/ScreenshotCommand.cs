namespace Loupedeck.ActionlyPlugin
{
    using System;
    using System.Drawing;
    using System.Drawing.Imaging;
    using System.IO;
    using System.Runtime.InteropServices;

    public class ScreenShotCommand : PluginDynamicCommand
    {
        public ScreenShotCommand()
            : base(displayName: "Take Screenshot", description: "Take a screenshot", groupName: "Commands")
        {
        }

        protected override void RunCommand(string actionParameter)
        {
            try
            {
                // Take the screenshot (always overwrites the same file)
                ScreenshotHelper.TakeScreenshot();

                // Optional: log the path
                PluginLog.Info($"Screenshot saved: {ScreenshotHelper.ScreenshotPath}");
            }
            catch (Exception ex)
            {
                PluginLog.Error($"Screenshot failed: {ex}");
            }

            // Notify Loupedeck that the command image / name may have changed
            this.ActionImageChanged();
        }

        protected override string GetCommandDisplayName(string actionParameter, PluginImageSize imageSize)
        {
            return "Take Screenshot";
        }
    }
}
