#if UNITY_EDITOR
using UnityEngine;
using System;
using UnityEditor;
using System.Reflection;
using System.Text.RegularExpressions;

/// <summary>
/// This Script contains the Buttons / Show/HideIf and The ReadOnly
/// </summary>
/// 
namespace AICore3lb.AICoreEditor
{
    /// <summary>
    /// -----------------------------------------CoreCustomEditorAI----------------------------------------------------------------------
    /// </summary>
    [CustomEditor(typeof(AICoreBehaviour),true), CanEditMultipleObjects]
    public class AICoreCustomEditor : Editor
    {
        public static Texture2D logoImage;
        public static Texture2D groBlockTexture;
        public override void OnInspectorGUI()
        {
            var targetType = target.GetType();

            DrawHeader(targetType.Name);
            DrawCoreButtons(targetType);
            PropertyEditing();
        }

        public static string AddSpacesToSentence(string text)
        {
            return Regex.Replace(text, "([a-z])([A-Z])", "$1 $2");
        }

        private void DrawHeader(string className)
        {
            className = AddSpacesToSentence(className);
            // Header style
            GUIStyle headerStyle = new GUIStyle(EditorStyles.boldLabel)
            {
                normal = { textColor = new Color(0.78f, 0.78f, 0.78f, 1) },
                fontStyle = FontStyle.Bold,
                fontSize = 20,
                alignment = TextAnchor.MiddleCenter
            };

            // Load the image from Resources
            if (logoImage == null)
            {
                logoImage = Resources.Load<Texture2D>("AICore_EditorCircle");
            }

            // Calculate the size and position
            //float imageWidth = 130; //Long Version!
            //float imageHeight = 32;
            float imageWidth = 32;
            float imageHeight = 32;
            float headerHeight = EditorGUIUtility.singleLineHeight * 2.5f;
            Rect totalRect = GUILayoutUtility.GetRect(0, headerHeight, GUILayout.ExpandWidth(true));
            float totalWidth = headerStyle.CalcSize(new GUIContent(className)).x + imageWidth + 5;

            // Draw the grey background
            EditorGUI.DrawRect(totalRect, new Color(0.18f, 0.18f, 0.18f, 0.75f));

            // Position for the image
            Rect imageRect = new Rect(totalRect.x + (totalRect.width - totalWidth) / 2, totalRect.y + (headerHeight - imageHeight) / 2, imageWidth, imageHeight);

            // Position for the text
            Rect textRect = new Rect(imageRect.xMax + 5, totalRect.y, totalWidth - imageWidth, headerHeight);

            if (logoImage != null)
            {
                GUI.DrawTexture(imageRect, logoImage);
            }

            // Draw the header text
            EditorGUI.LabelField(textRect, className, headerStyle);
        }

        private void DrawCoreButtons(Type targetType)
        {
            foreach (var method in targetType.GetMethods(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public))
            {
                var attributes = method.GetCustomAttributes(typeof(AICoreButton), true);
                if (attributes.Length > 0)
                {
                    var coreButton = attributes[0] as AICoreButton;
                    var label = coreButton.Label;

                    if (string.IsNullOrEmpty(label))
                    {
                        label = method.Name;
                    }

                    bool shouldDisableButton = !EditorApplication.isPlaying && !coreButton.UseInEditor;

                    EditorGUI.BeginDisabledGroup(shouldDisableButton);
                    if (GUILayout.Button(label))
                    {
                        method.Invoke(target, null);
                    }
                    EditorGUI.EndDisabledGroup();
                }
            }
        }


        public void PropertyEditing()
        {
            SerializedObject obj = serializedObject;
            obj.Update();

            SerializedProperty property = obj.GetIterator();
            if (property.NextVisible(true))
            {
                do
                {
                    bool showProperty = true;

                    // Check for CoreHideIf attribute
                    AICoreHideIf hideIfAttr = GetAttribute<AICoreHideIf>(property);
                    if (hideIfAttr != null && CheckCondition(hideIfAttr.ConditionBool, obj))
                    {
                        showProperty = false;
                    }

                    // Check for CoreShowIf attribute
                    AICoreShowIf showIfAttr = GetAttribute<AICoreShowIf>(property);
                    if (showIfAttr != null)
                    {
                        showProperty = CheckCondition(showIfAttr.ConditionBool, obj);
                    }

                    if (showProperty)
                    {
                        ProcessCoreReadOnly(property);
                    }


                } while (property.NextVisible(false));
            }

            obj.ApplyModifiedProperties();
        }

        private T GetAttribute<T>(SerializedProperty property) where T : PropertyAttribute
        {
            FieldInfo field = property.serializedObject.targetObject.GetType().GetField(property.name, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
            if (field != null)
            {
                return (T)field.GetCustomAttribute(typeof(T), true);
            }
            return null;
        }

        private bool CheckCondition(string conditionBool, SerializedObject serializedObject)
        {
            SerializedProperty conditionProperty = serializedObject.FindProperty(conditionBool);
            return conditionProperty != null && conditionProperty.boolValue;
        }

        private void ProcessCoreReadOnly(SerializedProperty property)
        {
            AICoreReadOnlyAttribute readOnlyAttr = GetAttribute<AICoreReadOnlyAttribute>(property);
            if (readOnlyAttr != null)
            {
                GUI.enabled = false;
            }

            EditorGUILayout.PropertyField(property, true);

            if (readOnlyAttr != null)
            {
                GUI.enabled = true;
            }
        }
    }


    [CustomPropertyDrawer(typeof(AICoreEmphasizeAttribute))]
    public class CoreEmphasizeDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            AICoreEmphasizeAttribute emphasisAttribute = (AICoreEmphasizeAttribute)attribute;

            if (property.propertyType == SerializedPropertyType.ObjectReference && property.objectReferenceValue != null || emphasisAttribute.forcedOn)
            {
                if (EditorGUIUtility.isProSkin)
                {
                    EditorGUI.DrawRect(position, new Color(0.15f, 0.3f, 0.45f));
                }
                else
                {
                    EditorGUI.DrawRect(position, new Color(0.4f, 0.65f, 0.9f));
                }
            }
            EditorGUI.PropertyField(position, property, label, true);
        }
    }



    ///--------------------------CORE HEADER EDITOR--------------------------------------------
    [CustomPropertyDrawer(typeof(AICoreHeaderAttribute))]
    public class CoreHeaderDrawer : DecoratorDrawer
    {
        const float spacing = 8f;
        const float padding = 2f;
        const float margin = -20f;
        const float barHeight = 4f;

        public override void OnGUI(Rect position)
        {
            var attr = (attribute as AICoreHeaderAttribute);
            var pos = EditorGUI.IndentedRect(position);
            var rowHeight = (EditorGUIUtility.singleLineHeight * attr.count) + (EditorGUIUtility.standardVerticalSpacing * attr.count);


            // draw header background and label
            var headerRect = new Rect(pos.x + margin, pos.y + spacing, (pos.width - margin) + (padding * 2), pos.height - (spacing + barHeight + spacing));
            EditorGUI.DrawRect(headerRect, ConstantsCoreHeader.BackgroundColor);
            EditorGUI.LabelField(headerRect, new GUIContent("   " + attr.label, attr.tooltip), ConstantsCoreHeader.LabelStyle);
        }

        public override float GetHeight()
        {
            return EditorGUIUtility.singleLineHeight * 2.5f;
        }
    }
    public static class ConstantsCoreHeader
    {
        private static readonly Color[] _barColors = new Color[5] {
                new Color(0.3411765f, 0.6039216f, 0.7803922f),
                new Color(0.145098f, 0.6f, 0.509804f),
                new Color(0.9215686f, 0.6431373f, 0.282353f),
                new Color(0.8823529f, 0.3529412f, 0.4039216f),
                new Color(0.9529412f, 0.9294118f, 0.682353f)
            };

        public static Color ColorForDepth(int depth) => _barColors[depth % _barColors.Length];

        public static Color BackgroundColor { get; } = EditorGUIUtility.isProSkin ? new Color(0.18f, 0.18f, 0.18f, 0.75f) : new Color(0.7f, 0.7f, 0.7f, 0.75f);

        public static GUIStyle LabelStyle { get; } = new GUIStyle(GUI.skin.label)
        {
            fontSize = 13,
            fontStyle = FontStyle.Bold,
            alignment = TextAnchor.MiddleLeft,
        };
    
    
    }

    [CustomPropertyDrawer(typeof(AICoreRequiredAttribute))]
    public class CoreRequiredDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (property.propertyType == SerializedPropertyType.ObjectReference && property.objectReferenceValue == null)
            {
                EditorGUI.DrawRect(position, new Color(.7f, .1f, .1f));
            }
            EditorGUI.PropertyField(position, property, label, true);
        }
    }

    //NAMESPACE END! 
}
#endif