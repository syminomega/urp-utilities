using UnityEngine;

namespace SyminStudio.Rendering.Universal
{
    public class RenderingLayerGroup : MonoBehaviour
    {
        [SerializeField] private RenderingLayer renderingLayerMask = RenderingLayer.LayerDefault;

        public RenderingLayer RenderingLayerMask
        {
            get => renderingLayerMask;
            set
            {
                renderingLayerMask = value;
                SetRenderingLayerMask();
            }
        }

        //添加新的layer
        public void AddLayer(RenderingLayer renderingLayer)
        {
            renderingLayerMask |= renderingLayer;
            SetRenderingLayerMask();
        }

        //移除layer
        public void RemoveLayer(RenderingLayer renderingLayer)
        {
            renderingLayerMask &= ~renderingLayer;
            SetRenderingLayerMask();
        }

        private void SetRenderingLayerMask()
        {
            var renderers = GetComponentsInChildren<Renderer>();
            foreach (var render in renderers)
            {
                render.renderingLayerMask = (uint)renderingLayerMask;
            }
        }

        private void OnValidate()
        {
            SetRenderingLayerMask();
        }
    }
}