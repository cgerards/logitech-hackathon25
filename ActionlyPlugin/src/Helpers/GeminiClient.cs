namespace Loupedeck.ActionlyPlugin.Helpers
{
    using System;
    using System.IO;
    using System.Threading.Tasks;
    using System.Text.Json;

    using Google.GenAI;
    using Google.GenAI.Types;
    using System.IO;

    using Loupedeck.ActionlyPlugin.Helpers.Models;
    using System.Reflection;

    public class GeminiClient
    {
        public GeminiClient()
        {
        }

        /// <summary>
        /// Sendet system prompt, user prompt und ein Bild (lokaler Pfad) an Gemini und gibt den Text der Antwort zurück.
        /// </summary>
        public async Task<AIResponse> GenerateFromTextAndImageAsync(string systemPrompt, string userPrompt)
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
            "Explanation", new Schema { Type = Google.GenAI.Types.Type.STRING, Title = "Explanation" }
          },
          {
            "Combinations", new Schema { Type = Google.GenAI.Types.Type.ARRAY,
                Title = "Combinations",
            Items = new Schema{ Type = Google.GenAI.Types.Type.STRING},
            }
          }
    },
                    PropertyOrdering = ["Explanation", "Combinations"],
                    Required = ["Explanation", "Combinations"],
                    Title = "CountryInfo",
                    Type = Google.GenAI.Types.Type.OBJECT
                };

                // Define a generation config
                GenerateContentConfig config = new()
                {
                    HttpOptions = new HttpOptions
                    {
                        Timeout = 120000
                    },
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
                    TopP = 0.8,
                    TopK = 40,
                };

                var contents = new List<Content>();
                try
                {
                    string prompt = Prompt.TEXT;
                    contents.Add(new Content
                    {
                        Role = "system",
                        Parts = [
          new Part { Text = prompt}
    ]
                    });

                }
                catch (Exception ex)
                {
                    PluginLog.Info("Could not read prompt.txt, using default prompt. " + ex.Message);
                }
                

                contents.Add(new Content
                {
                    Role = "user",
                    Parts = [
                          new Part { Text = userPrompt }
                    ]
                });
                
                try
                {
                    
                    ScreenshotHelper.TakeScreenshot();

                    byte[] imageBytes = File.ReadAllBytes(ScreenshotHelper.ScreenshotPath);



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
                }
                catch (Exception ex)
                {
                    PluginLog.Error("Could not read screenshot image, proceeding without image. " + ex.Message);
                    return null;
                }
                

                PluginLog.Info("Before response");

                var response = await client.Models.GenerateContentAsync(
                     model: "gemini-3-pro-preview",
                     contents: contents,
                     config: config);


                PluginLog.Info(response.Candidates[0].Content.Parts[0].Text);
                var responseString = response.Candidates[0].Content.Parts[0].Text;

                PluginLog.Info(responseString);

                AIResponse aiResponse = JsonSerializer.Deserialize<AIResponse>(responseString);

                PluginLog.Info("Explanation: " + aiResponse.Explanation);

                return aiResponse;

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error during content generation: {ex.Message}\n\n {ex.InnerException}");
                return null;
            }
        }

        private string ReadPrompt()
        {
            var assembly = Assembly.GetExecutingAssembly();

            // Namespace + Dateiname → sehr wichtig!
            string resourceName = "Loupedeck.ActionlyPlugin.Helpers.prompt.txt";

            using Stream stream = assembly.GetManifestResourceStream(resourceName)
                ?? throw new Exception("Resource nicht gefunden!");

            using StreamReader reader = new StreamReader(stream);
            return reader.ReadToEnd();
        }
    }
}
