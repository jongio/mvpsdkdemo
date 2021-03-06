# Azure SDK Demo - MVP Summit

Azure.Core Pipeline Policies & Azure.Identity DefaultAzureCredential

## Azure CLI

### Install Azure CLI

https://aka.ms/azcliget

### Login to Azure CLI

`az login`

## Azure Setup

### Set Active Subscription

`az account set -n {SUBSCRIPTION_NAME}`

Get subscription list with `az account list`

See current selected/default subscription with `az account show`

### Create Resource Group

`az group create -n {RG_NAME} -l westus`

### Add Resource Group to Azure Cli Config

So you don't need to include in subsequent commands.

`az configure -d group={RG_NAME}`

### Create Azure Storage Account

`az storage account create -n {STORAGE_NAME}`

### Assign Permissions to Current User

1. Get current user objectId

    `az ad signed-in-user show --query objectId -o tsv`

1. Assign current user to `Storage Blob Data Contributor` role

    `az role assignment create --assignee {CURRENT_USER_ID} --role ba92f5b4-2d11-453d-a403-e96b0029c9fe`

### Create Azure App Service Plan

`az appservice plan create -n {PLAN_NAME} --sku B1`

### Create Azure WebApp

`az webapp create -n {APP_NAME} -p {PLAN_NAME}`

### Assign System-Assigned Managed Identity to Azure WebApp

`az webapp identity assign -n {APP_NAME} --query principalId -o tsv`

### Assign Permissions to WebApp Managed Identity

1. Get WebApp's Managed Identity Id

    `az webapp identity show -n {APP_NAME}`

1. Assign WebApp's Managed Identity to `Storage Blob Data Contributor` role

    `az role assignment create --assignee {WEBAPP_SYSTEM_ASSIGNED_ID} --role ba92f5b4-2d11-453d-a403-e96b0029c9fe`

### Set WebApp AZURE_STORAGE_BLOB_URL Property

The code reads the storage account name from an environment variable named `AZURE_STORAGE_BLOB_URL`.

1. Get the storage account blob url

    `az storage account show -n {STORAGE_NAME} --query primaryEndpoints.blob -o tsv`

1. Set the `AZURE_STORAGE_BLOB_URL` property

    `az webapp config appsettings set -n {APP_NAME} --settings "AZURE_STORAGE_BLOB_URL"="{STORAGE_URL}"`

### Turn on Logging

1. Enable Application Logging - allows us to see our app logs in the log stream

    `az webapp log config --application-logging true --level information --web-server-logging filesystem -n {APP_NAME} -o yaml`

## Code Setup

### Clone repo

`git clone https://github.com/jongio/mvpsdkdemo`

## Console App Demo

### Get the storage account blob url

`az storage account show -n {STORAGE_NAME} --query primaryEndpoints.blob -o tsv`

### Set blob url

> Only required if the storage account name is different than what is checked in.

1. Open project folder `src/mvpsdkdemoconsole` in VS Code.
1. Open `src/mvpsdkdemoconsole/Program.cs`
1. Set the `blobServiceUri` variable to the full storage account blob url.

### Azure CLI Login

1. Run `az login` so `DefaultAzureCredential` uses your Azure account to authenticate

### Open the code and explain it

1. All the code can be found in `Program.cs`.  Open it and explain the BlobClientOptions, HttpPipeline.

### Run the app

1. Hit F5 to debug the application and step through it with your audience.

## API Demo

### Get the storage account blob url

`az storage account show -n {STORAGE_NAME} --query primaryEndpoints.blob -o tsv`

### Set environment variable

1. Open project folder `src/mvpsdkdemoapi` in VS Code.
1. Open `.vscode/launch.json`.
1. Set `configurations/env/AZURE_STORAGE_BLOB_URL` to storage account blob url.
1. Save the file.

### Run App Locally

1. Hit F5
1. View the Output in Browser.
1. View the Output in VS Code Output Console.
1. Show that Azure CLI Credential was used.

    You will see the following lines in the output:

    ```cmd
    info: Azure-Identity[1]
        AzureCliCredential.GetToken invoked. Scopes: [ https://storage.azure.com/.default ] ParentRequestId: c7e8b2b7-a86d-4710-a10e-99f44eedb8bc
    info: Azure-Identity[2]
        AzureCliCredential.GetToken succeeded. Scopes: [ https://storage.azure.com/.default ] ParentRequestId: c7e8b2b7-a86d-4710-a10e-99f44eedb8bc ExpiresOn: 2020-03-18T23:12:27.2820620+00:00
    info: Azure-Identity[2]
        DefaultAzureCredential.GetToken succeeded. Scopes: [ https://storage.azure.com/.default ] ParentRequestId: c7e8b2b7-a86d-4710-a10e-99f44eedb8bc ExpiresOn: 2020-03-18T23:12:27.2820620+00:00
    ```

1. Show the custom pipeline log entries.

    You will see the following lines in the output:

    ```cmd
    info: SimpleTracingPolicy[0]
      >> Response: 206 from GET https://mvpsdkdemostorage.blob.core.windows.net/blobs/blob.txt
    ```

### Deploy to Azure

1. Open the VS Code Azure Extension.
1. Under APP SERVICE, right-click on your app and select 'Deploy to Web App'.

### Start Log Streaming

1. Open the VS Code Azure Extension.
1. Under APP SERVICE, right-click on your app and select 'Start Streaming Logs'

### Run App on Azure

1. Open a browser and hit the web api URL: https://mvpsdkdemoapp.azurewebsites.net/api/blobber or https://{APP_NAME}.azurewebsites.net/api/blobber
1. View the output in the browser.

### View the Log Stream

1. Back to VS Code, go to the log stream and show that the policy logs are there and that Managed Identity was successfully used.

    You will see the following lines for the custom policy:

    ```cmd
    2020-03-18 22:48:13.757 +00:00 [Information] SimpleTracingPolicy: >> Response: 206 from GET https://mvpsdkdemostorage.blob.core.windows.net/blobs/blob.txt
    ```

    You will see the following lines for Managed Identity:

    ```cmd
    2020-03-18 22:48:13.295 +00:00 [Information] Azure-Identity: ManagedIdentityCredential.GetToken succeeded. Scopes: [ https://storage.azure.com/.default ] ParentRequestId: 1fce14b9-720a-45a0-af21-146098f3ec25 ExpiresOn: 2020-03-19T01:49:40.0000000+00:00

    2020-03-18 22:48:13.297 +00:00 [Information] Azure-Identity: DefaultAzureCredential.GetToken succeeded. Scopes: [ https://storage.azure.com/.default ] ParentRequestId: 1fce14b9-720a-45a0-af21-146098f3ec25 ExpiresOn: 2020-03-19T01:49:40.0000000+00:00
    ```

    If you do not see those lines, that means that you started the log streaming after the first request.  Since tokens are now obtained at the app level, you'll only see those Identity related messages once per app start, not once per request.
