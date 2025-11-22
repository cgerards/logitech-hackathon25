namespace Loupedeck.ActionlyPlugin
{
    using System;
    using System.Windows;
    using System.Windows.Forms;
    using System.Windows.Media;

    using Loupedeck.ActionlyPlugin.Helpers;
    using Loupedeck.ActionlyPlugin.Helpers.Models;

    // This class implements an example command that counts button presses.

    public class PromptCommand : PluginMultistateDynamicCommand
    {

        // Initializes the command class.
        public PromptCommand()
            : base(displayName: "Actionly", description: "Your AI Agent right in all MX-Master Mice!", groupName: "Commands")
        {
            this.AddState("Unleash the power of your MX Master with the seamless AI Agent!", "Initiate Prompt Session");
        }

        // This method is called when the user executes the command.
        protected override void RunCommand(String actionParameter)
        {
            WpfPopupHost.ShowDialogAndReturn(() =>
            {

                var popup = new PopUpWindow();

                // Place popup near mouse as a reasonable default
                var mousePos = Control.MousePosition;
                var source = PresentationSource.FromVisual(System.Windows.Application.Current?.MainWindow ?? new System.Windows.Window());
                double dpiX = 1.0, dpiY = 1.0;
                if (source?.CompositionTarget != null)
                {
                    dpiX = source.CompositionTarget.TransformToDevice.M11;
                    dpiY = source.CompositionTarget.TransformToDevice.M22;
                }

                popup.WindowStartupLocation = WindowStartupLocation.Manual;
                popup.Left = mousePos.X / dpiX + 10;
                popup.Top = mousePos.Y / dpiY - 10;

                popup.ShowActivated = true;
                popup.Topmost = true;

                return popup;
            });

            AIResponse response = AIResponseStore.Instance.Get();
            if (response != null)
            {
                CommandExecutor executor = new CommandExecutor(this.Plugin.ClientApplication, response);
                PluginLog.Info("Reponse: " + response);
                executor.ExecuteCombination();
            }
            AIResponseStore.Instance.Set(null);
        }

        // This method is called when Loupedeck needs to show the command on the console or the UI.
        protected override String GetCommandDisplayName(String actionParameter, PluginImageSize imageSize) =>
            $"Prompt Button!";
    }
}
