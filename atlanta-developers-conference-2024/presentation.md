---
marp: true
title: Setting Up Your C# Pit of Success
paginate: true
theme: gaia
author: Al Rodriguez
---

# Setting Up Your C# Pit of Success

with AL Rodriguez

---

# Me (AL)

- @ProgrammerAL
- ProgrammerAL.com
- Senior Azure Cloud Engineer at Microsoft

![bg right 80%](presentation-images/presentation_link_qrcode.png)

---

# Pit of Success

```text
The Pit of Success: in stark contrast to a summit, a peak, or a journey across a desert to find victory through many trials and surprises, we want our customers to simply fall into winning practices by using our platform and frameworks. To the extent that we make it easy to get into trouble we fail.
           
-Rico Mariani, MS Research MindSwap Oct 2003.
```

- https://blog.codinghorror.com/falling-into-the-pit-of-success/

---

# Why are we here?

- Discuss features of C#/.NET to enforce code quality
- Present ***Recommendations***
- Please limit your yelling

---

# Recommendation: Nullable References Types (NRTs)

- Stop yelling at me!
  - They're good! I swear.
- Not as hard as Rust!
- Some code hassle can be mitigated

---

# What are NRTs?

- Nullable Reference Types
- Requirement by compiler to check for null
- If something can be null, compiler warning is made

---

# Mitigating Extra Code for NRTs

- Attributes
  - `[NotNullWhen]`, `[MemberNotNullWhen]`, etc
    - Avoid [NotNull] when possible. Useful for IoC
    - Full List: https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/attributes/nullable-analysis
- `required` keyword


--- 

# Recommendation: Treat Warnings as Errors

- Don't compile if warning found
  - Ex: Not using method return
- Single Line in `*.csproj` file
- Can disable individual warnings

```xml
<PropertyGroup>
  <TreatWarningsAsErrors>true</TreatWarningsAsErrors>
  <NoWarn>$(NoWarn);NU5104</NoWarn>
<PropertyGroup>
```

---

# Recommendation: Static Code Analysis

- Check Code for common 
- Sometimes fixes

---

# .editorconfig File

- Extensible/ Open Standard / Configurable / etc
- Single file checked into source control

---

# Roslyn Analyzers

- Built-in ones
- Add with NuGets
- Build your own
- May include code fixes

---

# Built-In Roslyn Analyzers

```xml
<PropertyGroup>
  <IncludeOpenAPIAnalyzers>true</IncludeOpenAPIAnalyzers>
  <EnableNETAnalyzers>true</EnableNETAnalyzers>
  <EnforceCodeStyleInBuild>true</EnforceCodeStyleInBuild>
  <EnableRequestDelegateGenerator>true</EnableRequestDelegateGenerator>
  <EnableConfigurationBindingGenerator>true</EnableConfigurationBindingGenerator>
</PropertyGroup>
```

- Note: `Microsoft.CodeAnalysis.NetAnalyzers` Package or `<EnableNETAnalyzers>` replaced FxCop

---

# Add Analyzers with NuGet Packages

- `*.Analyzers` package
- `SonarAnalyzer.CSharp`

---

# Custom Roslyn Analyzers

- Make your own
  - Specific to your projects
- Example:
  - All ASP.NET Controller Endpoints must include `[Authorize]` or `[AllowAnonymous]` attribute

---

# Recommendation: Code Generators

- Like Roslyn An

---

# Protect Against NuGet CVEs

```xml
<PropertyGroup>
  <NuGetAuditMode>all</NuGetAuditMode>
  <NuGetAuditLevel>low</NuGetAuditLevel>
</PropertyGroup>
```

- Adds build warnings
  - https://learn.microsoft.com/en-us/nuget/concepts/auditing-packages

---

# .NET Aspire

- Handy for local feedback
- Set config with service discovery instead of hard coding

---

# Ahead of Time (AoT) Compilation

- Can probably skip it for your apps
- Consider it for public NuGets

---

# Online Info

- @ProgrammerAL
- programmerAL.com

![bg right 80%](presentation-images/presentation_link_qrcode.png)
