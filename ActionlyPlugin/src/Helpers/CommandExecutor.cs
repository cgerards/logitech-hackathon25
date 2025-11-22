namespace Loupedeck.ActionlyPlugin.Helpers
{
    using System;

    using Loupedeck.ActionlyPlugin.Helpers.Models;

    internal class CommandExecutor
    {
        ClientApplication ClientApp;
        public CommandExecutor(ClientApplication clientApp) {
            ApplicationSwitcher.SwitchToProcess("olk");
            this.ClientApp = clientApp;
        }

        public void ExecuteCombination(AIResponse aiResponse)
        {

            foreach (var combo in aiResponse.Combinations)
            {
                Thread.Sleep(200);
                PluginLog.Info($"Combination to execute: {combo}");
                if (string.IsNullOrEmpty(combo))
                {
                    continue;
                }

                if (combo.Equals("Wait"))
                {
                    PluginLog.Info(combo + " — waiting 3 seconds");
                    Thread.Sleep(3000);
                    continue;
                }
                
                const string prefix = "String>";
                if (combo.StartsWith(prefix, StringComparison.OrdinalIgnoreCase) && combo.EndsWith("<", StringComparison.Ordinal))
                {
                    var content = combo.Substring(prefix.Length, combo.Length - prefix.Length - 1);
                    PluginLog.Info($"Detected String command — writing content: '{content}'");
                    this.writeString(content);
                    continue;
                }
                else
                {
                    this.parseShortCuts(combo.Split('+'));
                }
            }
        }

        private void parseShortCuts(string[] strs)
        {
            ModifierKey mods = ModifierKey.None;

            foreach (var raw in strs)
            {
                var token = raw.Trim();
                PluginLog.Info($"Parsing shortcut part: {token}");
                var isModifier = Enum.TryParse<ModifierKey>(token, out var mod);
                if (Enum.TryParse<ModifierKey>(token, true, out var parsedMod))
                {
                    mods |= parsedMod; // kombiniere die Modifier
                    PluginLog.Info($"Parsed modifier: {parsedMod} -> combined mods = {mods}");
                    continue;
                }
                if (Enum.TryParse<VirtualKeyCode>(token, true, out var parsedKey))
                {
                    PluginLog.Info($"Parsed key: {parsedKey} with modifiers {mods}");
                    // Einmalig senden: VirtualKey + alle gesammelten Modifier
                    this.ClientApp.SendKeyboardShortcut(parsedKey, mods);
                }

            }
        }

        private void writeString(string str)
        {
            foreach (char c in str)
            {
                var mapping = MapCharToKey(c);
                if (mapping.key == VirtualKeyCode.None)
                {
                    PluginLog.Warning($"Unmapped character: '{c}'");
                    continue;
                }
                PluginLog.Info($"Sending: '{mapping}'");

                this.ClientApp.SendKeyboardShortcut(mapping.key, mapping.mods);
                Thread.Sleep(100);
            }
        }

        private (VirtualKeyCode key, ModifierKey mods) MapCharToKey(char ch)
        {
            // normalize first for simple decisions
            // letters: KeyA..KeyZ, digits: Key0..Key9
            if (char.IsLetter(ch))
            {
                var isUpper = char.IsUpper(ch);
                var key = (VirtualKeyCode)Enum.Parse(typeof(VirtualKeyCode), "Key" + char.ToUpperInvariant(ch));
                return (key, isUpper ? ModifierKey.Shift : ModifierKey.None);
            }

            if (char.IsDigit(ch))
            {
                var key = (VirtualKeyCode)Enum.Parse(typeof(VirtualKeyCode), "Key" + ch);
                return (key, ModifierKey.None);
            }

            // explicit mapping for punctuation / special characters (US QWERTY assumptions)
            switch (ch)
            {
                case ' ':
                    return (VirtualKeyCode.Space, ModifierKey.None);

                // common punctuation mapped to dedicated VK or OEM keys
                case '=':
                    return (VirtualKeyCode.Equals, ModifierKey.None);

                case ',':
                    return (VirtualKeyCode.Comma, ModifierKey.None);
                case '<':
                    return (VirtualKeyCode.Comma, ModifierKey.Shift);

                case '.':
                    return (VirtualKeyCode.Period, ModifierKey.None);
                case '>':
                    return (VirtualKeyCode.Period, ModifierKey.Shift);

                // Oem2 is typically '/' and '?' on many layouts
                case '/':
                    return (VirtualKeyCode.Oem2, ModifierKey.None);
                case '?':
                    return (VirtualKeyCode.Oem2, ModifierKey.Shift);

                // Oem1 is typically ';' and ':' 
                case ';':
                    return (VirtualKeyCode.Oem1, ModifierKey.None);
                case ':':
                    return (VirtualKeyCode.Oem1, ModifierKey.Shift);

                // brackets and braces
                case '[':
                    return (VirtualKeyCode.Oem4, ModifierKey.None);
                case '{':
                    return (VirtualKeyCode.Oem4, ModifierKey.Shift);
                case ']':
                    return (VirtualKeyCode.Oem6, ModifierKey.None);
                case '}':
                    return (VirtualKeyCode.Oem6, ModifierKey.Shift);

                // backslash / pipe — Oem5 on many layouts
                case '\\':
                    return (VirtualKeyCode.Oem5, ModifierKey.None);
                case '|':
                    return (VirtualKeyCode.Oem5, ModifierKey.Shift);

                // quote / double quote
                case '\'':
                    return (VirtualKeyCode.Oem7, ModifierKey.None);
                case '\"':
                    return (VirtualKeyCode.Oem7, ModifierKey.Shift);

                // grave / tilde
                case '`':
                    return (VirtualKeyCode.Oem3, ModifierKey.None);
                case '~':
                    return (VirtualKeyCode.Oem3, ModifierKey.Shift);

                // minus / underscore
                case '-':
                    return (VirtualKeyCode.Minus, ModifierKey.None);
                case '_':
                    return (VirtualKeyCode.Minus, ModifierKey.Shift);

                // arithmetic keys
                case '*':
                    return (VirtualKeyCode.Multiply, ModifierKey.None);
                case '+':
                    return (VirtualKeyCode.Add, ModifierKey.None);

                // parentheses (US: Shift+9 / Shift+0)
                case '(':
                    return (VirtualKeyCode.Key9, ModifierKey.Shift);
                case ')':
                    return (VirtualKeyCode.Key0, ModifierKey.Shift);

                // common shifted number symbols (US)
                case '!':
                    return (VirtualKeyCode.Key1, ModifierKey.Shift);
                case '@':
                    return (VirtualKeyCode.Key2, ModifierKey.Shift);
                case '#':
                    return (VirtualKeyCode.Key3, ModifierKey.Shift);
                case '$':
                    return (VirtualKeyCode.Key4, ModifierKey.Shift);
                case '%':
                    return (VirtualKeyCode.Key5, ModifierKey.Shift);
                case '^':
                    return (VirtualKeyCode.Key6, ModifierKey.Shift);
                case '&':
                    return (VirtualKeyCode.Key7, ModifierKey.Shift);

                default:
                    return (VirtualKeyCode.None, ModifierKey.None);
            }
        }
    }
}
