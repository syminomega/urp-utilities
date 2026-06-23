using System;
using UnityEngine;

namespace SyminStudio.Rendering.Universal
{
    public class RenderingLayerGroup : MonoBehaviour
    {
        [SerializeField] private RenderingLayer renderingLayerMask = RenderingLayer.LayerDefault;
        
        [Obsolete("Use SetLayerForAllChildren instead")]
        public RenderingLayer RenderingLayerMask
        {
            get => renderingLayerMask;
            set
            {
                renderingLayerMask = value;
                ApplyToAllChildren();
            }
        }

        //添加新的layer
        [Obsolete("Use AddLayerToAllChildren instead")]
        public void AddLayer(RenderingLayer renderingLayer)
        {
            renderingLayerMask |= renderingLayer;
            ApplyToAllChildren();
        }

        //移除layer
        [Obsolete("Use RemoveLayerFromAllChildren instead")]
        public void RemoveLayer(RenderingLayer renderingLayer)
        {
            renderingLayerMask &= ~renderingLayer;
            ApplyToAllChildren();
        }
        
        public void SetLayerForSelf(RenderingLayer renderingLayer)
        {
            renderingLayerMask = renderingLayer;
            ApplyToSelf();
        }
        
        public void SetLayerForAllChildren(RenderingLayer renderingLayer)
        {
            renderingLayerMask = renderingLayer;
            ApplyToAllChildren();
        }
        
        public void RemoveLayerFromSelf(RenderingLayer renderingLayer)
        {
            renderingLayerMask &= ~renderingLayer;
            ApplyToSelf();
        }
        
        public void RemoveLayerFromAllChildren(RenderingLayer renderingLayer)
        {
            renderingLayerMask &= ~renderingLayer;
            ApplyToAllChildren();
        }
        
        public void AddLayerToSelf(RenderingLayer renderingLayer)
        {
            renderingLayerMask |= renderingLayer;
            ApplyToSelf();
        }
        
        public void AddLayerToAllChildren(RenderingLayer renderingLayer)
        {
            renderingLayerMask |= renderingLayer;
            ApplyToAllChildren();
        }
        
        public void ApplyToSelf()
        {
            var renderers = GetComponents<Renderer>();
            foreach (var render in renderers)
            {
                render.renderingLayerMask = (uint)renderingLayerMask;
            }
        }
        
        public void ApplyToAllChildren()
        {
            var renderers = GetComponentsInChildren<Renderer>();
            foreach (var render in renderers)
            {
                render.renderingLayerMask = (uint)renderingLayerMask;
            }
        }
        
    }
}