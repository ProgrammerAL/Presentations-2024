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

# Nullable References Types (NRTs)

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

# Treat Warnings as Errors

- Single Line in `*.csproj` file
- Can disable individual warnings

```xml
<PropertyGroup>
	<TreatWarningsAsErrors>true</TreatWarningsAsErrors>
	<NoWarn>$(NoWarn);NU5104</NoWarn>
<PropertyGroup>
```

---

# Static Code Analysis

- Check Code for Violations
- 

---

# .editorconfig Files

- Static Code Analysis
- Extensible
- Open Standard / Configurable / etc

---

# Roslyn Analyzers

- Built-in ones
- Add with NuGets
- Build your own


---

# Built-In Roslyn Analyzers

```xml
<PropertyGroup>
  <EnableNETAnalyzers>true</EnableNETAnalyzers>
  <EnforceCodeStyleInBuild>true</EnforceCodeStyleInBuild>
</PropertyGroup>
```

---


# Online Info

- @ProgrammerAL
- programmerAL.com

![bg right 80%](presentation-images/presentation_link_qrcode.png)
