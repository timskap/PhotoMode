using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.Rendering;

namespace PhotoMode
{
    public class Blit : ScriptableRendererFeature
    {
        public Material blitMaterial = null;
        private BlitRenderPass blitRenderPass;

        // We will use RTHandle for the source render target
        private RTHandle sourceHandle;

        public override void Create()
        {
            blitRenderPass = new BlitRenderPass(RenderPassEvent.AfterRendering, blitMaterial, name);

            // Create the source handle (typically "_AfterPostProcessTexture" is a special render target)
            sourceHandle = RTHandles.Alloc("_AfterPostProcessTexture", name: "_AfterPostProcessTexture");
            blitRenderPass.source = sourceHandle;
        }

        public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
        {
            // Check to make sure the blit has a material and exit gracefully if it doesn't
            if (blitMaterial == null)
            {
                Debug.LogError("Blit is missing its Material. Make sure you have assigned a material in the renderer.");
                return;
            }

            // Add the blit render pass to the queue of render passes to execute
            renderer.EnqueuePass(blitRenderPass);
        }

        protected override void Dispose(bool disposing)
        {
            // Release the RTHandle when no longer needed
            RTHandles.Release(sourceHandle);
        }
    }
}