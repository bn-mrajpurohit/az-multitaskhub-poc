$SUBSCRIPTION = "Codeus Non-Production"

# log in and choose the subscription you want to work with
# az login
# az account set -s $SUBSCRIPTION

$RESOURCE_GROUP = "test-mrajpur-taskhubs"
$LOCATION = "eastus2" # change this to a location near you, (use az account list-locations -o table)

# create a resource group
#az group create --name $RESOURCE_GROUP `
#    --location $LOCATION

$RANDOM_IDENTIFIER = "785" # replace this with your own random number

# Define Task Hub configs
$TaskHubApps = @(
    @{ Name = "durablefnapp1"; HubName = "taskhub1" },
    @{ Name = "durablefnapp2"; HubName = "taskhub2" }
)

foreach ($app in $TaskHubApps) {
    $STORAGE_ACC_NAME = "$($app.Name)storage"
    
    # Create storage account
    az storage account create --name $STORAGE_ACC_NAME `
        --resource-group $RESOURCE_GROUP `
        --sku "Standard_LRS" `
        --location $LOCATION

    # note: app insights creation is now automatically part of az functionapp create
    
    # Create a new function app using the consumption plan
    az functionapp create -n $app.Name `
        --resource-group $RESOURCE_GROUP `
        --storage-account $STORAGE_ACC_NAME `
        --consumption-plan-location $LOCATION `
        --functions-version "4" `
        --runtime "dotnet-isolated" `
        --runtime-version "8"

    # Set the TaskHubName app setting
    az functionapp config appsettings set `
        --name $app.Name `
        --resource-group $RESOURCE_GROUP `
        --settings "TaskHubName=$($app.HubName)"

    # Deploy using same codebase
    func azure functionapp publish $app.Name
}
