using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Effects;

namespace Loupedeck.ExamplePlugin
{
    public partial class PopUpWindow : Window
    {
        public string EnteredText { get; private set; }

        public PopUpWindow()
        {
            InitializeComponent();

            // Create a simple drop shadow resource in code-behind so the XAML can reference it.
            var dropShadow = new DropShadowEffect
            {
                Color = System.Windows.Media.Colors.Black,
                BlurRadius = 12,
                Opacity = 0.5,
                ShadowDepth = 2
            };

            this.Resources["_DropShadow"] = dropShadow;
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }

        private void Ok_Click(object sender, RoutedEventArgs e)
        {
            this.EnteredText = this.InputTextBox?.Text ?? string.Empty;
            this.DialogResult = true;
            this.Close();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            this.InputTextBox?.Focus();
        }

        private void InputTextBox_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                Ok_Click(this, new RoutedEventArgs());
                e.Handled = true;
            }
            else if (e.Key == Key.Escape)
            {
                Close_Click(this, new RoutedEventArgs());
                e.Handled = true;
            }
        }
    }
}
