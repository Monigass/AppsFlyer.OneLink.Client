// <copyright file="OneLinkClient.cs">
// Copyright (c) Monigass.
// Licensed under the MIT License.
// </copyright>

namespace AppsFlyer.OneLink.Client;

using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;

/// <inheritdoc/>
public partial class AppsFlyerClient : IAppsFlyerClient
{
    static partial void UpdateJsonSerializerSettings(JsonSerializerOptions settings)
    {
        settings.PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower;
        settings.DictionaryKeyPolicy = JsonNamingPolicy.SnakeCaseLower;
        settings.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
        settings.Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping;
    }
}
