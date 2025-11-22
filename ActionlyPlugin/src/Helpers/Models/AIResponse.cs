namespace Loupedeck.ActionlyPlugin.Helpers.Models
{
    public class AIResponse
    {

        public AIResponse(string[] strings) {
            Combinations = strings;
        }
        public String Explanation { get; set; }
        public String[] Combinations { get; set; }
    }
}
