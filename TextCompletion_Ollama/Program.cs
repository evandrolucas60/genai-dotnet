using Microsoft.Extensions.AI;
using OllamaSharp;

namespace TextCompletion_Ollama;

public class Program
{
    static async Task Main(string[] args)
    {
        IChatClient client =
                    new OllamaApiClient(new Uri("http://10.11.10.4:11434"), "llama3.2");

        #region Basic Completion

        ////Send prompt and get response
        //string prompt = "Whats is AI? explain max 20 word";
        //Console.WriteLine($"user >>> {prompt}");

        //ChatResponse response = await client.GetResponseAsync(prompt);

        //Console.WriteLine($"assistant >>> {response}");
        //Console.WriteLine($"Tokens used: in={response.Usage?.InputTokenCount}, out={response.Usage?.OutputTokenCount}");

        #endregion

        #region Sentiment Analysis

        //var sentimentPrompt =
        //    """
        //    You will analyze the sentiment of the folowing product reviews.
        //    Each line is its own review. Output the sentiment of each review in a bulleted list, with the sentiment in parentheses after each review.

        //    I bought this product last week and it has exceeded my expectations!
        //    This product is terrible, it broke after one use.
        //    I'm not sure how I feel about this product, it's okay I guess.
        //    I found this product base on the other reviews. It worked for a bit, and then it stopped working. I would not recommend it.
        //    """;

        //Console.WriteLine($"user >>> {sentimentPrompt}");
        //ChatResponse sentimentResponse = await client.GetResponseAsync(sentimentPrompt);
        //Console.WriteLine($"assistant >>> \n{sentimentResponse}");

        #endregion

        #region Classification

        //var classificationPrompt =
        //    """
        //    Please classify the following sentences into categories:
        //    - 'Complaint'
        //    - 'Suggestion'
        //    - 'Praise'
        //    - 'other'

        //    1) "I love the new layout!"
        //    2) "you should add a dark mode option."
        //    3) "The app crashes every time I try to upload a photo."
        //    4) "this app is decent."
        //    """;
        //Console.WriteLine($"user >>> {classificationPrompt}");
        //ChatResponse classificationResponse = await client.GetResponseAsync(classificationPrompt);
        //Console.WriteLine($"assistant >>> \n {classificationResponse}");

        #endregion

        #region Structured output

        //var carListings = new[]
        //{
        //    "Check out this 2020 Tesla Model 3! It's in excellent condition, has a long range, and comes with autopilot features.",
        //    "Lease this 2018 Ford F-150! It's a reliable truck with a spacious cabin and great towing capacity.",
        //    "A classic 1967 Chevrolet Camaro is up for sale! This vintage car has been restored and is a collector's dream.",
        //    "Brand new 2023 BMW X5 for sale! It offers luxury, performance, and advanced technology features.",
        //    "Selling a 2015 Honda Civic! It's fuel-efficient, well-maintained, and perfect for daily commuting."
        //};
        //foreach (var listingText in carListings)
        //{
        //    var response = await client.GetResponseAsync<CarDetails>(
        //        $"""
        //        Convert the Following car listing into a JSON obejct matching this C# schema:
        //        Condition: "New" or "Used"
        //        Make: (car manufacturer)
        //        Model: (car model)
        //        Year: (four-digit year)
        //        ListingType: "Sale" or "Lease"
        //        Price: integer only
        //        Features: array of short strings
        //        tenWordSummary: exactly tne words to summarize this listing

        //        Here is the listing:
        //        {listingText}
        //        """);

        //    if (response.TryGetResult(out var info))
        //    {
        //        //Convert the CarDetails object to JSON for display
        //        Console.WriteLine(System.Text.Json.JsonSerializer.Serialize(
        //            info, new System.Text.Json.JsonSerializerOptions { WriteIndented = true }));
        //    }
        //    else
        //    {
        //        Console.WriteLine($"Response was not in the expected format.");
        //    }
        //}
        #endregion

        #region ChatApp
        List<ChatMessage> chatHistory = new()
        {
            new ChatMessage(ChatRole.System, """
                You are a freandly hiking enthusiast who helps people discover fun hikes in their area.
                You introduce yourself as when first saying hello.
                When helping people out, you always ask them for this information
                to inform the hiking recommendation you provie:

                1. the location they would like to hike
                2. What hiking intensity they are looking for

                You will then provide three suggestions for nearby hikes that vary in lenght
                after you get that information. you will also share an interesting fact about
                the loca nature on the hikes whem making a recommendation. At the end of your
                response, ask if there is anything else you can help with.
            """)
        };

        while (true)
        {
            //Get the user prompt and add it to the chat history
            Console.WriteLine("Your prompt: ");
            var userPrompt = Console.ReadLine();
            chatHistory.Add(new ChatMessage(ChatRole.User, userPrompt));

            //Get the AI response and add it to the chat history
            Console.WriteLine("AI Response: ");
            var response = "";
            await foreach (var item in client.GetStreamingResponseAsync(chatHistory))
            {
                Console.Write(item.Text);
                response += item.Text;
            }
            chatHistory.Add(new ChatMessage(ChatRole.Assistant, response));
            Console.WriteLine();

        }
        #endregion
    }
}
