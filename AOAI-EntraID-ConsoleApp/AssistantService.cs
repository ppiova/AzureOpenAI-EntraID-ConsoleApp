using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace AzureAOAI_EntraID_ConsoleApp
{
    // Class for assistant communication
    public class AssistantService
    {
        private readonly string _endpoint;
        private readonly string _token;

        public AssistantService(string endpoint, string token)
        {
            _endpoint = endpoint;
            _token = token;
        }

        public async Task<dynamic?> SendRequestAsync(List<object> messages)
        {
            // Prepare the payload for the POST request
            var payload = new
            {
                messages = messages.ToArray(),
                temperature = 0.7,
                top_p = 0.95,
                max_tokens = 800,
                stream = false
            };

            using (var httpClient = new HttpClient())
            {
                // Add the authorization header with the access token
                httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_token}");

                // Serialize the payload to JSON
                var jsonPayload = JsonConvert.SerializeObject(payload);
                var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

                // Send the POST request to the endpoint
                var response = await httpClient.PostAsync(_endpoint, content);

                if (response.IsSuccessStatusCode)
                {
                    // Read and deserialize the response content
                    var responseString = await response.Content.ReadAsStringAsync();
                    if (string.IsNullOrEmpty(responseString))
                    {
                        return null;
                    }
                    var responseData = JsonConvert.DeserializeObject<dynamic>(responseString);
                    return responseData;
                }
                else
                {
                    // Handle error responses
                    Console.WriteLine($"Error: {response.StatusCode}, {response.ReasonPhrase}");
                    var errorResponse = await response.Content.ReadAsStringAsync();

                    try
                    {
                        // Attempt to parse the error details
                        var errorData = JsonConvert.DeserializeObject<dynamic>(errorResponse);
                        var contentFilterResult = errorData?.error?.innererror?.content_filter_result;

                        if (contentFilterResult != null)
                        {
                            // Check for specific content filters
                            bool violenceFiltered = contentFilterResult?.violence?.filtered == true;
                            bool hateFiltered = contentFilterResult?.hate?.filtered == true;
                            bool sexualFiltered = contentFilterResult?.sexual?.filtered == true;
                            bool selfHarmFiltered = contentFilterResult?.self_harm?.filtered == true;

                            // Display appropriate messages based on the filter
                            if (violenceFiltered)
                            {
                                Console.WriteLine("Your request was filtered due to violent content.");
                            }
                            else if (hateFiltered)
                            {
                                Console.WriteLine("Your request was filtered due to hateful content.");
                            }
                            else if (sexualFiltered)
                            {
                                Console.WriteLine("Your request was filtered due to sexual content.");
                            }
                            else if (selfHarmFiltered)
                            {
                                Console.WriteLine("Your request was filtered due to self-harm content.");
                            }
                            else
                            {
                                Console.WriteLine("Your request was filtered by the assistant's policies.");
                            }
                        }
                        else
                        {
                            Console.WriteLine("Your request violates the assistant's policies, but no detailed information was obtained.");
                        }
                    }
                    catch (Exception)
                    {
                        Console.WriteLine("Your request was filtered, but the exact reason could not be determined.");
                    }

                    return null;
                }
            }
        }
    }
}
