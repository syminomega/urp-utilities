using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;
using UnityEngine.Rendering;
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
            var sortingCriteria = _renderQueueType == RenderQueueType.Transparent
                ? SortingCriteria.CommonTransparent
                : renderingData.cameraData.defaultOpaqueSortFlags;

            var drawingSettings = CreateDrawingSettings(_shaderTagIdList, ref renderingData, sortingCriteria);
            drawingSettings.overrideMaterial = OverrideMaterial;

            drawingSettings.overrideMaterialPassIndex = OverrideMaterialPassIndex;
            //直接使用封装好的 command buffer
            context.DrawRenderers(renderingData.cullResults, ref drawingSettings, ref _filteringSettings);
            //var commandBuffer = new CommandBuffer();
            //context.ExecuteCommandBuffer(commandBuffer);
        }
    }
}