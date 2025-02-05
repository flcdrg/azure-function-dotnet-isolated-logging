param storageAccountType string = 'Standard_LRS'

@description('Location for all resources.')
param location string = resourceGroup().location

@description('Location for Application Insights')
param appInsightsLocation string

var hostingPlanName = 'plan-funcs-logging-australiaeast'
var applicationInsightsName = 'appi-funcs-logging-australiaeast'
var storageAccountName = 'stfuncsloggingaue'

resource storageAccount 'Microsoft.Storage/storageAccounts@2022-05-01' = {
  name: storageAccountName
  location: location
  sku: {
    name: storageAccountType
  }
  kind: 'Storage'
  properties: {
    supportsHttpsTrafficOnly: true
    defaultToOAuthAuthentication: true
  }
}

resource hostingPlan 'Microsoft.Web/serverfarms@2021-03-01' = {
  name: hostingPlanName
  location: location
  sku: {
    name: 'F1'
    tier: 'Dynamic'
  }
  properties: {}
}

resource applicationInsights 'Microsoft.Insights/components@2020-02-02' = {
  name: applicationInsightsName
  location: appInsightsLocation
  kind: 'web'
  properties: {
    Application_Type: 'web'
    Request_Source: 'rest'
  }
}

module functionApp './function.bicep' = {
  name: 'func-funcs-net8-logging-australiaeast'
  params: {
    functionAppName: 'func-funcs-net8-logging-australiaeast'
    location: location
    hostingPlanId: hostingPlan.id
    appInsightsConnectionString: applicationInsights.properties.ConnectionString
    storageAccountName: storageAccount.name
    storageAccountKey: storageAccount.listKeys().keys[0].value
  }
}
