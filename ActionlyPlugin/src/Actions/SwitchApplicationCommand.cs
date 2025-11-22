namespace Loupedeck.ActionlyPlugin
{
    using System;
    using System.Threading.Tasks;

    using Loupedeck.ActionlyPlugin.Helpers;

    // This class implements an example command that counts button presses.

    public class SwitchApplicationCommand : PluginDynamicCommand
    {

        // Initializes the command class.
        public SwitchApplicationCommand()
            : base(displayName: "Firefox", description: "Switches to Firefox", groupName: "Commands")
        {
        }

        protected override void RunCommand(string actionParameter)
        {
            try
            {
                bool success = ApplicationSwitcher.SwitchToProcess("firefox");
                PluginLog.Info(success ? "Switched to Firefox!" : "Firefox not running.");
            }
            catch (Exception ex)
            {
                PluginLog.Error($"Failed to switch to Firefox: {ex}");
            }

            this.ActionImageChanged();
        }

        // This method is called when Loupedeck needs to show the command on the console or the UI.
        protected override String GetCommandDisplayName(String actionParameter, PluginImageSize imageSize) =>
            "Switch to Firefox";
    }
}
