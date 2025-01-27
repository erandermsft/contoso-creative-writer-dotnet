

using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Agents;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using Microsoft.SemanticKernel.Plugins.Web;
using Microsoft.SemanticKernel.Plugins.Web.Bing;
using Microsoft.SemanticKernel.PromptTemplates.Liquid;
using Microsoft.SemanticKernel.Prompty;


namespace ConsoleAgent.Agents.Researcher
{
    public static class Researcher
    {
#pragma warning disable SKEXP0040 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
#pragma warning disable SKEXP0110 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
#pragma warning disable SKEXP0110 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
#pragma warning disable SKEXP0001 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
#pragma warning disable SKEXP0050 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.

        public static ChatCompletionAgent CreateAgent(Kernel kernel, string apikey)
        {
            string researcherPrompty = File.ReadAllText("./agents/researcher/researcher.prompty");
            var researcherKernel = kernel.Clone();
            var bingConnector = new BingConnector(apikey);
            var plugin = new WebSearchEnginePlugin(bingConnector);
            kernel.ImportPluginFromObject(plugin, "BingPlugin");


            PromptTemplateConfig researcherTemplateConfig = KernelFunctionPrompty.ToPromptTemplateConfig(researcherPrompty);
            ChatCompletionAgent researcherAgent =
                new(researcherTemplateConfig, new LiquidPromptTemplateFactory())
                {
                    Kernel = kernel,
                    Name = "researcher_agent",

                    Arguments = new KernelArguments(
                new OpenAIPromptExecutionSettings()
                {
                    FunctionChoiceBehavior = FunctionChoiceBehavior.Auto()
                })

                };
            return researcherAgent;
        }

    }
}
