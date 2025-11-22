namespace Loupedeck.ActionlyPlugin
{
    using System;
    using System.Threading.Tasks;

    using Loupedeck.ActionlyPlugin.Helpers;

    // This class implements an example command that counts button presses.

    public class CounterCommand : PluginDynamicCommand
    {

        // Initializes the command class.
        public CounterCommand()
            : base(displayName: "AI-Agent", description: "Starts the AI-Agent Task", groupName: "Commands")
        {
        }

        // This method is called when the user executes the command.
        protected override async void RunCommand(String actionParameter)
        {

            PluginLog.Info("LLM Response on the way.");
            var http = new GeminiClient();
            PluginLog.Info("LLM Response on the wa.");

            var test = await http.GenerateFromTextAndImageAsync("Hilf dem User", "Was ist auf dem Bild zu sehen?", "C:\\Users\\Lenovo\\AppData\\Local\\Logi\\LogiPluginService\\PluginData\\Actionly\\bild.png");

            PluginLog.Info("LLM Response received." + test);
            PluginLog.Info("LLM Request sent.");

        }

        // This method is called when Loupedeck needs to show the command on the console or the UI.
        protected override String GetCommandDisplayName(String actionParameter, PluginImageSize imageSize) =>
            $"Press Counter{Environment.NewLine}";
    }
}
