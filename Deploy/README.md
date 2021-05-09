# Deploying the infrastructure

## Prerequisites
1. Install Powershell
1. Install the Azuure CLI
1. Download the TrashMob repo from GitHub
1. You must have an Azure subscription set up with a resource group named "rg-trashmob-<env>" where <env> represents a 2 or 3 character name representing your persona. i.e. My name is Joe Beernink, my resource group name is rg-trashmob-jb

## To deploy the Infrastructure needed to run TrashMob locally on your box, you will need to run the following steps:
1. Open a Powershell window and go to the $GitRoot\Deploy folder
1. Execute the following commands to log in and set your session to the correct subscription:
```
    az login
    az account set --subscription <subscriptionName>
```
  az login will ask you to log in with your Azure Credentials
  If you have more than one subscription tied to your Azure account, you will need to specify it in the second line
  
1. Execute the following command to deploy the Infrastructure needed
```

.\deployInfra.ps1 -environment <env> -region <regionName>

```
    where env is the same as set above (i.e. jb)
    and regionName is the Azure Region you wish to deploy to (i.e. uswest2)

## To deploy everything needed to run TrashMob in your own environment in the cloud, you will need to run the following steps:
1. Open a Powershell window and go to the $GitRoot\Deploy folder
1. Execute the following commands to log in and set your session to the correct subscription:
```
    az login
    az account set --subscription <subscriptionName>
```
  az login will ask you to log in with your Azure Credentials
  If you have more than one subscription tied to your Azure account, you will need to specify it in the second line
  
1. Execute the following command to deploy the Infrastructure needed
```

.\deployAll.ps1 -subscriptionId <subscriptionId> -environment <env> -region <regionName>

```
    where subscriptionId is your current subscriptionId
    and env is the same as set above (i.e. jb)
    and regionName is the Azure Region you wish to deploy to (i.e. uswest2)
