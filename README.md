# AppsFlyer.OneLink.Client

A reusable OpenAPI client generation framework for .NET built on top of NSwag.

This project automates the generation of strongly typed .NET SDKs from OpenAPI specifications.

The repository is provider-agnostic and can generate clients for any API that exposes an OpenAPI specification.

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
    "BaseUrl": "https://onelink.appsflyer.com",
    "ApiToken": "<your-api-token>",
    "OneLinkId": "<your-onelink-id>"
  }
}
```

### Options Class

```csharp
public sealed class AppsFlyerOptions
{
    public string BaseUrl { get; set; } = default!;
    public string ApiToken { get; set; } = default!;
    public string OneLinkId { get; set; } = default!;
}
```

---

## Dependency Injection

```csharp
builder.Services.Configure<AppsFlyerOptions>(
    builder.Configuration.GetSection("AppsFlyer"));

builder.Services.AddHttpClient<IAppsFlyerClient, AppsFlyerClient>();
```

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

var response = await appsFlyerClient.CreateLinkAsync(request);

Console.WriteLine(response.ShortlinkUrl);
```

### Example Output

```text
https://myapp.onelink.me/abc123/qwer987
```

---

## Retrieve Link

```csharp
var link = await appsFlyerClient.GetLinkAsync(
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

var response = await appsFlyerClient.UpdateLinkAsync(
    oneLinkId,
    shortLinkId,
    request);
```

---

## Delete Link

```csharp
await appsFlyerClient.DeleteLinkAsync(
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

# API Interface

```csharp
public interface IAppsFlyerClient
{
    Task<ShortLinkResponse> CreateLinkAsync(
        CreateLinkRequest request,
        CancellationToken cancellationToken = default);

    Task<GetLinkResponse> GetLinkAsync(
        string oneLinkId,
        string shortLinkId,
        CancellationToken cancellationToken = default);

    Task<ShortLinkResponse> UpdateLinkAsync(
        string oneLinkId,
        string shortLinkId,
        UpdateLinkRequest request,
        CancellationToken cancellationToken = default);

    Task DeleteLinkAsync(
        string oneLinkId,
        string shortLinkId,
        CancellationToken cancellationToken = default);

    Task<byte[]> GetQrCodeAsync(
        string oneLinkId,
        string shortLinkId,
        CancellationToken cancellationToken = default);

    Task<QuotaResponse> GetQuotaAsync(
        string accountId,
        CancellationToken cancellationToken = default);
}
```

---

# Architecture

```text
AppsFlyer.OneLink.Client
│
├── specs
│   └── AppsFlyer.OneLink.v2.openapi.yaml
│
├── src
│   ├── Generated
│   │   └── OneLinkClient.cs
│   │
│   ├── Models
│   │
│   ├── Options
│   │
│   ├── Services
│   │   └── AppsFlyerClient.cs
│   │
│   └── Extensions
│       └── ServiceCollectionExtensions.cs
│
└── tests
    └── AppsFlyer.OneLink.Client.Tests
```

---

# Regenerating the Client

Whenever the OneLink OpenAPI specification changes, regenerate the client using NSwag.

```bash
nswag run nswag.json
```

Example configuration:

```json
{
  "runtime": "Net90",
  "documentGenerator": {
    "fromDocument": {
      "url": "./specs/AppsFlyer.OneLink.v2.openapi.yaml"
    }
  },
  "codeGenerators": {
    "openApiToCSharpClient": {
      "className": "OneLinkClient",
      "namespace": "AppsFlyer.OneLink.Client.Generated"
    }
  }
}
```

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
NSwag Generated OneLinkClient
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