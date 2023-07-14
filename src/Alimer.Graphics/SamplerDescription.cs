﻿// Copyright © Amer Koleci and Contributors.
// Licensed under the MIT License (MIT). See LICENSE in the repository root for more information.

namespace Alimer.Graphics;

/// <summary>
/// Structure that describes the <see cref="Sampler"/>.
/// </summary>
public readonly record struct SamplerDescription
{
    public SamplerDescription()
    {

    }

    /// <summary>
    /// Gets or sets the min filter of <see cref="Sampler"/>
    /// </summary>
    public SamplerMinMagFilter MinFilter { get; init; } = SamplerMinMagFilter.Nearest;

    /// <summary>
    /// Gets or sets the mag filter of <see cref="Sampler"/>
    /// </summary>
    public SamplerMinMagFilter MagFilter { get; init; } = SamplerMinMagFilter.Nearest;

    /// <summary>
    /// Gets or sets the mip filter of <see cref="Sampler"/>
    /// </summary>
    public SamplerMipFilter MipFilter { get; init; } = SamplerMipFilter.Nearest;

    public SamplerAddressMode AddressModeU { get; init; } = SamplerAddressMode.Wrap;
    public SamplerAddressMode AddressModeV { get; init; } = SamplerAddressMode.Wrap;
    public SamplerAddressMode AddressModeW { get; init; } = SamplerAddressMode.Wrap;

    public float LodMinClamp { get; init; } = 0.0f;
    public float LodMaxClamp { get; init; } = float.MaxValue;
    public ushort MaxAnisotropy { get; init; } = 1;

    public SamplerReductionType ReductionType { get; init; } = SamplerReductionType.Standard;

    public CompareFunction Compare { get; init; } = CompareFunction.Never;


    /// <summary>
    /// Gets or sets the label of <see cref="Sampler"/>.
    /// </summary>
    public string? Label { get; init; }
}
