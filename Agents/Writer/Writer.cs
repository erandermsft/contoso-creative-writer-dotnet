using Microsoft.SemanticKernel.Agents;
using Microsoft.SemanticKernel.PromptTemplates.Liquid;
using Microsoft.SemanticKernel.Prompty;
using Microsoft.SemanticKernel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleAgent.Agents.Writer
{
    public static class Writer
    {
#pragma warning disable SKEXP0040 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
#pragma warning disable SKEXP0110 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
#pragma warning disable SKEXP0110 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
#pragma warning disable SKEXP0001 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
#pragma warning disable SKEXP0050 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
        public static ChatCompletionAgent CreateAgent(Kernel kernel)
        {
            string writerPrompty = File.ReadAllText("./agents/writer/writer.prompty");
            // Convert to a prompt template config
            PromptTemplateConfig writerTemplateConfig = KernelFunctionPrompty.ToPromptTemplateConfig(writerPrompty);
            ChatCompletionAgent influencerAgent =
                new(writerTemplateConfig, new LiquidPromptTemplateFactory())
                {
                    Kernel = kernel,
                    Name = "writer_agent",
                    // Provide default values for template parameters
                    Arguments = new KernelArguments()
                    {
                        { "firstName", "Erik" },
                        {"products", File.ReadAllText("./agents/writer/products.json") },
            //{"research", File.ReadAllText("./agents/writer/research.json")   }
                    }
                };
            return influencerAgent;
        }
    }
}
