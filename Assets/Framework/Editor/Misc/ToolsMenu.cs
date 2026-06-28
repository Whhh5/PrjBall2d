using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.Profiling;

public class ToolsMenu
{
    [MenuItem("Tools/Debug Window", priority = 101)]
    public static void OpenWindow_Debug()
    {
        var window = EditorWindow.GetWindow<DebugWindow>();
        window.Show();
    }
    [UnityEditor.MenuItem("Tools/Mat", priority = 301)]
    public static void ExecuteMatToUrp()
    {
        var shader = Shader.Find("Universal Render Pipeline/Lit");
        if (shader == null)
            return;
        for (int i = 0; i < Selection.objects.Length; i++)
        {
            var obj = Selection.objects[i];
            var coms = obj as Material;
            if (coms == null)
                continue;
            if (coms.shader.name != $"Standard")
                continue;
            var mainTxt = coms.mainTexture;
            coms.shader = shader;
            coms.mainTexture = mainTxt;
        }
    }
    [UnityEditor.MenuItem("Tools/Mat Floder", priority = 302)] 
    public static void ExecuteFloderMatToUrp()
    {
        var shader = Shader.Find("Universal Render Pipeline/Lit");
        if (shader == null)
            return;
        var selectedObjects = Selection.GetFiltered(typeof(Object), SelectionMode.Assets);
        foreach (Object selectedObject in selectedObjects)
        {
            // 获取选中对象的路径
            string path = AssetDatabase.GetAssetPath(selectedObject);
            if (!AssetDatabase.IsValidFolder(path))
                return;
            var mats = AssetDatabase.FindAssets("t:material", new string[] { path });
            for (int i = 0; i < mats.Length; i++)
            {
                var guid = AssetDatabase.GUIDToAssetPath(mats[i]);
                var coms = AssetDatabase.LoadAssetAtPath<Material>(guid);
                if (coms.shader.name != $"Standard")
                    continue;
                var mainTxt = coms.mainTexture;
                coms.shader = shader;
                coms.mainTexture = mainTxt;
            }
        }
    }
    [MenuItem("Tools/Clean Editor Memory", priority = 201)]
    public static void CleanEditorMemory()
    {
        // 获取总内存使用量
        long totalMemory = Profiler.GetTotalReservedMemoryLong();
        // 获取已使用的堆内存
        long usedHeapMemory = Profiler.GetTotalAllocatedMemoryLong();
        // 获取未使用的堆内存
        long unusedHeapMemory = Profiler.GetTotalUnusedReservedMemoryLong();

        ExcelEditorUtil.ClearCache();
        AssetDatabase.Refresh();
        Caching.ClearCache();
        // 立即卸载未使用的资源
        EditorUtility.UnloadUnusedAssetsImmediate();
        // 强制进行垃圾回收
        System.GC.Collect();

        var usedHeapMemory2 = Profiler.GetTotalAllocatedMemoryLong();
        long unusedHeapMemory2 = Profiler.GetTotalUnusedReservedMemoryLong();
        var clearValue = (float)(usedHeapMemory - usedHeapMemory2) / 1024 / 1024;
        var unusedValue = (float)unusedHeapMemory2 / 1024 / 1024;
        var totalValue = (float)totalMemory / 1024 / 1024;
        Debug.LogWarning($"Editor memory has been cleaned. {clearValue:0.0}MB, {unusedValue:0}/{totalValue:0}MB");
    }
}
