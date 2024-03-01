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

- "Assume Breach"
- HaveIBeenPwned.com
- Committed to Source Control
  - https://www.csoonline.com/article/571363/how-corporate-data-and-secrets-leak-from-github-repositories.html
- Can't avoid them 100%

---

# Azure Service Principals

- An "App" in Azure Entra
- Permissions assigned to it
  - Like a User
- Client Id and Client Secret
  - Anything can use this

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

# Managed Identities

- Identity object you assign permissions to
  - A type of Service Principal
  - Only works for Azure Managed Services
- You don't see Client Secret
- Assigned to services

---

# How to Create a Managed Identity

- Enable on individual services
  - AKA System Assigned
  - 1 instance exists
  - Deleted if service is deleted
- Create a service
  - AKS User Assigned
  - Separate service
  - Can be assigned to multiple services

---

# C# Code Changes

- Use `new Microsoft.Data.SqlClient.DefaultAzureCredential()`
  - Reference `Azure.Identity` NuGet package
- Instead of `new TableClient(MyConnectionString)`
- Use `new ConnectionClient(new DefaultAzureCredential())`
  - Plus assign permissions

#
