---
marp: true
title: Managed Identities: Connect Without Connection Strings
paginate: true
theme: gaia
author: Al Rodriguez
---

# Azure Managed Identities: Connect Without Connection Strings

with AL Rodriguez

![bg right 80%](presentation-images/presentation_link_qrcode.png)

---

![bg](presentation-images/sponsors.png)

---

# Me (AL)

- @ProgrammerAL
- ProgrammerAL.com
- Senior Azure Cloud Engineer at Microsoft

![bg right 80%](presentation-images/presentation_link_qrcode.png)

---

# Today's Goal

- Learn what Azure Managed Identities are
- Convert app using Connection Strings to use Managed Identities
- Azure-specific session

---

# Security has Layers

- Today's topic: 1 of those layers
- Secrets Management

---

# What are Secrets?

- Tokens/Passwords/Connection Strings
- Plain text
- Easy to use
  - Anyone can use them
- You have to manage:
  - Where they're stored
  - Access to them
  - Rotate them

---

# Secrets Leak

- HaveIBeenPwned.com
- Committed to Source Control
  - https://www.csoonline.com/article/571363/how-corporate-data-and-secrets-leak-from-github-repositories.html
- Python Personal Access Token leaked to a Docker Image
  - https://www.techradar.com/pro/security/github-token-leak-could-have-put-the-entire-python-language-at-risk
- Disclaimer: Can't always avoid using secrets

---

# Azure Key Vault

- Meant for storing sensitive data like secrets
- Who uses it?
- How do you use it?
- How do you connect to it?
  - Connection String?
  - Service Principal?
  - Something else?

---

# Azure Service Principals

- An "App" in Microsot Entra ID (formerly Azure AD)
- Permissions assigned to it
  - Like a User or Service Account
- Service Principal credentials given to apps to use
- Credentials are:
  - Client Id and Client Secret
  - Plain text values - anything can use them

---

# Managed Identities

- Abstraction over a Service Principal
- Only works for Azure Managed Services
  - You don't see Client Secret

---

# "Managed" means...

- Microsoft does the hard work
- Specific to Azure
- Managed Identitied authenticate to "Managed" PaaS Services like:
  - Managed SQL Server
  - Storage Accounts
  - Service Bus
  - Key Vault
  - Etc

---

# Which Azure services can use Managed Identities?

- Azure App Service
- Azure Functions
- Azure Container Apps
- Azure Kubernetes Service (AKS)
- Azure Container Instances
- Full list at: https://learn.microsoft.com/en-us/entra/identity/managed-identities-azure-resources/managed-identities-status

---

# How to Create a Managed Identity in Azure

- User Assigned
  - Separate service
  - Can be assigned to multiple services
- System Assigned
  - Exists as part of the parent service
  - Deleted if service is deleted

---

# DefaultAzureCredential

- Abstraction to load signed in credential
- Loads credentials from many places
  - Managed Identity
  - Local Azure CLI
  - Local Azure PowerShell
  - Environment Variables
  - and more
- Can disable ones you don't use

---

# ChainedTokenCredential 

- Similar to `DefaultAzureCredential`
- Only add the ones you use

---

# C# Code Changes

1. Reference `Azure.Identity` NuGet package
1. Instead of `new TableClient(MyConnectionString)`
1. Use `new TableClient("https...", new Azure.Identity.DefaultAzureCredential())`
  - Note: Some cases are a little different. Ex: Managed SQL Server uses a sligthly different connection string

---

# Online Info

- @ProgrammerAL
- programmerAL.com

![bg right 80%](presentation-images/presentation_link_qrcode.png)
