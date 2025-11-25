namespace Loupedeck.ActionlyPlugin.Helpers
{
    using System;
    using System.IO;
    using System.Text.Json;

    using Google.GenAI;
    using Google.GenAI.Types;

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
        public async Task<AIResponse> GenerateFromTextAndImageAsync(string apiKey, string userPrompt, string model)
        {

            //  Gemini Developer API
            var client = new Client(apiKey: apiKey);

            PluginLog.Info("Starting Gemini API call" + apiKey);

            if (!model.Equals("mock"))
            {

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
                            Role = "model",
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


                    PluginLog.Info("Using model " + model);

                    var response = await client.Models.GenerateContentAsync(
                         model: model,
                         contents: contents,
                         config: config);

                    PluginLog.Info("Response finished");

                    PluginLog.Info(response.Candidates[0].Content.Parts[0].Text);
                    var responseString = response.Candidates[0].Content.Parts[0].Text;

                    PluginLog.Info(responseString);

                    AIResponse aiResponse = JsonSerializer.Deserialize<AIResponse>(responseString);

                    return aiResponse;
                }
                catch (Exception ex)
                {
                    PluginLog.Error($"Error {ex.Message}");
                    throw ex;
                }

            }
            else
            {
                var aiResponse = new AIResponse
                {
                    Explanation = "This is a placeholder explanation.",
                    Combinations = new string[1]
                };

                await Task.Delay(5000);


                if (userPrompt.ToLower().Contains("outlook"))
                {
                    aiResponse.Explanation = "I add the provided timeline of HACKATUM into the Outlook Calendar";
                    aiResponse.Combinations = new String[] {
                    "Control + KeyN",
                    "Wait",
                    "String>Project Submission Deadline<",
                    "Tab",
                    "Tab",
                    "Tab",
                    "String>23.11.2025<",
                    "Tab",
                    "String>10:00<",
                    "Tab",
                    "String>10:00<",
                    "Tab",
                    "Control + KeyS",
                    "Wait",
                    "Control + KeyN",
                    "Wait",
                    "String>Project Pitches<",
                    "Tab",
                    "Tab",
                    "Tab",
                    "String>23.11.2025<",
                    "Tab",
                    "String>10:15<",
                    "Tab",
                    "String>12:30<",
                    "Tab",
                    "Control + KeyS",
                    "Wait",
                    "Control + KeyN",
                    "Wait",
                    "String>Final Pitches & Awards Ceremony<",
                    "Tab",
                    "Tab",
                    "Tab",
                    "String>23.11.2025<",
                    "Tab",
                    "String>14:00<",
                    "Tab",
                    "String>16:30<",
                    "Tab",
                    "Control + KeyS",
                    "Wait",

                };

                }

                if (userPrompt.ToLower().Contains("excel"))
                {
                    aiResponse.Explanation = "Based on the goal, I will navigate to the first empty row in the Excel list (Row 5), input the data extracted from the PDF (Year: 2025, Revenue: 124, Profit: 42, Cost: 82 (calculated as 124-42), Employees: 295). Then, I will use the 'Refresh All' shortcut to update the pivot table with the new data and switch to the 'Visualization' sheet.";
                    aiResponse.Combinations = [
                        "Control + Home",
                    "Control + ArrowDown",
                    "ArrowDown",
                    "String>2025<",
                    "Tab",
                    "String>124<",
                    "Tab",
                    "String>42<",
                    "Tab",
                    "String>82<",
                    "Tab",
                    "String>295<",
                    "Return",
                    "Control + Alt + F5",
                    "Control + PageDown"
                      ];
                }
                return aiResponse;
            }

            return null;

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
