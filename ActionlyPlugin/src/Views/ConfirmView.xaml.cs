namespace Loupedeck.ActionlyPlugin.Views
{
    using System;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;
    using Loupedeck.ActionlyPlugin.Helpers.Models;


    public partial class ConfirmView : UserControl
    {

        public AIResponse Response { get; set; }

        public ConfirmView(AIResponse response)
        {
            InitializeComponent();
            Response = response;
            DataContext = this;
            PluginLog.Info("ConfirmView initialized with response." + response.Explanation);
        }



        private void Confirm_Click(Object sender, RoutedEventArgs e)
        {
            // TODO!
        }
    }
}