using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using UnityEditor;
using UnityEngine;

[CustomUnityToolbarAttribute("ExportExcel", EnCustomUnityToolbarType.Left, EnCustomUnityToolbarOrder.ExportExcel)]
public class ExportExcelToolbar : ICustomUnityToolbar
{
    public void OnClick()
    {
        string[] arrParams = new string[]
            {
                Path.Combine(ABBUtil.GetUnityMiscPath(), "Excel"),
                Path.Combine(ABBUtil.GetDataPath(), "GameRes/HotfixRes", "GameCfgJson"),
                Path.Combine(ABBUtil.GetDataPath(), "Framework/Hotfix/Scripts", "GameCfgCS"),
            };
        var paramsStr = "";
        for (int i = 0; i < arrParams.Length; i++)
        {
            paramsStr += arrParams[i] + " ";
        }
        var pro2 = new ProcessStartInfo()
        { 
#if UNITY_STANDALONE_OSX
            FileName = Path.Combine(ABBUtil.GetUnityRootPath(), GlobalConfigEditorSO.Instance.MacExcelToolsPath),
#else
            FileName = Path.Combine(ABBUtil.GetUnityRootPath(), GlobalConfigEditorSO.Instance.ExcelToolsPath),
#endif
            RedirectStandardOutput = true, // 重定向标准输出
            UseShellExecute = false, // 不使用系统外壳程序启动
            CreateNoWindow = true, // 不创建新窗口
            Arguments = paramsStr,
            ErrorDialog = true,
            //StandardErrorEncoding = System.Text.Encoding.UTF8,
            RedirectStandardError = true,
            StandardOutputEncoding = System.Text.Encoding.UTF8,
            //RedirectStandardInput = true,
        };
        pro2.UseShellExecute = false;
        using (var process = Process.Start(pro2))
        {
            process.OutputDataReceived += (item, e) =>
            {
                if (string.IsNullOrEmpty(e.Data))
                    return;
                UnityEngine.Debug.Log($"{item},{e.Data}"); // 打印结果
            };
            process.ErrorDataReceived += (item, e) =>
            {
                if (string.IsNullOrEmpty(e.Data))
                    return;
                UnityEngine.Debug.LogError($"{item},{e.Data}"); // 打印结果
            };

            process.BeginOutputReadLine();

            process.BeginErrorReadLine();

            process.WaitForExit();
            UnityEngine.Debug.Log($"Export Excel Finish"); // 打印结果
        }
        AssetDatabase.Refresh();
    }

}
