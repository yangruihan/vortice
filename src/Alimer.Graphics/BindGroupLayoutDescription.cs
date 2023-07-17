﻿// Copyright © Amer Koleci and Contributors.
// Licensed under the MIT License (MIT). See LICENSE in the repository root for more information.

using CommunityToolkit.Diagnostics;

namespace Alimer.Graphics;

/// <summary>
/// Structure that describes a <see cref="BindGroupLayout"/>.
/// </summary>
public readonly record struct BindGroupLayoutDescription
{
    public BindGroupLayoutDescription()
    {
        Entries = Array.Empty<BindGroupLayoutEntry>();
    }

    public BindGroupLayoutDescription(params BindGroupLayoutEntry[] entries)
    {
        Guard.IsGreaterThan(entries.Length, 0, nameof(entries));

        Entries = entries;
    }

    public BindGroupLayoutEntry[] Entries { get; init; }

    /// <summary>
    /// The label of <see cref="BindGroupLayout"/>.
    /// </summary>
    public string? Label { get; init; }
}

/// <summary>
/// Single entry for <see cref="BindGroupLayout"/>.
/// </summary>
public readonly record struct BindGroupLayoutEntry
{
    public BindGroupLayoutEntry(DescriptorType type, uint binding, ShaderStages visibility)
    {
        Type = type;
        Binding = binding;
        Visibility = visibility;
    }

    /// <summary>
    /// Type of resources in this descriptor binding.
    /// </summary>
    public DescriptorType Type { get; init; }

    /// <summary>
    /// Register index to bind to (supplied in shader).
    /// </summary>
    public uint Binding { get; init; }

    /// <summary>
    /// The shader stage the resources will be accessible to.
    /// </summary>
    public ShaderStages Visibility { get; init; }
}

public enum DescriptorType : byte
{
    ConstantBuffer,
}