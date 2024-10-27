#if UNITY_EDITOR
using System;
using UnityEditor;
using UnityEngine;

public class GroKitCoreAndroidBuildHelper : EditorWindow
{


    GUIStyle calloutStyle;
    [MenuItem("GroKit-Core/Android Build Helper")]
    static void Init()
    {
        GetWindow<GroKitCoreAndroidBuildHelper>(false, "GroKit Android Build Helper", true);
    }

    void OnGUI()
    {
        if (calloutStyle == null)
        {
            calloutStyle = new GUIStyle(EditorStyles.label);
            calloutStyle.richText = true;
            calloutStyle.wordWrap = true;
        }

        GUILayout.BeginHorizontal(EditorStyles.helpBox);
        GUILayout.BeginVertical();
        EditorGUILayout.LabelField("Use this to generate Android builds - Don't forget entitlement checks!", calloutStyle);
        GUILayout.EndVertical();
        GUILayout.EndHorizontal();

        //Company Name
        PlayerSettings.companyName = EditorGUILayout.TextField(new GUIContent("Company Name"), PlayerSettings.companyName, GUILayout.Width(600));
        //Version
        PlayerSettings.bundleVersion = EditorGUILayout.TextField(new GUIContent("Version"), PlayerSettings.bundleVersion, GUILayout.Width(600));
        //Product Name
        PlayerSettings.productName = EditorGUILayout.TextField(new GUIContent("Product Name"), PlayerSettings.productName, GUILayout.Width(600));
        //BUILD ID
        PlayerSettings.Android.bundleVersionCode = EditorGUILayout.IntField(new GUIContent("Bundle Version Code"),
               PlayerSettings.Android.bundleVersionCode, GUILayout.Width(220));
        //Version CODE
        GUILayout.Space(10);
        PlayerSettings.applicationIdentifier = EditorGUILayout.TextField(new GUIContent("Package Name"), PlayerSettings.applicationIdentifier, GUILayout.Width(500));

        //Development Check Box
        EditorUserBuildSettings.development = EditorGUILayout.Toggle(new GUIContent("Development Build"),
        EditorUserBuildSettings.development);

        //Use Custom KeyStore

        //Keystore used
        PlayerSettings.Android.useCustomKeystore = EditorGUILayout.Toggle(new GUIContent("Use Custom KeyStore"), PlayerSettings.Android.useCustomKeystore);


        if (PlayerSettings.Android.useCustomKeystore)
        {
            EditorGUILayout.LabelField("Keystore Path", PlayerSettings.Android.keystoreName);
            PlayerSettings.Android.keystorePass = EditorGUILayout.PasswordField("Keystore Password", PlayerSettings.Android.keystorePass);

            EditorGUILayout.LabelField("Key Alias Name", PlayerSettings.Android.keyaliasName);
            PlayerSettings.Android.keyaliasPass = EditorGUILayout.PasswordField("Alias Password", PlayerSettings.Android.keyaliasPass);
        }
        //PasswordStuff


        GUILayout.Space(10);
        if (GUILayout.Button("Set Test Build"))
        {
            EditorUserBuildSettings.development = true;
            PlayerSettings.Android.useCustomKeystore = false;
            if (!PlayerSettings.applicationIdentifier.Contains(".TEST"))
            {
                PlayerSettings.applicationIdentifier += ".TEST";
            }
            SetDate();
        }
        if (GUILayout.Button("Set Prod Build"))
        {
            PlayerSettings.Android.useCustomKeystore = true;
            PlayerSettings.Android.bundleVersionCode++;
            if (PlayerSettings.applicationIdentifier.Contains(".TEST"))
            {
                PlayerSettings.applicationIdentifier = PlayerSettings.applicationIdentifier.Replace(".TEST", "");
            }
            EditorUserBuildSettings.development = false;
            SetDate();
        }
        GUILayout.Space(10);
        if (GUILayout.Button("Increment Build"))
        {
            PlayerSettings.Android.bundleVersionCode++;
        }
        //GUILayout.Space(10);
        //if (GUILayout.Button("Close"))
        //{
        //    this.Close();
        //}
        if (GUILayout.Button("Build", GUILayout.Height(40)))
        {
            GetWindow(System.Type.GetType("UnityEditor.BuildPlayerWindow,UnityEditor"));
        }
    }

    public void SetDate()
    {
        string test = DateTime.Now.ToString("yy.MM.dd");
        PlayerSettings.bundleVersion = test;
    }
}
#endif
