namespace Loupedeck.ActionlyPlugin.Helpers
{
    using System;
    using System.Threading.Tasks;

    using Google.GenAI;
    using Google.GenAI.Types;
    using System.IO;


    public class GeminiClient
    {

        public GeminiClient()
        {

        }

        /// <summary>
        /// Sendet system prompt, user prompt und ein Bild (lokaler Pfad) an Gemini und gibt den Text der Antwort zurück.
        /// </summary>
        public async Task<string> GenerateFromTextAndImageAsync(string systemPrompt, string userPrompt, string imagePath)
        {

            //  Gemini Developer API
            var client = new Client(apiKey: "AIzaSyCJGcQzSHRi-_jfCGlFxdGYhBDoQNTXDog");

            try
            {
                // Define a response schema
                Schema countryInfo = new()
                {
                    Properties = new Dictionary<string, Schema> {
          {
            "explanation", new Schema { Type = Google.GenAI.Types.Type.STRING, Title = "explanation" }
          },
          {
            "combinations", new Schema { Type = Google.GenAI.Types.Type.ARRAY,
                Title = "combinations",
            Items = new Schema{ Type = Google.GenAI.Types.Type.STRING},
            }
          }
    },
                    PropertyOrdering = ["explanation", "combinations"],
                    Required = ["explanation", "combinations"],
                    Title = "CountryInfo",
                    Type = Google.GenAI.Types.Type.OBJECT
                };

                // Define a generation config
                GenerateContentConfig config = new()
                {
                    ResponseSchema = countryInfo,
                    ResponseMimeType = "application/json",
                    SystemInstruction = new Content
                    {
                        Parts = [
                              new Part {Text = "Explain the process to get to the user specified goal. In combinations put the keyboard combinations for it."}
                        ]
                    },
                    MaxOutputTokens = 5000,
                    Temperature = 1,
                };

                var contents = new List<Content>();

                contents.Add(new Content
                {
                    Role = "system",
                    Parts = [
                          new Part { Text = "I am a really helpful assistant for excel spreadsheets." }
                    ]
                });

                contents.Add(new Content
                {
                    Role = "user",
                    Parts = [
                          new Part { Text = "How do I make a text bold?" }
                    ]
                });


                byte[] imageBytes = File.ReadAllBytes("C:\\Users\\Lenovo\\Pictures\\Screenshots\\screen.png");



                contents.Add(new Content
                {
                    Parts = [
                          new Part
          {
              InlineData = new Google.GenAI.Types.Blob
              {
                  MimeType = "image/png",
                  Data = imageBytes
              }
          }
                          ]
                });



                var response = await client.Models.GenerateContentAsync(
                     model: "gemini-3-pro-preview",
                     contents: contents,
                     config: config);


                Console.WriteLine(response);
                Console.WriteLine(response.Candidates[0].Content.Parts[0].Text);
                return response.Candidates[0].Content.Parts[0].Text;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error during content generation: {ex.Message}\n\n {ex.InnerException}");
                return $"Error: {ex.Message}";
            }
        }
    }
}
