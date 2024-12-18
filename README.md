# Azure Open AI Rest API -  EntraID -  ConsoleApp

## Project Description
Azure OpenAI EntraID ConsoleApp is a console application written in C# that integrates with Azure OpenAI REST API and Entra ID services. The application demonstrates how to authenticate with Azure services and interact with an AI assistant via the command line.

## Key Features
- Authenticate with Azure using Tenant ID, Client ID, and Client Secret.
- Communicate with an AI assistant using the command line.
- Optionally include images in queries to the AI assistant.

## Installation

### Prerequisites
- .NET SDK (version 6.0 or higher)
- Azure account with necessary permissions
- Configuration settings for Azure services

### Setup
1. Clone the repository:
   ```bash
   git clone https://github.com/ppiova/AzureOpenAI-EntraID-ConsoleApp.git
   cd AzureOpenAI-EntraID-ConsoleApp
   ```

2. Configure the application:
   - Create a `appsettings.json` file in the root directory with the following content:
     ```json
     {
       "AzureSettings": {
         "TenantId": "<Your Tenant ID>",
         "ClientId": "<Your Client ID>",
         "ClientSecret": "<Your Client Secret>",
         "Resource": "<Your Resource>",
         "Endpoint": "<Your Endpoint>"
       }
     }
     ```

## Usage
1. Run the application:
   ```bash
   dotnet run --project AOAI-EntraID-ConsoleApp
   ```

2. Follow the prompts to interact with the AI assistant and optionally include images in your queries.

## Example
```bash
[assistant]: Hello! How can I assist you today?
Do you want to include an image in the query? (y/n)
Enter your question (type 'exit' to end):
```

## Resources
- [How to configure Azure OpenAI Service with Microsoft Entra ID authentication](https://learn.microsoft.com/en-us/azure/ai-services/openai/how-to/managed-identity?WT.mc_id=AI-MVP-5004753)
- [Azure OpenAI Service REST API reference](https://learn.microsoft.com/en-us/azure/ai-services/openai/reference?WT.mc_id=AI-MVP-5004753)


## Contributing
If you wish to contribute to the project, please fork the repository and create a pull request with your changes.

## License
This project is licensed under the MIT License.

## Contact
For any questions or inquiries, please contact the repository owner.

