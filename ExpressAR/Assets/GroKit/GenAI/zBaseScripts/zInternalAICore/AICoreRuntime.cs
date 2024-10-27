using UnityEngine;

using System;
using System.Text.RegularExpressions;

namespace AICore3lb
{
    public static class AICoreExtensions
    {
        public static T GetComponentIfNull<T>(this GameObject baseObject, Component component) where T : Component
        {
            if (component != null)
            {
                return component as T;
            }
            T foundComponent = baseObject.GetComponent<T>();
            if (foundComponent == null)
            {
                Debug.LogError("Component not set on this object", baseObject.gameObject);
            }
            return foundComponent;
        }

        /// <summary>
        /// Sanitize a string for JSON to remove any extra systems
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string SanitizeForJson(string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return input;
            }

            // Replace problematic characters
            input = input.Replace("\\", "\\\\");  // Escape backslashes
            input = input.Replace("\"", "\\\"");  // Escape double quotes
            input = input.Replace("\n", "\\n");   // Escape newlines
            input = input.Replace("\r", "\\r");   // Escape carriage returns
            input = input.Replace("\t", "\\t");   // Escape tabs

            // Remove any other control characters
            input = Regex.Replace(input, @"[\x00-\x1F\x7F]", string.Empty);

            return input;
        }
    }

    [AttributeUsage(AttributeTargets.Method)]
    public class AICoreButton : Attribute
    {
        public string Label { get; private set; }
        public bool UseInEditor { get; private set; } = false;  // Default value is false

        public AICoreButton(string label, bool useInEditor = false)
        {
            Label = label;
            UseInEditor = useInEditor;
        }

        public AICoreButton()
        {
            Label = string.Empty;
        }
    }

    public class AICoreEmphasizeAttribute : PropertyAttribute
    {
        public bool forcedOn;
        public AICoreEmphasizeAttribute(bool alwaysOn = false)
        {
            this.forcedOn = alwaysOn;
        }
    }



    public class AICoreHeaderAttribute : PropertyAttribute
    {
        public int count;
        public int depth;

        public string label;
        public string tooltip;

        /// <summary>
        /// Add a header above a field
        /// </summary>
        /// <param name="label">A title for the header label</param>
        /// <param name="count">the number of child elements under this header</param>
        /// <param name="depth">the depth of this header element in the inspector foldout</param>
        public AICoreHeaderAttribute(string label)
        {
            this.label = label;
        }

        /// <summary>
        /// Add a header above a field with a tooltip
        /// </summary>
        /// <param name="label">A title for the header label</param>
        /// <param name="tooltip">A note or instruction shown when hovering over the header</param>
        /// <param name="count">the number of child elements under this header</param>
        /// <param name="depth">the depth of this header element in the inspector foldout</param>
        public AICoreHeaderAttribute(string label, string tooltip)
        {
            this.label = label;
            this.tooltip = tooltip;
        }
    }

    public class AICoreReadOnlyAttribute : PropertyAttribute { }

    [AttributeUsage(AttributeTargets.Field, Inherited = true)]
    public class AICoreShowIf : PropertyAttribute
    {
        public string ConditionBool;

        public AICoreShowIf(string conditionBool)
        {
            ConditionBool = conditionBool;
        }
    }

    // CoreHideIf attribute definition
    [AttributeUsage(AttributeTargets.Field, Inherited = true)]
    public class AICoreHideIf : PropertyAttribute
    {
        public string ConditionBool;

        public AICoreHideIf(string conditionBool)
        {
            ConditionBool = conditionBool;
        }
    }

    public class AICoreRequiredAttribute : PropertyAttribute { }
}

