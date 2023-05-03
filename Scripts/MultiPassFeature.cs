using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace SyminStudio.Rendering.Universal
{
    [Serializable]
    public class FilterSettings
    {
        //渲染目标的 RenderQueue
        public RenderQueueType renderQueueType;

        //渲染目标的 Layer
        public LayerMask layerMask;

        //渲染指定 renderingLayerMask 的 render 对象
        public RenderingLayer renderingLayerMask;
        
        public FilterSettings()
        {
            renderQueueType = RenderQueueType.Opaque; //默认不透明
            layerMask = -1; //默认渲染所有层
            renderingLayerMask = (RenderingLayer)(-1); //默认渲染全部
        }
    }

    /// <summary>
    /// 支持多个 pass 的 render feature
    /// </summary>
    public class MultiPassFeature : ScriptableRendererFeature
    {
        //渲染时机
        public RenderPassEvent renderEvent = RenderPassEvent.AfterRenderingOpaques;
        public FilterSettings filterSettings;
        public Material material;
        public int[] passToRender;

        [Space(10)] 
        public bool overrideDepthState = false;
        public CompareFunction depthCompareFunction = CompareFunction.LessEqual;
        public bool enableWrite = true;

        private List<SyminRenderPass> _renderPasses = new();

        /// <summary>
        /// 创建 RenderPass
        /// </summary>
        public override void Create()
        {
            if (passToRender == null) return;
            _renderPasses.Clear();
            //根据Shader的Pass数生成多个RenderPass
            foreach (var passIndex in passToRender)
            {
                var renderPass = new SyminRenderPass(name, renderEvent, filterSettings)
                {
                    OverrideMaterial = material,
                    OverrideMaterialPassIndex = passIndex
                };

                //设置深度测试
                if (overrideDepthState)
                {
                    renderPass.SetDepthState(enableWrite, depthCompareFunction);
                }

                _renderPasses.Add(renderPass);
            }
        }

        //添加Pass到渲染队列
        public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
        {
            if (passToRender == null) return;
            foreach (var pass in _renderPasses)
            {
                renderer.EnqueuePass(pass);
            }
        }
    }
}