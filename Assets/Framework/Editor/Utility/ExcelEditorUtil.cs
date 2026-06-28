using UnityEngine;
using OfficeOpenXml;
using System.IO;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using Newtonsoft.Json;
using System;

public static class ExcelEditorUtil
{
    public static bool IsValid(this ExcelWorksheet sheet, int row)
    {
        var value = sheet.GetValue<string>(row, 1);
        var isvalid = string.IsNullOrWhiteSpace(value);
        return !isvalid;
    }
    private static Dictionary<Type, object> _EditorCfgData = new();
    public static void ClearCache()
    {
        _EditorCfgData.Clear();
    }
    public static void ClearCache<TCfg>()
        where TCfg : ICfg
    {
        _EditorCfgData.Remove(typeof(TCfg));
    }
    public static int GetCfgCount<TCfg>()
        where TCfg : ICfg
    {
        var cfgList = ReadEditorCfgList<TCfg>();
        return cfgList.Count;
    }
    public static TCfg GetCfgByIndex<TCfg>(int index)
        where TCfg : ICfg
    {
        var list = ReadEditorCfgList<TCfg>();
        var item = list[index];
        return item;
    }
    public static bool Contains<TCfg>(int id)
        where TCfg : ICfg
    {
        var cfg = GetCfg<TCfg>(id);
        return cfg != null;
    }
    public static TCfg GetCfg<TCfg>(int id)
        where TCfg : ICfg
    {
        var list = ReadEditorCfgList<TCfg>();
        var item = list.Find(item => item.GetID() == id);
        return item;
    }
    public static string GetExcelPath<T>()
        where T : ICfg
    {
        var excelName = typeof(T).Name;
        var files = Directory.GetFiles(Path.Combine(ABBUtil.GetUnityMiscPath(), "Excel"), $"*.xlsx", SearchOption.AllDirectories);
        for (int i = 0; i < files.Length; i++)
        {
            var fileInfo = new FileInfo(files[i]);
            if(fileInfo.Name.StartsWith(excelName))
                return fileInfo.FullName;
        }
        return null;
    }
    public static ExcelPackage ReadExcel<T>()
        where T : ICfg
    {
        var path = GetExcelPath<T>();
        var fileInfo = new FileInfo(path);
        var excelPackage = new ExcelPackage(fileInfo);
        return excelPackage;
    }

    public static void SetCfgValue(ICfg cfg, string fieldName, object value)
    {
        var strValue = $"{value}";
        var fieldType = cfg.GetType().GetField(fieldName, (BindingFlags)int.MaxValue);
        if (fieldType.FieldType.IsValueType && string.IsNullOrWhiteSpace(strValue))
            return;
        if (fieldType == null)
            return;
        // var jsonStr = fieldType.FieldType == typeof(string) ? strValue : JsonConvert.DeserializeObject(strValue, fieldType.FieldType);
        var fieldValue = Convert.ChangeType(value, fieldType.FieldType);
        fieldType.SetValue(cfg, fieldValue);
    }
    public static void AddCfg<TCfg>(TCfg cfg)
        where TCfg : ICfg
    {
        var cfgList = ReadEditorCfgList<TCfg>();
        cfgList.Add(cfg);
    }
    private static List<T> ReadEditorCfgList<T>()
        where T : ICfg
    {
        if (!_EditorCfgData.TryGetValue(typeof(T), out var cfgList))
        {
            cfgList = ReadEditorCfgData<T>();
            _EditorCfgData.Add(typeof(T), cfgList);
        }
        return cfgList as List<T>;
    }

    public static int GetNextIndex<TCfg>()
        where TCfg : ICfg
    {
        var count = ExcelEditorUtil.GetCfgCount<TCfg>();
        for (int i = 0; i < count; i++)
        {
            if (!ExcelEditorUtil.Contains<TCfg>(i + 1))
                return i + 1;
        }
        return count + 1;
    }
    private static List<T> ReadEditorCfgData<T>()
        where T : ICfg
    {
        var result = ReadEditorCfgData<T, T>();
        return result;
    }
    private static List<T2> ReadEditorCfgData<T, T2>()
        where T : ICfg
        where T2 : ICfg
    {
        var result = new List<T2>();
        var excelPackage = ExcelEditorUtil.ReadExcel<T>();
        var workSheet = excelPackage.Workbook.Worksheets[1];
        var skillCatalog = ReadExportExcelInfo<T>();
        var skillEditorType = typeof(T2);
        var fields = skillEditorType.GetFields(BindingFlags.Public | BindingFlags.Instance);
        for (int i = skillCatalog.excelInfo.dataStartRow; i <= workSheet.Dimension.End.Row; i++)
        {
            if (!workSheet.IsValid(i))
                continue;

            // 类型转换
            T2 insSkillCfgEditor = CreateTypeInstance<T2>();

            //var insSkillCfgEditor = Activator.CreateInstance<T2>();
            for (int j = 0; j < fields.Length; j++)
            {
                var field = fields[j];
                if (!skillCatalog.field2ColList.TryGetValue(field.Name, out var col))
                    goto next;
                var excelStr = workSheet.GetValue<string>(i, col) ?? "";
                if (!(field.FieldType.IsValueType && string.IsNullOrWhiteSpace(excelStr)))
                {
                    var jsonStr = field.FieldType == typeof(string) ? excelStr : JsonConvert.DeserializeObject(excelStr, field.FieldType);
                    var value = Convert.ChangeType(jsonStr, field.FieldType);
                    field.SetValue(insSkillCfgEditor, value);
                }
            }
            result.Add(insSkillCfgEditor);
        next:;
        }
        return result;
    }
    public static T2 CreateTypeInstance<T2>()
        where T2 : ICfg
    {
        // 获取 PrivateConstructorClass 的 Type 对象
        Type type = typeof(T2);
        // 获取指定参数类型的私有构造函数
        ConstructorInfo constructor = type.GetConstructor(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance, null, new Type[0], null);
        // 使用构造函数创建实例
        object instance = constructor.Invoke(new object[] { });
        // 类型转换
        T2 insSkillCfgEditor = (T2)instance;
        return insSkillCfgEditor;
    }
    public static object ReadCfg(string fileName, Type type)
    {
        var path = GameSchedule.GetCfgPath(fileName);
        var ass = UnityEditor.AssetDatabase.LoadAssetAtPath<TextAsset>(path);
        var arrType = Array.CreateInstance(type, 0).GetType();

        var cfg = JsonConvert.DeserializeObject(ass.text, arrType);
        return cfg;
    }
    public static ExportExcelInfo ReadExportExcelInfo<T>()
        where T : ICfg
    {
        var cfgName = typeof(T).Name;
        var catalogObj = ReadCfg("CfgCatalog", typeof(ExportExcelInfo));
        var catalogList = catalogObj as ExportExcelInfo[];
        var skillCatalog = Array.Find<ExportExcelInfo>(catalogList, (item) => item.excelInfo.excelName == cfgName);
        return skillCatalog;
    }
    public static void SaveExcel<T>(Dictionary<string, string>[] data)
        where T : ICfg
    {
        var excelPackage = ExcelEditorUtil.ReadExcel<T>();
        var workSheet = excelPackage.Workbook.Worksheets[1];
        var skillCatalog = ReadExportExcelInfo<T>();
        var startRowIndex = skillCatalog.excelInfo.dataStartRow;
        var endRowIndex = startRowIndex + data.Length;

        for (int i = 0; i < data.Length; i++)
        {
            var rowDatas = data[i];
            var curRow = startRowIndex + i;
            workSheet.SetValue(curRow, 1, "*");
            workSheet.Cells[curRow, 1].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
            foreach (var rowData in rowDatas)
            {
                var fieldName = rowData.Key;
                if (!skillCatalog.field2ColList.TryGetValue(fieldName, out var col))
                    continue;
                workSheet.SetValue(curRow, col, rowData.Value);
            }
        }

        for (int i = endRowIndex; i <= workSheet.Dimension.End.Row; i++)
            for (int j = 1; j <= workSheet.Dimension.End.Column; j++)
                workSheet.SetValue(i, j, null);

        excelPackage.Save();

        var result = EditorUtility.DisplayDialog($"{typeof(T)}", "success", "ok", "open");
        if (!result)
            excelPackage.File.OpenFile();

        excelPackage.Dispose();
    }
    public static void SaveExcel<T>()
        where T : ICfg
    {
        var cfgList = ReadEditorCfgList<T>();
        cfgList.Sort((item1, item2) => item1.GetID() < item2.GetID() ? -1 : 1);
        SaveExcel<T, T>(cfgList);
    }
    public static void SaveExcel<T, T2>(List<T2> data)
        where T : ICfg
        where T2 : ICfg
    {
        var excelPackage = ExcelEditorUtil.ReadExcel<T>();
        var workSheet = excelPackage.Workbook.Worksheets[1];
        var skillCatalog = ReadExportExcelInfo<T>();
        var startRowIndex = skillCatalog.excelInfo.dataStartRow;
        var endRowIndex = startRowIndex + data.Count;

        var skillEditorType = typeof(T2);
        var fields = skillEditorType.GetFields(BindingFlags.Public | BindingFlags.Instance);

        for (int i = 0; i < data.Count; i++)
        {
            var rowDatas = data[i];
            var curRow = startRowIndex + i;
            workSheet.SetValue(curRow, 1, "*");
            workSheet.Cells[curRow, 1].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;


            for (int k = 0; k < fields.Length; k++)
            {
                var field = fields[k];
                if (!skillCatalog.field2ColList.TryGetValue(field.Name, out var col))
                    continue;
                var value = field.GetValue(rowDatas) ?? (field.FieldType.IsValueType ? Activator.CreateInstance(field.FieldType) : null);
                var jsonStr = field.FieldType == typeof(string) ? value : JsonConvert.SerializeObject(value);
                workSheet.SetValue(curRow, col, jsonStr);
            }
        }

        for (int i = endRowIndex; i <= workSheet.Dimension.End.Row; i++)
            for (int j = 1; j <= workSheet.Dimension.End.Column; j++)
                workSheet.SetValue(i, j, null);

        excelPackage.Save();

        var result = EditorUtility.DisplayDialog($"{typeof(T)}", "success", "ok", "open");
        if (!result)
            excelPackage.File.OpenFile();

        excelPackage.Dispose();
    }
}
