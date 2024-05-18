---
marp: true
title: IaC for Developers with Pulumi
paginate: true
theme: gaia
author: Al Rodriguez
---

# IaC for Developers with Pulumi

with AL Rodriguez

![bg right 80%](presentation-images/presentation_link_qrcode.png)

---

# Me (AL)

- @ProgrammerAL
- programmerAL.com
- NOT affiliated with Pulumi

![bg right 80%](presentation-images/presentation_link_qrcode.png)

---

# What this session is

- Outline of Infrastructure as Code (IaC)
- Introduction to Pulumi
- Demo with C# and Azure
  - Concepts apply to other languages/clouds supported by Pulumi

---

# Server History Lesson

- Physical Hardware
- Physical Hardware running VMs
- Co-located hardware running VMs
- VMs in a "cloud"
- Cloud with IaaS/PaaS/etc services <--today!
- Skynet <-- coming soon!

---

# Cloud needs Automation

- On-Demand Resources
  - NOT Pristine and Pampered
- Tangent: Azure wouldn't exist without PowerShell

---

![bg contain](presentation-images/padme-meme.jpg)

---

# Benefits of Automated Cloud

- On-Demand
- Add/Remove cloud resources as needed
- Scriptable
- Updated with a PR

---

# Infrastructure as Code (IaC)

- Codify your environments
- Automation to avoid manual steps
  - Manual steps always mess up eventually
- Easily repeatable
  - Create multiple environments with ease

---

# IaC Tools

- Azure ARM/Bicep
- AWS Cloudformation
- Terraform
- Pulumi

---

# Why should devs care?

- Devs know their apps
  - What services their apps need
- Con: More work shifted-left

---

# What's different about IaC code?

- Web UI, Backend Server, Embedded, Video Game, etc
  - New Type: IaC
- Still code
  - YAML, JSON, Custom DSL, or Your Choice of Language
- LIke a script

---

# What is Pulumi?

- Tooling for Cloud IaC
  - Create/Read/Update/Delete services
  - DSC - Desired State Configuration
- Use your choice or programming language
  - No YAML
  - No custom DSL
- Procedural and Imperative

---

```csharp
using Pulumi;
using Pulumi.AzureNative.Cache;
using Pulumi.AzureNative.Cache.Inputs;
using Pulumi.AzureNative.Web;
using Pulumi.AzureNative.Web.Inputs;
using System.Collections.Generic;

return await Deployment.RunAsync(() =>
{
    // Create an Azure Resource Group
    var resourceGroup = new Pulumi.AzureNative.Resources.ResourceGroup("resourceGroup");

    // Create an Azure Redis Cache instance
    var redisCache = new Redis("redisCache", new RedisArgs
    {
        ResourceGroupName = resourceGroup.Name,
        Location = resourceGroup.Location, // Use the same location as the resource group
        Sku = new SkuArgs
        {
            Name = SkuName.Basic, // Choose the SKU for Redis cache
            Family = SkuFamily.C,
            Capacity = 0 // Basic Tier, 250MB Cache
        }
    });

    // Create an App Service Plan for the App Service
    var appServicePlan = new AppServicePlan("appServicePlan", new AppServicePlanArgs
    {
        ResourceGroupName = resourceGroup.Name,
        Location = resourceGroup.Location,
        Kind = "App",
        Sku = new SkuDescriptionArgs
        {
            Name = "B1",
            Tier = "Basic"
        }
    });

    // Create the App Service instance
    var appService = new WebApp("appService", new WebAppArgs
    {
        ResourceGroupName = resourceGroup.Name,
        Location = resourceGroup.Location,
        ServerFarmId = appServicePlan.Id,
        SiteConfig = new SiteConfigArgs
        {
            // Storing the connection string to the Redis cache as an app setting
            AppSettings = new[]
            {
                new NameValuePairArgs
                {
                    Name = "RedisCacheConnection",
                    Value = Output.Format($"{redisCache.HostName}:6380,password={redisCache.PrimaryKey},ssl=True,abortConnect=False")
                }
            }
        }
    });

    // Export the App Service URL and the Redis cache primary key
    return new Dictionary<string, object?>
    {
        ["appServiceUrl"] = appService.DefaultHostName.Apply(hostName => $"https://{hostName}"),
        ["redisCachePrimaryKey"] = redisCache.PrimaryKey
    };
});
```

---

# What Pulumi isn't

- NOT a Cross-Platform abstraction
  - Clouds are target specifically
  - Ex: Cloud storage different between AWS S3 and Azure Blob Storage

---

# Programming Languages Supported

- Many programming languages supported and being added
  ![w:500 h:400](presentation-images/pulumi-languages-and-sdks.png)

---

# Cloud Providers Supported

- All the big ones
  - AWS, Azure, GCP, etc
- Other big but lesser known ones
  - DigitalOcean, Fastly, Scaleway, etc
- Utility SaaS Providers
  - Auth0, RedisCloud, DNSimple, GitHub, etc
- View all at: [pulumi.com/registry](https://www.pulumi.com/registry)
  - 1st party and 3rd party

---

# Demo 1 Time!

- Code
- Pulumi CLI
- Web Portal

---

# More Pulumi and IaC concepts

---

#

![bg contain](presentation-images/pulumi-state-flow.png)

---

# Input and Output Objects

- Object for a resource to be created
- Inputs
  - Become Outputs
- Outputs
  - Will have a value eventually...in the future
    - Ex: GUID id of a storage account
  - Used to modify a dependency in code

---

# Config

- YAML Files
- Per Stack
- Individual Key-Value pairs
  - Or objects

---

# Config Secrets

- Encrypted in config
  - Encryption key stored on Pulumi servers (by default)
- Per Stack
- Loaded as an Output value
- Plain-Text viewable via Pulumi CLI
  - When signed in

---

# Stack Outputs

- Set by You, your code
- Usable in:
  - Stack References
  - `Pulumi.README.md` files

---

# Pulumi A.I.

- Generate Pulumi Code using that thing everyone's talking about
- https://www.pulumi.com/ai

---

# Demo 2 Time!

- Pulumi Code
- Demo App

![bg right 80%](diagrams/demo-app.svg)

---

# Online Info

- @ProgrammerAL
- programmerAL.com

![bg right 80%](presentation-images/presentation_link_qrcode.png)
