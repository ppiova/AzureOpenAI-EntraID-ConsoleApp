using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace AzureAOAI_EntraID_ConsoleApp
{
    class Program
    {
        static async Task Main()
        {
            // Build the configuration from appsettings.json and environment variables
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables();

            IConfiguration config = builder.Build();

            // Retrieve configuration settings or throw an exception if not configured
            var tenantId = config["AzureSettings:TenantId"]
                ?? throw new InvalidOperationException("TenantId is not configured");
            var clientId = config["AzureSettings:ClientId"]
                ?? throw new InvalidOperationException("ClientId is not configured");
            var clientSecret = config["AzureSettings:ClientSecret"]
                ?? throw new InvalidOperationException("ClientSecret is not configured");
            var resource = config["AzureSettings:Resource"]
                ?? throw new InvalidOperationException("Resource is not configured");
            var endpoint = config["AzureSettings:Endpoint"]
                ?? throw new InvalidOperationException("Endpoint is not configured");

            // Instantiate the authentication service and acquire the token
            var authService = new AzureAuthenticationService(tenantId, clientId, clientSecret, resource);
            string token = await authService.GetAccessTokenAsync();

            // Instantiate the assistant service
            var assistantService = new AssistantService(endpoint, token);

            // Display the assistant's initial message
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("[assistant]: Hello! How can I assist you today?");
            Console.ResetColor();

            // Initialize the conversation messages
            var messages = new List<object>
            {
                new {
                    role = "system",
                    content = new object[]
                    {
                        new {
                            type = "text",
                            text = "You are an AI assistant that helps people find information."
                        }
                    }
                },
                new {
                    role = "assistant",
                    content = new object[]
                    {
                        new {
                            type = "text",
                            text = "Hello! How can I assist you today?"
                        }
                    }
                }
            };

            // Prompt the user to include an image in the query
            Console.WriteLine("Do you want to include an image in the query? (y/n)");
            string? inputInclude = Console.ReadLine();
            string includeResponse = (inputInclude ?? "").Trim().ToLower();
            bool includeImage = includeResponse == "y";

            string? encodedImage = null;
            if (includeImage)
            {
                // Ask for the image path
                Console.WriteLine("Enter the image path:");
                string? imagePathInput = Console.ReadLine();
                string imagePath = (imagePathInput ?? "").Trim();
                // Debug statements
                Console.WriteLine($"Image path entered: '{imagePath}'");
                
                if (!File.Exists(imagePath))
                {
                    Console.WriteLine("The image path does not exist. Proceeding without an image.");
                    includeImage = false;
                }
                else
                {
                    // Encode the image to a Base64 string
                    encodedImage = Convert.ToBase64String(File.ReadAllBytes(imagePath));
                }
            }

            Console.WriteLine("Enter your question (type 'exit' to end):");
            while (true)
            {
                // Read user input
                string? userInputRead = Console.ReadLine();
                if (userInputRead is null)
                {
                    Console.WriteLine("Null input detected. Terminating the program...");
                    break;
                }

                string userInput = userInputRead.Trim();
                if (userInput.ToLower() == "exit") break;

                var userContent = new List<object>();

                // Include the image in the first message if applicable
                if (includeImage && messages.Count == 2 && encodedImage != null)
                {
                    userContent.Add(new
                    {
                        type = "image_url",
                        image_url = new
                        {
                            url = $"data:image/jpeg;base64,{encodedImage}"
                        }
                    });
                }

                // Add the user's text input to the content
                userContent.Add(new
                {
                    type = "text",
                    text = userInput
                });

                // Add the user's message to the conversation
                messages.Add(new
                {
                    role = "user",
                    content = userContent.ToArray()
                });

                // Display the user's message
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine($"[user]: {userInput}");
                Console.ResetColor();

                // Send the request to the assistant and get the response
                var responseData = await assistantService.SendRequestAsync(messages);

                if (responseData != null)
                {
                    // Extract the assistant's message from the response
                    var assistantMessage = responseData?.choices?[0]?.message;
                    var assistantRole = assistantMessage?.role;
                    var assistantContent = assistantMessage?.content;

                    if (assistantRole != null && assistantContent != null)
                    {
                        // Display the assistant's response
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.WriteLine($"[{assistantRole}]: {assistantContent}");
                        Console.ResetColor();

                        // Display token usage information
                        var usage = responseData?.usage;
                        if (usage != null)
                        {
                            Console.ForegroundColor = ConsoleColor.Green;
                            Console.WriteLine("Usage:");
                            Console.WriteLine($"  completion_tokens: {usage?.completion_tokens}");
                            Console.WriteLine($"  prompt_tokens: {usage?.prompt_tokens}");
                            Console.WriteLine($"  total_tokens: {usage?.total_tokens}");
                            Console.ResetColor();
                        }

                        // Add the assistant's response to the conversation
                        messages.Add(new
                        {
                            role = assistantRole,
                            content = assistantContent
                        });
                    }
                    else
                    {
                        Console.WriteLine("No valid response was obtained from the assistant.");
                    }
                }
                else
                {
                    Console.WriteLine("No response from the assistant or an error occurred.");
                }

                // Prompt for the next question
                Console.WriteLine("Enter your next question (or 'exit' to finish):");
            }

            Console.WriteLine("Program terminated.");
        }
    }
}
