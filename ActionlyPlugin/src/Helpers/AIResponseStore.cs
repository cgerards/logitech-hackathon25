namespace Loupedeck.ActionlyPlugin.Helpers
{
    using System;

    using Loupedeck.ActionlyPlugin.Helpers.Models;

    internal class AIResponseStore
    {
        public static AIResponseStore Instance { get; } = new AIResponseStore();
        private readonly object _sync = new object();
        private AIResponse? _value;

        public event EventHandler<AIResponse>? ResponseSet;

        private AIResponseStore() { }

        // Setzt die AIResponse; überschreibt vorhandene Werte
        public void Set(AIResponse response)
        {
            //PluginLog.Info("SET IN STORE");
            //if (response is null)
            //    return;

            lock (this._sync)
            {
                this._value = response;
            }

            try
            {
                this.ResponseSet?.Invoke(this, response);
            }
            catch (Exception ex)
            {
                PluginLog.Error(ex, "Error in AIResponseStore.ResponseSet handler");
            }
        }

        public AIResponse? Get()
        {
            lock (this._sync)
                return this._value;
        }

        // Liefert die AIResponse und löscht sie atomar (consuming get)
        public bool TryGetAndClear(out AIResponse? response)
        {
            lock (this._sync)
            {
                response = this._value;
                this._value = null;
                return response != null;
            }
        }

        // Prüft, ob aktuell ein Wert gesetzt ist
        public bool HasValue
        {
            get { lock (this._sync) return this._value != null; }
        }
    }
}
