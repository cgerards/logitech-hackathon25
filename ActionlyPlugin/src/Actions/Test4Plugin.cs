namespace Loupedeck.ActionlyPlugin
{
    using System;

    // Minimal multistate command example: toggles between On and Off states.
    public class ToggleMultistateDynamicCommand : PluginMultistateDynamicCommand
    {
        private readonly string[] _states = new[] { "On", "Off" };
        private int _currentStateIndex = 0;

        private string StateName => this._states[this._currentStateIndex];

        public ToggleMultistateDynamicCommand()
            : base(displayName: "Toggle Multistate", description: null, groupName: "Test")
        {
            // Register states with the SDK (so UI knows there are multiple states)
            foreach (var s in this._states)
            {
                this.AddState(s, $"Set state to {s}");
            }

            this._currentStateIndex = 0; // default to first state
        }

        // This is called when the user presses the action. Toggle the current state and notify the host.
        protected override void RunCommand(String actionParameter)
        {
            this._currentStateIndex = (this._currentStateIndex + 1) % this._states.Length;
            // don't assign to StateName (it's read-only); update index and notify host
            this.ActionImageChanged(); // notify UI that display (name/image) may have changed
            PluginLog.Info($"Multistate toggled to: {this._states[this._currentStateIndex]}");
            PluginLog.Info($"Corresponding string: {this.StateName}");
        }

        protected override String GetCommandDisplayName(String actionParameter, PluginImageSize imageSize) =>
            $"Press Counter{Environment.NewLine}{this.StateName}";
    }
}