using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.RenderGraphModule;
using UnityEngine.Rendering.Universal;

namespace PhotoMode
{
    public class BlitRenderPass : ScriptableRenderPass, System.IDisposable
    {
        public Material blitMaterial;
        private readonly string profilerTag;
        private readonly Mesh fullscreenMesh;
        private readonly MaterialPropertyBlock properties = new MaterialPropertyBlock();
        private static readonly int MainTexture = Shader.PropertyToID("_MainTex");

        private class PassData
        {
            public TextureHandle source;
            public Material material;
            public MaterialPropertyBlock properties;
            public Mesh mesh;
            public Matrix4x4 view;
            public Matrix4x4 projection;
        }

        public BlitRenderPass(RenderPassEvent passEvent, Material material, string tag)
        {
            renderPassEvent = passEvent;
            blitMaterial = material;
            profilerTag = tag;
            requiresIntermediateTexture = true;
            // Existing Unlit Shader Graph filters require a quad, not Blitter's procedural vertex contract.
            fullscreenMesh = new Mesh { name = "PhotoMode Fullscreen Quad", hideFlags = HideFlags.HideAndDontSave };
            fullscreenMesh.vertices = new[]
            {
                new Vector3(-1, -1, 0), new Vector3(-1, 1, 0),
                new Vector3(1, -1, 0), new Vector3(1, 1, 0)
            };
            fullscreenMesh.uv = new[] { new Vector2(0, 0), new Vector2(0, 1), new Vector2(1, 0), new Vector2(1, 1) };
            fullscreenMesh.SetIndices(new[] { 0, 1, 2, 2, 1, 3 }, MeshTopology.Triangles, 0, false);
            fullscreenMesh.UploadMeshData(true);
        }

        public void Dispose()
        {
            CoreUtils.Destroy(fullscreenMesh);
        }
        public override void RecordRenderGraph(RenderGraph renderGraph, ContextContainer frameData)
        {
            var resources = frameData.Get<UniversalResourceData>();
            if (blitMaterial == null || resources.isActiveTargetBackBuffer)
                return;

            var source = resources.activeColorTexture;
            var descriptor = renderGraph.GetTextureDesc(source);
            descriptor.name = "PhotoMode Filter Color";
            descriptor.clearBuffer = false;
            var destination = renderGraph.CreateTexture(descriptor);
            var camera = frameData.Get<UniversalCameraData>();

            using (var builder = renderGraph.AddRasterRenderPass<PassData>(profilerTag, out var data))
            {
                data.source = source;
                data.material = blitMaterial;
                data.properties = properties;
                data.mesh = fullscreenMesh;
                data.view = camera.GetViewMatrix();
                data.projection = camera.GetProjectionMatrix();
                builder.UseTexture(source, AccessFlags.Read);
                builder.SetRenderAttachment(destination, 0, AccessFlags.Write);
                builder.AllowGlobalStateModification(true);
                builder.SetRenderFunc(static (PassData pass, RasterGraphContext context) =>
                {
                    // Legacy Unlit Shader Graph filters expect object-space mesh vertices and _MainTex.
                    // Draw in clip space, then restore camera matrices for subsequent URP passes.
                    pass.properties.Clear();
                    pass.properties.SetTexture(MainTexture, pass.source);
                    context.cmd.SetViewProjectionMatrices(Matrix4x4.identity, Matrix4x4.identity);
                    context.cmd.DrawMesh(pass.mesh, Matrix4x4.identity, pass.material, 0, 0, pass.properties);
                    context.cmd.SetViewProjectionMatrices(pass.view, pass.projection);
                });
            }
            resources.cameraColor = destination;
        }
    }
}
