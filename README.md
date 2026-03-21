# Response Caching in ASP.NET Core (.NET 10)

> **Server-side, client-side, and proxy caching in one beginner-friendly sample** — 7 practical Minimal API endpoints covering every caching scenario with Swagger/OpenAPI.

[![.NET](https://img.shields.io/badge/.NET-10.0-512BD4?logo=dotnet)](https://dotnet.microsoft.com/)
[![ASP.NET Core](https://img.shields.io/badge/ASP.NET%20Core-10.0-512BD4)](https://docs.microsoft.com/aspnet/core)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](LICENSE)
[![Visit CodingDroplets](https://img.shields.io/badge/Website-codingdroplets.com-blue?style=flat&logo=google-chrome&logoColor=white)](https://codingdroplets.com/)
[![YouTube](https://img.shields.io/badge/YouTube-CodingDroplets-red?style=flat&logo=youtube&logoColor=white)](https://www.youtube.com/@CodingDroplets)
[![Patreon](https://img.shields.io/badge/Patreon-Support%20Us-orange?style=flat&logo=patreon&logoColor=white)](https://www.patreon.com/CodingDroplets)
[![Buy Me a Coffee](https://img.shields.io/badge/Buy%20Me%20a%20Coffee-Support%20Us-yellow?style=flat&logo=buy-me-a-coffee&logoColor=black)](https://buymeacoffee.com/codingdroplets)
[![GitHub](https://img.shields.io/badge/GitHub-codingdroplets-black?style=flat&logo=github&logoColor=white)](http://github.com/codingdroplets/)

---

## 🚀 Support the Channel — Join on Patreon

If this sample saved you time, consider joining our Patreon community.
You'll get **exclusive .NET tutorials, premium code samples, and early access** to new content — all for the price of a coffee.

👉 **[Join CodingDroplets on Patreon](https://www.patreon.com/CodingDroplets)**

Prefer a one-time tip? [Buy us a coffee ☕](https://buymeacoffee.com/codingdroplets)

---

## 🎯 What You'll Learn

- How to configure **Response Caching Middleware** in ASP.NET Core
- How to cache responses on the **server**, **client**, and **proxy** with `Cache-Control`
- How to use **Vary by query key** for query-parameter-dependent caching
- How to use **Vary by header** for header-based cache variations
- How to **disable caching** for sensitive or dynamic endpoints
- How to inspect cache behaviour using `Cache-Control`, `Vary`, and `Age` response headers

---

## 🗺️ Architecture Overview

```
Client Request
      │
      ▼
┌─────────────────────────────────────────────────────────────┐
│                     Client Request                          │
└─────────────────────┬───────────────────────────────────────┘
                      │
                      ▼
┌─────────────────────────────────────────────────────────────┐
│              Response Caching Middleware                    │
│  ┌─────────────────────────────────────────────────────┐   │
│  │  Cached response exists?                            │   │
│  │  YES → return cached response (Age header added)    │   │
│  │  NO  → forward to endpoint → cache the response     │   │
│  └─────────────────────────────────────────────────────┘   │
└─────────────────────┬───────────────────────────────────────┘
                      │
                      ▼
┌─────────────────────────────────────────────────────────────┐
│                   Minimal API Endpoints                     │
│  Products / User Profile / Time / Random / Private / Login  │
└─────────────────────┬───────────────────────────────────────┘
                      │
                      ▼
┌─────────────────────────────────────────────────────────────┐
│              HTTP Response + Cache Headers                  │
│         Cache-Control · Vary · Age · ETag                   │
└─────────────────────────────────────────────────────────────┘
```

---

## 📋 Endpoints & Caching Strategies

| Endpoint | Method | Cache Strategy | Duration |
|----------|--------|----------------|----------|
| `/api/products` | GET | Server + public | 60 seconds |
| `/api/products/by-category` | GET | Vary by `category` query | 120 seconds |
| `/api/user-profile` | GET | Client-only | 300 seconds |
| `/api/time` | GET | Vary by `Accept-Language` header | 10 seconds |
| `/api/random-number` | GET | No cache (`no-store`) | — |
| `/api/private-data` | GET | No cache (private) | — |
| `/api/login` | POST | No cache (security) | — |
| `/api/cache-status` | GET | No cache (diagnostic) | — |

---

## 📁 Project Structure

```
dotnet-response-caching/
├── DotNetResponseCaching.sln
└── src/
    └── DotNetResponseCaching.Api/
        ├── Program.cs           # Middleware setup, endpoints, cache profiles
        ├── appsettings.json
        └── Properties/
            └── launchSettings.json
```

---

## 🛠️ Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)
- Any IDE: Visual Studio 2022+, VS Code, or JetBrains Rider

---

## ⚡ Quick Start

```bash
# Clone the repo
git clone https://github.com/codingdroplets/dotnet-response-caching.git
cd dotnet-response-caching

# Run the API
dotnet run --project src/DotNetResponseCaching.Api

# Open Swagger UI → http://localhost:5000/swagger
```

---

## 🔧 How It Works

### Step 1 — Register Middleware and Cache Profiles

```csharp
builder.Services.AddResponseCaching();

builder.Services.AddControllers(options =>
{
    options.CacheProfiles.Add("Default",
        new CacheProfile { Duration = 60 });       // 60s, any location
    options.CacheProfiles.Add("Short",
        new CacheProfile { Duration = 30, Location = ResponseCacheLocation.Client });
    options.CacheProfiles.Add("Long",
        new CacheProfile { Duration = 300, VaryByQueryKeys = new[] { "category", "page" } });
});

// Must be placed before routing middleware
app.UseResponseCaching();
```

### Step 2 — Decorate Endpoints with Cache Behaviour

```csharp
// Server-side cache for 60 seconds
app.MapGet("/api/products", GetProducts)
   .CacheOutput(p => p.Expire(TimeSpan.FromSeconds(60)));

// Vary by query key — each category cached separately
app.MapGet("/api/products/by-category", GetByCategory)
   .CacheOutput(p => p.Expire(TimeSpan.FromSeconds(120)).SetVaryByQuery("category"));

// Client-only — no server-side store
app.MapGet("/api/user-profile", GetUserProfile)
   .CacheOutput(p => p.Expire(TimeSpan.FromSeconds(300)).NoStore());

// No caching for sensitive/dynamic endpoints
app.MapPost("/api/login", Login)
   .CacheOutput(p => p.NoStore());
```

### Step 3 — Read the Cache Headers

Make a request, then check response headers:

```bash
curl -I http://localhost:5000/api/products
# Cache-Control: public, max-age=60
# Age: 0   ← first hit (not yet cached)

curl -I http://localhost:5000/api/products
# Age: 4   ← served from cache
```

---

## 🤔 Cache Headers Explained

| Header | Description |
|--------|-------------|
| `Cache-Control` | Directives: `public`, `private`, `no-store`, `max-age` |
| `Vary` | Which request headers affect cache key (`Accept-Language`, etc.) |
| `Age` | Seconds this response has been in cache |
| `ETag` | Identifier for a specific version of the content |

---

## 🤔 Response Caching vs Related Concepts

| Type | What It Caches | Storage | Use When |
|------|---------------|---------|----------|
| **Response Caching** | HTTP responses (this sample) | Server memory / proxy | Public, rarely-changing API responses |
| **Output Caching** | Entire rendered output | In-memory (server) | Razor Pages, full page caching |
| **In-Memory Caching** | Arbitrary objects (`IMemoryCache`) | Server memory | Internal data, computed results |
| **Distributed Caching** | Arbitrary objects (Redis / SQL) | External store | Multi-server / load-balanced apps |

---

## 📚 References

- [Response caching in ASP.NET Core — Microsoft Learn](https://learn.microsoft.com/en-us/aspnet/core/performance/caching/response)
- [Cache-Control directives — MDN Web Docs](https://developer.mozilla.org/en-US/docs/Web/HTTP/Headers/Cache-Control)
- [HTTP Caching — RFC 9111](https://www.rfc-editor.org/rfc/rfc9111)

---

## 📄 License

This project is licensed under the [MIT License](LICENSE).

---

## 🔗 Connect with CodingDroplets

| Platform | Link |
|----------|------|
| 🌐 Website | https://codingdroplets.com/ |
| 📺 YouTube | https://www.youtube.com/@CodingDroplets |
| 🎁 Patreon | https://www.patreon.com/CodingDroplets |
| ☕ Buy Me a Coffee | https://buymeacoffee.com/codingdroplets |
| 💻 GitHub | http://github.com/codingdroplets/ |

> **Want more samples like this?** [Support us on Patreon](https://www.patreon.com/CodingDroplets) or [buy us a coffee ☕](https://buymeacoffee.com/codingdroplets) — every bit helps keep the content coming!
