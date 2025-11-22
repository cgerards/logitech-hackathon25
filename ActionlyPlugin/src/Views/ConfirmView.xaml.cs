namespace Loupedeck.ActionlyPlugin.Views
{
    using System;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;

    using Loupedeck.ActionlyPlugin.Helpers;
    using Loupedeck.ActionlyPlugin.Helpers.Models;

    public partial class ConfirmView : UserControl
    {

        public AIResponse Response { get; set; }
        public event EventHandler CloseRequested;


        public ConfirmView(AIResponse response)
        {
            InitializeComponent();
            Response = response;
            DataContext = this;
            PluginLog.Info("ConfirmView initialized with response." + response.Explanation);
        }


        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            // Event auslösen
            CloseRequested?.Invoke(this, EventArgs.Empty);
        }


        private void Confirm_Click(Object sender, RoutedEventArgs e)
        {
            var win = Window.GetWindow(this) as PopUpWindow;
            PluginLog.Info("Confirm_Click triggered in ConfirmView.");
            win.DialogResult = true;
            win.Close();
        }

        private void Cancel_Click(Object sender, RoutedEventArgs e)
        {
            PluginLog.Info("Cancel_Click triggered in ConfirmView.");
            var win = Window.GetWindow(this);

            if (win != null)
            {
                AIResponseStore.Instance.Set(null);
                win.DialogResult = false;
                win.Close();
            }
        }
    }
}