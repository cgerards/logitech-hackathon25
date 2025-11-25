namespace Loupedeck.ActionlyPlugin.Views
{
    using System;
    using System.Windows.Controls;
    using System.Windows.Input;
    using System.Windows.Threading;
    using System.Windows;
    using System.IO;

    public partial class SettingsView : UserControl
    {

        public event EventHandler CloseRequested;

        private const string ModelFile = "model.txt";
        private const string ApiKeyFile = "apikey.txt";
        public string PluginPath { get; set; }

        public SettingsView(string PluginPath)
        {
            InitializeComponent();
            this.PluginPath = PluginPath;
            LoadSettings();

        }


        public void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            // Event auslösen
            CloseRequested?.Invoke(this, EventArgs.Empty);
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

        private void LoadSettings()
        {
            try
            {
                var pluginDataDirectory = this.PluginPath;

                PluginLog.Info("Loading settings..." + pluginDataDirectory);
                var modelFilePath = Path.Combine(pluginDataDirectory, ModelFile);
                var apiKeyFilePath = Path.Combine(pluginDataDirectory, ApiKeyFile);



                PluginLog.Info("Afer paths");
                if (!File.Exists(modelFilePath))
                {
                    File.WriteAllText(modelFilePath, string.Empty);
                    PluginLog.Info("Model file created: " + modelFilePath);
                }

                if (File.Exists(modelFilePath))
                {
                    PluginLog.Info("Model file exists");
                    string model = File.ReadAllText(modelFilePath);
                    foreach (ComboBoxItem item in MyComboBox.Items)
                    {
                        if (item.Tag?.ToString() == model)
                        {
                            MyComboBox.SelectedItem = item;
                            break;
                        }
                    }
                }

                PluginLog.Info("Afer model");
                if (!File.Exists(apiKeyFilePath))
                {
                    File.WriteAllText(apiKeyFilePath, string.Empty);
                    PluginLog.Info("API Key file created: " + apiKeyFilePath);
                }

                if (File.Exists(apiKeyFilePath))
                {
                    PluginLog.Info("API Key file exists");
                    var api = File.ReadAllText(apiKeyFilePath);
                    if(api != null)
                    {
                        ApiKeyTextBox.Text = api;
                    }
                }
                PluginLog.Info("Afer Key");

            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading settings: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void SendButton_Click(object sender, RoutedEventArgs e)
        {
            // Raise the same SubmitRequested event as pressing Enter
            this.SubmitRequested?.Invoke(this, EventArgs.Empty);
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            PluginLog.Info("Saving settings...");
            var selectedModel = (MyComboBox.SelectedItem as ComboBoxItem)?.Tag?.ToString() ?? "";
            var apiKey = ApiKeyTextBox.Text ?? "";

            try
            {
                var pluginDataDirectory = this.PluginPath;
                var modelFilePath = Path.Combine(pluginDataDirectory, ModelFile);
                var apiKeyFilePath = Path.Combine(pluginDataDirectory, ApiKeyFile);

                File.WriteAllText(modelFilePath, selectedModel);
                File.WriteAllText(apiKeyFilePath, apiKey);

                this.SubmitRequested?.Invoke(this, EventArgs.Empty);


            }
            catch (Exception ex)
            {
                PluginLog.Error("Error saving settings: " + ex.Message);
            }


        }
    }

}