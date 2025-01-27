using Microsoft.SemanticKernel.Agents;

using Microsoft.SemanticKernel.PromptTemplates.Liquid;
using Microsoft.SemanticKernel.Prompty;
using Microsoft.SemanticKernel;

namespace ConsoleAgent.Agents.Influencer
{
    public class Influencer
    {
#pragma warning disable SKEXP0040 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
#pragma warning disable SKEXP0110 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
#pragma warning disable SKEXP0110 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
#pragma warning disable SKEXP0001 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
#pragma warning disable SKEXP0050 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.

        public static ChatCompletionAgent CreateAgent(Kernel kernel)
        {
            string influencerPrompty = File.ReadAllText("./agents/influencer/influencer.prompty");
            // Convert to a prompt template config
            PromptTemplateConfig influencerTemplateConfig = KernelFunctionPrompty.ToPromptTemplateConfig(influencerPrompty);
            ChatCompletionAgent influencerAgent =
                new(influencerTemplateConfig, new LiquidPromptTemplateFactory())
                {
                    Kernel = kernel,
                    Name = "influencer_agent",
                    // Provide default values for template parameters
                    Arguments = new KernelArguments()
                    {
                        { "firstName", "Erik" },
                        {"customers", File.ReadAllText("./agents/influencer/customers.json") }
                    }
                };
            return influencerAgent;
        }

    }
}


