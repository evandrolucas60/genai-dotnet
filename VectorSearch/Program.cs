using Microsoft.Extensions.AI;
using Microsoft.Extensions.Configuration;
using Microsoft.SemanticKernel.Connectors.InMemory;
using OpenAI;
using System.ClientModel;

namespace VectorSearch;

public class Program
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

        // Create embedding generator
        IEmbeddingGenerator<string, Embedding<float>> embeddingGenerator =
            new OpenAIClient(credential, options).GetEmbeddingClient("text-embedding-3-small").AsIEmbeddingGenerator();

        // Create vector store and collection
        var vectorStore = new InMemoryVectorStore();

        var moviesStore = vectorStore.GetCollection<int, Movie>("movies");

        await moviesStore.EnsureCollectionExistsAsync();

        foreach (var movie in MovieData.Movies)
        {
            // Generate vector for each movie description and store it in the vector store
            movie.Vector = await embeddingGenerator.GenerateVectorAsync(movie.Description);

            await moviesStore.UpsertAsync(movie);
        }

        //1- Embed the user's query
        //2- Vectorize search
        //3- Return the most relevant movies based on cosine similarity

        // Example user query
        var query = "I want to watch a movie about space exploration and adventure.";
        var queryEmbedding = await embeddingGenerator.GenerateVectorAsync(query);

        // Search for relevant movies in the vector store
        var searchResults = moviesStore.SearchAsync(queryEmbedding, top: 2);

        await foreach (var result in searchResults)
        {
            Console.WriteLine($"Title: {result.Record.Title}, Description: {result.Record.Description}, Score: {result.Score}");
            Console.WriteLine();
        }
    }
}
