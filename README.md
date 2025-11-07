# Logging .NET Azure Functions in isolated worker model

Examples of 'vanilla' App Insights logging in .NET Azure Function apps, and integrating [Serilog](https://serilog.net/), with both .NET 8 and 9.

- [.NET 8](net8/Func/Program.cs)
- [.NET 8 with Serilog](net8/FuncWithSerilog/Program.cs)
- [.NET 9](net9/Func/Program.cs)
- [.NET 9 with Serilog](net9/FuncWithSerilog/Program.cs)

<!-- Azure Functions are using the new Flex Consumption plan. This is not available yet in all regions. Run `az functionapp list-flexconsumption-locations --output table` to list compatible regions. -->

## Azure environment configuration

```bash
az group create --name rg-funcs-logging-australiaeast --location australiaeast

# Prepare a service principal for Login with OIDC 
az ad sp create-for-rbac --name sp-funcs-logging-australiaeast --role Contributor --scopes /subscriptions/<yoursubscription>/resourceGroups/rg-funcs-logging-australiaeast
```

Make a note of the appId value, as you'll enter that as the `--id` parameter.

<https://learn.microsoft.com/entra/workload-id/workload-identity-federation-create-trust?pivots=identity-wif-apps-methods-azp&WT.mc_id=DOP-MVP-5001655#github-actions>

```bash
az ad app federated-credential create --id xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx --parameters credential.json
```

Create credential.json

```json
{
    "name": "Testing",
    "issuer": "https://token.actions.githubusercontent.com",
    "subject": "repo:octo-org/octo-repo:environment:Production",
    "description": "Testing",
    "audiences": [
        "api://AzureADTokenExchange"
    ]
}
```

Get the Azure subscription ID:

```bash
az account subscription list
```

and then set the following as GitHub secrets

- AZURE_CLIENT_ID the Application (client) ID
- AZURE_TENANT_ID the Directory (tenant) ID
- AZURE_SUBSCRIPTION_ID your subscription ID
