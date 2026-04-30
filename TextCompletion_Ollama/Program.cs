using Microsoft.Extensions.AI;
using OllamaSharp;

namespace TextCompletion_Ollama;

public class Program
{
    static void Main(string[] args)
    {
        IChatClient client =
                    new OllamaApiClient(new Uri("http://10.11.10.4:11434"), "llama3.2");
    }
}
