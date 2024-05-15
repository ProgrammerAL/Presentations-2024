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

![bg contain](presentation-images/padme-meme.jpg)

---

# Server History Lesson

- Physical Hardware
- Physical Hardware running VMs
- Co-located hardware running VMs
- VMs in a "cloud"
- Cloud with IaaS/PaaS/etc services <--today!

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

- DevOps
- 

---

# What's different about IaC code?

- Different type of application
  - Like everything else: Web UI, Backend Server, Embedded, Video Game, IaC
- Still code
  - YAML, JSON, Custom DSL, or Your Choice of Language

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

# C# Example Code

```csharp
using System.Threading.Tasks;
using Pulumi;
using Pulumi.Aws.S3;

await Deployment.RunAsync(() =>
{
    // Create an AWS resource (S3 Bucket)
    var bucket = new Bucket("my-bucket");

    // Export the name of the bucket
    return new Dictionary<string, object>
    {
        { "bucketName", bucket.Id },
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
