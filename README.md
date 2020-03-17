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

Get account list with `az account list`

See current account with `az account show`

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

## Code Setup

### Clone repo

`git clone https://github.com/jongio/mvpsdkdemo`

## Console App Demo

### Get the storage account blob url

`az storage account show -n {STORAGE_NAME} --query primaryEndpoints.blob -o tsv`

### Set environment variable

1. Open project folder `src/mvpsdkdemoconsole` in VS Code.
1. Open `src/mvpsdkdemoconsole/.env`
1. Set the AZURE_STORAGE_BLOB_URL variable to the full storage account blob url.

### Azure CLI Login

1. Run `az login` so `DefaultAzureCredential` uses your Azure account to authenticate

### Open the code and explain it

1. All the code can be found in `Program.cs`.  Open it and explain the BlobClientOptions, HttpPipeline.

### Run the app

1. Hit F5 to debug the application and step through it with your audience.

## API Demo

### Get the storage account blob url

1. `az storage account show -n {STORAGE_NAME} --query primaryEndpoints.blob -o tsv`

### Set environment variable

1. Open project folder `src/mvpsdkdemoapi` in VS Code.
1. Open `.vscode/launch.json`.
1. Set `configurations/env/AZURE_STORAGE_BLOB_URL` to storage account blob url.
1. Save the file.

## Run App

1. Hit F5

## Deploy to Azure