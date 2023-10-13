# Deploying the infrastructure

## Prerequisites
1. Install PowerShell
1. Install the latest version of Azure CLI
1. Download the TrashMob repo from GitHub

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

.\deployInfra.ps1 -environment <env> -region <regionName> -subscriptionId <subscriptionId> -sqlAdminPassword <password>

i.e.

.\deployInfra.ps1 -environment jb -region westus2 -subscriptionId <your subscription Id> -sqlAdminPassword "TestP$$wrd1"

```
where:
  env is the same as set above (i.e. jb)
  regionName is the Azure Region you wish to deploy to (i.e. uswest2)
  subscriptionId is the current subscription id
  password is the password value you want to set for your sql server that is being created. The password must meet Azure minimum password standards
