// Copyright © Amer Koleci and Contributors.
// Licensed under the MIT License (MIT). See LICENSE in the repository root for more information.

using Vortice.Vulkan;
using static Vortice.Vulkan.Vulkan;
using static Vortice.Vulkan.Vma;

namespace Alimer.Graphics.Vulkan;

internal unsafe class VulkanTexture : Texture
{
    private readonly VkImage _handle = VkImage.Null;
    private readonly VmaAllocation _allocation = VmaAllocation.Null;

    public VulkanTexture(VulkanGraphicsDevice device, in TextureDescriptor descriptor, void* initialData)
        : base(device, descriptor)
    {
        VkFormat = device.ToVkFormat(descriptor.Format);
        bool isDepthStencil = descriptor.Format.IsDepthStencilFormat();
        VkImageCreateFlags flags = VkImageCreateFlags.None;
        VkImageType imageType = descriptor.Dimension.ToVk();
        VkImageUsageFlags usage = VkImageUsageFlags.None;
        VkImageTiling tiling = VkImageTiling.Optimal;
        uint depth = 1u;
        uint arrayLayers = 1u;

        switch (descriptor.Dimension)
        {
            case TextureDimension.Texture1D:
                arrayLayers = (uint)descriptor.DepthOrArrayLayers;
                break;

            case TextureDimension.Texture2D:
                arrayLayers = (uint)descriptor.DepthOrArrayLayers;

                if (descriptor.Width == descriptor.Height &&
                    descriptor.DepthOrArrayLayers >= 6)
                {
                    flags |= VkImageCreateFlags.CubeCompatible;
                }
                break;
            case TextureDimension.Texture3D:
                flags |= VkImageCreateFlags.Array2DCompatible;
                depth = (uint)descriptor.DepthOrArrayLayers;
                break;
        }

        if ((descriptor.Usage & TextureUsage.ShaderRead) != 0)
        {
            usage |= VkImageUsageFlags.Sampled;
        }
        if ((descriptor.Usage & TextureUsage.ShaderWrite) != 0)
        {
            usage |= VkImageUsageFlags.Storage;

            //if (IsFormatSRGB(texture->desc.format))
            //{
            //    imageInfo.flags |= VK_IMAGE_CREATE_EXTENDED_USAGE_BIT;
            //}
        }

        if ((descriptor.Usage & TextureUsage.RenderTarget) != 0)
        {
            if (isDepthStencil)
            {
                usage |= VkImageUsageFlags.DepthStencilAttachment;
            }
            else
            {
                usage |= VkImageUsageFlags.ColorAttachment;
            }
        }

        if ((descriptor.Usage & TextureUsage.Transient) != 0)
        {
            usage |= VkImageUsageFlags.TransientAttachment;

        }
        else
        {
            usage |= VkImageUsageFlags.TransferSrc | VkImageUsageFlags.TransferDst;
        }

        VkExternalMemoryImageCreateInfo externalInfo = new();
        bool isShared = false;
        if ((descriptor.Usage & TextureUsage.Shared) != 0)
        {
            isShared = true;

            // Ensure that the handle type is supported.
            
            VkExternalImageFormatProperties external_props = new();

            VkPhysicalDeviceExternalImageFormatInfo externalFormatInfo = new();
            externalFormatInfo.handleType = VkExternalMemoryHandleTypeFlags.D3D11Texture; // create_info.external.memory_handle_type;

            VkPhysicalDeviceImageFormatInfo2 info = new()
            {
                pNext = &externalFormatInfo,
                format = VkFormat,
                type = imageType,
                tiling = tiling,
                usage = usage,
                flags = flags
            };

            VkImageFormatProperties2 props2 = new()
            {
                pNext = &external_props
            };
            VkResult res = vkGetPhysicalDeviceImageFormatProperties2(device.PhysicalDevice, &info, &props2);

            if (res != VkResult.Success)
            {
                Log.Error($"Vulkan: Image format is not supported for external memory type {externalFormatInfo.handleType}.");
                return;
            }

            bool supportsImport = (external_props.externalMemoryProperties.externalMemoryFeatures & VkExternalMemoryFeatureFlags.Importable) != 0;
            bool supportsExport = (external_props.externalMemoryProperties.externalMemoryFeatures & VkExternalMemoryFeatureFlags.Exportable) != 0;

            //if (!supportsImport && create_info.external)
            //{
            //    LOGE("Attempting to import with handle type #%x, but it is not supported.\n",
            //         create_info.external.memory_handle_type);
            //    return;
            //}
            //else if (!supports_export && !create_info.external)
            //{
            //    LOGE("Attempting to export with handle type #%x, but it is not supported.\n",
            //         create_info.external.memory_handle_type);
            //    return;
            //}

            externalInfo.handleTypes = externalFormatInfo.handleType;
            externalInfo.pNext = info.pNext;
            info.pNext = &externalInfo;
        }

        VmaAllocationInfo allocationInfo = default;
        VmaAllocationCreateInfo memoryInfo = new()
        {
            usage = VmaMemoryUsage.Auto
        };

        VkImageCreateInfo createInfo = new()
        {
            flags = flags,
            imageType = imageType,
            format = VkFormat,
            extent = new(descriptor.Width, descriptor.Height, depth),
            mipLevels = MipLevelCount,
            arrayLayers = arrayLayers,
            samples = SampleCount.ToVkSampleCount(),
            tiling = VkImageTiling.Linear,
            usage = usage
        };


        uint* sharingIndices = stackalloc uint[3];
        device.FillImageSharingIndices(ref createInfo, sharingIndices);

        VkResult result = vmaCreateImage(device.MemoryAllocator,
            &createInfo,
            &memoryInfo,
            out _handle,
            out _allocation,
            &allocationInfo);

        if (result != VkResult.Success)
        {
            Log.Error("Vulkan: Failed to create image.");
            return;
        }

        if (!string.IsNullOrEmpty(descriptor.Label))
        {
            OnLabelChanged(descriptor.Label!);
        }
    }

    public VulkanTexture(GraphicsDevice device, VkImage existingTexture, in TextureDescriptor descriptor)
        : base(device, descriptor)
    {
        _handle = existingTexture;
        VkFormat = ((VulkanGraphicsDevice)device).ToVkFormat(descriptor.Format);

        if (!string.IsNullOrEmpty(descriptor.Label))
        {
            OnLabelChanged(descriptor.Label!);
        }
    }

    public VkImage Handle => _handle;
    public VkFormat VkFormat { get; }

    /// <summary>
    /// Finalizes an instance of the <see cref="VulkanTexture" /> class.
    /// </summary>
    ~VulkanTexture() => Dispose(disposing: false);

    /// <inheitdoc />
    protected internal override void Destroy()
    {
        VmaAllocator memoryAllocator = ((VulkanGraphicsDevice)Device).MemoryAllocator;

        if (!_allocation.IsNull)
        {
            vmaDestroyImage(memoryAllocator, _handle, _allocation);
        }
    }

    /// <inheritdoc />
    protected override void OnLabelChanged(string newLabel)
    {
        ((VulkanGraphicsDevice)Device).SetObjectName(VkObjectType.Image, _handle.Handle, newLabel);
    }
}