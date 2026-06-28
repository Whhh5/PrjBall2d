using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEditor.VersionControl;
using System;

public class DebugWindow : EditorWindow
{
    private Dictionary<int, GameObject> _GameObjects = new();

    private List<int> _TempEntityID = new();
    private void OnGUI()
    {
        //if (GUILayout.Button("load"))
        //{
        //    LoadAsync();
        //}
        //if (GUILayout.Button("unload"))
        //{
        //    Unload();
        //}
        //if (GUILayout.Button("load - unload"))
        //{
        //    LoadAsync();
        //    Unload();
        //    LoadAsync();
        //    Unload();
        //}
        if (GUILayout.Button("test load"))
        {
            //AbbAssetbundleLoader.Instance.Test();
        }

        EditorGUILayout.BeginHorizontal();
        {
            if (GUILayout.Button($"Load", GUILayout.Width(100), GUILayout.Height(30)))
            {
                CreateScene();
            }
            if (GUILayout.Button($"Unload", GUILayout.Width(100), GUILayout.Height(30)))
            {
                ClearScene();
            }
            if (GUILayout.Button($"Unload -> Load", GUILayout.Width(100), GUILayout.Height(30)))
            {
                ClearScene();
                CreateScene();
            }
            if (GUILayout.Button($"Ab Log", GUILayout.Width(100), GUILayout.Height(30)))
            {
                //AbbAssetbundleLoader.Instance.Log();
            }
        }
        EditorGUILayout.EndHorizontal();
    }
    private void Load2()
    {
    }
    private void Unload2()
    {
    }
    private void CreatePlayer()
    {
    }
    private void DestroyPlayer()
    {
    }
    private void CreateScene()
    {
    }
    private void ClearScene()
    {
    }

    //private async void LoadAsync()
    //{
    //    //var assetID = 1;
    //    //var asset = await AbbAssetbundleLoader.Instance.PullObject<GameObject>(assetID);
    //    //Debug.Log(asset);
    //}
    //private void Unload()
    //{
    //    var assetID = 80;
    //    //AbbAssetbundleLoader.Instance.PushObject(in assetID);
    //}
}
