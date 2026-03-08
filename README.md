# ASP.NET Core Response Caching Sample

A beginner-friendly ASP.NET Core Minimal API sample demonstrating **Response Caching Middleware** with practical examples and Swagger/OpenAPI integration.

![.NET](https://img.shields.io/badge/.NET-10.0-blue)
![C#](https://img.shields.io/badge/C%23-12-blue)
![License](https://img.shields.io/badge/License-MIT-green)

## What is Response Caching?

Response Caching in ASP.NET Core allows you to cache HTTP responses on the server, client, or proxy servers. This sample demonstrates:
- **Server-side caching** via Response Caching Middleware
- **Client-side caching** with `ResponseCacheLocation.Client`
- **Vary by query keys** for query-dependent caching
- **Vary by headers** for header-based cache variations
- **Disabling cache** for sensitive data

## Key Features

- ✅ **Minimal API** architecture (no controllers, clean and simple)
- ✅ **Swagger/OpenAPI** integration for easy testing
- ✅ **7 practical endpoints** covering different caching scenarios
- ✅ **Beginner-friendly** inline comments explaining each concept
- ✅ **Repository Pattern** ready structure
- ✅ **.NET 10** with latest features

## API Endpoints

| Endpoint | Method | Description | Cache Duration |
|----------|--------|-------------|----------------|
| `/api/products` | GET | List all products | 60 seconds |
| `/api/products/by-category` | GET | Filter by category | 120 seconds (varies by query) |
| `/api/user-profile` | GET | User profile | 300 seconds (client-only) |
| `/api/time` | GET | Current UTC time | 10 seconds (varies by header) |
| `/api/random-number` | GET | Random number | No cache |
| `/api/private-data` | GET | Private data | No cache |
| `/api/login` | POST | Login endpoint | No cache (security) |
| `/api/cache-status` | GET | Check cache headers | No cache |

## Architecture Overview

```
┌─────────────────────────────────────────────────────────────┐
│                     Client Request                          │
└─────────────────────┬───────────────────────────────────────┘
                      │
                      ▼
┌─────────────────────────────────────────────────────────────┐
│              Response Caching Middleware                    │
│  ┌─────────────────────────────────────────────────────┐   │
│  │ • Checks Cache-Control headers                      │   │
│  │ • Stores cached responses in memory                 │   │
│  │ • Returns cached response if available              │   │
│  └─────────────────────────────────────────────────────┘   │
└─────────────────────┬───────────────────────────────────────┘
                      │
                      ▼
┌─────────────────────────────────────────────────────────────┐
│                   Minimal API Endpoints                     │
│  (Products, User Profile, Time, etc.)                       │
└─────────────────────┬───────────────────────────────────────┘
                      │
                      ▼
┌─────────────────────────────────────────────────────────────┐
│                 HTTP Response with Headers                  │
│  Cache-Control, Vary, Age, ETag                             │
└─────────────────────────────────────────────────────────────┘
```

## Getting Started

### Prerequisites

- [.NET 10.0 SDK](https://dotnet.microsoft.com/download/dotnet/10.0) or later

### Clone and Run

```bash
# Clone the repository
git clone https://github.com/codingdroplets/dotnet-response-caching.git
cd dotnet-response-caching

# Run the project
dotnet run --project src/DotNetResponseCaching.Api

# Open in browser
# Visit: http://localhost:5000/swagger
```

### Using Swagger

1. Run the application
2. Navigate to `http://localhost:5000/swagger`
3. Try different endpoints and observe cache headers:
   - Check **Response Headers** for `Cache-Control`, `Vary`, `Age`
   - Make same request twice - second request should be faster (cached)

### Testing Cache Behavior

```bash
# Test basic caching
curl -I http://localhost:5000/api/products
# Look for: Cache-Control: public, max-age=60

# Test query-based caching
curl "http://localhost:5000/api/products/by-category?category=Audio"
curl "http://localhost:5000/api/products/by-category?category=Electronics"
# Each category has its own cached version

# Test no-cache headers
curl -I http://localhost:5000/api/random-number
# Look for: Cache-Control: no-store, no-cache
```

## Cache Headers Explained

| Header | Description |
|--------|-------------|
| `Cache-Control` | Directives for caching (public, private, no-store, max-age) |
| `Vary` | Specifies which request headers affect caching |
| `Age` | Time in seconds the response was cached |
| `ETag` | Identifier for a specific version of content |

## Cache Profiles

The sample includes predefined cache profiles in `Program.cs`:

```csharp
// Default: 60 seconds, any location
// Short: 30 seconds, client only
// Long: 300 seconds, varies by category and page
```

## Use Cases

- **Products listing**: Cache for frequently accessed data
- **User profiles**: Client-side cache for personalized but stable data
- **Time/Date**: Short cache for real-time but not critical data
- **Login/Auth**: Never cache security-sensitive endpoints
- **Search results**: Query-based caching for filtered data

## What's NOT Covered

This sample covers **Response Caching** (HTTP response caching). Related but different topics:

- **Output Caching** - Caches entire rendered output (in-memory)
- **In-Memory Caching** - `IMemoryCache` for programmatic caching
- **Distributed Caching** - Redis/SQL Server for multi-server setups

## Tech Stack

- **.NET 10.0** - Latest .NET runtime
- **ASP.NET Core** - Web framework
- **Minimal APIs** - Lightweight API pattern
- **Swashbuckle** - Swagger/OpenAPI support
- **C# 12** - Modern C# features

## Project Structure

```
dotnet-response-caching/
├── src/
│   └── DotNetResponseCaching.Api/
│       ├── Program.cs           # Main application code
│       ├── appsettings.json     # Configuration
│       └── Properties/
│           └── launchSettings.json
├── DotNetResponseCaching.sln    # Solution file
└── README.md
```

## Contributing

Contributions are welcome! Please feel free to submit a Pull Request.

## License

This project is licensed under the MIT License.

## Author

**Coding Droplets**
- YouTube: [codingdroplets.com](https://codingdroplets.com)
- Patreon: [patreon.com/codingdroplets](https://www.patreon.com/codingdroplets)

---

**Visit Now**: https://codingdroplets.com

**Join our Patreon to Learn & Level Up**: https://www.patreon.com/codingdroplets

⭐ Star this repository if you found it helpful!