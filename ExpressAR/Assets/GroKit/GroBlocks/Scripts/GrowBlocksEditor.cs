#if UNITY_EDITOR
using Core3lb;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using Unity.EditorCoroutines.Editor;
using System.Collections;

class GrowBlocksEditor : EditorWindow
{
    string searchResult;

    GUIStyle headerStyle = new GUIStyle();
    GUIStyle buttonStyle = new GUIStyle();
    GUIStyle buttonStyleCentered = new GUIStyle();
    GUIStyle textStyle = new GUIStyle();
    GUIStyle foldoutStyle = new GUIStyle();
    GUIStyle infoStyle = new GUIStyle();

    string currentSelectedPrefabName;
    string currentSelectionDescription = "Click on a GroBlock to the left to learn more about it and it to your scene.";

    public SO_GroBlocksDataScriptableObject groBlocksData;

    Vector2 scrollPos;
    bool isButtonActive;

    public List<AllFolders> allFolders = new List<AllFolders>();

    // Add menu item named "My Window" to the Window menu
    [MenuItem("GroKit-Core/GroBlocks")]
    public static void ShowWindow()
    {
        //Show existing window instance. If one doesn't exist, make one.
        EditorWindow window = EditorWindow.GetWindow(typeof(GrowBlocksEditor), true, "GroBlocks");
        window.maxSize = new Vector2(1000f, 1000f);
        window.minSize = new Vector2(800f, 700f);
    }

    public class AllFolders
    {
        public string folder;
        public bool folderOn;
        public List<string> buttons = new List<string>();
        public List<bool> buttonsOn = new List<bool>();
        public List<string> descriptions = new List<string>();
        public List<string> prefabNames = new List<string>();
    }

    void OnEnable()
    {
        RefreshDatabase();
    }

    public void RefreshDatabase(bool forceRefresh = false)
    {
        allFolders.Clear();

        groBlocksData = Resources.Load<SO_GroBlocksDataScriptableObject>("GroBlocksData");

        if (groBlocksData == null)
        {
            EditorCoroutineUtility.StartCoroutineOwnerless(GenerateGroBlocksDataBase());
            groBlocksData = Resources.Load<SO_GroBlocksDataScriptableObject>("GroBlocksData");
        }
        if(forceRefresh)
        {
            EditorCoroutineUtility.StartCoroutineOwnerless(GenerateGroBlocksDataBase());
            groBlocksData = Resources.Load<SO_GroBlocksDataScriptableObject>("GroBlocksData");
        }
        searchResult = "";

        for (int i = 0; i < groBlocksData.groBlockInfos.Count; i++)
        {
            allFolders.Add(new AllFolders());
            allFolders[allFolders.Count - 1].folderOn = true;
            allFolders[allFolders.Count - 1].folder = groBlocksData.groBlockInfos[i].folder;
        }

        searchResult = "";

        allFolders = groBlocksData.groBlockInfos
            .Select(groblockInfo => groblockInfo.folder)
            .Distinct()
            .Select(folder => new AllFolders
            {
                folderOn = true,
                folder = folder,
                buttons = groBlocksData.groBlockInfos.Where(groblockInfo => groblockInfo.folder == folder).Select(groblockInfo => groblockInfo.displayName).ToList(),
                buttonsOn = groBlocksData.groBlockInfos.Where(groblockInfo => groblockInfo.folder == folder).Select(_ => false).ToList(),
                descriptions = groBlocksData.groBlockInfos.Where(groblockInfo => groblockInfo.folder == folder).Select(groblockInfo => groblockInfo.description).ToList(),
                prefabNames = groBlocksData.groBlockInfos.Where(groblockInfo => groblockInfo.folder == folder).Select(groblockInfo => groblockInfo.prefabName).ToList()
            })
            .ToList();
        //for (int i = 0; i < allFolders.Count; i++)
        //{
        //    for (int j = 0; j < allFolders.Count; j++)
        //    {
        //        if (i != j)
        //        {
        //            if (allFolders[i].folder == allFolders[j].folder)
        //            {
        //                allFolders.RemoveAt(j);
        //            }
        //        }
        //    }
        //}

        //for (int i = 0; i < groBlocksData.groBlockInfos.Count; i++)
        //{
        //    for (int j = 0; j < allFolders.Count; j++)
        //    {
        //        if (allFolders[j].folder == groBlocksData.groBlockInfos[i].folder)
        //        {
        //            allFolders[j].buttons.Add(groBlocksData.groBlockInfos[i].displayName);
        //            allFolders[j].buttonsOn.Add(false);
        //            allFolders[j].descriptions.Add(groBlocksData.groBlockInfos[i].description);
        //            allFolders[j].prefabNames.Add(groBlocksData.groBlockInfos[i].prefabName);
        //        }
        //    }
        //}
    }

    private static IEnumerator GenerateGroBlocksDataBase()
    {
        string[] guids = AssetDatabase.FindAssets("t:Prefab");
        SO_GroBlocksDataScriptableObject config = CreateInstance<SO_GroBlocksDataScriptableObject>();
        List<GroBlockInformation> blockList = new List<GroBlockInformation>();

        for (int i = 0; i < guids.Length; i++)
        {
            string guid = guids[i];
            string path = AssetDatabase.GUIDToAssetPath(guid);
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
            GroBlockInformation holder;
            if (prefab != null && prefab.TryGetComponent<GroBlockInformation>(out holder))
            {
                string prefabName = Path.GetFileNameWithoutExtension(path); // Get the prefab name without the path
                string directoryPath = Path.GetDirectoryName(path).Replace("\\", "/");
                string[] pathSegments = directoryPath.Split('/');

                // Determine the correct folder name to use
                string folderName = pathSegments.Length > 0 ? pathSegments[pathSegments.Length - 1] : "";
                if (folderName == "GroBlocks" || folderName == "Prefabs")
                {
                    // If the specific folder is "GroBlocks" or "Prefabs", go up one more level if possible
                    folderName = pathSegments.Length > 1 ? pathSegments[pathSegments.Length - 2] : folderName;
                }
                holder.folderName = folderName;
                holder.prefabName = prefabName;
                blockList.Add(holder);
            }

            // Yield every X iterations to keep the Editor responsive
            if (i % 20 == 0)
            {
                yield return null;
            }
        }

        if (!AssetDatabase.IsValidFolder("Assets/Resources"))
        {
            AssetDatabase.CreateFolder("Assets", "Resources");
        }
        config.groBlockInfos = new List<SO_GroBlocksDataScriptableObject.GroBlockInfo>();
        foreach (var item in blockList)
        {
            SO_GroBlocksDataScriptableObject.GroBlockInfo info = new SO_GroBlocksDataScriptableObject.GroBlockInfo
            {
                description = item.description,
                folder = item.folderName,
                displayName = item.displayName,
                prefabName = item.prefabName
            };
            if (info.displayName == "")
            {
                info.displayName = item.prefabName;
            }
            config.groBlockInfos.Add(info);
        }

        Debug.Log($"Found: {config.groBlockInfos.Count} GroBlocks");
        AssetDatabase.CreateAsset(config, "Assets/Resources/GroBlocksData.asset");
        AssetDatabase.SaveAssets();
    }


void OnGUI()
    {
        int uniformPadding = 10;
        RectOffset padding = new RectOffset(uniformPadding, uniformPadding, uniformPadding, uniformPadding);
        Rect area = new Rect(padding.right, padding.top, position.width - (padding.right + padding.left), position.height - (padding.top + padding.bottom));
        
        headerStyle.fontSize = 25;
        headerStyle.fontStyle = FontStyle.Bold;
        headerStyle.normal.textColor = Color.white;
        headerStyle.wordWrap = true;
        headerStyle.alignment = TextAnchor.MiddleCenter; 
        buttonStyle = new GUIStyle(GUI.skin.button);

        infoStyle = new GUIStyle(EditorStyles.label);
        infoStyle.fontSize = 15;
        infoStyle.normal.textColor = Color.white;
        infoStyle.wordWrap = true;
        infoStyle.alignment = TextAnchor.UpperLeft;

        buttonStyle.normal.textColor = Color.white;
        buttonStyle.fontSize = 14;
        buttonStyle.fontStyle = FontStyle.Bold;
        buttonStyle.alignment = TextAnchor.MiddleLeft;

        buttonStyleCentered = new GUIStyle(GUI.skin.button);
        buttonStyleCentered.normal.textColor = Color.white;
        buttonStyleCentered.fontSize = 14;
        buttonStyleCentered.fontStyle = FontStyle.Bold;
        buttonStyleCentered.alignment = TextAnchor.MiddleCenter;
        //buttonStyleCentered.normal.background = GenerateColoredTexture(new Color(.1f, .50f, .7f));

        textStyle.normal.textColor = Color.white;
        textStyle.fontSize = 12;
        textStyle.fontStyle = FontStyle.Bold;

        foldoutStyle = new GUIStyle(EditorStyles.foldout);
        foldoutStyle.normal.textColor = Color.white;
        foldoutStyle.fontSize = 14;
        foldoutStyle.fontStyle = FontStyle.Bold;

        GUILayout.BeginArea(area);
        GUILayout.Space(10);
        Texture logoBanner = Resources.Load<Texture>("GroBlocks");
        EditorGUILayout.BeginHorizontal();
        if (logoBanner != null)
        {
            GUILayout.Label(logoBanner, headerStyle);
        }
        EditorGUILayout.EndHorizontal();
        //GUILayout.Label("Welcome to GroBlocks!", headerStyle);
        GUILayout.Space(10);
        GUILayout.Space(10);

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.BeginVertical("box", GUILayout.ExpandWidth(true));
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Search ", GUILayout.Height(19), GUILayout.Width(50));
        searchResult = EditorGUILayout.TextField(searchResult);
        if (GUILayout.Button("Refresh Database", GUILayout.Height(19), GUILayout.Width(130)))
        {
            RefreshDatabase(true);
        }
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.EndVertical();
        EditorGUILayout.EndHorizontal();

        GUILayout.Space(5);

        EditorGUILayout.BeginHorizontal(GUILayout.ExpandWidth(true));
        if (searchResult != null)
        {
            scrollPos =
            EditorGUILayout.BeginScrollView(scrollPos, "box", GUILayout.MaxWidth(350), GUILayout.MinWidth(350), GUILayout.ExpandHeight(true));
            for (int i = 0; i < allFolders.Count; i++)
            {
                EditorGUILayout.BeginVertical("box", GUILayout.ExpandWidth(true));
                allFolders[i].folderOn = EditorGUILayout.Foldout(allFolders[i].folderOn, allFolders[i].folder, foldoutStyle);
                if (allFolders[i].folderOn)
                {
                    EditorGUILayout.BeginVertical();
                    for (int j = 0; j < allFolders[i].buttons.Count; j++)
                    {
                        EditorGUILayout.BeginHorizontal();
                        if (allFolders[i].buttons[j].Contains(searchResult, System.StringComparison.OrdinalIgnoreCase) || searchResult.Length == 0)
                        {
                            if (GUILayout.Button(allFolders[i].buttons[j], buttonStyle, GUILayout.Height(50)))
                            {
                                isButtonActive = true;
                                currentSelectionDescription = allFolders[i].descriptions[j];
                                currentSelectedPrefabName = allFolders[i].prefabNames[j];                               
                            }
                            //if (GUILayout.Button("i", buttonStyle, GUILayout.Height(50), GUILayout.Width(50)))
                            //{                                
                            //    currentSelectionDescription = allFolders[i].descriptions[j];
                            //}
                        }
                        EditorGUILayout.EndHorizontal();
                    }
                    EditorGUILayout.EndVertical();
                }
                GUILayout.EndVertical();
                GUILayout.Space(5);
            }
            EditorGUILayout.EndScrollView();
            EditorGUILayout.BeginVertical("box", GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
            GUILayout.Space(5);
            if (isButtonActive)
            {
                if (GUILayout.Button("Add To Scene", buttonStyleCentered, GUILayout.Height(35)))
                {
                    SpawnPrefabByName(currentSelectedPrefabName);
                }
            }
                GUILayout.Space(10);
            EditorGUILayout.BeginVertical("box", GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
            GUILayout.Label(currentSelectionDescription, infoStyle, GUILayout.ExpandWidth(true));
            EditorGUILayout.EndVertical();
            EditorGUILayout.EndVertical();
        }
        EditorGUILayout.EndHorizontal();
        
        GUILayout.EndArea();
    }

    private static void SpawnPrefabByName(string name)
    {
        string[] guids = AssetDatabase.FindAssets(name + " t:Prefab");
        if (guids.Length > 0)
        {
            string path = AssetDatabase.GUIDToAssetPath(guids[0]);
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
            if (prefab != null)
            {
                PrefabUtility.InstantiatePrefab(prefab);
                Debug.Log($"Prefab {name} spawned successfully.");
            }
            else
            {
                Debug.LogError("Prefab not found.");
            }
        }
        else
        {
            Debug.LogError("Prefab not found.");
        }
    }

    private Texture2D GenerateColoredTexture(Color color)
    {
        Texture2D texture = new Texture2D(1, 1);
        texture.SetPixel(0, 0, color);
        texture.Apply();
        return texture;
    }
}
#endif
