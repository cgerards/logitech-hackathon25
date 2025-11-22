namespace Loupedeck.ActionlyPlugin.Helpers.Models
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    internal class AIResponse
    {

        public AIResponse(params string[] combinations)
        {
            this.Combinations = combinations ?? Array.Empty<string>();
        }
        public string Explanation { get; set; }
        public string[] Combinations { get; set; }

        public string[] returnMockData()
        {
            return ["VK_CONTROL+G", "String>D10<",];
        }
    }
}
