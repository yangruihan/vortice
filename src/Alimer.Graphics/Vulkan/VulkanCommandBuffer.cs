// Copyright © Amer Koleci and Contributors.
// Licensed under the MIT License (MIT). See LICENSE in the repository root for more information.

using Vortice.Vulkan;
using static Vortice.Vulkan.Vulkan;
using static Alimer.Graphics.Constants;

namespace Alimer.Graphics.Vulkan;

internal unsafe class VulkanCommandBuffer : CommandBuffer
{
    public readonly VulkanCommandQueue Queue;
    private readonly VkCommandPool[] _commandPools = new VkCommandPool[MaxFramesInFlight];
    private readonly VkCommandBuffer[] _commandBuffers = new VkCommandBuffer[MaxFramesInFlight];
    private VkCommandBuffer _commandBuffer; // recording command buffer
    private bool _hasLabel;

    public VulkanCommandBuffer(VulkanCommandQueue queue)
        : base(queue.Device)
    {
        Queue = queue;

        for (uint i = 0; i < MaxFramesInFlight; ++i)
        {
            VkCommandPoolCreateInfo poolInfo = new()
            {
                sType = VkStructureType.CommandPoolCreateInfo,
                flags = VkCommandPoolCreateFlags.Transient
            };

            switch (queue.QueueType)
            {
                case CommandQueue.Graphics:
                    poolInfo.queueFamilyIndex = queue.Device.GraphicsFamily;
                    break;

                case CommandQueue.Compute:
                    poolInfo.queueFamilyIndex = queue.Device.ComputeFamily;
                    break;

                case CommandQueue.Copy:
                    poolInfo.queueFamilyIndex = queue.Device.CopyFamily;
                    break;
                //case CommandQueue.VideoDecode:
                //    poolInfo.queueFamilyIndex = videoFamily;
                //break;

                default:
                    throw new GraphicsException($"Invalid queue: {queue.QueueType}");
            }
            vkCreateCommandPool(queue.Device.Handle, &poolInfo, null, out _commandPools[i]).DebugCheckResult();

            VkCommandBufferAllocateInfo commandBufferInfo = new();
            commandBufferInfo.sType = VkStructureType.CommandBufferAllocateInfo;
            commandBufferInfo.commandPool = _commandPools[i];
            commandBufferInfo.level = VkCommandBufferLevel.Primary;
            commandBufferInfo.commandBufferCount = 1;
            vkAllocateCommandBuffer(queue.Device.Handle, &commandBufferInfo, out _commandBuffers[i]).DebugCheckResult();

            //binderPools[i].Init(device);
        }
    }

    public void Destroy()
    {
        for (int i = 0; i < _commandPools.Length; ++i)
        {
            //vkFreeCommandBuffers(Queue.Device.Handle, _commandPools[i], 1, &commandBuffers[i]);
            vkDestroyCommandPool(Queue.Device.Handle, _commandPools[i]);
            //binderPools[i].Shutdown();
        }
    }

    public void Begin(uint frameIndex, string? label = null)
    {
        base.Reset(frameIndex);
        //waits.clear();
        //hasPendingWaits.store(false);
        //currentPipeline.Reset();
        //currentPipelineLayout.Reset();
        //binderPools[frameIndex].Reset();
        //binder.reset();
        //presentSwapChains.clear();

        vkResetCommandPool(Queue.Device.Handle, _commandPools[frameIndex], 0).DebugCheckResult();
        _commandBuffer = _commandBuffers[frameIndex];

        VkCommandBufferBeginInfo beginInfo = new()
        {
            sType = VkStructureType.CommandBufferBeginInfo,
            flags = VkCommandBufferUsageFlags.OneTimeSubmit,
            pInheritanceInfo = null // Optional
        };
        vkBeginCommandBuffer(_commandBuffer, &beginInfo).DebugCheckResult();

        if (Queue.QueueType == CommandQueue.Graphics)
        {
            VkRect2D* scissors = stackalloc VkRect2D[16];
            for (uint i = 0; i < 16; ++i)
            {
                scissors[i].offset.x = 0;
                scissors[i].offset.y = 0;
                scissors[i].extent.width = 65535;
                scissors[i].extent.height = 65535;
            }
            vkCmdSetScissor(_commandBuffer, 0, 16, scissors);

            vkCmdSetBlendConstants(_commandBuffer, 1.0f, 1.0f, 1.0f, 1.0f);
            vkCmdSetStencilReference(_commandBuffer, VkStencilFaceFlags.FrontAndBack, ~0u);

            if (Queue.Device.PhysicalDeviceFeatures2.features.depthBounds == true)
            {
                vkCmdSetDepthBounds(_commandBuffer, 0.0f, 1.0f);
            }

            // Silence validation about uninitialized stride:
            //const VkDeviceSize zero = {};
            //vkCmdBindVertexBuffers2(commandBuffer, 0, 1, &nullBuffer, &zero, &zero, &zero);
        }

        if (!string.IsNullOrEmpty(label))
        {
            _hasLabel = true;
            PushDebugGroup(label);
        }
        else
        {
            _hasLabel = false;
        }
    }

    public override void PushDebugGroup(string groupLabel)
    {
        if (!Queue.Device.DebugUtils)
            return;

        fixed (sbyte* pLabelName = groupLabel.GetUtf8Span())
        {
            VkDebugUtilsLabelEXT label = new()
            {
                sType = VkStructureType.DebugUtilsLabelEXT,
                pLabelName = pLabelName
            };
            label.color[0] = 0.0f;
            label.color[1] = 0.0f;
            label.color[2] = 0.0f;
            label.color[3] = 1.0f;
            vkCmdBeginDebugUtilsLabelEXT(_commandBuffer, &label);
        }
    }

    public override void PopDebugGroup()
    {
        if (!Queue.Device.DebugUtils)
            return;

        vkCmdEndDebugUtilsLabelEXT(_commandBuffer);
    }

    public override void InsertDebugMarker(string debugLabel)
    {
        if (!Queue.Device.DebugUtils)
            return;

        fixed (sbyte* pLabelName = debugLabel.GetUtf8Span())
        {
            VkDebugUtilsLabelEXT label = new()
            {
                sType = VkStructureType.DebugUtilsLabelEXT,
                pLabelName = pLabelName
            };
            label.color[0] = 0.0f;
            label.color[1] = 0.0f;
            label.color[2] = 0.0f;
            label.color[3] = 1.0f;
            vkCmdInsertDebugUtilsLabelEXT(_commandBuffer, &label);
        }
    }

    public override void Commit()
    {

    }
}
