using System.Collections.Generic;
#if UNITY_6000_0_OR_NEWER
using Unity.Collections;
#endif
using UnityEngine;

using UnityEngine.Rendering;
#if UNITY_6000_0_OR_NEWER
using UnityEngine.Rendering.RenderGraphModule;
#endif
using UnityEngine.Rendering.Universal;

namespace SyminStudio.Rendering.Universal
{
    /// <summary>
    /// 自定义RenderPass
    /// </summary>
    public class SyminRenderPass : ScriptableRenderPass
    {
        private RenderStateBlock _renderStateBlock;

        private readonly RenderQueueType _renderQueueType;
        private FilteringSettings _filteringSettings;
#if UNITY_6000_0_OR_NEWER
        private PassData _passData;
        private static readonly ShaderTagId[] ShaderTagValues = { ShaderTagId.none };
        private static readonly RenderStateBlock[] RenderStateBlocks = new RenderStateBlock[1];
#endif

        public Material OverrideMaterial { get; set; }
        public int OverrideMaterialPassIndex { get; set; }

        private List<ShaderTagId> _shaderTagIdList = new()
        {
            new ShaderTagId("SRPDefaultUnlit"),
            new ShaderTagId("UniversalForward"),
            new ShaderTagId("UniversalForwardOnly"),
            new ShaderTagId("LightweightForward")
        };

        //Pass的构造方法，参数都由Feature传入
        public SyminRenderPass(string profilerTag, RenderPassEvent renderPassEvent, FilterSettings filterSettings)
        {
            profilingSampler = new ProfilingSampler(nameof(SyminRenderPass) + "-" + profilerTag);

            this.renderPassEvent = renderPassEvent;
            _renderQueueType = filterSettings.renderQueueType;

            var renderQueueRange = filterSettings.renderQueueType == RenderQueueType.Transparent
                ? RenderQueueRange.transparent
                : RenderQueueRange.opaque;
            var renderingLayerMask = (uint)filterSettings.renderingLayerMask;

            _filteringSettings = new FilteringSettings(renderQueueRange, filterSettings.layerMask, renderingLayerMask);

            _renderStateBlock = new RenderStateBlock(RenderStateMask.Nothing);
#if UNITY_6000_0_OR_NEWER
            _passData = new PassData();
#endif
        }

        /// <summary>
        /// 设置深度测试
        /// </summary>
        public void SetDepthState(bool writeEnabled, CompareFunction function = CompareFunction.Less)
        {
            _renderStateBlock.mask |= RenderStateMask.Depth;
            _renderStateBlock.depthState = new DepthState(writeEnabled, function);
        }
        //TODO:设置 stencil state

        /// <summary>
        /// 定义渲染队列的CommandBuffer并执行
        /// </summary>
        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
#if UNITY_6000_0_OR_NEWER
            var drawingSettings = CreatePassDrawingSettings(renderingData);

            var rendererListParams = CreateRendererListParams(renderingData.cullResults, drawingSettings);
            _passData.rendererList = context.CreateRendererList(ref rendererListParams);

            var commandBuffer = CommandBufferPool.Get();
            using (new ProfilingScope(commandBuffer, profilingSampler))
            {
                var rasterCommandBuffer = CommandBufferHelpers.GetRasterCommandBuffer(commandBuffer);
                rasterCommandBuffer.DrawRendererList(_passData.rendererList);
            }

            context.ExecuteCommandBuffer(commandBuffer);
            CommandBufferPool.Release(commandBuffer);
#else
            var drawingSettings = CreatePassDrawingSettings(renderingData);
            context.DrawRenderers(renderingData.cullResults, ref drawingSettings, ref _filteringSettings);
#endif
        }

#if UNITY_6000_0_OR_NEWER
        private class PassData
        {
            public TextureHandle color;
            public RendererList rendererList;
            public RendererListHandle rendererListHandle;
        }

        private DrawingSettings CreatePassDrawingSettings(UniversalRenderingData renderingData, UniversalCameraData cameraData,
            UniversalLightData lightData)
        {
            var sortingCriteria = _renderQueueType == RenderQueueType.Transparent
                ? SortingCriteria.CommonTransparent
                : cameraData.defaultOpaqueSortFlags;

            var drawingSettings = RenderingUtils.CreateDrawingSettings(_shaderTagIdList, renderingData, cameraData, lightData,
                sortingCriteria);
            drawingSettings.overrideMaterial = OverrideMaterial;
            drawingSettings.overrideMaterialPassIndex = OverrideMaterialPassIndex;
            return drawingSettings;
        }

        private DrawingSettings CreatePassDrawingSettings(RenderingData renderingData)
        {
            var sortingCriteria = _renderQueueType == RenderQueueType.Transparent
                ? SortingCriteria.CommonTransparent
                : renderingData.cameraData.defaultOpaqueSortFlags;

            var drawingSettings = CreateDrawingSettings(_shaderTagIdList, ref renderingData, sortingCriteria);
            drawingSettings.overrideMaterial = OverrideMaterial;
            drawingSettings.overrideMaterialPassIndex = OverrideMaterialPassIndex;
            return drawingSettings;
        }

        public override void RecordRenderGraph(RenderGraph renderGraph, ContextContainer frameData)
        {
            var cameraData = frameData.Get<UniversalCameraData>();
            var renderingData = frameData.Get<UniversalRenderingData>();
            var lightData = frameData.Get<UniversalLightData>();
            var resourceData = frameData.Get<UniversalResourceData>();

            using (var builder = renderGraph.AddRasterRenderPass<PassData>(passName, out var passData, profilingSampler))
            {
                passData.color = resourceData.activeColorTexture;

                var drawingSettings = CreatePassDrawingSettings(renderingData, cameraData, lightData);
                var rendererListParams = CreateRendererListParams(renderingData.cullResults, drawingSettings);
                passData.rendererListHandle = renderGraph.CreateRendererList(rendererListParams);

                if (!passData.rendererListHandle.IsValid())
                    return;

                builder.UseRendererList(passData.rendererListHandle);
                builder.SetRenderAttachment(resourceData.activeColorTexture, 0, AccessFlags.Write);
                builder.SetRenderAttachmentDepth(resourceData.activeDepthTexture, AccessFlags.Write);
                builder.AllowGlobalStateModification(true);

                builder.SetRenderFunc((PassData data, RasterGraphContext context) =>
                {
                    context.cmd.DrawRendererList(data.rendererListHandle);
                });
            }
        }
#else
        private DrawingSettings CreatePassDrawingSettings(RenderingData renderingData)
        {
            var sortingCriteria = _renderQueueType == RenderQueueType.Transparent
                ? SortingCriteria.CommonTransparent
                : renderingData.cameraData.defaultOpaqueSortFlags;

            var drawingSettings = CreateDrawingSettings(_shaderTagIdList, ref renderingData, sortingCriteria);
            drawingSettings.overrideMaterial = OverrideMaterial;
            drawingSettings.overrideMaterialPassIndex = OverrideMaterialPassIndex;
            return drawingSettings;
        }
#endif

#if UNITY_6000_0_OR_NEWER
        private RendererListParams CreateRendererListParams(CullingResults cullingResults, DrawingSettings drawingSettings)
        {
            RenderStateBlocks[0] = _renderStateBlock;
            var tagValues = new NativeArray<ShaderTagId>(ShaderTagValues, Allocator.Temp);
            var stateBlocks = new NativeArray<RenderStateBlock>(RenderStateBlocks, Allocator.Temp);

            return new RendererListParams(cullingResults, drawingSettings, _filteringSettings)
            {
                tagValues = tagValues,
                stateBlocks = stateBlocks,
                isPassTagName = false
            };
        }
#endif
    }
}
