// Copyright © Amer Koleci and Contributors.
// Licensed under the MIT License (MIT). See LICENSE in the repository root for more information.

using TerraFX.Interop.DirectX;
using static TerraFX.Interop.DirectX.D3D12;
using static TerraFX.Interop.DirectX.D3D12_DESCRIPTOR_RANGE_TYPE;
using static TerraFX.Interop.DirectX.D3D12_DESCRIPTOR_RANGE_FLAGS;
using System.Diagnostics;

namespace Alimer.Graphics.D3D12;

internal unsafe class D3D12BindGroupLayout : BindGroupLayout
{
    private readonly D3D12GraphicsDevice _device;
    public readonly List<D3D12_DESCRIPTOR_RANGE1> _cbvUavSrvDescriptorRanges = new();
    public readonly List<D3D12_DESCRIPTOR_RANGE1> _samplerDescriptorRanges = new();

    public D3D12BindGroupLayout(D3D12GraphicsDevice device, in BindGroupLayoutDescription description)
        : base(description)
    {
        _device = device;

        int bindingCount = description.Entries.Length;

        DescriptorType currentType = (DescriptorType)byte.MaxValue;
        uint currentBinding = ~0u;

        for (int i = 0; i < bindingCount; i++)
        {
            ref readonly BindGroupLayoutEntry entry = ref description.Entries[i];

            D3D12_DESCRIPTOR_RANGE_TYPE descriptorRangeType = entry.Type.ToD3D12();
            if (!AreResourceTypesCompatible(entry.Type, currentType) || currentBinding == ~0u || entry.Binding != currentBinding + 1)
            {
                D3D12_DESCRIPTOR_RANGE1 range = new()
                {
                    RangeType = descriptorRangeType,
                    NumDescriptors = 1,
                    BaseShaderRegister = entry.Binding,
                    RegisterSpace = D3D12_DRIVER_RESERVED_REGISTER_SPACE_VALUES_START,
                    OffsetInDescriptorsFromTableStart = D3D12_DESCRIPTOR_RANGE_OFFSET_APPEND
                };

                switch (descriptorRangeType)
                {
                    case D3D12_DESCRIPTOR_RANGE_TYPE_SRV:
                        range.Flags = D3D12_DESCRIPTOR_RANGE_FLAG_DESCRIPTORS_VOLATILE | D3D12_DESCRIPTOR_RANGE_FLAG_DATA_VOLATILE;
                        break;

                    case D3D12_DESCRIPTOR_RANGE_TYPE_CBV:
                        range.Flags = D3D12_DESCRIPTOR_RANGE_FLAG_DATA_STATIC_WHILE_SET_AT_EXECUTE;
                        break;


                    case D3D12_DESCRIPTOR_RANGE_TYPE_SAMPLER:
                        range.Flags = D3D12_DESCRIPTOR_RANGE_FLAG_NONE;
                        break;
                }

                List<D3D12_DESCRIPTOR_RANGE1> descriptorRanges =
                    descriptorRangeType == D3D12_DESCRIPTOR_RANGE_TYPE_SAMPLER ? _samplerDescriptorRanges : _cbvUavSrvDescriptorRanges;
                // Try to join this range with the previous one, if the current range is a continuation
                // of the previous. This is possible because the binding infos in the base type are
                // sorted.
                if (descriptorRanges.Count >= 2)
                {
                    D3D12_DESCRIPTOR_RANGE1 previous = descriptorRanges[descriptorRanges.Count - 1];
                    if (previous.RangeType == range.RangeType &&
                        previous.BaseShaderRegister + previous.NumDescriptors == range.BaseShaderRegister)
                    {
                        previous.NumDescriptors += range.NumDescriptors;
                        descriptorRanges[descriptorRanges.Count - 1] = previous;
                        continue;
                    }
                }

                descriptorRanges.Add(range);
                currentType = entry.Type;
                currentBinding = entry.Binding;
            }
            else
            {
                // Extend the current range
                Debug.Assert(_cbvUavSrvDescriptorRanges.Count > 0);
                D3D12_DESCRIPTOR_RANGE1 range = _cbvUavSrvDescriptorRanges[_cbvUavSrvDescriptorRanges.Count - 1];

                range.NumDescriptors += 1;
                //descriptorTableSizeSRVetc += 1;
                //bindingLayoutsSRVetc.push_back(binding);
                currentBinding = entry.Binding;
            }
        }
    }

    /// <summary>
    /// Finalizes an instance of the <see cref="D3D12BindGroupLayout" /> class.
    /// </summary>
    ~D3D12BindGroupLayout() => Dispose(disposing: false);

    /// <inheritdoc />
    public override GraphicsDevice Device => _device;

    /// <inheitdoc />
    protected internal override void Destroy()
    {
    }

    private static bool AreResourceTypesCompatible(DescriptorType a, DescriptorType b)
    {
        if (a == b)
            return true;

        //a = GetNormalizedResourceType(a);
        //b = GetNormalizedResourceType(b);
        //
        //if (a == ResourceType::TypedBuffer_SRV && b == ResourceType::Texture_SRV ||
        //    b == ResourceType::TypedBuffer_SRV && a == ResourceType::Texture_SRV ||
        //    a == ResourceType::TypedBuffer_SRV && b == ResourceType::RayTracingAccelStruct ||
        //    a == ResourceType::Texture_SRV && b == ResourceType::RayTracingAccelStruct ||
        //    b == ResourceType::TypedBuffer_SRV && a == ResourceType::RayTracingAccelStruct ||
        //    b == ResourceType::Texture_SRV && a == ResourceType::RayTracingAccelStruct)
        //    return true;
        //
        //if (a == ResourceType::TypedBuffer_UAV && b == ResourceType::Texture_UAV ||
        //    b == ResourceType::TypedBuffer_UAV && a == ResourceType::Texture_UAV)
        //    return true;

        return false;
    }

}