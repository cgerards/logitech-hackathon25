namespace Loupedeck.ActionlyPlugin.Views
{
    using System;
    using System.Windows.Controls;
    using System.Windows.Input;

    public partial class LoadingView : UserControl
    {
        public LoadingView()
        {
            InitializeComponent();
            this.InnerInputTextBox.KeyDown += InnerInputTextBox_KeyDown;
        }

        private void InnerInputTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                this.SubmitRequested?.Invoke(this, EventArgs.Empty);
                e.Handled = true;
            }
        }

        public event EventHandler SubmitRequested;

        public string Text => this.InnerInputTextBox?.Text ?? string.Empty;

        public void FocusInput() => this.InnerInputTextBox?.Focus();
    }
}