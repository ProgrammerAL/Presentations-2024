---
marp: true
title: Managed Identities: Connect Without Connection Strings
paginate: true
theme: gaia
author: Al Rodriguez
---

# Managed Identities: Connect Without Connection Strings

with AL Rodriguez

<!-- ![bg right 80%](presentation-images/presentation_link_qrcode.png) -->

---

# Me (AL)

- @ProgrammerAL
- ProgrammerAL.com

<!-- ![bg right 80%](presentation-images/presentation_link_qrcode.png) -->

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
- Can try to avoid using secrets
  - But not 100%

---

# Azure Service Principals

- An "App" in Azure Entra
- Permissions assigned to it
  - Like a User or Service Account
- Client Id and Client Secret
  - Anything can use this

---

# Managed Identities

- Identity object you assign permissions to
  - A type of Service Principal
  - Only works for Azure Managed Services
- You don't see Client Secret
- Assigned to services

---

# Managed Services

- Specific to Azure
- "Managed" means Microsoft does the hard work
- PaaS Services
  - Managed SQL Server
  - Storage Accounts
  - Service Bus
  - Etc

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

1. Reference `Azure.Identity` NuGet package
1. Instead of `new TableClient(MyConnectionString)`
1. Use `new TableClient("https...", new DefaultAzureCredential())`
1. Note: Not for everything. ex: Managed SQL Server uses a sligthly different connection string

#
