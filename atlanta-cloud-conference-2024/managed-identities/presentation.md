---
marp: true
title: Managed Identities: Connect Without Connection Strings
paginate: true
theme: gaia
author: Al Rodriguez
---

# Managed Identities: Connect Without Connection Strings

with AL Rodriguez

![bg right 80%](presentation-images/presentation_link_qrcode.png)

---

# Me (AL)

- @ProgrammerAL
- ProgrammerAL.com

![bg right 80%](presentation-images/presentation_link_qrcode.png)

---

# Today's Goal

- Learn what Azure Managed Identities are
- Convert app using Connection Strings to use Managed Identities

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
- Persistent Threats to Microsoft after 1 leaked secret
  - https://msrc.microsoft.com/blog/2024/03/update-on-microsoft-actions-following-attack-by-nation-state-actor-midnight-blizzard/
- Can't always avoid using secrets

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

- Identity object you assign permissions to
  - A type of Service Principal
  - Only works for Azure Managed Services
- You don't see Client Secret
- An account assigned to services

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

# How to Create a Managed Identity

- Create a service
  - AKA User Assigned
  - Separate service
  - Can be assigned to multiple services
- Enable on individual services
  - AKA System Assigned
  - Exists as part of the parent service
  - Deleted if service is deleted

---

# C# Code Changes

- Use `new Microsoft.Data.SqlClient.DefaultAzureCredential()`
  - Note: Some cases are a little different. Ex: Managed SQL Server uses a sligthly different connection string

1. Reference `Azure.Identity` NuGet package
1. Instead of `new TableClient(MyConnectionString)`
1. Use `new TableClient("https...", new DefaultAzureCredential())`

# Online Info

- @ProgrammerAL
- programmerAL.com

![bg right 80%](presentation-images/presentation_link_qrcode.png)
