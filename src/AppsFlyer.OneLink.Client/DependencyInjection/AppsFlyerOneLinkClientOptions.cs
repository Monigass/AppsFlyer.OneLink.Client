// <copyright file="OneLinkClient.cs">
// Copyright (c) Monigass.
// Licensed under the MIT License.
// </copyright>

namespace AppsFlyer.OneLink.Client.DependencyInjection;

using System;

/// <summary>
/// Options used to configure the AppsFlyer OneLink client registration.
/// </summary>
public sealed class AppsFlyerOneLinkClientOptions
{
    /// <summary>
    /// Gets or sets the base address used by the typed HTTP client.
    /// </summary>
    public Uri BaseAddress { get; set; }
}
