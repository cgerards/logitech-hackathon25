namespace Loupedeck.ActionlyPlugin
{
    using System;
    using System.IO;
    using System.Threading.Tasks;
    using System.Windows;
    using System.Windows.Forms;
    using System.Windows.Input;
    using System.Windows.Media.Effects;
    using System.Windows.Threading;

    using Loupedeck.ActionlyPlugin.Helpers;
    using Loupedeck.ActionlyPlugin.Helpers.Models;
    using Loupedeck.ActionlyPlugin.Views;


    public enum PopUpState
    {
        Default,
        Confirm,
        Loading,
        Settings
    }



    public partial class PopUpWindow : Window
    {
        public String EnteredText { get; private set; }
        public String ApiKey { get; set; }
        public String Model { get; set; }
        public PopUpState CurrentState { get; private set; } = PopUpState.Default;
        public Task<AIResponse> AiResponseTask { get; set; }
        public AIResponse AiResponse { get; set; }
        public String PluginPath { get; set; }


        public PopUpWindow(string path)
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

            this.PluginPath = path;
            PluginLog.Info("PATH: " + path);

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

                        LoadSettings();

                        GeminiClient aiClient = new GeminiClient();


                        this.AiResponseTask = aiClient.GenerateFromTextAndImageAsync(this.ApiKey, this.EnteredText, this.Model);

                        // Switch UI to loading state while keeping the popup open.
                        SetState(PopUpState.Loading);
                    };

                    viewDefault.SettingsRequested += (s, e) =>
                    {
                        PluginLog.Info("Settings requested from DefaultView.");
                        // Switch to Settings view
                        SetState(PopUpState.Settings);
                    };

                    viewDefault.CloseRequested += (s, e) =>
                    {
                        // Handle close request from DefaultView
                        this.DialogResult = false;
                        this.Close();
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
                    this.Dispatcher.BeginInvoke(new Action(async () =>
                    {
                        AdjustWindowSize();
                        // If LoadingView exposes FocusInput, call it

                    }), DispatcherPriority.Loaded);

                    // Start non-blocking loading sequence that transitions to Confirm after delay
                    _ = Task.Run(async () =>
                    {
                        try
                        {
                            this.AiResponse = await this.AiResponseTask;
                            AIResponseStore.Instance.Set(this.AiResponse);
                            this.Dispatcher.Invoke(() => this.SetState(PopUpState.Confirm));

                        }
                        catch (Exception ex)
                        {
                            PluginLog.Error($"Error getting AI response: {ex.InnerException}");
                            this.Dispatcher.Invoke(() => this.SetState(PopUpState.Confirm));
                            // Fehlerbehandlung im UI
                            // z.B. zurück zum Default-State oder Fehlermeldung anzeigen

                        }
                    });
                    break;


                case PopUpState.Settings:
                    PluginLog.Info("Setting opöen");
                    var viewSett = new SettingsView(this.PluginPath);
                    ContentHost.Content = viewSett;

                    viewSett.CloseRequested += (s, e) =>
                    {
                        PluginLog.Info("Settings view requested close.");
                        // Return to Default view after closing settings
                        SetState(PopUpState.Default);
                    };

                    viewSett.SubmitRequested += (s, e) =>
                    {
                        PluginLog.Info("Settings view submitted.");
                        // Return to Default view after submitting settings
                        SetState(PopUpState.Default);
                    };

                    this.Dispatcher.BeginInvoke(new Action(() =>
                    {
                        AdjustWindowSize();
                        // If LoadingView exposes FocusInput, call it
                    }), DispatcherPriority.Loaded);


                    break;

                case PopUpState.Confirm:
                    PluginLog.Info("Displaying AI Response in Confirm View." + this.AiResponse.Explanation);
                    var viewConfirm = new ConfirmView(this.AiResponse);

                    ContentHost.Content = viewConfirm;
                    this.Dispatcher.BeginInvoke(new Action(() =>
                    {
                        AdjustWindowSize();
                        // If LoadingView exposes FocusInput, call it
                    }), DispatcherPriority.Loaded);


                    break;
            }
        }

        private void LoadSettings()
        {
            try
            {
                var modelFile = Path.Combine(this.PluginPath, "model.txt");
                var keyFile = Path.Combine(this.PluginPath, "apikey.txt");
                PluginLog.Info("Loading settings from " + modelFile + " and " + keyFile);

                if (File.Exists(modelFile))
                {
                    this.Model = File.ReadAllText(modelFile);
                }
                if (File.Exists(keyFile))
                {
                    this.ApiKey = File.ReadAllText(keyFile);
                }
            }
            catch (Exception ex)
            {
                PluginLog.Error("Error loading settings in popup: " + ex.Message);
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
                // Reduce added padding so popup feels tighter
                this.Width = Math.Max(240, contentWidth + 20);
                this.Height = Math.Max(90, contentHeight + 30);

                // Force layout update
                this.UpdateLayout();
            }
        }

        private void Close_Click(Object sender, RoutedEventArgs e)
        {
            AIResponseStore.Instance.Set(null);
            PluginLog.Info("PopUpWindow closed by user.");
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
