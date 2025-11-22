namespace Loupedeck.ActionlyPlugin.Helpers
{
    using System.Threading.Tasks;

    // Lightweight stub implementation so the project builds without the external Google GenAI package.
    // Replace this with a proper implementation and add the required NuGet package when ready.
    public class GeminiClient
    {
        public GeminiClient()
        {
        }

        public Task<string> GenerateFromTextAndImageAsync(string systemPrompt, string userPrompt, string imagePath)
        {
            // TODO: Implement real call to Gemini. For now return a placeholder response so the plugin can compile.
            return Task.FromResult("[stub] Generated response would appear here.");
        }
    }
}
