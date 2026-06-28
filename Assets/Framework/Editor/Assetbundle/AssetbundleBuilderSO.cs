using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "SO/AbBuilderConfig", fileName = "AbBuilderConfig")]
public class AssetbundleBuilderSO : ScriptableObject
{
#if UNITY_EDITOR
    private static AssetbundleBuilderSO _Instance = null;
    private static HashSet<string> _TypeHash = null;
    public static AssetbundleBuilderSO Instance
    {
        get
        {
            if (_Instance != null)
                return _Instance;
            var assetPD = AssetbundleUtil.GetAbSoPath();
            _Instance = UnityEditor.AssetDatabase.LoadAssetAtPath<AssetbundleBuilderSO>(assetPD);
            _TypeHash = new(_Instance.hotTypes.Length);
            for (int i = 0; i < _Instance.hotTypes.Length; i++)
            {
                _TypeHash.Add(_Instance.hotTypes[i].ToLower());
            }
            return _Instance;
        }
    }
    public bool IsHotType(string suffix)
    {
        return _TypeHash.Contains(suffix.ToLower());
    }
#endif

    public string[] abPaths;
    public string[] hotTypes;
}
