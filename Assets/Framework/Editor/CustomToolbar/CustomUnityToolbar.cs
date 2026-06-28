
using System.Reflection;
using UnityEditor;
using UnityEngine;
using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using UnityEngine.UIElements;
using Debug = UnityEngine.Debug;
using HybridCLR.Editor;
using System.Linq;
using System.Collections.Generic;
using static Codice.CM.WorkspaceServer.WorkspaceTreeDataStore;

public enum EnCustomUnityToolbarType
{
    Left,
    Right,
}
public enum EnCustomUnityToolbarOrder
{
    None,
    UpdateLoadTarget,
    ExportExcel,
    BuildAssetbundle,
    BuildBundle,
    ResourcePattern,

    ClearCache,
    OpenPersistentDataRoot,
    TestBtn1,
}
public class CustomUnityToolbarAttribute : Attribute
{
    public Type target;
    public EnCustomUnityToolbarType toolbarType;
    public string btnName;
    public EnCustomUnityToolbarOrder order;
    public CustomUnityToolbarAttribute(string btnName, EnCustomUnityToolbarType toolbarType, EnCustomUnityToolbarOrder order)
    {
        this.toolbarType = toolbarType;
        this.btnName = btnName;
        this.order = order;
    }
    private CustomUnityToolbarAttribute()
    { }
}
public interface ICustomUnityToolbar
{
    public void OnClick();
}
public interface ICustomUnityToolbarBtnName
{
    public string GetBtnName();
}
/// <summary>
/// 扩展Unity的按钮栏
/// </summary>
[InitializeOnLoad]
public static class CustomUnityToolbar
{
    public static Type m_toolbarType = typeof(Editor).Assembly.GetType("UnityEditor.Toolbar");
    public static ScriptableObject m_currentToolbar;

    private class ItemInfo
    {
        public string btnName;
        public ICustomUnityToolbar instace;
        public EnCustomUnityToolbarOrder order;
    }

    private static Dictionary<EnCustomUnityToolbarType, List<ItemInfo>> _BtnList = new();

    static CustomUnityToolbar()
    {
        EditorApplication.delayCall += OnUpdate;


        var arrTypes = typeof(CustomUnityToolbar).Assembly.GetTypes();
        for (int i = 0; i < arrTypes.Count(); i++)
        {
            var type = arrTypes[i];
            var att = type.GetCustomAttribute<CustomUnityToolbarAttribute>();
            if (att == null)
                continue;
            if (!_BtnList.TryGetValue(att.toolbarType, out var list))
            {
                list = new();
                _BtnList.Add(att.toolbarType, list);
            }
            if (!typeof(ICustomUnityToolbar).IsAssignableFrom(type))
            {
                Debug.LogError($"{type} 添加 {typeof(CustomUnityToolbarAttribute)} 特性的类要实现 {typeof(ICustomUnityToolbar)} 接口)");
                continue;
            }
            list.Add(new()
            {
                btnName = att.btnName,
                instace = Activator.CreateInstance(type) as ICustomUnityToolbar,
                order = att.order,
            });
        }
        foreach (var item in _BtnList)
        {
            item.Value.Sort((item1, item2) =>
            {
                if (item1.order < item2.order)
                    return -1;
                if (item1.order > item2.order)
                    return 1;
                return 0;
            });
        }
    }

    public static void OnUpdate()
    {
        // Relying on the fact that toolbar is ScriptableObject and gets deleted when layout changes
        if (m_currentToolbar == null)
        {
            // Find toolbar
            var toolbars = Resources.FindObjectsOfTypeAll(m_toolbarType);
            m_currentToolbar = toolbars.Length > 0 ? (ScriptableObject)toolbars[0] : null;

            if (m_currentToolbar != null)
            {
                var root = m_currentToolbar.GetType().GetField("m_Root", BindingFlags.NonPublic | BindingFlags.Instance);
                var rawRoot = root.GetValue(m_currentToolbar);
                var mRoot = rawRoot as VisualElement;
                RegisterCallback("ToolbarZoneLeftAlign", FlexDirection.Row, () => GUIToolbar(EnCustomUnityToolbarType.Left));
                RegisterCallback("ToolbarZoneRightAlign", FlexDirection.RowReverse, () => GUIToolbar(EnCustomUnityToolbarType.Right));

                void RegisterCallback(string root, FlexDirection dir, Action cb)
                {
                    var toolbarZone = mRoot.Q(root);

                    var parent = new VisualElement()
                    {
                        style = {
                            flexGrow = 1,
                            flexDirection = dir,
                        }
                    };
                    var container = new IMGUIContainer();
                    container.onGUIHandler += () =>
                    {
                        cb?.Invoke();
                    };
                    parent.Add(container);
                    toolbarZone.Add(parent);
                }
            }
        }

    }

    //private static BaseType baseType = new();
    /// <summary>
    /// 绘制左侧的元素
    /// </summary>
    private static void GUIToolbar(EnCustomUnityToolbarType barType)
    {
        if (!_BtnList.TryGetValue(barType, out var list))
            return;

        GUILayout.BeginHorizontal();
        GUILayout.Space(100);
        for (int i = 0; i < list.Count; i++)
        {
            var item = list[i];
            var name = item.instace is ICustomUnityToolbarBtnName btnName ? btnName.GetBtnName() : item.btnName;
            if (GUILayout.Button(name))
            {
                item.instace.OnClick();
            }
        }
        GUILayout.Space(100);
        GUILayout.EndHorizontal();
    }

    private static readonly string _BuffClassPath = "Assets/AbbFramework/Scripts/EntityBuff";
    private static readonly string _BuffEnumName = "EnBuff";
    private static readonly string _BuffTypeEnumName = "EnBuffType";

    private static string GetBuffClassName(string buffName)
    {
        return $"Entity{buffName}BuffData";
    }
    private static void UpdateBuff()
    {
        var unityPath = ABBUtil.GetUnityRootPath();
        var fileRootPath = Path.Combine(unityPath, _BuffClassPath);
        if (!Directory.Exists(fileRootPath))
            Directory.CreateDirectory(fileRootPath);


        CreateBuffData(in fileRootPath);
        CreateBuffUtil(in fileRootPath);
        CreateBuffTypeEnum(in fileRootPath);
        CreateBuffEnum(in fileRootPath);

        AssetDatabase.Refresh();
    }
    private static void CreateBuffTypeEnum(in string rootPath)
    {
        var filePath = Path.Combine(rootPath, $"{_BuffTypeEnumName}.cs");
        var content = new StringBuilder();

        content.AppendLine($"public enum {_BuffTypeEnumName}");
        content.AppendLine($"{{");

        content.AppendLine($"\tNone = 0,");
        var buffCount = ExcelEditorUtil.GetCfgCount<BuffTypeCfg>();
        for (int i = 0; i < buffCount; i++)
        {
            var buffTypeCfg = ExcelEditorUtil.GetCfgByIndex<BuffTypeCfg>(i);
            content.AppendLine($"\t[{typeof(EditorFieldNameAttribute)}(\"{buffTypeCfg.strDescEditor}\")]");
            content.AppendLine($"\t{buffTypeCfg.strEnumNameEditor} = {buffTypeCfg.nTypeID},");
        }

        content.AppendLine($"}}");
        File.WriteAllText(filePath, content.ToString());
    }
    private static void CreateBuffEnum(in string rootPath)
    {
        var filePath = Path.Combine(rootPath, $"{_BuffEnumName}.cs");
        var content = new StringBuilder();

        content.AppendLine($"public enum {_BuffEnumName}");
        content.AppendLine($"{{");

        content.AppendLine($"\tNone = 0,");
        var buffCount = ExcelEditorUtil.GetCfgCount<BuffCfg>();
        for (int i = 0; i < buffCount; i++)
        {
            var buffCfg = ExcelEditorUtil.GetCfgByIndex<BuffCfg>(i);
            content.AppendLine($"\t[{typeof(EditorFieldNameAttribute)}(\"{buffCfg.strDescEditor}\")]");
            content.AppendLine($"\t{buffCfg.strEnumNameEditor} = {buffCfg.nBuffID},");
        }

        content.AppendLine($"}}");
        File.WriteAllText(filePath, content.ToString());
    }
    private static void CreateBuffUtil(in string rootPath)
    {
        var filePath = Path.Combine(rootPath, $"BuffUtil.cs");

        var content = new StringBuilder();

        content.AppendLine("public partial class BuffUtil");
        content.AppendLine("{");


        content.AppendLine($"\tpublic static {typeof(ISkillEntityBuff)} CreateBuffData({_BuffEnumName} buff, {typeof(IClassPoolUserData)} data)");
        content.AppendLine("\t{");
        content.AppendLine($"\t\treturn buff switch");
        content.AppendLine($"\t\t{{");
        var buffCount = ExcelEditorUtil.GetCfgCount<BuffCfg>();
        for (int i = 0; i < buffCount; i++)
        {
            var buffCfg = ExcelEditorUtil.GetCfgByIndex<BuffCfg>(i);
            var className = GetBuffClassName(buffCfg.strEnumNameEditor);
            content.AppendLine($"\t\t\t{_BuffEnumName}.{buffCfg.strEnumNameEditor} => {typeof(ClassPoolMgr)}.Instance.Pull<{className}>(data),");
        }
        content.AppendLine($"\t\t\t_ => default,");
        content.AppendLine($"\t\t}};");
        content.AppendLine("\t}");


        content.AppendLine("}");


        File.WriteAllText(filePath, content.ToString());
    }
    private static void CreateBuffData(in string rootPath)
    {
        var count = ExcelEditorUtil.GetCfgCount<BuffCfg>();
        for (int i = 0; i < count; i++)
        {
            var buffCfg = ExcelEditorUtil.GetCfgByIndex<BuffCfg>(i);
            var className = GetBuffClassName(buffCfg.strEnumNameEditor);
            var filePath = Path.Combine(rootPath, $"{className}.cs");
            if (File.Exists(filePath))
                continue;
            var fileContent = new StringBuilder();
            var dataName = $"{className}Data";
            fileContent.AppendLine("using UnityEngine;");

            fileContent.AppendLine($"public class {dataName} : {nameof(IEntityBuffParams)}");
            fileContent.AppendLine($"public void {nameof(IEntityBuffParams.OnPoolDestroy)}()");
            fileContent.AppendLine($"{{");
            fileContent.AppendLine($"}}");

            fileContent.AppendLine($"public void {nameof(IEntityBuffParams.DeserializeFromIntArray)}({nameof(ISerializeToIntArrayUserData)} userData)");
            fileContent.AppendLine($"{{");
            fileContent.AppendLine($"    {nameof(IEntityBuffParams.DeserializeFromIntArray)}(userData.{nameof(ISerializeToIntArrayUserData.arrParams)}, userData.{nameof(ISerializeToIntArrayUserData.startIndex)}, userData.{nameof(ISerializeToIntArrayUserData.count)}, userData);");
            fileContent.AppendLine($"}}");

            fileContent.AppendLine($"public EnBuff GetBuff() => EnBuff.NoMovement;");
            fileContent.AppendLine($"public EnTypeId {nameof(IEntityBuffParams.GetTypeDefineId)}() => {nameof(EnTypeId)}.{dataName};");

            fileContent.AppendLine($"public List<int> {nameof(IEntityBuffParams.SerializeToInt)}()");
            fileContent.AppendLine($"    return null;");
            fileContent.AppendLine($"}}");
            fileContent.AppendLine($"public void {nameof(IEntityBuffParams.DeserializeFromIntArray)}(int[] datas, int start, int count, {nameof(ISerializeToIntArrayUserData)} userData)");
            fileContent.AppendLine($"}}");

            fileContent.AppendLine($"}}");
            fileContent.AppendLine($"public class {className} : {typeof(SkillEntityBuff)}<{dataName}>");
            fileContent.AppendLine("{");
            fileContent.AppendLine("\t");
            fileContent.AppendLine("}");
            File.WriteAllText(filePath, fileContent.ToString(), Encoding.UTF8);
        }
    }
}