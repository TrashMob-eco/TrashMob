# Signup/signin journey


## Setup

To use these policies setup your dev environment as [described here](https://github.com/mrochon/b2csamples#dev-environment-setup).

Installing policies in your tenant:

1. Update conf.json for your environment.
2. The value of the Prefix will be injected into your policy names at load time, unless you use the -NoPrefix flag in the *Import-IefPolicies* command
2. You can create separate configuration files for individual B2C tenant by naming them b2c1.json, b2c2.json where b2cN stands for the name of your b2c tenant, e.g. *mrochonb2cprod.json*
3. Use the following to install the policies in your tenant

```PowerShell
cd <folder with these policies>
Connect-IefPolicies <your b2c tenant name, *.onmicrosoft.com* may be omitted>
Import-IefPolicies
```
