using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.PackageManager.UI;
using UnityEditor.VersionControl;
using UnityEngine;


[ExecuteInEditMode]
public class SceneBuildManager : EditorWindow
{
    [MenuItem("Tools/SceneBuildManager")]
    public static void ShowWindow() { GetWindow(typeof(SceneBuildManager)); } //show window

    private List<string> SceneNamesList = new List<string>();

    void Awake()
    {
        string[] ScenesList = new string[] {};

        this.maxSize = new Vector2(500f, 400f);
        this.minSize = this.maxSize;

        FetchCurrentBuildScenesInfo();
    }
    Vector2 scrollPosition;
    List<SceneAsset> scenes = new List<SceneAsset>();

    private void OnGUI()
    {
        string sceneNamesString = String.Empty;

        foreach(string name in SceneNamesList)
        {
            sceneNamesString += name + "\n";
        }

        GUILayout.Label("Current scenes:");
        GUIStyle scrollViewStyle = GUIStyle.none;

        EditorGUILayout.BeginVertical();
        scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition, GUILayout.Width(500), GUILayout.Height(300));

        GUILayout.Label(sceneNamesString);

        EditorGUILayout.EndScrollView();
        EditorGUILayout.EndVertical();

        if(displayRemoveWarning) RemoveAllScenesWarning();

        if (GUILayout.Button("Clear Build Scenes")) { RemoveScenes(); } 
        if(scenes.Count > 0)
        {
            if (GUILayout.Button("Add Scenes")) { AddScenes(); }
        }
        else
        {
            GUILayout.Label("Select scenes to add in project window");
        }
        
    }
    void RemoveAllScenesWarning()
    {
        if (EditorUtility.DisplayDialog("Clear All Build Scenes",
                "Are you sure?", "Yes", "No"))
        {
            EditorBuildSettings.scenes = null;
            FetchCurrentBuildScenesInfo();
            displayRemoveWarning = false;
        }
        else
        {
            displayRemoveWarning = false;
        }
    }
    bool displayRemoveWarning = false;
    private void RemoveScenes()
    {
        displayRemoveWarning = true;
        Repaint();
    }
    private void AddScenes()
    {
        List<EditorBuildSettingsScene> prewBuildScenesSettingsList = new List<EditorBuildSettingsScene>();
        foreach(EditorBuildSettingsScene edBuildScene in EditorBuildSettings.scenes)
            prewBuildScenesSettingsList.Add(edBuildScene);

        foreach (SceneAsset scene in scenes)
        {
            string scenePath = AssetDatabase.GetAssetOrScenePath(scene);
            if(!SceneNamesList.Contains(scenePath))
            {
                SceneNamesList.Add(scenePath);
                EditorBuildSettingsScene _buildSettingsScene = new EditorBuildSettingsScene();
                _buildSettingsScene.enabled = true;
                _buildSettingsScene.path = scenePath;

                prewBuildScenesSettingsList.Add(_buildSettingsScene);

                EditorBuildSettings.scenes = prewBuildScenesSettingsList.ToArray();
            }
        }
        Repaint();
    }
    private void FetchCurrentBuildScenesInfo()
    {
        EditorBuildSettingsScene[] scenesList = EditorBuildSettings.scenes;

        SceneNamesList.Clear();
        foreach (EditorBuildSettingsScene scene in scenesList)
        {
            SceneNamesList.Add(scene.path);
        }
    }
    void OnSelectionChange()
    {
        UnityEngine.Object[] selectedObjects = Selection.objects;
        scenes.Clear();
        foreach (UnityEngine.Object selectedObject in selectedObjects)
        {
            if(selectedObject.GetType().Name == "SceneAsset")
            {
                scenes.Add(selectedObject as SceneAsset);
            }
        }
        Repaint();
    }
}
