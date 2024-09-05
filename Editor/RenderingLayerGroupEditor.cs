using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

namespace SyminStudio.Rendering.Universal.Editor
{
    [CustomEditor(typeof(RenderingLayerGroup))]
    public class RenderingLayerGroupEditor : UnityEditor.Editor
    {
        private RenderingLayerGroup _renderingLayerGroup;
        //初始化
        private void OnEnable()
        {
            _renderingLayerGroup = (RenderingLayerGroup)target;
        }

        //绘制 Inspector UI
        public override void OnInspectorGUI()
        {
            // 绘制默认的 Inspector UI
            // base.OnInspectorGUI();
            //绘制自定义的 Inspector UI
            EditorGUILayout.LabelField("Rendering Layer Mask");
            var layerNames = GraphicsSettings.currentRenderPipeline.renderingLayerMaskNames;
            // 将layerNames作为枚举的选项
            _renderingLayerGroup.RenderingLayerMask = (RenderingLayer)EditorGUILayout.MaskField((int)_renderingLayerGroup.RenderingLayerMask, layerNames);
        }
    }
}
