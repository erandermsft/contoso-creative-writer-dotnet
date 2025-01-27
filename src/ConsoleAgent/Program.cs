// See https://aka.ms/new-console-template for more information
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Prompty;
using Microsoft.SemanticKernel.Agents;
using Azure.Identity;
using System.Net.NetworkInformation;
using Microsoft.Extensions.Configuration;
using Microsoft.SemanticKernel.PromptTemplates.Liquid;
using Microsoft.SemanticKernel.Agents.History;
using Microsoft.SemanticKernel.Agents.Chat;
using Microsoft.SemanticKernel.ChatCompletion;
using System.Text.Json;
using ConsoleAgent.Agents.Researcher;
using ConsoleAgent.Agents.Influencer;
using ConsoleAgent.Agents.Writer;

#pragma warning disable SKEXP0040 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
#pragma warning disable SKEXP0110 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
#pragma warning disable SKEXP0110 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
#pragma warning disable SKEXP0001 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.



// Build the configuration
var configuration = new ConfigurationBuilder()
    .AddUserSecrets<Program>()
    .Build();

var kernel = Kernel.CreateBuilder()
    .AddAzureOpenAIChatCompletion(configuration["AZURE_OPENAI_DEPLOYMENT"],
        configuration["AZURE_OPENAI_ENDPOINT"],
        configuration["AZURE_OPENAI_APIKEY"])
    .Build();


// Pass the service to your Researcher agent
var researcherAgent = Researcher.CreateAgent(kernel,
    configuration["BING_SEARCH_APIKEY"]);

// ... rest of your code
//Add user secrets to configuration
var kernelConfiguration = new ConfigurationBuilder()
    .AddUserSecrets<Program>()
    .Build();


var influencerName = "influencer_agent";
var writerName = "writer_agent";
var researcherName = "researcher_agent";
const string terminationToken = "yes";

var influencerAgent = Influencer.CreateAgent(kernel);
var writerAgent = Writer.CreateAgent(kernel);

//Can be used to select the next agent in the sequence when selection sequence is not just sequential.
//For example, if the editor agent should be used to review the output of the other agents. 
KernelFunction selectionFunction =
            AgentGroupChat.CreatePromptFunctionForStrategy(
                $$$"""
                Examine the provided RESPONSE and choose the next participant.
                State only the name of the chosen participant without explanation.
                Never choose the participant named in the RESPONSE.

                Choose only from these participants:
                - {{{influencerName}}}
                - {{{writerName}}}
                - {{{researcherName}}}

                Always follow these rules when choosing the next participant:
                - If RESPONSE is user input, it is {{{researcherName}}}'s turn.
                - If RESPONSE is by {{{researcherName}}}, it is {{{writerName}}}'s turn.
                - If RESPONSE is by {{{writerName}}}, it is {{{influencerName}}}'s turn.
    
                RESPONSE:
                {{$lastmessage}}
                """,
                safeParameterNames: "lastmessage");



KernelFunction terminationFunction =
    AgentGroupChat.CreatePromptFunctionForStrategy(
        $$$"""
                Examine the RESPONSE and determine whether the content has been deemed satisfactory.
                If content is satisfactory, respond with a single word without explanation: {{{terminationToken}}}.
                If specific suggestions are being provided, it is not satisfactory.
                If no correction is suggested, it is satisfactory.

                RESPONSE:
                {{$lastmessage}}
                """,
        safeParameterNames: "lastmessage");

ChatHistoryTruncationReducer historyReducer = new(1);
AgentGroupChat chat =
            new(researcherAgent, writerAgent, influencerAgent)
            {
                ExecutionSettings = new AgentGroupChatSettings
                {
                    SelectionStrategy =new SequentialSelectionStrategy(),
                    TerminationStrategy = 
                        new KernelFunctionTerminationStrategy(terminationFunction, kernel)
                        {
                            // Only evaluate for influencer's response
                            Agents = [influencerAgent],
                            // Save tokens by only including the final response
                            HistoryReducer = historyReducer,
                            // The prompt variable name for the history argument.
                            HistoryVariableName = "lastmessage",
                            // Limit total number of turns
                            MaximumIterations = 10,
                            // Customer result parser to determine if the response is "yes"
                            ResultParser = (result) => result.GetValue<string>()?.Contains(terminationToken, StringComparison.OrdinalIgnoreCase) ?? false
                        }
                }
            };
chat.AddChatMessage(new ChatMessageContent(AuthorRole.User, "Summer camping trends in 2025"));
chat.IsComplete = false;

try
{
    await foreach (ChatMessageContent response in chat.InvokeAsync())
    {
        Console.WriteLine();
        Console.WriteLine($"{response.AuthorName.ToUpperInvariant()}:{Environment.NewLine}{response.Content}");
    }
}
catch (HttpOperationException exception)
{
    Console.WriteLine(exception.Message);
    if (exception.InnerException != null)
    {
        Console.WriteLine(exception.InnerException.Message);
        if (exception.InnerException.Data.Count > 0)
        {
            Console.WriteLine(JsonSerializer.Serialize(exception.InnerException.Data, new JsonSerializerOptions() { WriteIndented = true }));
        }
    }
}
