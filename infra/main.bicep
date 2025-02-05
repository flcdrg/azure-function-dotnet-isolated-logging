param storageAccountType string = 'Standard_LRS'

@description('Location for all resources.')
param location string = resourceGroup().location

var hostingPlanName = 'plan-funcs-logging-australiaeast'
var applicationInsightsName = 'appi-funcs-logging-australiaeast'
var storageAccountName = 'stfuncsloggingaue'

resource storageAccount 'Microsoft.Storage/storageAccounts@2022-05-01' = {
  name: storageAccountName
  location: location
  sku: {
    name: storageAccountType
  }
  kind: 'StorageV2'
  properties: {
    supportsHttpsTrafficOnly: true
    defaultToOAuthAuthentication: true
    allowBlobPublicAccess: true
    minimumTlsVersion: 'TLS1_2'
    publicNetworkAccess: 'Enabled'
  }
}

resource hostingPlan 'Microsoft.Web/serverfarms@2021-03-01' = {
  name: hostingPlanName
  location: location
  sku: {
    name: 'Y1'
    // name: 'FC1'
    // tier: 'FlexConsumption'
  }
  kind: 'functionapp'
  properties: {
    reserved: true
  }
}

resource logAnalyticsWorkspace 'Microsoft.OperationalInsights/workspaces@2023-09-01' = {
  name: 'law-funcs-logging-australiaeast'
  location: location
  properties: {
    sku: {
      name: 'PerGB2018'
    }
  }
}

resource applicationInsights 'Microsoft.Insights/components@2020-02-02' = {
  name: applicationInsightsName
  location: location
  kind: 'web'
  properties: {
    Application_Type: 'web'
    Request_Source: 'rest'
    WorkspaceResourceId: logAnalyticsWorkspace.id
  }
}

module functionAppNet8 './function.bicep' = {
  name: 'func-funcs-net8-logging-australiaeast'
  params: {
    functionAppName: 'func-funcs-net8-logging-australiaeast'
    location: location
    hostingPlanId: hostingPlan.id
    applicationInsightsName: applicationInsights.name
    storageAccountName: storageAccount.name
  }
}

module functionAppNet9 './function.bicep' = {
  name: 'func-funcs-net9-logging-australiaeast'
  params: {
    functionAppName: 'func-funcs-net9-logging-australiaeast'
    location: location
    hostingPlanId: hostingPlan.id
    applicationInsightsName: applicationInsights.name
    storageAccountName: storageAccount.name
    functionAppRuntimeVersion: '9.0'
  }
}
