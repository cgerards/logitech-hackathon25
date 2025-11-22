namespace Loupedeck.ActionlyPlugin.Helpers.Models
{
    public class AIResponse
    {
        public AIResponse()
        {
            this.Explanation = string.Empty;
            this.Combinations = System.Array.Empty<string>();
        }
        public AIResponse(string[] strings) {
            Combinations = strings;
        }
        public String Explanation { get; set; }
        public String[] Combinations { get; set; }
    }
}
