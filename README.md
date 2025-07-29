# Azure OpenAI Console App with Entra ID Authentication

A .NET 8.0 console application that demonstrates secure integration with Azure OpenAI services using Entra ID (Azure Active Directory) authentication. This application supports both text and image-based conversations with Azure OpenAI models while maintaining proper authentication and security practices.

## Features

- 🔐 **Secure Authentication**: Uses Microsoft Identity Platform (Entra ID) with client credentials flow
- 💬 **Interactive Chat**: Maintains conversation history for natural dialogue
- 🖼️ **Multimodal Support**: Process both text and image inputs in your queries
- 📊 **Token Usage Tracking**: Monitor API usage and costs
- 🛡️ **Content Filtering**: Proper handling of Azure OpenAI content filters
- ⚙️ **Configuration Management**: Flexible configuration via JSON and environment variables

## Prerequisites

- [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0) or later
- Azure subscription with access to:
  - Azure OpenAI Service
  - Entra ID (Azure Active Directory)
- Visual Studio 2022 or Visual Studio Code (optional)

## Azure Setup

### 1. Create Azure OpenAI Resource

1. In the Azure portal, create an Azure OpenAI resource
2. Deploy a model (e.g., GPT-4, GPT-4 Vision) in Azure OpenAI Studio
3. Note the endpoint URL and deployment name

### 2. Create Entra ID App Registration

1. Go to **Azure Active Directory** > **App registrations** > **New registration**
2. Provide a name (e.g., "Azure OpenAI Console App")
3. Select **Accounts in this organizational directory only**
4. Click **Register**
5. Note the **Application (client) ID** and **Directory (tenant) ID**
6. Go to **Certificates & secrets** > **New client secret**
7. Create a secret and copy its **Value** (not the Secret ID)

### 3. Configure API Permissions

1. In your app registration, go to **API permissions**
2. Click **Add a permission** > **APIs my organization uses**
3. Search for and select **Cognitive Services**
4. Select **Application permissions** > **user_impersonation**
5. Click **Add permissions**
6. Click **Grant admin consent** for your directory

### 4. Assign Azure OpenAI Role

1. Go to your Azure OpenAI resource
2. Select **Access control (IAM)** > **Add** > **Add role assignment**
3. Select **Cognitive Services OpenAI User** role
4. Assign access to your app registration created above

## Configuration

### Method 1: Using appsettings.json

1. Open `appsettings.json` in the project
2. Replace the placeholder values with your Azure configuration:

```json
{
  "AzureSettings": {
    "TenantId": "your-tenant-id-here",
    "ClientId": "your-client-id-here", 
    "ClientSecret": "your-client-secret-here",
    "Resource": "https://cognitiveservices.azure.com/.default",
    "Endpoint": "https://your-endpoint-here.openai.azure.com/openai/deployments/your-deployment-id/chat/completions?api-version=2024-02-15-preview"
  }
}
```

### Method 2: Using Environment Variables

Set the following environment variables:

- `AzureSettings__TenantId`
- `AzureSettings__ClientId`
- `AzureSettings__ClientSecret`
- `AzureSettings__Resource`
- `AzureSettings__Endpoint`

### Configuration Parameters

| Parameter | Description | Example |
|-----------|-------------|---------|
| `TenantId` | Your Azure AD tenant ID | `12345678-1234-1234-1234-123456789012` |
| `ClientId` | Application (client) ID from app registration | `87654321-4321-4321-4321-210987654321` |
| `ClientSecret` | Client secret value from app registration | `your-secret-value` |
| `Resource` | Azure Cognitive Services scope | `https://cognitiveservices.azure.com/.default` |
| `Endpoint` | Azure OpenAI deployment endpoint | `https://myopenai.openai.azure.com/openai/deployments/gpt-4/chat/completions?api-version=2024-02-15-preview` |

## Installation and Setup

1. **Clone the repository**:
   ```bash
   git clone https://github.com/ppiova/AzureOpenAI-EntraID-ConsoleApp.git
   cd AzureOpenAI-EntraID-ConsoleApp
   ```

2. **Restore dependencies**:
   ```bash
   dotnet restore
   ```

3. **Configure settings** (see Configuration section above)

4. **Build the project**:
   ```bash
   dotnet build
   ```

5. **Run the application**:
   ```bash
   dotnet run --project AOAI-EntraID-ConsoleApp
   ```

## Usage

### Basic Text Conversation

1. Run the application
2. When prompted about including an image, type `n` and press Enter
3. Start chatting with the AI assistant
4. Type `exit` to end the conversation

```
Do you want to include an image in the query? (y/n)
n
Enter your question (type 'exit' to end):
Hello, how are you?
[user]: Hello, how are you?
[assistant]: Hello! I'm doing well, thank you for asking. I'm here and ready to help you with any questions or tasks you might have...
```

### Image-Based Conversation

1. Run the application
2. When prompted about including an image, type `y` and press Enter
3. Enter the path to your image file (e.g., `images/peoplebeach.jpg`)
4. Ask questions about the image

```
Do you want to include an image in the query? (y/n)
y
Enter the image path:
images/peoplebeach.jpg
Enter your question (type 'exit' to end):
What do you see in this image?
[user]: What do you see in this image?
[assistant]: I can see a group of people enjoying time at a beach...
```

### Sample Images

The project includes sample images in the `images/` folder that you can use for testing:
- `peoplebeach.jpg` - People at a beach
- `GlobalAIBotcamp.jpg` - Conference/event image
- `RiseoftheResistance.jpg` - Theme park attraction

## Architecture

### Components

- **Program.cs**: Main application entry point and user interaction
- **AzureAuthenticationService.cs**: Handles Entra ID authentication using MSAL
- **AssistantService.cs**: Manages communication with Azure OpenAI API

### Authentication Flow

1. Application reads configuration from `appsettings.json` and environment variables
2. `AzureAuthenticationService` uses client credentials flow to obtain access token
3. Access token is used to authenticate requests to Azure OpenAI
4. Conversation continues with maintained chat history

### Dependencies

- **Microsoft.Extensions.Configuration**: Configuration management
- **Microsoft.Identity.Client**: Azure AD authentication (MSAL)
- **Newtonsoft.Json**: JSON serialization and deserialization

## Troubleshooting

### Common Issues

**Authentication Error: "TenantId is not configured"**
- Ensure `appsettings.json` contains valid configuration values
- Check that environment variables are set correctly if using that method

**HTTP 401 Unauthorized**
- Verify your client secret is correct and hasn't expired
- Ensure the app registration has proper API permissions
- Check that admin consent has been granted

**HTTP 403 Forbidden**
- Verify the app registration has been assigned the "Cognitive Services OpenAI User" role
- Ensure you're using the correct Azure OpenAI endpoint and deployment name

**Image Not Found**
- Check the image file path is correct and the file exists
- Ensure the image is in a supported format (JPEG, PNG, etc.)
- Use relative paths from the application directory

**Content Filtered**
- The application includes handling for Azure OpenAI content filters
- Review your input for potentially sensitive content
- Check Azure OpenAI content filter settings in the Azure portal

### Debug Tips

1. **Enable detailed logging**: The application displays token usage information
2. **Verify configuration**: Check that all required settings are populated
3. **Test authentication separately**: Ensure your app registration works with other Azure services
4. **Check Azure portal**: Review activity logs in both Azure OpenAI and Entra ID

## Contributing

1. Fork the repository
2. Create a feature branch
3. Make your changes
4. Test thoroughly
5. Submit a pull request

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## Support

For issues related to:
- **Azure OpenAI**: Check the [Azure OpenAI documentation](https://docs.microsoft.com/azure/cognitive-services/openai/)
- **Entra ID**: Review the [Microsoft Identity Platform documentation](https://docs.microsoft.com/azure/active-directory/develop/)
- **This application**: Create an issue in this repository

## Additional Resources

- [Azure OpenAI Service Documentation](https://docs.microsoft.com/azure/cognitive-services/openai/)
- [Microsoft Identity Platform](https://docs.microsoft.com/azure/active-directory/develop/)
- [MSAL.NET Documentation](https://docs.microsoft.com/azure/active-directory/develop/msal-net-overview)
- [Azure OpenAI REST API Reference](https://docs.microsoft.com/azure/cognitive-services/openai/reference)