using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

namespace SyminStudio.Rendering.Universal.Editor
{
    [CustomPropertyDrawer(typeof(FilterSettings))]
    public class FilterSettingsEditor : PropertyDrawer
    {
        bool _showFilterSettings = true;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            // Calculate rects
            var foldoutRect = new Rect(
                position.x,
                position.y,
                position.width,
                EditorGUIUtility.singleLineHeight);
            var renderQueueTypeRect = new Rect(
                position.x,
                position.y + EditorGUIUtility.singleLineHeight + 2,
                position.width,
                EditorGUIUtility.singleLineHeight);
            var layerMaskRect = new Rect(
                position.x,
                position.y + 2 * (EditorGUIUtility.singleLineHeight + 2),
                position.width,
                EditorGUIUtility.singleLineHeight);
            var renderingLayerMaskRect = new Rect(
                position.x,
                position.y + 3 * (EditorGUIUtility.singleLineHeight + 2),
                position.width,
                EditorGUIUtility.singleLineHeight);

            _showFilterSettings = EditorGUI.Foldout(foldoutRect, _showFilterSettings, label);
            // Draw fields
            var renderLayerProp = property.FindPropertyRelative(nameof(FilterSettings.renderingLayerMask));
            if (_showFilterSettings)
            {
                EditorGUI.indentLevel++;
                EditorGUI.PropertyField(renderQueueTypeRect,
                    property.FindPropertyRelative(nameof(FilterSettings.renderQueueType)));
                EditorGUI.PropertyField(layerMaskRect, property.FindPropertyRelative(nameof(FilterSettings.layerMask)));
                if (renderLayerProp != null)
                {
                    renderLayerProp.intValue = EditorGUI.MaskField(renderingLayerMaskRect, "Rendering Layer Mask",
                        renderLayerProp.intValue, GraphicsSettings.currentRenderPipeline.renderingLayerMaskNames);
                }

                EditorGUI.indentLevel--;
            }

            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            if (_showFilterSettings)
            {
                return 4 * (EditorGUIUtility.singleLineHeight + 2);
            }

            return EditorGUIUtility.singleLineHeight + 2;
        }
    }
}