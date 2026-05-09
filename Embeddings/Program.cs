using Microsoft.Extensions.AI;
using Microsoft.Extensions.Configuration;
using OpenAI;
using System.ClientModel;
using System.Numerics.Tensors;

namespace Embeddings;

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
            new OpenAIClient(credential, options).GetChatClient("openai/gpt-4.1-mini").AsIChatClient();

        // Create embedding generator
        IEmbeddingGenerator<string, Embedding<float>> embeddingGenerator =
            new OpenAIClient(credential, options).GetEmbeddingClient("text-embedding-3-small").AsIEmbeddingGenerator();

        // Generate an embedding for the input text
        //var embedding = await embeddingGenerator.GenerateVectorAsync("Hello World!");
        //Console.WriteLine($"Embedding dimensions: {embedding.Span.Length}");

        //foreach (var value in embedding.Span)
        //{
        //    Console.Write("{0:00.0}, ", value);
        //}

        var catVector = await embeddingGenerator.GenerateVectorAsync("cat");
        var dogVector = await embeddingGenerator.GenerateVectorAsync("dog");
        var kittenVector = await embeddingGenerator.GenerateVectorAsync("kitten");

        Console.WriteLine($"cat-dog similarity: {TensorPrimitives.CosineSimilarity(catVector.Span, dogVector.Span)}");
        Console.WriteLine($"cat-kitten similarity: {TensorPrimitives.CosineSimilarity(catVector.Span, kittenVector.Span)}");
        Console.WriteLine($"dog-kitten similarity: {TensorPrimitives.CosineSimilarity(dogVector.Span, kittenVector.Span)}");
    }
}
