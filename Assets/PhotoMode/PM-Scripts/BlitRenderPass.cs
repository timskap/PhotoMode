using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using PhotoMode;
using UnityEngine.Rendering.RenderGraphModule;

namespace PhotoMode
{
    public class BlitRenderPass : ScriptableRenderPass
    {
        public Material blitMaterial = null;
        public RTHandle source;

        RTHandle temporaryColorTexture;
        RTHandle destinationTexture;

        string profilerTag;

        // Default constructor for the Blit Render Pass
        public BlitRenderPass(RenderPassEvent renderPassEvent, Material blitMat, string tag)
        {
            this.renderPassEvent = renderPassEvent;
            blitMaterial = blitMat;
            profilerTag = tag;

            // Use RTHandles to initialize render textures
            temporaryColorTexture = RTHandles.Alloc("_TemporaryColorTexture");
            destinationTexture = RTHandles.Alloc("_AfterPostProcessTexture");
        }

        // Override the Execute function declared in the scriptable render pass class.
        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            // Create a command buffer, a list of graphical instructions to execute
            CommandBuffer cmd = CommandBufferPool.Get(profilerTag);

            RenderTextureDescriptor opaqueDesc = renderingData.cameraData.cameraTargetDescriptor;
            opaqueDesc.depthBufferBits = 0;

            // Configure temporary RTs with RTHandle
            RenderingUtils.ReAllocateIfNeeded(ref destinationTexture, opaqueDesc);
            RenderingUtils.ReAllocateIfNeeded(ref temporaryColorTexture, opaqueDesc);

            // Copy what the camera is rendering to the render texture and apply the blit material
            Blit(cmd, source, temporaryColorTexture, blitMaterial);

            // Copy what the temporary render texture is rendering back to the camera
            Blit(cmd, temporaryColorTexture, source);

            // Execute the graphic commands
            context.ExecuteCommandBuffer(cmd);

            // Release the command buffer
            CommandBufferPool.Release(cmd);
        }

        public override void RecordRenderGraph(RenderGraph renderGraph, ContextContainer frameData)
        {
            base.RecordRenderGraph(renderGraph, frameData);
        }

        public override void FrameCleanup(CommandBuffer cmd)
        {
            // RTHandles do not need explicit release like temporary RTs, but cleanup logic can go here if needed
        }

        public override void OnCameraCleanup(CommandBuffer cmd)
        {
            if (temporaryColorTexture != null)
            {
                RTHandles.Release(temporaryColorTexture);
                RTHandles.Release(destinationTexture);
            }
        }
    }
}
