## Dotnet implementation of Contoso Creative Writer with Prompty
This is a small console app that demos a subset of the Contoso Creative Writer app.
It is implemented using Semantic Kernel and is meant to demonstrate the portability of prompty.
Essentially, you can design and evaluate your Prompty files in Python and then re-use them in C#.

In contrast to the Python implementation, this app uses the built in AgentChat capabilities of Semantic Kernel for orcehstration.
We also make use of the WebSearchEngine plugin and Bing Search connector that are built-in Semantic Kernel.

## Running the app
In order to run the app, we need to add a few configuration items. We're using user secrets for simplicity.


### Add user secrets
```bash
dotnet user-secrets init
dotnet user-secrets set "AZURE_OPENAI_ENDPOINT" "https://your-endpoint.openai.azure.com/"
dotnet user-secrets set "AZURE_OPENAI_DEPLOYMENT" "your-deployment-name"
dotnet user-secrets set "AZURE_OPENAI_APIKEY" "your-apikey"
dotnet user-secrets set "BING_SEARCH_APIKEY" "your-bing-search-apikey"

```
### Run the app
```bash
dotnet run
```
