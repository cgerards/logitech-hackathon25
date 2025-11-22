namespace Loupedeck.ActionlyPlugin
{
    using System;
    using System.Threading.Tasks;
    using System.Windows;
    using System.Windows.Input;
    using System.Windows.Media.Effects;
    using System.Windows.Threading;
    using Loupedeck.ActionlyPlugin.Views;



    public enum PopUpState
    {
        Default,
        Confirm,
        Loading
    }



    public partial class PopUpWindow : Window
    {
        public String EnteredText { get; private set; }
        public PopUpState CurrentState { get; private set; } = PopUpState.Default;

        public PopUpWindow()
        {
            InitializeComponent();

            // register drop shadow
            this.Resources["_DropShadow"] = new DropShadowEffect
            {
                Color = System.Windows.Media.Colors.Black,
                BlurRadius = 12,
                Opacity = 0.5,
                ShadowDepth = 2
            };

            // default content
            this.SetState(PopUpState.Default);
        }


        public void SetState(PopUpState state)
        {
            CurrentState = state;

            switch (state)
            {
                case PopUpState.Default:
                    var viewDefault = new DefaultView();
                    viewDefault.SubmitRequested += (s, e) =>
                    {
                        // Capture entered text but do NOT set DialogResult or close the window here.
                        // Setting DialogResult when shown with ShowDialog() closes the window automatically.
                        this.EnteredText = viewDefault.Text;
                        PluginLog.Info($"Prompt value is {this.EnteredText}");

                        // Switch UI to loading state while keeping the popup open.
                        SetState(PopUpState.Loading);
                    };

                    ContentHost.Content = viewDefault;
                    // Defer sizing/focus until WPF has measured the new content
                    this.Dispatcher.BeginInvoke(new Action(() =>
                    {
                        AdjustWindowSize();
                        viewDefault.FocusInput();
                    }), DispatcherPriority.Loaded);
                    break;

                case PopUpState.Loading:
                    var viewLoading = new LoadingView();
                    ContentHost.Content = viewLoading;
                    this.Dispatcher.BeginInvoke(new Action(() =>
                    {
                        AdjustWindowSize();
                        // If LoadingView exposes FocusInput, call it

                    }), DispatcherPriority.Loaded);

                    // Start non-blocking loading sequence that transitions to Confirm after delay
                    _ = StartLoadingSequenceAsync();
                    break;

                case PopUpState.Confirm:
                    var viewConfirm = new ConfirmView();
                    ContentHost.Content = viewConfirm;
                    this.Dispatcher.BeginInvoke(new Action(() =>
                    {
                        AdjustWindowSize();
                        // If LoadingView exposes FocusInput, call it
                    }), DispatcherPriority.Loaded);
                    break;
            }
        }

        private async Task StartLoadingSequenceAsync()
        {
            try
            {
                // Give UI a moment to render the loading view
                await Task.Delay(200);

                // Simulate work for 5 seconds without blocking the UI thread
                await Task.Delay(5000);

                // Only transition if still in Loading state (user may have closed or changed state)
                if (this.CurrentState == PopUpState.Loading)
                {
                    // Ensure state transition happens on UI thread
                    this.Dispatcher.Invoke(() => this.SetState(PopUpState.Confirm));
                }
            }
            catch (TaskCanceledException)
            {
                // ignore cancellation if any
            }
            catch (Exception ex)
            {
                PluginLog.Error($"Error during loading sequence: {ex}");
            }
        }

        private void AdjustWindowSize()
        {
            // Size window to hosted content with some padding. Use DesiredSize after Measure to get a reliable size.
            if (ContentHost.Content is FrameworkElement fe)
            {
                fe.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
                var contentWidth = (!double.IsNaN(fe.Width) && fe.Width > 0) ? fe.Width : fe.DesiredSize.Width;
                var contentHeight = (!double.IsNaN(fe.Height) && fe.Height > 0) ? fe.Height : fe.DesiredSize.Height;

                // Provide sensible minimums so the window is not too small
                this.Width = Math.Max(260, contentWidth + 40);
                this.Height = Math.Max(110, contentHeight + 80);

                // Force layout update
                this.UpdateLayout();
            }
        }

        private void Close_Click(Object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }

        private void Window_Loaded(Object sender, RoutedEventArgs e)
        {
            // Focus input when window is shown
            if (ContentHost.Content is DefaultView dv)
            {
                dv.FocusInput();
            }
        }
    }
}
