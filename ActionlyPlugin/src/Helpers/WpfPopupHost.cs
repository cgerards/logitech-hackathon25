namespace Loupedeck.ExamplePlugin
{
    using System;
    using System.Threading;
    using System.Windows;
    using System.Windows.Threading;

    internal static class WpfPopupHost
    {
        private static readonly object Sync = new object();
        private static Thread _uiThread;
        private static Dispatcher _dispatcher;

        private static void EnsureStarted()
        {
            if (_dispatcher != null) return;
            lock (Sync)
            {
                if (_dispatcher != null) return;
                var started = new ManualResetEventSlim(false);

                _uiThread = new Thread(() =>
                {
                    // Create a single Application instance that will live for the lifetime of the thread.
                    var app = new Application
                    {
                        ShutdownMode = ShutdownMode.OnExplicitShutdown
                    };

                    _dispatcher = Dispatcher.CurrentDispatcher;

                    // Signal the creator that the dispatcher is ready.
                    started.Set();

                    // Start the Dispatcher event loop. We never call Shutdown() so the Application won't be "shutting down".
                    Dispatcher.Run();
                });

                _uiThread.SetApartmentState(ApartmentState.STA);
                _uiThread.IsBackground = true;
                _uiThread.Start();

                started.Wait();
            }
        }

        public static void ShowDialog(Func<Window> createWindow)
        {
            EnsureStarted();

            // Create and show the window on the UI dispatcher. ShowDialog will block that dispatcher until the window is closed,
            // but the caller here will also block until dispatcher.Invoke returns, so the behavior is synchronous for the caller.
            _dispatcher.Invoke(() =>
            {
                var w = createWindow();
                w.ShowDialog();
            });
        }

        public static void Invoke(Action action)
        {
            EnsureStarted();
            _dispatcher.Invoke(action);
        }
    }
}
