# AppsFlyer.OneLink.Client

A strongly typed .NET client for the AppsFlyer OneLink API, generated from its OpenAPI specification with NSwag.

## Disclaimer

This project is an independent community project.

This repository is not affiliated with, endorsed by, sponsored by, or maintained by any API provider referenced in this repository.

All trademarks, service marks, product names, and company names are the property of their respective owners.

The OpenAPI specifications included in this repository are derived from publicly available API documentation and are intended solely to facilitate interoperability and client generation.

Generated SDKs are intended to simplify integration with publicly exposed APIs.

Users are responsible for complying with the terms, conditions, and usage policies of the APIs they choose to consume.

---

## Overview

`AppsFlyer.OneLink.Client` provides a typed client for creating, updating, retrieving, deleting, and managing AppsFlyer OneLink short links.

The project is built around:

- OpenAPI specification (`AppsFlyer.OneLink.v2.openapi.yaml`)
- NSwag code generation
- Dependency Injection support
- Typed request and response models
- HttpClientFactory integration

This library abstracts the OneLink REST API and allows consumers to work with .NET objects instead of manually building HTTP requests.

## Features

- Create OneLink links
- Get OneLink data
- Update OneLink links
- Delete OneLink links
- Generate QR codes
- Get account quota information
- Strongly typed contracts
- Async-first API
- DI-friendly design
- Auto-generated from OpenAPI

---

## Supported Endpoints

| Operation | Endpoint |
|------------|------------|
| Create Link | POST `/api/v2.0/shortlinks/{onelink-id}` |
| Get Link Data | GET `/api/v2.0/shortlinks/{onelink-id}/{shortlink-id}` |
| Update Link | PUT `/api/v2.0/shortlinks/{onelink-id}/{shortlink-id}` |
| Delete Link | DELETE `/api/v2.0/shortlinks/{onelink-id}/{shortlink-id}` |
| Get QR Code | GET `/api/v2.0/shortlinks/{onelink-id}/{shortlink-id}/qr` |
| Get Quota | GET `/api/v2.0/shortlinks-quota/{account-id}` |

---

## Installation

### NuGet

```bash
dotnet add package AppsFlyer.OneLink.Client
```

---

## Configuration

### appsettings.json

```json
{
  "AppsFlyer": {
    "BaseAddress": "https://onelink.appsflyer.com/",
    "OneLinkApiToken": "<your-api-token>"
  }
}
```

### Built-in Options Class

```csharp
public sealed class AppsFlyerOneLinkClientOptions
{
    public Uri BaseAddress { get; set; }
}
```

---

## Dependency Injection

```csharp
using AppsFlyer.OneLink.Client.DependencyInjection;

builder.Services.AddAppsFlyerOneLinkClient(options =>
{
    options.BaseAddress = new Uri("https://onelink.appsflyer.com/");
});
```

The client can then be injected into any registered service:

```csharp
public sealed class LinkService
{
    private readonly IAppsFlyerClient client;

    public LinkService(IAppsFlyerClient client)
    {
        this.client = client;
    }
}
```

### Custom HTTP Client Configuration

Use the generic registration when the HTTP client must be configured from another service, such as options loaded from `appsettings.json`:

```csharp
using System.Net.Http.Headers;
using System.Net.Mime;
using AppsFlyer.OneLink.Client;
using AppsFlyer.OneLink.Client.DependencyInjection;
using Microsoft.Extensions.Options;

public sealed class DeepLinkOptions
{
    public Uri BaseAddress { get; set; }
    public string OneLinkApiToken { get; set; }
}

builder.Services.Configure<DeepLinkOptions>(
    builder.Configuration.GetSection("AppsFlyer"));

builder.Services.AddAppsFlyerClient<IAppsFlyerClient, AppsFlyerClient>(
    (provider, client) =>
    {
        var options = provider
            .GetRequiredService<IOptions<DeepLinkOptions>>()
            .Value;

        client.BaseAddress = options.BaseAddress;
        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", options.OneLinkApiToken);
        client.DefaultRequestHeaders.Accept.Add(
            new MediaTypeWithQualityHeaderValue(MediaTypeNames.Application.Json));
    });
```

The typed client is registered through `HttpClientFactory` and includes transient HTTP error retries.

---

# Usage

## Create Link

```csharp
var request = new CreateLinkRequest
{
    Ttl = "30d",
    Data = new Dictionary<string, object>
    {
        ["pid"] = "email",
        ["c"] = "welcome-campaign",
        ["af_dp"] = "myapp://home"
    }
};

var response = await appsFlyerClient.CreateOneLinkAsync(request);

Console.WriteLine(response.ShortlinkUrl);
```

### Example Output

```text
https://myapp.onelink.me/abc123/qwer987
```

---

## Retrieve Link

```csharp
var link = await appsFlyerClient.GetOneLinkAsync(
    oneLinkId,
    shortLinkId);

Console.WriteLine(link.ShortlinkUrl);
```

---

## Update Link

```csharp
var request = new UpdateLinkRequest
{
    Ttl = "90d",
    Data = new Dictionary<string, object>
    {
        ["pid"] = "push",
        ["c"] = "winter-campaign"
    }
};

var response = await appsFlyerClient.UpdateOneLinkAsync(
    oneLinkId,
    shortLinkId,
    request);
```

---

## Delete Link

```csharp
await appsFlyerClient.DeleteOneLinkAsync(
    oneLinkId,
    shortLinkId);
```

---

## Get QR Code

```csharp
var qrBytes = await appsFlyerClient.GetQrCodeAsync(
    oneLinkId,
    shortLinkId);

await File.WriteAllBytesAsync(
    "onelink-qr.png",
    qrBytes);
```

---

## Get Account Quota

```csharp
var quota = await appsFlyerClient.GetQuotaAsync(accountId);

Console.WriteLine(quota.RemainingQuota);
```

---

The generated `IAppsFlyerClient` interface is included in the package and is the recommended abstraction to inject into application services.

---

# Architecture

```text
AppsFlyer.OneLink.Client
├── src
│   └── AppsFlyer.OneLink.Client
│       ├── AppsFlyer.OneLink.Client.csproj
│       ├── AppsFlyerClient.cs
│       ├── IAppsFlyerClient.cs
│       ├── DependencyInjection
│       │   ├── AppsFlyerOneLinkClientOptions.cs
│       │   └── ServiceCollectionExtensions.cs
│       └── Resources
│           └── AppsFlyer.OneLink.v2.openapi.yaml
├── config
│   ├── properties
│   │   └── AssemblyCommon.props
│   └── stylecop.json
└── README.md
```

---

# Regenerating the Client

Whenever `src/AppsFlyer.OneLink.Client/Resources/AppsFlyer.OneLink.v2.openapi.yaml` changes, regenerate the client through the project build:

```bash
dotnet build src/AppsFlyer.OneLink.Client/AppsFlyer.OneLink.Client.csproj
```

The project uses the `<OpenApiReference>` configured in `src/AppsFlyer.OneLink.Client/AppsFlyer.OneLink.Client.csproj` and the `NSwagCSharp` code generator. Generated files are build artifacts and are not stored as source files in the repository.

---

# Design Principles

The generated NSwag client should not be consumed directly by applications.

Recommended architecture:

```text
Consumer
    ↓
IAppsFlyerClient
    ↓
AppsFlyerClient
    ↓
NSwag-generated client
    ↓
AppsFlyer OneLink API
```

This separation provides:

- Easier unit testing
- Cleaner public API
- Centralized error handling
- Ability to evolve independently from NSwag-generated code
- Backward compatibility for consumers

---

# License

MIT
