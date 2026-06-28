using UnityEditor;
using UnityEngine;

//[CreateAssetMenu(menuName = "SO/GlobalConfigEditorSO", fileName = "GlobalConfigEditorSO")]
public class GlobalConfigEditorSO // : ScriptableObject
{
    public static GlobalConfigEditorSO Instance = new();

    public BuildTarget BuildTarget => EditorUserBuildSettings.activeBuildTarget;
    [Header("打表工具路径")]
    public string ExcelToolsPath = $"Misc/ExcelTools/ExcelTools/bin/Debug/net6.0/ExcelTools.exe";
    [Header("Mac打表工具路径")]
    public string MacExcelToolsPath = $"Misc/ExcelTools/ExcelTools/bin/Debug/net6.0/ExcelTools";
}
