// Copyright © Amer Koleci and Contributors.
// Licensed under the MIT License (MIT). See LICENSE in the repository root for more information.

using System.Runtime.CompilerServices;
using TerraFX.Interop.DirectX;
using TerraFX.Interop.Windows;
using static TerraFX.Interop.Windows.Windows;
using static TerraFX.Interop.DirectX.D3D12_FENCE_FLAGS;

namespace Alimer.Graphics.D3D12;

internal static unsafe partial class ID3D12Extensions
{
    public static void SetName<TID3D12Object>(ref this TID3D12Object self, ReadOnlySpan<char> name)
        where TID3D12Object : unmanaged, ID3D12Object.Interface
    {
        fixed (char* namePtr = name)
        {
            _ = self.SetName((ushort*)namePtr);
        }
    }

    public static void SetName<TID3D12Object>(ref this TID3D12Object self, string name)
        where TID3D12Object : unmanaged, ID3D12Object.Interface
    {
        fixed (char* namePtr = name)
        {
            _ = self.SetName((ushort*)namePtr);
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static HRESULT CheckFeatureSupport<TD3D12Device, TFeature>(ref this TD3D12Device self, D3D12_FEATURE feature, ref TFeature featureData)
       where TD3D12Device : unmanaged, ID3D12Device.Interface
       where TFeature : unmanaged
    {
        fixed (TFeature* featureDataPtr = &featureData)
        {
            return self.CheckFeatureSupport(feature, featureDataPtr, (uint)sizeof(TFeature));
        }
    }

    /// <summary>
    /// Creates a new <see cref="ID3D12Fence"/> for a given device.
    /// </summary>
    /// <param name="d3D12Device">The target <see cref="ID3D12Device"/> to use to create the fence.</param>
    /// <returns>A pointer to the newly allocated <see cref="ID3D12Fence"/> instance.</returns>
    /// <exception cref="Exception">Thrown when the creation of the command queue fails.</exception>
    public static ComPtr<ID3D12Fence> CreateFence<TD3D12Device>(this ref TD3D12Device d3D12Device, bool shared = false)
        where TD3D12Device : unmanaged, ID3D12Device.Interface
    {
        using ComPtr<ID3D12Fence> d3D12Fence = default;

        ThrowIfFailed(d3D12Device.CreateFence(
            0,
            shared ? D3D12_FENCE_FLAG_SHARED : D3D12_FENCE_FLAG_NONE,
            __uuidof<ID3D12Fence>(),
            d3D12Fence.GetVoidAddressOf())
            );

        return d3D12Fence.Move();
    }
}
