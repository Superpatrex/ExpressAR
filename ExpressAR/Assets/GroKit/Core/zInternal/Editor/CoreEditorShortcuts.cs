using UnityEngine;
using UnityEditor;
using TMPro;
using Core3lb;
using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class CoreEditorShortcuts : EditorWindow
{
    private Transform targetTransform;
    private bool useLocal = false;
    float buttonTallHeight = 25f; // You can change this value to increase or decrease button height
    // Adds a menu item to open the window
    [MenuItem("GroKit-Core/Shortcuts Window")]
    public static void ShowWindow()
    {
        GetWindow(typeof(CoreEditorShortcuts), false, "GroKit Shortcuts");
    }

    public Transform followerTransform
    {
        get
        {
            if (Selection.activeGameObject)
            {
                return Selection.activeGameObject.transform;
            }
            else
            {
                return null;
            }
        }
    }

    private void OnGUI()
    {
        //GUILayout.Label("Transform Manipulation - Target is Selection", EditorStyleExtensions.ColorBanner());
        GUILayout.Label("Transform Manipulation", EditorStyleExtensions.FancyHeader());
        // Fields to set the target and follower transforms
        targetTransform = EditorGUILayout.ObjectField("Target Transform", targetTransform, typeof(Transform), true) as Transform;

        useLocal = EditorGUILayout.Toggle("useLocal", useLocal);
        bool hasValidTransforms = targetTransform != null && followerTransform != null;

        // Specify the desired button height
        // Buttons for Move, Parent, Rotate, and Scale in the same row with custom height
        if (GUILayout.Button("Set Target", GUILayout.Height(buttonTallHeight-5)))
        {
            if (Selection.activeGameObject)
            {
                targetTransform = Selection.activeGameObject.transform;
            }
        }
        EditorGUI.BeginDisabledGroup(!hasValidTransforms);
        GUILayout.BeginHorizontal(); // Start a horizontal group
        if (GUILayout.Button("Move", GUILayout.Height(buttonTallHeight)))
        {
            MoveToPosition();
        }
        if (GUILayout.Button("Parent", GUILayout.Height(buttonTallHeight)))
        {
            ParentToTarget();
        }
        if (GUILayout.Button("Rotate", GUILayout.Height(buttonTallHeight)))
        {
            RotateToTarget();
        }
        if (GUILayout.Button("Scale", GUILayout.Height(buttonTallHeight)))
        {
            ScaleTo();
        }
        GUILayout.EndHorizontal(); // End the horizontal group
        EditorGUI.EndDisabledGroup();


        if (!hasValidTransforms)
        {
            EditorGUILayout.HelpBox("Please assign both the Target and Select a something in the scene.", MessageType.Warning);
        }

        // Label for Selection-Based Actions
        GUILayout.Label("Creation", EditorStyleExtensions.FancyHeader());
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Cube", GUILayout.Height(buttonTallHeight)))
        {
            CreateCube();
        }

        if (GUILayout.Button("TMP_TXT", GUILayout.Height(buttonTallHeight)))
        {
            CreateTMPText();
        }
        //if (GUILayout.Button("SFXEvent", GUILayout.Height(buttonTallHeight)))
        //{
        //    CreateSFXEvent();
        //}
        //if (GUILayout.Button("FXEvent", GUILayout.Height(buttonTallHeight)))
        //{
        //    CreateSFXEvent();
        //}
        GUILayout.EndHorizontal();


        //##################################### Base Activator ###################
        //########################################################################
        GUILayout.Label("Activators", EditorStyleExtensions.FancyHeader());

        baseActivator = EditorGUILayout.ObjectField("Activator Target", baseActivator, typeof(BaseActivator), true) as BaseActivator;

        //// Disable buttons if no Activator is found on the selected GameObject
        if (GUILayout.Button("Set Activator", GUILayout.Height(buttonTallHeight -5)))
        {
            if(Selection.activeGameObject)
            {
                if (Selection.activeGameObject.TryGetComponent(out BaseActivator activator))
                {
                    baseActivator = activator;
                }
            }
        }
        EditorGUI.BeginDisabledGroup(baseActivator == null);
        GUILayout.BeginHorizontal();

        //Single Target Buttons
        if (GUILayout.Button("Add On", GUILayout.Height(buttonTallHeight)))
        {
            foreach (GameObject obj in Selection.gameObjects)
            {
                AddSetActiveToEvent(baseActivator.onEvent, obj, true);
            }
        }

        if (GUILayout.Button("Select Activator", GUILayout.Height(buttonTallHeight)))
        {
            Selection.activeGameObject = baseActivator.gameObject;
        }

        if (GUILayout.Button("Add Off", GUILayout.Height(buttonTallHeight)))
        {
            foreach (GameObject obj in Selection.gameObjects)
            {
                AddSetActiveToEvent(baseActivator.offEvent, obj, false);
            }
        }
        EditorGUI.EndDisabledGroup();

        GUILayout.EndHorizontal();


        //##################################### OFF ON START ###################
        //########################################################################

        GUILayout.Label("_OOS(Off On Start)", EditorStyleExtensions.FancyHeader());
        GUILayout.BeginHorizontal();

        if (GUILayout.Button("Add _OOS", GUILayout.Height(buttonTallHeight)))
        {
            foreach (GameObject obj in Selection.gameObjects)
            {
                //This might need to be EndsWith
                if (!obj.name.Contains("_OOS"))
                {
                    obj.name += "_OOS";
                }
            }
        }

        if (GUILayout.Button("Set _OOS In Scene", GUILayout.Height(buttonTallHeight)))
        {
            // Display a confirmation dialog
            if (EditorUtility.DisplayDialog("Confirm Action",
                                            "Are you sure you want to set all '_OOS' objects in the scene as inactive?",
                                            "Yes", "No"))
            {
                // Get all root objects in the active scene
                GameObject[] allObjects = SceneManager.GetActiveScene().GetRootGameObjects();

                // Iterate through all root objects and their children
                foreach (GameObject obj in allObjects)
                {
                    // Use a recursive function to search through the hierarchy
                    if (obj.name.EndsWith("_OOS"))
                    {
                        obj.SetActive(false);
                    }

                    foreach (Transform child in obj.GetComponentsInChildren<Transform>(true))
                    {
                        if (child.gameObject.name.EndsWith("_OOS"))
                        {
                            child.gameObject.SetActive(false);
                        }
                    }
                }
            }
        }

        if (GUILayout.Button("Remove _OOS", GUILayout.Height(buttonTallHeight)))
        {
            foreach (GameObject obj in Selection.gameObjects)
            {
                if (obj.name.EndsWith("_OOS"))
                {
                    obj.name = obj.name.Replace("_OOS", "");
                }
            }
        }

        GUILayout.EndHorizontal();
        GUILayout.Label("Set Active", EditorStyleExtensions.FancyHeader());
        GUILayout.BeginHorizontal();


        if (GUILayout.Button("True", GUILayout.Height(buttonTallHeight)))
        {
            foreach (GameObject obj in Selection.gameObjects)
            {
                obj.SetActive(true);
            }
        }

        if (GUILayout.Button("Toggle", GUILayout.Height(buttonTallHeight)))
        {
            foreach (GameObject obj in Selection.gameObjects)
            {
                obj.SetActive(!obj.activeInHierarchy);
            }
        }

        if (GUILayout.Button("False", GUILayout.Height(buttonTallHeight)))
        {
            foreach (GameObject obj in Selection.gameObjects)
            {
                obj.SetActive(false);
            }
        }
        GUILayout.EndHorizontal();
    }

    private void MoveToPosition()
    {
        if (targetTransform != null && followerTransform != null)
        {
            foreach (Transform t in Selection.transforms)
            {
                Undo.RecordObject(t, "Move To Position");
                if(useLocal)
                {
                    t.localPosition = targetTransform.localPosition;
                }
                else
                {
                    t.position = targetTransform.position;
                }
                EditorUtility.SetDirty(t);
            }
        }
    }

    private void ParentToTarget()
    {
        if (targetTransform != null && followerTransform != null)
        {
            foreach (Transform t in Selection.transforms)
            {
                Undo.SetTransformParent(t, targetTransform, "Parent To Target");
                EditorUtility.SetDirty(t);
            }
        }
    }

    private void RotateToTarget()
    {
        if (targetTransform != null && followerTransform != null)
        {
            foreach (Transform t in Selection.transforms)
            {
                Undo.RecordObject(t, "Rotate To Target");
                if (useLocal)
                {
                    t.rotation = targetTransform.rotation;
                }
                else
                {
                    t.rotation = targetTransform.rotation;
                }

                EditorUtility.SetDirty(t);
            }
        }
    }

    private void ScaleTo()
    {
        if (targetTransform != null && followerTransform != null)
        {
            foreach (Transform t in Selection.transforms)
            {
                Undo.RecordObject(t, "Rotate To Target");
                if (useLocal)
                {
                    t.localScale = targetTransform.localScale;
                }
                else
                {
                    t.localScale = targetTransform.lossyScale;
                }

                EditorUtility.SetDirty(t);
            }
        }
    }

    private void CreateEmptyGameObject()
    {
        GameObject newObj = new GameObject("Empty GameObject");
        PositionAtSceneView(newObj);
        Undo.RegisterCreatedObjectUndo(newObj, "Create Empty GameObject");
        Selection.activeGameObject = newObj;
    }

    private void CreateEmptyChild()
    {
        if (Selection.activeTransform != null)
        {
            GameObject newChild = new GameObject("Empty Child");
            newChild.transform.parent = Selection.activeTransform;
            newChild.transform.localPosition = Vector3.zero;
            Undo.RegisterCreatedObjectUndo(newChild, "Create Empty Child");
            Selection.activeGameObject = newChild;
        }
    }

    private void CreateEmptyParent()
    {
        if (Selection.transforms.Length > 0)
        {
            // Get the shared parent of all selected objects
            Transform sharedParent = Selection.transforms[0].parent;
            foreach (Transform selected in Selection.transforms)
            {
                if (selected.parent != sharedParent)
                {
                    sharedParent = null; // If different parents are found, set to null (no shared parent)
                    break;
                }
            }

            // Create a new empty parent GameObject
            GameObject parentObject = new GameObject("Empty Parent");
            Undo.RegisterCreatedObjectUndo(parentObject, "Create Empty Parent");

            // Set the position to the average position of selected objects
            Vector3 averagePosition = Vector3.zero;
            foreach (Transform selected in Selection.transforms)
            {
                averagePosition += selected.position;
            }
            parentObject.transform.position = averagePosition / Selection.transforms.Length;

            // Assign shared parent if it exists
            if (sharedParent != null)
            {
                parentObject.transform.SetParent(sharedParent, true);
            }

            // Parent all selected transforms to the new parent
            foreach (Transform selected in Selection.transforms)
            {
                Undo.SetTransformParent(selected, parentObject.transform, "Parent to Empty Parent");
            }

            Selection.activeGameObject = parentObject;
        }
    }

    private void CreateCube()
    {
        GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        PositionAtSceneView(cube);
        Undo.RegisterCreatedObjectUndo(cube, "Create Cube");
        Selection.activeGameObject = cube;
    }

    private void AddFXSpawner()
    {
        GameObject sfxEvent = new GameObject("FXEvent");
        sfxEvent.AddComponent<FXSpawner>();
        if (Selection.activeTransform != null)
        {
            sfxEvent.transform.position = Selection.activeTransform.position;
            sfxEvent.transform.SetParent(Selection.activeTransform, true);
        }
        else
        {
            PositionAtSceneView(sfxEvent);
        }

        Undo.RegisterCreatedObjectUndo(sfxEvent, "Created SFXEvent");
        Selection.activeGameObject = sfxEvent;
    }

    private void CreateSFXEvent()
    {
        GameObject sfxEvent = new GameObject("SFXEvent");
        sfxEvent.AddComponent<SFXEvent>();
        if (Selection.activeTransform != null)
        {
            sfxEvent.transform.position = Selection.activeTransform.position;
            sfxEvent.transform.SetParent(Selection.activeTransform, true);
        }
        else
        {
            PositionAtSceneView(sfxEvent);
        }

        Undo.RegisterCreatedObjectUndo(sfxEvent, "Created SFXEvent");
        Selection.activeGameObject = sfxEvent;
    }

    private void CreateTMPText()
    {
        GameObject tmpTextObj = new GameObject("TMP Text");
        TextMeshPro tmpText = tmpTextObj.AddComponent<TextMeshPro>();
        tmpText.text = "New Text";

        if (Selection.activeTransform != null)
        {
            tmpTextObj.transform.position = Selection.activeTransform.position;
            tmpTextObj.transform.SetParent(Selection.activeTransform, true);
        }
        else
        {
            PositionAtSceneView(tmpTextObj);
        }

        Undo.RegisterCreatedObjectUndo(tmpTextObj, "Create TMP Text");
        Selection.activeGameObject = tmpTextObj;
    }

    // Position the GameObject at the Scene View if no selection exists
    private void PositionAtSceneView(GameObject obj)
    {
        if (Selection.activeTransform != null)
        {
            obj.transform.position = Selection.activeTransform.position;
        }
        else
        {
            SceneView sceneView = SceneView.lastActiveSceneView;
            if (sceneView != null)
            {
                obj.transform.position = sceneView.camera.transform.position + sceneView.camera.transform.forward * 5f;
            }
        }
    }

    /// <summary>
    /// ############################################################# ACTIVATOR SYSTEM ####################################
    /// ###################################################################################################################
    /// </summary>

    private BaseActivator baseActivator;



    /// <summary>
    /// Adds SetActive calls to the UnityEvent for a list of GameObjects.
    /// </summary>
    /// <param name="unityEvent"></param>
    /// <param name="targets"></param>
    /// <param name="value"></param>
    private void AddObjectsToEvent(UnityEvent unityEvent, List<GameObject> targets, bool value)
    {
        foreach (GameObject target in targets)
        {
            if (target != null)
            {
                AddSetActiveToEvent(unityEvent, target, value);
            }
        }
    }

    /// <summary>
    /// Adds the SetActive function with the specified value to a UnityEvent.
    /// </summary>
    /// <param name="unityEvent">The UnityEvent to modify.</param>
    /// <param name="targetObject">The target GameObject to set active.</param>
    /// <param name="value">The active state to set (true/false).</param>
    private void AddSetActiveToEvent(UnityEvent unityEvent, GameObject targetObject, bool value)
    {
        if (unityEvent == null || targetObject == null)
        {
            Debug.LogError("Invalid parameters for adding SetActive to UnityEvent.");
            return;
        }

        // Serialize the Activator to modify its UnityEvent
        SerializedObject activatorSerializedObject = new SerializedObject(baseActivator);
        SerializedProperty eventProperty = null;

        // Determine which event we're modifying
        if (unityEvent == baseActivator.onEvent)
        {
            eventProperty = activatorSerializedObject.FindProperty("onEvent");
        }
        else if (unityEvent == baseActivator.offEvent)
        {
            eventProperty = activatorSerializedObject.FindProperty("offEvent");
        }
        else
        {
            Debug.LogError("Unknown UnityEvent.");
            return;
        }

        // Add a new listener to the event
        int index = eventProperty.FindPropertyRelative("m_PersistentCalls.m_Calls").arraySize;
        eventProperty.FindPropertyRelative("m_PersistentCalls.m_Calls").InsertArrayElementAtIndex(index);
        SerializedProperty call = eventProperty.FindPropertyRelative("m_PersistentCalls.m_Calls").GetArrayElementAtIndex(index);

        // Set the target object
        call.FindPropertyRelative("m_Target").objectReferenceValue = targetObject;

        // Set the method name
        call.FindPropertyRelative("m_MethodName").stringValue = "SetActive";

        // Set the mode to accept a boolean argument
        call.FindPropertyRelative("m_Mode").enumValueIndex = (int)PersistentListenerMode.Bool;

        // Set the call state to RuntimeOnly
        call.FindPropertyRelative("m_CallState").enumValueIndex = (int)UnityEngine.Events.UnityEventCallState.RuntimeOnly;

        // Set the argument
        SerializedProperty arguments = call.FindPropertyRelative("m_Arguments");
        arguments.FindPropertyRelative("m_ObjectArgument").objectReferenceValue = null;
        arguments.FindPropertyRelative("m_ObjectArgumentAssemblyTypeName").stringValue = string.Empty;
        arguments.FindPropertyRelative("m_IntArgument").intValue = 0;
        arguments.FindPropertyRelative("m_FloatArgument").floatValue = 0f;
        arguments.FindPropertyRelative("m_StringArgument").stringValue = string.Empty;
        arguments.FindPropertyRelative("m_BoolArgument").boolValue = value;

        // Apply the modified properties
        activatorSerializedObject.ApplyModifiedProperties();

        // Mark the Activator as dirty to ensure it saves
        EditorUtility.SetDirty(baseActivator);

        //Debug.Log($"Added SetActive({value}) for {targetObject.name} to the UnityEvent with RuntimeOnly call state.");
    }
}
