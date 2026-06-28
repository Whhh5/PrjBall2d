using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using HybridCLR.Editor;
using HybridCLR.Editor.Commands;
using UnityEditor;
using UnityEngine;
using static Codice.Client.Commands.WkTree.WorkspaceTreeNode;


[CustomUnityToolbarAttribute("Hotfix Player", EnCustomUnityToolbarType.Left, EnCustomUnityToolbarOrder.BuildBundle)]
public class HotfixPlayer : ICustomUnityToolbar
{
    public void OnClick()
    {
        
        var buildTarget = GlobalConfigEditorSO.Instance.BuildTarget;

        // 拷贝外部程序集
        BuildBundleToolbar.CopyExtendDlls(buildTarget);

        HybridCLR.Editor.Commands.CompileDllCommand.CompileDll(buildTarget);
        //HybridCLR.Editor.Commands.Il2CppDefGeneratorCommand.GenerateIl2CppDef();
        //HybridCLR.Editor.Commands.LinkGeneratorCommand.GenerateLinkXml(buildTarget);
        //HybridCLR.Editor.Commands.StripAOTDllCommand.GenerateStripedAOTDlls(buildTarget);
        //HybridCLR.Editor.Commands.MethodBridgeGeneratorCommand.GenerateMethodBridgeAndReversePInvokeWrapper(buildTarget);
        //HybridCLR.Editor.Commands.AOTReferenceGeneratorCommand.GenerateAOTGenericReference(buildTarget);

        // 拷贝 热更 dll
        BuildBundleToolbar.CopyHotfixDlls(buildTarget);

        // 拷贝 matadata dll
        BuildBundleToolbar.CopyStrippedDlls(buildTarget);

        // 生成 assetbundle
        AssetbundleToolEditor.BuildAssetbundle();


        Debug.Log($"热更完成");
    }
}

[CustomUnityToolbarAttribute("BuildPlayer", EnCustomUnityToolbarType.Left, EnCustomUnityToolbarOrder.BuildBundle)]
public class BuildBundleToolbar : ICustomUnityToolbar
{
    public void OnClick()
    {

        //var hotAssemblys = HybridCLR.Editor.SettingsUtil.HotUpdateAssemblyNamesExcludePreserved;

        //HybridCLR.Editor.Commands.MethodBridgeGeneratorCommand.GenerateMethodBridgeAndReversePInvokeWrapper();

        var buildTarget = GlobalConfigEditorSO.Instance.BuildTarget;

        // 拷贝外部程序集
        CopyExtendDlls(buildTarget);

        // 生成 热更 程序集
        CreateHotfixDlls(buildTarget);

        // 拷贝 热更 dll
        CopyHotfixDlls(buildTarget);

        // 拷贝 matadata dll
        CopyStrippedDlls(buildTarget);

        // 生成 assetbundle
        AssetbundleToolEditor.BuildAssetbundle();

        //// 开始打包
        //var buildDir = EditorUtil.GetBuildPlayerPath(buildTarget);
        //var buildPath = Path.Combine(buildDir, "play");
        //var buildOperation = BuildOptions.Development
        //    | BuildOptions.ConnectWithProfiler
        //    | BuildOptions.EnableDeepProfilingSupport
        //    | BuildOptions.AllowDebugging
        //    | BuildOptions.WaitForPlayerConnection;
        //UnityEditor.BuildPipeline.BuildPlayer(new string[]
        //{
        //    "Assets/Scenes/SampleScene.unity",
        //}, buildPath, buildTarget, buildOperation);

        //Debug.Log($"打包完成：{buildPath}");
        //EditorUtil.OpenFolder(buildDir);
    }
    private void CopyBuildPlayerLibly()
    {
        //HybridCLR.Editor.SettingsUtil.ProjectDir.
    }
    public static void CopyExtendDlls(BuildTarget buildTarget)
    {
        var names = HybridCLR.Editor.SettingsUtil.HotUpdateAssemblyNamesExcludePreserved;
        var dirs = new List<string>()
        {
            Path.Combine(ABBUtil.GetUnityRootPath(), "Assets/GameRes/HotfixRes/HotfixDlls")
        };
        //var dirs = HybridCLR.Editor.SettingsUtil.HybridCLRSettings.externalHotUpdateAssembliyDirs;
        var targetDir = Path.Combine(ABBUtil.GetUnityRootPath(), "HybridCLRData", "ExternalDlls", $"{buildTarget}");
        for (int i = 0; i < names.Count; i++)
        {
            var name = $"{names[i]}.dll";
            var sourceDirs = dirs.Where(dir =>
            {
                var path = Path.Combine(ABBUtil.GetUnityRootPath(), dir, name);
                return File.Exists(path);
            }).ToList();
            if (sourceDirs.Count > 1)
                throw new System.Exception($"存在多个相同的 dll dll: {name}");
            for (int j = 0; j < sourceDirs.Count; j++)
            {
                if (!Directory.Exists(targetDir))
                    Directory.CreateDirectory(targetDir);
                var curFile = Path.Combine(ABBUtil.GetUnityRootPath(), sourceDirs[j], name);
                var path = Path.Combine(ABBUtil.GetUnityRootPath(), targetDir, name);
                File.Copy(curFile, path, true);
            }
        }
    }
    public static void CreateHotfixDlls(BuildTarget buildTarget)
    {
        //HybridCLR.Editor.Commands.PrebuildCommand.GenerateAll();
        HybridCLR.Editor.Commands.CompileDllCommand.CompileDll(buildTarget);
        HybridCLR.Editor.Commands.Il2CppDefGeneratorCommand.GenerateIl2CppDef();
        HybridCLR.Editor.Commands.LinkGeneratorCommand.GenerateLinkXml(buildTarget);
        HybridCLR.Editor.Commands.StripAOTDllCommand.GenerateStripedAOTDlls(buildTarget);
        HybridCLR.Editor.Commands.MethodBridgeGeneratorCommand.GenerateMethodBridgeAndReversePInvokeWrapper(buildTarget);
        HybridCLR.Editor.Commands.AOTReferenceGeneratorCommand.GenerateAOTGenericReference(buildTarget);

    }
    public static void CopyStrippedDlls(BuildTarget buildTarget)
    {
        var dllMatadataFullPath = ABBUtil.GetFullPathByUnityPath(GlobalConfigSO.Instance.DllMatadataBytesDirectory);
        var strippedDllFullPath = ABBUtil.GetFullPathByUnityPath(Path.Combine(SettingsUtil.HybridCLRSettings.strippedAOTDllOutputRootDir, buildTarget.ToString()));
        for (int i = 0; i < AOTGenericReferences.PatchedAOTAssemblyList.Count; i++)
        {
            var name = AOTGenericReferences.PatchedAOTAssemblyList[i];
            var dllFullPath = Path.Combine(strippedDllFullPath, name);
            if (!File.Exists(dllFullPath))
            {
                Debug.LogError($"hotfix mata dll 不存在 {dllFullPath}");
                continue;
            }
            var targetPath = Path.Combine(dllMatadataFullPath, $"{name}.bytes");
            if (!Directory.Exists(dllMatadataFullPath))
                Directory.CreateDirectory(dllMatadataFullPath);
            File.Copy(dllFullPath, targetPath, true);
        }
    }
    public static void CopyHotfixDlls(BuildTarget buildTarget)
    {
        var dllBytesFullPath = ABBUtil.GetFullPathByUnityPath(GlobalConfigSO.Instance.DllBytesDirectory);
        var dllCompileFullPath = ABBUtil.GetFullPathByUnityPath(Path.Combine(SettingsUtil.HybridCLRSettings.hotUpdateDllCompileOutputRootDir, buildTarget.ToString()));

        for (int i = 0; i < SettingsUtil.HybridCLRSettings.hotUpdateAssemblyDefinitions.Length; i++)
        {
            var dll = SettingsUtil.HybridCLRSettings.hotUpdateAssemblyDefinitions[i];
            var dllName = $"{dll.name}.dll";
            var dllPath = Path.Combine(dllCompileFullPath, dllName);
            if (!File.Exists(dllPath))
            {
                Debug.LogError($"hotfix dll 不存在 {dllPath}");
                continue;
            }

            if (!Directory.Exists(dllBytesFullPath))
                Directory.CreateDirectory(dllBytesFullPath);
            var targetPath = Path.Combine(dllBytesFullPath, $"{dllName}.bytes");
            File.Copy(dllPath, targetPath, true);
        }

        for (int i = 0; i < SettingsUtil.HybridCLRSettings.hotUpdateAssemblies.Length; i++)
        {
            var dll = SettingsUtil.HybridCLRSettings.hotUpdateAssemblies[i];
            var dllName = $"{dll}.dll";

            var selectArr = SettingsUtil.HybridCLRSettings.externalHotUpdateAssembliyDirs.Where((dirRoot, index) =>
            {
                var dllPath = Path.Combine(dirRoot, dllName);
                var fullPath = ABBUtil.GetFullPathByUnityPath(dllPath);
                return File.Exists(fullPath);
            }).ToList();
            if (selectArr.Count == 0)
            {
                Debug.LogError($"hotfix dll 不存在 {dllName}");
                continue;
            }
            if (selectArr.Count > 1)
            {
                Debug.LogError($"hotfix dll 存在多个 不存在 {dllName}");
                continue;
            }
            for (int j = 0; j < selectArr.Count; j++)
            {
                var dllPath = Path.Combine(selectArr[i], dllName);
                var targetPath = Path.Combine(dllBytesFullPath, $"{dllName}.bytes");
                if (!Directory.Exists(dllBytesFullPath))
                    Directory.CreateDirectory(dllBytesFullPath);
                File.Copy(dllPath, targetPath, true);
            }
        }
    }
}
