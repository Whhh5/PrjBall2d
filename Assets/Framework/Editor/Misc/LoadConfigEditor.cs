using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using Unity.Plastic.Newtonsoft.Json;
using System;
using System.Linq;
using System.Reflection;
using System.Text;

public class LoadConfigEditor
{
    private static Dictionary<string, string> m_Suffix2NamePrefix = new()
    {
        {".asset", "SO_" },
        {".prefab", "Pre_" },
        {".png", "Sp_" },
        {".FBX", "Anim_" },
        {".anim", "Anim_" },
        {".mask", "AvaMask_" },
        {".json", "Json_" },
        {".txt", "Txt_" },
        {".bytes", "Byte_" },
    };
    private static string[] m_SearchPaths = new string[]
    {
        "Assets/GameRes/HotfixRes",
        "Assets/Framework/Hotfix/Res",
        "Assets/Framework/Editor/EditorRes",
    };
    public static string GetLoadTargetName(string assetName)
    {
        var fix = Path.GetExtension(assetName);
        var name = Path.GetFileNameWithoutExtension(assetName);
        if (!m_Suffix2NamePrefix.TryGetValue(fix, out var value))
            return null;
        var result = $"{value}{name}"
            .Replace(".", "_")
            .Replace(" ", "")
            .Replace("-", "_")
            .Replace(")", "")
            .Replace("(", "");
        return result;
    }
    private static string m_ConfigJsonPath => ABBUtil.GetFullPathByUnityPath(GlobalConfig.LoadConfigRecordsJson);
    private static string m_EnLoadTargetPath => ABBUtil.GetFullPathByUnityPath(GlobalConfig.LoadTargetEnum);
    [MenuItem("Tools/Load/UpdateConfigJson")]
    public static void CreateLoadConfigJson()
    {
        var curAssetDic = new Dictionary<string, AssetCfg>();
        var assetCfgCount = ExcelEditorUtil.GetCfgCount<AssetCfg>();
        for (int i = 0; i < assetCfgCount; i++)
        {
            var cfg = ExcelEditorUtil.GetCfgByIndex<AssetCfg>(i);
            curAssetDic.Add(cfg.strDescEditor, cfg);
        }


        foreach (var item in m_Suffix2NamePrefix)
        {
            foreach (var searchPath in m_SearchPaths)
            {
                var fullDir = ABBUtil.GetFullPathByUnityPath(searchPath);
                var dirInfo = new DirectoryInfo(fullDir);
                var fileList = dirInfo.GetFiles($"*{item.Key}", SearchOption.AllDirectories);
                foreach (var fileInfo in fileList)
                {
                    var fullPath = fileInfo.FullName;
                    var strPath = ABBUtil.GetUnityPathByFullPath(fullPath);
                    var strDescEditor = GetLoadTargetName(fullPath);

                    if (string.IsNullOrEmpty(strDescEditor))
                        continue;
                    if (!curAssetDic.TryGetValue(strDescEditor, out var cfg))
                    {
                        cfg = ExcelEditorUtil.CreateTypeInstance<AssetCfg>();
                        var assetID = ExcelEditorUtil.GetNextIndex<AssetCfg>();
                        ExcelEditorUtil.SetCfgValue(cfg, nameof(cfg.strDescEditor), strDescEditor);
                        ExcelEditorUtil.SetCfgValue(cfg, nameof(cfg.nAssetID), assetID);
                        curAssetDic.Add(strDescEditor, cfg);
                        ExcelEditorUtil.AddCfg(cfg);
                    }
                    if (cfg.strPath != strPath)
                        ExcelEditorUtil.SetCfgValue(cfg, nameof(cfg.strPath), strPath);
                }
            }
        }

        ExcelEditorUtil.SaveExcel<AssetCfg>();
        WriteEnLoadTargetFile();
    }

    private static void WriteEnLoadTargetFile()
    {
        var enumStr = new StringBuilder();
        enumStr.AppendLine($"public enum EnLoadTarget");
        enumStr.AppendLine($"{{");
        enumStr.AppendLine($"\tNone = 0,");


        var count = ExcelEditorUtil.GetCfgCount<AssetCfg>();
        for (int i = 0; i < count; i++)
        {
            var cfg = ExcelEditorUtil.GetCfgByIndex<AssetCfg>(i);

            enumStr.AppendLine($"\t{cfg.strDescEditor} = {cfg.nAssetID}, // {cfg.strPath}");
        }

        enumStr.AppendLine("}");
        File.WriteAllText(m_EnLoadTargetPath, enumStr.ToString());
        AssetDatabase.Refresh();
    }
}

