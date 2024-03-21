param (
    [string]$resourceGroupName,
    [string]$location,
    [string]$cosmosDbAccountName,
    [string]$databaseName,
    [string]$containerName,
    [string]$partitionKeyPath
)

# Login to Azure. In Azure DevOps, the Service Principal authentication is used.
# Connect-AzAccount is typically not required in the pipeline as the AzurePowerShell task handles authentication.

# Creating Cosmos DB Account
$accountExists = Get-AzCosmosDBAccount -Name $cosmosDbAccountName -ResourceGroupName $resourceGroupName -ErrorAction SilentlyContinue
if (-not $accountExists) {
    New-AzCosmosDBAccount -Name $cosmosDbAccountName -ResourceGroupName $resourceGroupName -Location $location -Kind GlobalDocumentDB -DefaultConsistencyLevel "Session" -EnableAutomaticFailover:$true
}

# Creating Database
$databaseExists = Get-AzCosmosDBSqlDatabase -AccountName $cosmosDbAccountName -ResourceGroupName $resourceGroupName -Name $databaseName -ErrorAction SilentlyContinue
if (-not $databaseExists) {
    New-AzCosmosDBSqlDatabase -AccountName $cosmosDbAccountName -ResourceGroupName $resourceGroupName -Name $databaseName
}

# Creating Container
$containerExists = Get-AzCosmosDBSqlContainer -AccountName $cosmosDbAccountName -ResourceGroupName $resourceGroupName -DatabaseName $databaseName -Name $containerName -ErrorAction SilentlyContinue
if (-not $containerExists) {
    New-AzCosmosDBSqlContainer -AccountName $cosmosDbAccountName -ResourceGroupName $resourceGroupName -DatabaseName $databaseName -Name $containerName -PartitionKeyKind Hash -PartitionKeyPath $partitionKeyPath
}