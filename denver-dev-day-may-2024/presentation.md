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

- Overview of Infrastructure as Code (IaC)
  - For Developers
- Introduction to Pulumi
- Demo with C# and Azure
  - Concepts apply to other languages/clouds

---

# Server History Lesson

- Physical hardware
- VMs on physical hardware
- VMs on Co-located hardware
- VMs in a "cloud"
- Cloud with IaaS/PaaS/etc services <--today!

---

![bg contain](presentation-images/epic-handshake-meme.jpg)

---

![bg contain](presentation-images/padme-meme.jpg)

---

# Infrastructure as Code (IaC)

- A concept, not a technology
- Usually Desired State Config (DSC)
- Code!
  - Repeatable
  - Create more environments with ease
  - Updated with a PR
  - YAML, JSON, Custom DSL, or Your Choice of Language

---

# How is IaC run?

- CI/CD pipeline
- Manually from someone's machine

---

# Why should devs care?

- Devs know their apps
  - What cloud resources their apps need
  - When the cloud resources need to be added
- Con: More work shifted-left

---

# Cloud IaC Tools

- Azure ARM/Bicep
- AWS Cloudformation
- Terraform/OpenTofu
- Pulumi

---

# What do I need to know for IaC?

- Cloud(s)
- IaC Tool(s)
- Programming Language(s)*

---

# What is Pulumi?

- Tooling for Cloud IaC
  - Create/Read/Update/Delete cloud services
- Use your choice or programming language*
- Procedural code
  - Imperative runtime
  - DSC - Desired State Configuration

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
    var resourceGroup = new Pulumi.AzureNative.Resources.ResourceGroup("myresourceGroup");

    // Create an App Service Plan for the App Service
    var appServicePlan = new AppServicePlan("myappServicePlan", new AppServicePlanArgs
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
    var appService = new WebApp("myappService", new WebAppArgs
    {
        ResourceGroupName = resourceGroup.Name,
        Location = resourceGroup.Location,
        ServerFarmId = appServicePlan.Id,
    });

    // Export the App Service URL and the Redis cache primary key
    return new Dictionary<string, object?>
    {
        ["appServiceUrl"] = appService.DefaultHostName.Apply(hostName => $"https://{hostName}"),
    };
});
```
---

# What Pulumi isn't

- NOT an abstraction over clouds
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

# Config

- Built in Config
  - YAML Files
  - Per Stack
  - Individual Key-Value pairs
    - Or objects
- Encrypted in config
  - Encryption key stored on Pulumi servers (by default)
- Or whatever you want*
  - Custom code
- OR Pulumi ESC

---

# Stack Outputs

- Set by You, your code
- Usable in:
  - Stack References
  - `Pulumi.README.md` files

---

# Resource References

- "Get" Functions
- Loaded as Read Only variables

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
