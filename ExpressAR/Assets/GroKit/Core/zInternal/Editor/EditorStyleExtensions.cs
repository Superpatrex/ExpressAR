using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;



namespace Core3lb
{
    public static class EditorStyleExtensions
    {
        //Colors
        public static readonly Color CautiousOrange = new Color(1.0f, 0.65f, 0.0f);   // Orange color often used for warnings
        public static readonly Color ErrorRed = new Color(0.8f, 0.0f, 0.0f);           // Dark red for errors

        // Standard Colors for Success and Information
        public static readonly Color SuccessGreen = new Color(0.2f, 0.8f, 0.2f);       // Green for success
        public static readonly Color InfoBlue = new Color(0.0f, 0.5f, 0.8f);           // Blue for information

        // Neutral and Highlight Colors
        public static readonly Color NeutralGray = new Color(0.5f, 0.5f, 0.5f);        // Gray for neutral text or background
        public static readonly Color HighlightYellow = new Color(1.0f, 1.0f, 0.2f);    // Yellow for highlighting important elements

        // Debug and Warning Colors
        public static readonly Color DebugPurple = new Color(0.5f, 0.0f, 0.5f);        // Purple for debug text or elements
        public static readonly Color WarningAmber = new Color(1.0f, 0.49f, 0.0f);      // Amber color for intermediate warnings

        public static GUIStyle ColorBanner(Color backgroundColor = default,Color textColor = default,TextAnchor anchor = TextAnchor.MiddleCenter,int fontSize = 14,FontStyle style = FontStyle.Bold)
        {
            GUIStyle bannerStyle = new GUIStyle(GUI.skin.box);
            backgroundColor = backgroundColor == default ? new Color(.1f, .70f, .9f) : backgroundColor;
            bannerStyle.normal.background = MakeTex(2, 2, backgroundColor);
            bannerStyle.alignment = anchor;
            bannerStyle.normal.textColor = textColor == default ? new Color(.8f, .8f, .8f) : textColor;
            bannerStyle.fontSize = fontSize;
            bannerStyle.fontStyle = style;
            bannerStyle.fixedWidth = EditorGUIUtility.currentViewWidth;
            return bannerStyle;
        }

        public static GUIStyle FancyHeader(TextAnchor anchor = TextAnchor.MiddleCenter, int fontSize = 14, FontStyle style = FontStyle.Bold)
        {
            GUIStyle bannerStyle = new GUIStyle(GUI.skin.box);
            Color backgroundColor = new Color(0.18f, 0.18f, 0.18f, 0.75f);
            bannerStyle.normal.background = MakeTex(2, 2, backgroundColor);
            bannerStyle.normal.textColor = new Color(.8f,.8f,.8f);
            bannerStyle.alignment = anchor;
            bannerStyle.fontSize = fontSize;
            bannerStyle.fontStyle = style;
            bannerStyle.fixedWidth = EditorGUIUtility.currentViewWidth;
            return bannerStyle;
        }

        //GUIStyle myButtonStyle = this.CustomButtonStyle(40, 16, Color.green, FontStyle.Bold);
        //GUIStyle redButtonStyle = this.CustomButtonStyle(50, 18, Color.red, FontStyle.Italic);
        //GUIStyle defaultButtonStyle = this.CustomButtonStyle(30); // Uses default values

        public static GUIStyle CustomButtonStyle(this GUILayout layout, float height, int fontSize = 12, Color? textColor = null, FontStyle fontStyle = FontStyle.Normal)
        {
            // Create a new GUIStyle based on the Editor's button style
            GUIStyle buttonStyle = new GUIStyle(GUI.skin.button);

            // Set the height of the button
            buttonStyle.fixedHeight = height;

            // Set the font size and style
            buttonStyle.fontSize = fontSize;
            buttonStyle.fontStyle = fontStyle;

            // Set the text color (use Color.black as default)
            buttonStyle.normal.textColor = textColor ?? Color.black;

            return buttonStyle;
        }

        public static void CustomLabel(this GUILayout layout, string text, int fontSize = 12, bool isBold = false, Color? fontColor = null, TextAnchor alignment = TextAnchor.MiddleCenter)
        {
            // Create a new GUIStyle based on the Editor's label style
            GUIStyle labelStyle = new GUIStyle(EditorStyles.label);

            // Set the font size, bold style, and alignment
            labelStyle.fontSize = fontSize;
            labelStyle.fontStyle = isBold ? FontStyle.Bold : FontStyle.Normal;
            labelStyle.alignment = alignment;

            // Set the font color (default to black if not specified)
            labelStyle.normal.textColor = fontColor ?? Color.black;

            // Draw the label using the custom style
            GUILayout.Label(text, labelStyle);
        }


        public static Texture2D MakeTex(int width, int height, Color col)
        {
            Color[] pix = new Color[width * height];
            for (int i = 0; i < pix.Length; i++)
                pix[i] = col;
            Texture2D result = new Texture2D(width, height);
            result.SetPixels(pix);
            result.Apply();
            return result;
        }

    }
}
