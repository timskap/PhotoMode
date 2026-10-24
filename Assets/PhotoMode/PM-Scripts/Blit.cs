using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace PhotoMode
{
    public class Blit : ScriptableRendererFeature
    {
        public Material blitMaterial;
        private BlitRenderPass blitRenderPass;

        public override void Create()
        {
            blitRenderPass?.Dispose();
            blitRenderPass = new BlitRenderPass(RenderPassEvent.AfterRenderingPostProcessing, blitMaterial, name);
        }

        protected override void Dispose(bool disposing)
        {
            blitRenderPass?.Dispose();
            blitRenderPass = null;
        }
        public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
        {
            // A missing optional material is a no-op, including while editing the renderer.
            if (blitRenderPass == null || blitMaterial == null || renderingData.cameraData.cameraType != CameraType.Game)
                return;

            blitRenderPass.blitMaterial = blitMaterial;
            renderer.EnqueuePass(blitRenderPass);
        }
    }
}
