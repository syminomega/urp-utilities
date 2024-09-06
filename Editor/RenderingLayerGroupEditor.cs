using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

namespace SyminStudio.Rendering.Universal.Editor
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(RenderingLayerGroup))]
    public class RenderingLayerGroupEditor : UnityEditor.Editor
    {
        //private RenderingLayerGroup _renderingLayerGroup;

        private SerializedProperty _renderingLayerMaskProp;

        //初始化
        private void OnEnable()
        {
            //_renderingLayerGroup = (RenderingLayerGroup)target;
            _renderingLayerMaskProp = serializedObject.FindProperty("renderingLayerMask");
        }

        //绘制 Inspector UI
        public override void OnInspectorGUI()
        {
            // 更新 SerializedObject
            serializedObject.Update();

            // 绘制默认的 Inspector UI
            // base.OnInspectorGUI();

            // 绘制自定义的 Inspector UI
            EditorGUILayout.LabelField("Rendering Layer Mask");
            var layerNames = GraphicsSettings.currentRenderPipeline.renderingLayerMaskNames;

            // 处理多个对象的不同值
            EditorGUI.showMixedValue = _renderingLayerMaskProp.hasMultipleDifferentValues;
            EditorGUI.BeginChangeCheck();
            var maskValue =
                EditorGUILayout.MaskField(_renderingLayerMaskProp.intValue, layerNames);
            if (EditorGUI.EndChangeCheck())
            {
                _renderingLayerMaskProp.intValue = maskValue;
            }

            EditorGUI.showMixedValue = false;
            // 设置属性按钮
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Apply To Self"))
            {
                foreach (var targetObject in targets)
                {
                    var renderingLayerGroup = (RenderingLayerGroup)targetObject;
                    renderingLayerGroup.ApplyToSelf();
                }
            }

            if (GUILayout.Button("Apply To All Children"))
            {
                foreach (var targetObject in targets)
                {
                    var renderingLayerGroup = (RenderingLayerGroup)targetObject;
                    renderingLayerGroup.ApplyToAllChildren();
                }
            }

            EditorGUILayout.EndHorizontal();


            // 应用属性修改
            serializedObject.ApplyModifiedProperties();
        }
    }
}