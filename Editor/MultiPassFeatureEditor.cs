using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

namespace SyminStudio.Rendering.Universal.Editor
{
    [CustomEditor(typeof(MultiPassFeature))]
    public class MultiPassFeatureEditor : UnityEditor.Editor
    {
        private MultiPassFeature _multiPassFeature;
        //初始化
        private void OnEnable()
        {
            var prop = serializedObject.FindProperty("filterSettings");
            //_multiPassFeature = (MultiPassFeature)target;
        }

        //绘制 Inspector UI
        public override void OnInspectorGUI()
        {
            //绘制默认的 Inspector UI
            base.OnInspectorGUI();
            //绘制自定义的 Inspector UI
            //_multiPassFeature.filterSettings
            //var layerNames = GraphicsSettings.currentRenderPipeline.renderingLayerMaskNames;

        }
        
        
    }
}
