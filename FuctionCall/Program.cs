using Microsoft.Extensions.AI;
using Microsoft.Extensions.Configuration;
using OpenAI;
using System.ClientModel;

namespace FuctionCall
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

            //Create chat client
            IChatClient client =
                new ChatClientBuilder(new OpenAIClient(credential, options).GetChatClient("openai/gpt-4.1-mini").AsIChatClient())
                .UseFunctionInvocation()
                .Build();

            var chatOptions = new ChatOptions
            {
                Tools = [AIFunctionFactory.Create((string location, string unit) => {
                    // Here you would call a weather API to get the weather for the location
                    var temperature = Random.Shared.Next(5, 20);
                    var conditions = Random.Shared.Next(0, 1) == 0 ? "sunny" : "rainy";

                    return $"The weather is {temperature} degrees C and {conditions}";
                },
                "get_current_weather",
                "Get the current weather in a given location")]
            };

            // Create a chat history with a system message and a user message
            List<ChatMessage> chatHistory = [new(ChatRole.System, """
                You are a hiking enthusiast who helps people discover fun hikes in their area.
                You are upbeat and friedly.
                """)];

            // Add a user message to the chat history
            chatHistory.Add(new (ChatRole.User, """
                I live in Recife, Pernambuco, Brazil and I'm looking for a moderate instensity hike.
                What's the current weather like there? Can you recommend a hike for me?
                """));

            Console.WriteLine($"{chatHistory.Last().Role} >>> {chatHistory.Last()}");
            
            ChatResponse response = await client.GetResponseAsync(chatHistory, chatOptions);

            chatHistory.Add(new(ChatRole.Assistant, response.Text));

            Console.WriteLine($"{chatHistory.Last().Role} >>> {chatHistory.Last()}");
        }
    }
}
