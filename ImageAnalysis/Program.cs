using Microsoft.Extensions.AI;
using Microsoft.Extensions.Configuration;
using OpenAI;
using System.ClientModel;

namespace ImageAnalysis
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            //get Credentials from user secrets
            IConfigurationRoot config = new ConfigurationBuilder().AddUserSecrets<Program>().Build();

            var credential = new ApiKeyCredential(config["GitHubModels:Token"] ?? throw new InvalidOperationException());
            var options = new OpenAIClientOptions()
            {
                Endpoint = new Uri("https://models.github.ai/inference")
            };

            IChatClient client =
                new OpenAIClient(credential, options).GetChatClient("openai/gpt-4.1-mini").AsIChatClient();

            //user prompts
            var promptDescribe = "Describe the image";
            var promptAnalyze = "How many red cars are in the picture? and what other car colors are there?";

            //prompts
            var systemPrompt = "You are a userful assistent that describes images using a direct stule.";

            var userPrompt = promptAnalyze;

            List<ChatMessage> messages =
            [
                new ChatMessage(ChatRole.System, systemPrompt),
                new ChatMessage (ChatRole.User, userPrompt),
            ];

            var imageFileName = "cars.png";
            string image = Path.Combine(Directory.GetCurrentDirectory(), "images", imageFileName);

            AIContent aic = new DataContent(File.ReadAllBytes(image), "image/png");
            var message = new ChatMessage(ChatRole.User, [aic]);
            messages.Add(message);

            var response = await client.GetResponseAsync(messages);
            Console.WriteLine($"Prompt: {userPrompt}");
            Console.WriteLine($"Image: {imageFileName}");
            Console.WriteLine($"Response: {response.Messages[0]}");
        }
    }
}
