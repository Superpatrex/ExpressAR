#if UNITY_EDITOR 
using UnityEditor;
using UnityEngine;
//This is not built at all
namespace CoreSecurity
{
    [CustomEditor(typeof(SO_StringEncrypted))]
    public class SO_AIStringEncryptedEditor : Editor
    {
        private GUIStyle warningStyle;

        public override void OnInspectorGUI()
        {
            SO_StringEncrypted script = (SO_StringEncrypted)target;
            if (!script.SOEncrypted)
            {
                EditorGUILayout.HelpBox("WARNING: This is not encrypted please encrypt before building", MessageType.Error);
            }
            // Display disabled fields
            script.plainString = EditorGUILayout.TextField("Plain Text", script.plainString);
            EditorGUI.BeginDisabledGroup(true);
            EditorGUILayout.TextField("Encrypted Storage", script.SOEncryptedStorage);
            EditorGUILayout.Toggle("Is Encrypted", script.SOEncrypted);
            EditorGUI.EndDisabledGroup();

            // Buttons for encryption and decryption
            if (GUILayout.Button("Encrypt String"))
            {
                script.Editor_Encrypt();
                EditorUtility.SetDirty(target);
            }

            if (GUILayout.Button("Decrypt String"))
            {
                script.Editor_Decrypt();
                EditorUtility.SetDirty(target);
            }
        }
    }
}
#endif