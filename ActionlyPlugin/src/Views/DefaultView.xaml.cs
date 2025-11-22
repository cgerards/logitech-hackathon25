namespace Loupedeck.ActionlyPlugin.Views
{
    using System;
    using System.Windows.Controls;
    using System.Windows.Input;
    using System.Windows.Threading;
    using System.Windows;

    public partial class DefaultView : UserControl
    {
        public DefaultView()
        {
            InitializeComponent();
            this.InnerInputTextBox.KeyDown += InnerInputTextBox_KeyDown;
        }

        private void InnerInputTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            // If Enter is pressed without Shift, submit. If Shift+Enter, insert newline.
            if (e.Key == Key.Enter && (Keyboard.Modifiers & ModifierKeys.Shift) == 0)
            {
                this.SubmitRequested?.Invoke(this, EventArgs.Empty);
                e.Handled = true;
            }
        }

        public event EventHandler SubmitRequested;

        public string Text => this.InnerInputTextBox?.Text ?? string.Empty;

        public void FocusInput()
        {
            // Ensure focus request runs on the UI dispatcher and sets keyboard focus explicitly
            this.Dispatcher.BeginInvoke(DispatcherPriority.Input, new Action(() =>
            {
                this.InnerInputTextBox?.Focus();
                Keyboard.Focus(this.InnerInputTextBox);
            }));
        }

        private void SendButton_Click(object sender, RoutedEventArgs e)
        {
            // Raise the same SubmitRequested event as pressing Enter
            this.SubmitRequested?.Invoke(this, EventArgs.Empty);
        }
    }
}