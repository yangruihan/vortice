// Copyright (c) Amer Koleci and Contributors.
// Licensed under the MIT License (MIT). See LICENSE in the repository root for more information.

namespace Alimer.Graphics;

/// <summary>
/// Define a pixel format.
/// </summary>
public enum PixelFormat
{
    Undefined = 0,
    // 8-bit formats
    R8Unorm,
    R8Snorm,
    R8Uint,
    R8Sint,
    // 16-bit formats
    R16Unorm,
    R16Snorm,
    R16Uint,
    R16Sint,
    R16Float,
    RG8Unorm,
    RG8Snorm,
    RG8Uint,
    RG8Sint,
    // Packed 16-Bit formats
    BGRA4Unorm,
    B5G6R5Unorm,
    BGR5A1Unorm,
    // 32-bit formats
    R32Uint,
    R32Sint,
    R32Float,
    RG16Unorm,
    RG16Snorm,
    RG16Uint,
    RG16Sint,
    RG16Float,
    RGBA8Unorm,
    RGBA8UnormSrgb,
    RGBA8Snorm,
    RGBA8Uint,
    RGBA8Sint,
    BGRA8Unorm,
    BGRA8UnormSrgb,
    // Packed 32-Bit Pixel Formats
    RGB10A2Unorm,
    RGB10A2Uint,
    RG11B10UFloat,
    RGB9E5UFloat,
    // 64-bit formats
    RG32Uint,
    RG32Sint,
    RG32Float,
    RGBA16Unorm,
    RGBA16Snorm,
    RGBA16Uint,
    RGBA16Sint,
    RGBA16Float,
    // 128-bit formats
    RGBA32Uint,
    RGBA32Sint,
    RGBA32Float,
    // Depth-stencil formats
    Depth16Unorm,
    Depth24UnormStencil8,
    Depth32Float,
    Depth32FloatStencil8,
    // BC compressed formats
    BC1RGBAUnorm,
    BC1RGBAUnormSrgb,
    BC2RGBAUnorm,
    BC2RGBAUnormSrgb,
    BC3RGBAUnorm,
    BC3RGBAUnormSrgb,
    BC4RUnorm,
    BC4RSnorm,
    BC5RGUnorm,
    BC5RGSnorm,
    BC6HRGBUfloat,
    BC6HRGBFloat,
    BC7RGBAUnorm,
    BC7RGBAUnormSrgb,
    // ETC2/EAC compressed formats
    ETC2RGB8Unorm,
    ETC2RGB8UnormSrgb,
    ETC2RGB8A1Unorm,
    ETC2RGB8A1UnormSrgb,
    ETC2RGBA8Unorm,
    ETC2RGBA8UnormSrgb,
    EACR11Unorm,
    EACR11Snorm,
    EACRG11Unorm,
    EACRG11Snorm,
    // ASTC compressed formats
    ASTC4x4Unorm,
    ASTC4x4UnormSrgb,
    ASTC5x4Unorm,
    ASTC5x4UnormSrgb,
    ASTC5x5Unorm,
    ASTC5x5UnormSrgb,
    ASTC6x5Unorm,
    ASTC6x5UnormSrgb,
    ASTC6x6Unorm,
    ASTC6x6UnormSrgb,
    ASTC8x5Unorm,
    ASTC8x5UnormSrgb,
    ASTC8x6Unorm,
    ASTC8x6UnormSrgb,
    ASTC8x8Unorm,
    ASTC8x8UnormSrgb,
    ASTC10x5Unorm,
    ASTC10x5UnormSrgb,
    ASTC10x6Unorm,
    ASTC10x6UnormSrgb,
    ASTC10x8Unorm,
    ASTC10x8UnormSrgb,
    ASTC10x10Unorm,
    ASTC10x10UnormSrgb,
    ASTC12x10Unorm,
    ASTC12x10UnormSrgb,
    ASTC12x12Unorm,
    ASTC12x12UnormSrgb,

    // MultiAspect format
    //R8BG8Biplanar420Unorm,
    //R10X6BG10X6Biplanar420Unorm,

    Count
}
