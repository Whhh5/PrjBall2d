using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using Unity.Collections.LowLevel.Unsafe;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using static UnityEditor.GenericMenu;

[Flags]
public enum EnMouseControllerType
{
    None,
    SliderBox = 1 << 0,
    StageScheduleBoxSlider = 1 << 1,
    StageInfoSlider_AtkEndTime = 1 << 2,
    StageInfoSlider_CanNextTime = 1 << 3,

    Ai_ViewMove = 1 << 4,
}

[Flags]
public enum EnEditorGlobalController
{
    EnumCount = -100,
    None = 0,

    [EditorFieldName("点击"), Keyboard(KeyCode.Q)]
    Click = 1 << 0,

    [EditorFieldName("移动"), Keyboard(KeyCode.W)]
    Move = 1 << 1,

    [EditorFieldName("播放"), Keyboard(KeyCode.E)]
    Play = 1 << 2,

    [EditorFieldName("前进"), Keyboard(KeyCode.R)]
    PlayNext = 1 << 3,
}

public enum EnMenuPriority
{
    None,
    Ai_Decision = 1000,
}

[Flags]
public enum EnEditorViewModelType
{
    None = 0,
    Normalize = 1 << 0,
    Details = 1 << 1,
}

public static class EditorUtil
{
    private static HashSet<KeyCode> _CurKeyCode = new(10);

    public static void ClearData()
    {
        _CurKeyCode.Clear();
    }

    public static void EditorUpdate(Event e)
    {
        if (e.type == EventType.KeyDown)
        {
            _CurKeyCode.Add(e.keyCode);
        }
        else if (e.type == EventType.KeyUp)
        {
            _CurKeyCode.Remove(e.keyCode);
        }
    }

    public static bool KeyCodeHas(KeyCode key)
    {
        return _CurKeyCode.Contains(key);
    }

    #region view model

    private static EnEditorViewModelType GetCurEditorViewModelType()
    {
        var result = EnEditorViewModelType.None;
        if (KeyCodeHas(KeyCode.LeftShift))
            result |= EnEditorViewModelType.Details;
        return result;
    }

    public static bool ViewModelIs(EnEditorViewModelType viewModelType)
    {
        var curViewModelType = GetCurEditorViewModelType();
        var result = curViewModelType == viewModelType;
        return result;
    }

    public static bool ViewModelHas(EnEditorViewModelType viewModelType)
    {
        var curViewModelType = GetCurEditorViewModelType();
        var result = (curViewModelType & viewModelType) == viewModelType;
        return result;
    }

    #endregion

    #region global controller

    private static EnEditorGlobalController _CurController;

    public static void SetControllerState(EnEditorGlobalController controller)
    {
        if (ControllerIsActive(controller))
            _CurController &= ~controller;
        else
            _CurController |= controller;
    }

    public static bool ControllerIsActive(EnEditorGlobalController controller)
    {
        return (_CurController & controller) == controller;
    }

    #endregion

    #region mouse

    private static EnMouseControllerType _MouseControllerType;

    public static void AddMouseController(EnMouseControllerType mouseCtrlType)
    {
        _MouseControllerType |= mouseCtrlType;
    }

    public static void ClearMouseController(EnMouseControllerType mouseCtrlType)
    {
        _MouseControllerType &= ~mouseCtrlType;
    }

    public static bool MouseControllerIsNone()
    {
        return _MouseControllerType == EnMouseControllerType.None;
    }

    public static bool MouseControllerIs(EnMouseControllerType mouseCtrlType)
    {
        return (_MouseControllerType & mouseCtrlType) != 0;
    }

    public static bool HasMouseInRect(in Rect rect)
    {
        return rect.Contains(GetEditorMousePos(Vector2.zero));
    }

    private static Vector2 _LastMousePos;

    public static Vector2 GetEditorMousePos(Vector2 offset)
    {
        // 跳过布局计算阶段
        if (Event.current.type != EventType.Layout)
            _LastMousePos = Event.current.mousePosition;
        return _LastMousePos + offset;
    }

    public static Vector2 GetMouseLocalPos(Vector2 pos, Vector2 offset)
    {
        var localPos = GetEditorMousePos(offset) - pos;
        return localPos;
    }

    public static Vector2 GetMouseLocalSchedule(Vector2 pos, Vector2 size, Vector2 offset)
    {
        var localPos = GetMouseLocalPos(pos, offset);
        var slider = new Vector2(Mathf.Clamp01(localPos.x / size.x), Mathf.Clamp01(localPos.y / size.y));
        return slider;
    }

    public static Vector2 GetMouseLocalSchedule(in Rect rect, Vector2 offset)
    {
        var slider = GetMouseLocalSchedule(rect.position, rect.size, offset);
        return slider;
    }

    #endregion

    #region misc

    public static T[] AddArrayElement<T>(T[] array, T element)
    {
        var list = array?.ToList() ?? new List<T>();
        list.Add(element);
        return list.ToArray();
    }

    public static T[] RemoveArrayElement<T>(T[] array, int index)
    {
        var list = array?.ToList() ?? new List<T>();
        list.RemoveAt(index);
        return list.ToArray();
    }

    public static object Copy(object source, Type targetType)
    {
        var sourceType = source.GetType();
        var target = Activator.CreateInstance(targetType);


        Dictionary<string, FieldInfo> sourceFieldList = new();

        var tempType = sourceType;
        do
        {
            var arrField = sourceType.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance |
                                                BindingFlags.Default);
            foreach (var item in arrField)
            {
                if (sourceFieldList.ContainsKey(item.Name))
                    continue;
                sourceFieldList.Add(item.Name, item);
            }

            tempType = tempType.BaseType;
        } while (tempType != null);

        Dictionary<string, FieldInfo> targetFieldList = new();
        tempType = targetType;
        do
        {
            var arrField = tempType.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance |
                                              BindingFlags.Default);
            foreach (var item in arrField)
            {
                if (targetFieldList.ContainsKey(item.Name))
                    continue;
                targetFieldList.Add(item.Name, item);
            }

            tempType = tempType.BaseType;
        } while (tempType != null);


        foreach (var item in sourceFieldList)
        {
            if (!targetFieldList.TryGetValue(item.Key, out var fieldInfo))
                continue;
            var sourceValue = item.Value.GetValue(source);
            fieldInfo.SetValue(target, sourceValue);
        }

        var result = target;
        return result;
    }

    public static TResult Copy<TResult>(object source, Type targetType)
        where TResult : class
    {
        var result = Copy(source, targetType);
        return result as TResult;
    }

    public static T Copy<T>(object source)
        where T : class, new()
    {
        var result = Copy<T>(source, typeof(T));
        return result;
    }

    public static string GetClassName(object target)
    {
        var type = target.GetType();
        var att = type.GetCustomAttribute<EditorFieldNameAttribute>();
        var name = att?.fieldName ?? target.ToString();
        return name;
    }

    public static string GetEnumName(Enum target)
    {
        var type = target.GetType();
        var fieldInfo = type.GetField(target.ToString());
        var att = fieldInfo.GetCustomAttribute<EditorFieldNameAttribute>();
        var name = att?.fieldName ?? target.ToString();
        return name;
    }

    public static T GetEnumAttribute<T>(Enum target)
        where T : Attribute
    {
        var type = target.GetType();
        var fieldInfo = type.GetField(target.ToString());
        var att = fieldInfo.GetCustomAttribute<T>();
        return att;
    }

    public static string GetBuildPlayerPath(BuildTarget buildTarget)
    {
        var path = Path.Combine(ABBUtil.GetUnityMiscPath(), "BuildPlayer", buildTarget.ToString());
        return path;
    }

    public static void OpenFolder(string folderPath)
    {
        // 检查路径是否存在
        if (!Directory.Exists(folderPath))
        {
            Console.WriteLine("文件夹路径不存在：" + folderPath);
            return;
        }

        try
        {
            // 启动资源管理器打开指定路径
            // Process.Start 会调用系统默认的文件夹打开程序（通常是资源管理器）
            Process.Start(new ProcessStartInfo
            {
                FileName = folderPath,
                UseShellExecute = true, // 必须设为 true，否则可能无法启动
                Verb = "open" // 执行“打开”操作
            });
        }
        catch (Exception ex)
        {
            Console.WriteLine("打开文件夹失败：" + ex.Message);
        }
    }

    #endregion

    #region type change

    public static void ChangeEditorStruct(IEditor obj)
    {
        var propertyInfos = obj.GetType()
            .GetProperties(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance |
                           BindingFlags.DeclaredOnly);
        for (var i = 0; i < propertyInfos.Length; i++)
        {
            var propertyInfo = propertyInfos[i];
            if (!propertyInfo.HasAttribute<EditorFieldAttribute>())
                continue;
            var att = propertyInfo.GetCustomAttribute<EditorFieldAttribute>();

            ChangeEditorStruct(obj, att.fieldName);
        }
    }

    private static void ChangeEditorStruct(IEditor obj, string fieldName)
    {
        var fieldInfo = GetBaseFieldInfo(obj, fieldName);
        if (fieldInfo == null)
            throw new Exception($"获取 {obj.GetType()} 不存在属性 {fieldName}");
        var oriValue = fieldInfo.GetValue(obj);
        if (oriValue == null)
            return;
        var oriValueType = oriValue.GetType();
        if (oriValueType.IsArray)
        {
            var arrEleType = oriValueType.GetElementType();
            var getValueMethod = typeof(Array).GetMethod("GetValue", new[] { typeof(int) });
            var setValueMethod = typeof(Array).GetMethod("SetValue", new[] { typeof(object), typeof(int) });
            var lengthPro = typeof(Array).GetProperty("Length", BindingFlags.Instance | BindingFlags.Public);
            var length = (int)lengthPro.GetValue(oriValue);
            var arrEleEditorType = arrEleType.IsValueType ? arrEleType : SkillEditorDefine.GetEditorType(arrEleType);
            var newValue = Array.CreateInstance(arrEleEditorType, length);
            for (var i = 0; i < length; i++)
            {
                var value = getValueMethod.Invoke(oriValue, new object[] { i });
                var elementType = value.GetType();
                var editorType = SkillEditorDefine.GetEditorType(elementType);
                if (elementType.IsClass || elementType.IsInterface)
                {
                    var editorObj = ChangeValueType(editorType, value);
                    setValueMethod.Invoke(newValue, new[] { editorObj, i });
                    ChangeEditorStruct(editorObj as IEditor);
                }
                else
                {
                    var eleValue = ToEditorStruct(editorType, value);
                    if (eleValue == null)
                        continue;
                    setValueMethod.Invoke(newValue, new[] { eleValue, i });
                }
            }

            fieldInfo.SetValue(obj, newValue);
        }
        else if (oriValueType.IsStruct())
        {
            //var editorType = SkillEditorDefine.GetEditorType(oriValueType);
            //var value = ToEditorStruct(editorType, oriValue);
            //fieldInfo.SetValue(obj, value);
        }
        else if (oriValueType.IsClass || oriValueType.IsInterface)
        {
            var editorType = SkillEditorDefine.GetEditorType(oriValueType);
            //var value = ToEditorStruct(editorType, oriValue);
            //fieldInfo.SetValue(obj, value);
            var editorObj = ChangeValueType(editorType, oriValue);
            fieldInfo.SetValue(obj, editorObj);
            ChangeEditorStruct(editorObj as IEditor);
        }
    }

    private static object ChangeValueType(Type type, object value)
    {
        var result = Activator.CreateInstance(type);
        var fields = GetAllInstanceFields(value.GetType());
        foreach (var field in fields)
        {
            var fieldValue = field.GetValue(value);
            field.SetValue(result, fieldValue);
        }

        return result;
    }

    private static object ToEditorStruct(Type editorType, object value)
    {
        var settings = new JsonSerializerSettings
        {
            ContractResolver = new PrivateFieldContractResolver(),
            Formatting = Formatting.Indented // 格式化输出，便于阅读,
        };
        var jsonStr = JsonConvert.SerializeObject(value, settings);
        var editorInstance = JsonConvert.DeserializeObject(jsonStr, editorType, settings);
        return editorInstance;
    }

    private static IEnumerable<FieldInfo> GetAllInstanceFields(Type type)
    {
        var list = new List<FieldInfo>();
        for (var t = type; t != null; t = t.BaseType)
        {
            var fis = t.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic |
                                  BindingFlags.DeclaredOnly);
            list.AddRange(fis);
        }

        return list;
    }

    public static FieldInfo GetBaseFieldInfo(IEditor obj, string fieldName)
    {
        var baseType = obj.GetType().BaseType;
        while (baseType != null)
        {
            var fieldInfo = baseType.GetField(fieldName,
                BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public | BindingFlags.FlattenHierarchy);
            if (fieldInfo != null)
                return fieldInfo;
            baseType = baseType.BaseType;
        }

        return null;
    }

    #endregion

    #region Menu

    private class MenuItem : GUIContent
    {
        public GUIContent content;
        public MenuFunction2 action;
        public object userData;

        public MenuItem(GUIContent content, MenuFunction2 action, object userData)
        {
            this.content = content;
            this.action = action;
            this.userData = userData;
        }
    }

    private static List<MenuItem> _MenuItemList = new(10);

    public static void AddMenuItem(GUIContent menuName, MenuFunction2 callback, object userData)
    {
        _MenuItemList.Add(new MenuItem(menuName, callback, userData));
    }

    public static void RemoveMenuItem(string menuName)
    {
        _MenuItemList.FindIndex(item => item.content.text == menuName);
    }

    public static void ShowMenuItems()
    {
        var genMenu = new GenericMenu();
        for (int i = 0; i < _MenuItemList.Count; i++)
        {
            var meunItem = _MenuItemList[i];
            genMenu.AddItem(meunItem.content, true, meunItem.action, meunItem.userData);
        }

        genMenu.ShowAsContext();
    }

    #endregion
}

public static class EditorConfig
{
    public static Color AreaColor = new(1, 1, 1, 0.07f);
    public static Vector4 BoxPadding = new(5, 5, 20, 5);
    public static Vector2 LayerInterval = new(0, 100);
}

public static class EditorGuiUtil
{
    public static GUIStyle GetBoxTitleStyle()
    {
        return new(GUI.skin.label)
        {
            alignment = TextAnchor.UpperLeft,
            fontSize = 10,
            normal =
            {
                textColor = new Color(GUI.skin.label.normal.textColor.r, GUI.skin.label.normal.textColor.g,
                    GUI.skin.label.normal.textColor.b, 0.5f)
            },
        };
    }

    public static void DrawBox(Rect rect, string title)
    {
        var labelRect = new Rect(rect) { y = rect.y + 5, };
        GUI.Label(labelRect, title, EditorGuiUtil.GetBoxTitleStyle());
        EditorGUI.DrawRect(rect, EditorConfig.AreaColor);
    }

    public static void DrawTriangleHit(in Rect hitRect, in float radius, in Color color, in string hitMsg)
    {
        var center = hitRect.center;
        var colCache = Handles.color;
        Handles.color = color;
        Handles.DrawAAConvexPolygon(new Vector3[]
        {
            new Vector2(center.x, center.y + radius), new Vector2(center.x - radius, center.y - radius),
            new Vector2(center.x + radius, center.y - radius)
        });
        Handles.color = colCache;

        if (hitMsg != null && EditorUtil.HasMouseInRect(in hitRect))
        {
            GUIStyle style = EditorStyles.helpBox;
            var width = Mathf.Min(20, hitMsg.Length) * 10;
            float height = style.CalcHeight(new GUIContent(hitMsg), width);
            var boxSize = new Vector2(width, height);
            EditorGUI.HelpBox(new Rect(EditorUtil.GetEditorMousePos(Vector2.zero), boxSize), hitMsg, MessageType.Info);
        }
    }

    public static void DrawSlider(Rect rect, float slider)
    {
        var scheduleRect = new Rect(rect)
        {
            width = rect.width * slider
        };
        EditorGUI.DrawRect(rect, new Color(0.5f, 0.5f, 0.5f, 0.5f));
        EditorGUI.DrawRect(scheduleRect, new Color(0, 0.5f, 0, 0.8f));
    }

    public static void DrawLinkHor(float height)
    {
        var space = height - 3f;

        EditorGUILayout.BeginVertical();
        {
            GUILayout.Space(space / 2);
            GUILayout.Button("", GUILayout.Height(3f), GUILayout.ExpandWidth(true));
            GUILayout.Space(space / 2);
        }
        EditorGUILayout.EndVertical();
    }

    public static void DrawLinkVer(float weight)
    {
        var space = weight - 3f;

        EditorGUILayout.BeginHorizontal();
        {
            GUILayout.Space(space / 2);
            GUILayout.Button("", GUILayout.Width(3f), GUILayout.ExpandHeight(true));
            GUILayout.Space(space / 2);
        }
        EditorGUILayout.EndHorizontal();
    }

    public static void DrawEnum<T>(Action<T> callback, string label = null, Func<T, bool> isShow = null,
        params GUILayoutOption[] options)
        where T : Enum
    {
        DrawEnum(default(T), callback, label, isShow);
    }

    public static void DrawEnum<T>(T defValue, Action<T> callback, string label = null, Func<T, bool> isShow = null,
        params GUILayoutOption[] options)
        where T : Enum
    {
        var btnName = string.IsNullOrEmpty(label) ? EditorUtil.GetEnumName(defValue) : label;

        if (!GUILayout.Button(btnName, options))
            return;

        var type = typeof(T);
        var values = Enum.GetValues(type);
        var menu = new GenericMenu();
        for (var i = 0; i < values.Length; i++)
        {
            var value = values.GetValue(i);
            var eObj = (Enum)value;
            if (isShow != null && !isShow((T)eObj))
                continue;
            var name = EditorUtil.GetEnumName(eObj);
            menu.AddItem(new GUIContent(name), object.Equals(eObj, defValue), () => { callback.Invoke((T)eObj); });
        }

        menu.ShowAsContext();
    }

    public static void DrawEnum<T>(Rect rect, string name, Action<T> callback)
        where T : Enum
    {
        DrawEnum(rect, default(T), new GUIContent(name), callback);
    }

    public static void DrawEnum<T>(Rect rect, Action<T> callback)
        where T : Enum
    {
        DrawEnum(rect, default(T), new GUIContent(EditorUtil.GetEnumName(default(T))), callback);
    }

    public static void DrawEnum<T>(Rect rect, T defValue, Action<T> callback)
        where T : Enum
    {
        DrawEnum(rect, defValue, new GUIContent(EditorUtil.GetEnumName(defValue)), callback);
    }

    public static void DrawEnum<T>(Rect rect, T defValue, string name, Action<T> callback)
        where T : Enum
    {
        DrawEnum(rect, defValue, new GUIContent(name), callback);
    }

    public static void DrawEnum<T>(Rect rect, GUIContent content, Action<T> callback)
        where T : Enum
    {
        DrawEnum(rect, default(T), content, callback);
    }

    public static void DrawEnum<T>(Rect rect, T defValue, GUIContent content, Action<T> callback)
        where T : Enum
    {
        if (!GUI.Button(rect, content))
            return;

        var type = typeof(T);
        var values = Enum.GetValues(type);
        var menu = new GenericMenu();
        for (var i = 0; i < values.Length; i++)
        {
            var value = values.GetValue(i);
            var eObj = (Enum)value;
            var name = EditorUtil.GetEnumName(eObj);
            menu.AddItem(new GUIContent(name), object.Equals(eObj, defValue), () => { callback.Invoke((T)eObj); });
        }

        menu.ShowAsContext();
    }


    static GUIContent _AssetContent = new GUIContent()
    {
        text = "AssetID:",
        tooltip = "AssetCfg -> id",
    };

    static string _SearchAsset = "";

    public static void DrawAssetID<T>(int assetID, Action<int> selectID, Func<AssetCfg, bool> isShow)
        where T : UnityEngine.Object
    {
        EditorGUILayout.BeginHorizontal(GUILayout.Width(250), GUILayout.ExpandWidth(false));
        {
            EditorGUILayout.LabelField(_AssetContent, GUILayout.Width(50));

            var content = new GUIContent()
            {
                text = $"{assetID}",
            };
            if (assetID > 0)
            {
                var assetCfg = ExcelEditorUtil.GetCfg<AssetCfg>(assetID);
                if (assetCfg != null && AssetDatabase.LoadAssetAtPath<T>(assetCfg.strPath))
                {
                    var name = Path.GetFileName(assetCfg.strPath);
                    content.text += $"-{name}";
                }
                else
                {
                    content.text += "-Error";
                }
            }
            else
            {
                content.text += "-Error";
            }

            var rect = GUILayoutUtility.GetRect(new GUIContent(), GUI.skin.box, GUILayout.Width(190),
                GUILayout.ExpandWidth(false));

            var isShowSearch = rect.Contains(Event.current.mousePosition);
            var btnRect = new Rect(rect)
            {
                width = isShowSearch ? 90 : 190,
            };
            if (isShowSearch)
            {
                var searchRect = new Rect(rect)
                {
                    x = rect.x + 90,
                    width = 100,
                };
                _SearchAsset = GUI.TextField(searchRect, _SearchAsset);
            }

            if (GUI.Button(btnRect, content))
            {
                var menu = new GenericMenu();
                var assetCfgCount = ExcelEditorUtil.GetCfgCount<AssetCfg>();
                for (int j = 0; j < assetCfgCount; j++)
                {
                    var itemAssetCfg = ExcelEditorUtil.GetCfgByIndex<AssetCfg>(j);
                    if (!string.IsNullOrWhiteSpace(_SearchAsset))
                        if (!itemAssetCfg.strPath.Contains(_SearchAsset, StringComparison.CurrentCultureIgnoreCase))
                            continue;

                    if (isShow != null && !isShow(itemAssetCfg))
                        continue;
                    var itemAsset = AssetDatabase.LoadAssetAtPath<T>(itemAssetCfg.strPath);
                    if (itemAsset == null)
                        continue;
                    menu.AddItem(new()
                    {
                        text = $"{itemAssetCfg.nAssetID}-{itemAssetCfg.strDescEditor}",
                    }, itemAssetCfg.nAssetID == assetID, () => { selectID(itemAssetCfg.nAssetID); });
                }

                menu.ShowAsContext();
            }
        }
        EditorGUILayout.EndHorizontal();
    }


    public static void DrawCfgField<TCfg>(string title, int cfgID, Action<int> selectID, float width)
        where TCfg : ICfg
    {
        EditorGUILayout.BeginHorizontal();
        {
            var titleWidth = width / 3;
            GUILayout.Label(title, GUILayout.Width(titleWidth), GUILayout.ExpandWidth(false));
            var rect = GUILayoutUtility.GetRect(new GUIContent(), GUI.skin.box, GUILayout.Width(width - titleWidth),
                GUILayout.ExpandWidth(false));
            DrawCfgField<TCfg>(rect, cfgID, selectID);
        }
        EditorGUILayout.EndHorizontal();
    }

    public static void DrawCfgField<TCfg>(int cfgID, Action<int> selectID, int width)
        where TCfg : ICfg
    {
        var rect = GUILayoutUtility.GetRect(new GUIContent(), GUI.skin.box, GUILayout.Width(width),
            GUILayout.ExpandWidth(false));
        DrawCfgField<TCfg>(rect, cfgID, selectID);
    }

    private static Dictionary<Type, string> _SearchDic = new();

    public static void DrawCfgField<TCfg>(Rect rect, int cfgID, Action<int> selectID)
        where TCfg : ICfg
    {
        EditorGUILayout.BeginHorizontal();
        {
            var content = new GUIContent()
            {
                text = $"{cfgID}",
            };
            var descField = typeof(TCfg).GetField("strDescEditor", (BindingFlags)int.MaxValue);
            var curItem = ExcelEditorUtil.GetCfg<TCfg>(cfgID);
            if (curItem != null)
            {
                var strObj = descField?.GetValue(curItem);
                var str = string.IsNullOrWhiteSpace($"{strObj}") ? "" : $"{strObj}";
                content.text += $"-{str}";
            }
            else
            {
                content.text += "-Error";
            }

            if (!_SearchDic.TryGetValue(typeof(TCfg), out var searchStr))
                _SearchDic.Add(typeof(TCfg), "");

            var isShowSearchTxt = rect.Contains(Event.current.mousePosition);
            var btnWidth = rect.width / (isShowSearchTxt ? 2 : 1);
            var btnRect = new Rect(rect) { width = btnWidth, };

            if (isShowSearchTxt)
            {
                var searchTxtRect = new Rect(rect) { x = rect.x + btnWidth, width = rect.width - btnWidth, };
                _SearchDic[typeof(TCfg)] = GUI.TextField(searchTxtRect, _SearchDic[typeof(TCfg)]);
            }

            if (GUI.Button(btnRect, content))
            {
                var menu = new GenericMenu();

                var count = ExcelEditorUtil.GetCfgCount<TCfg>();
                for (int j = 0; j < count; j++)
                {
                    var item = ExcelEditorUtil.GetCfgByIndex<TCfg>(j);
                    var key = item.GetID();

                    var strObj = descField?.GetValue(item);
                    var str = string.IsNullOrWhiteSpace($"{strObj}") ? "null" : $"{strObj}";

                    if (!string.IsNullOrWhiteSpace(_SearchDic[typeof(TCfg)]))
                        if (!key.ToString().Contains(_SearchDic[typeof(TCfg)],
                                StringComparison.CurrentCultureIgnoreCase))
                            if (!str.Contains(_SearchDic[typeof(TCfg)], StringComparison.CurrentCultureIgnoreCase))
                                continue;

                    menu.AddItem(new()
                    {
                        text = $"{key}-{str}"
                    }, key == cfgID, () => { selectID(key); });
                }

                menu.ShowAsContext();
            }
        }
        EditorGUILayout.EndHorizontal();
    }

    public static void DrawSliderRange(Rect rect, ref float start, ref float end, float min, float max)
    {
        start = Mathf.Clamp(start, min, max);
        end = Mathf.Max(start, Mathf.Min(end, max));

        EditorGUI.DrawRect(rect, new Color(0.5f, 0.5f, 0.5f, 0.5f));
        var greenRect = new Rect(rect)
        {
            x = rect.xMin + start / (max - min) * rect.width,
            width = (end - start) / (max - min) * rect.width,
        };
        EditorGUI.DrawRect(greenRect, new Color(0, 0.5f, 0, 1f));

        var leftBtnRect = new Rect(rect)
        {
            x = greenRect.xMin - 15,
            width = 30,
        };
        var rightBtnRect = new Rect(rect)
        {
            x = greenRect.xMax - 15,
            width = 30,
        };

        var leftTxtRect = new Rect(rect)
        {
            x = rect.x,
            width = 30,
        };
        var rightTxtRect = new Rect(rect)
        {
            x = rect.xMax - 30,
            width = 30,
        };


        var isEnable = rightBtnRect.Contains(Event.current.mousePosition) ||
                       leftBtnRect.Contains(Event.current.mousePosition);
        GUI.enabled = !isEnable;
        if (GUI.enabled)
        {
            var input = Mathf.RoundToInt(start * 100) / 100f;
            var tempStart = EditorGUI.FloatField(leftTxtRect, input,
                new GUIStyle(GUI.skin.textField) { alignment = TextAnchor.MiddleLeft });
            var isStart = tempStart != input;
            if (isStart)
                start = tempStart;
            var input2 = Mathf.RoundToInt(end * 100) / 100f;
            var tempEnd = EditorGUI.FloatField(rightTxtRect, input2,
                new GUIStyle(GUI.skin.textField) { alignment = TextAnchor.MiddleRight });
            var isEnd = tempEnd != input2;
            if (isEnd)
                end = tempEnd;
        }
        else
        {
            EditorGUI.FloatField(leftTxtRect, Mathf.RoundToInt(start * 100) / 100f,
                new GUIStyle(GUI.skin.textField) { alignment = TextAnchor.MiddleLeft });
            EditorGUI.FloatField(rightTxtRect, Mathf.RoundToInt(end * 100) / 100f,
                new GUIStyle(GUI.skin.textField) { alignment = TextAnchor.MiddleRight });
        }

        GUI.enabled = true;

        GUI.RepeatButton(leftBtnRect, new GUIContent($"{start / (max - min) * 100:0}"));
        GUI.RepeatButton(rightBtnRect, new GUIContent($"{end / (max - min) * 100:0}"));


        if (rightBtnRect.Contains(Event.current.mousePosition))
        {
            switch (Event.current.type)
            {
                case EventType.MouseDown:
                {
                    Event.current.Use();
                }
                    break;
                case EventType.MouseDrag:
                {
                    end += Event.current.delta.x / rect.width * (max - min);
                    //Debug.Log($"        {Event.current.delta}");
                    Event.current.Use();
                }
                    break;
                case EventType.MouseUp:
                {
                    Event.current.Use();
                }
                    break;
                default:
                    break;
            }
        }

        if (leftBtnRect.Contains(Event.current.mousePosition))
        {
            switch (Event.current.type)
            {
                case EventType.MouseDown:
                {
                    Event.current.Use();
                }
                    break;
                case EventType.MouseDrag:
                {
                    start += Event.current.delta.x / rect.width * max;
                    //Debug.Log($"        {Event.current.delta}");
                    Event.current.Use();
                }
                    break;
                case EventType.MouseUp:
                {
                    Event.current.Use();
                }
                    break;
                default:
                    break;
            }
        }
    }

    public static void LinkRectA2b(in Rect rectA, in Rect rectB, in float slider)
    {
        // 起点/终点选择为两矩形的最靠近对方中心的边缘点
        Vector2 start = ClosestPointOnRectEdge(rectA, rectB.center);
        Vector2 end = ClosestPointOnRectEdge(rectB, rectA.center);

        // 在 GUI 模式下使用 Handles 绘制平滑曲线
        Handles.BeginGUI();
        Color c = Color.red;
        Color c2 = Color.green;
        float width = 5f;

        // 根据 slider（可用于控制弯曲程度），计算切线
        var straighten2 = Mathf.Clamp01(0); // slider 越大曲线越弯
        Vector2 dir = (end - start);
        Vector2 tangentOffset = new Vector2(-dir.y, dir.x).normalized * 40f * straighten2;
        Vector3 startTangent = start + (dir * 0.25f) + tangentOffset;
        Vector3 endTangent = end - (dir * 0.25f) + tangentOffset * -1f;

        Handles.DrawBezier(start, end, startTangent, endTangent, c, null, width);
        Handles.DrawLine(start, end, width - 2);
        //var end2 = Vector2.Lerp(start, end, slider);
        //var endTangent2 = end2 - (dir * 0.25f) + tangentOffset * -1f;
        //var color = Color.Lerp(c, c2, slider);
        //Handles.DrawBezier(start, end2, start, end2, color, null, width - 2);

        // 终点/起点小方块标记（用 EditorGUI.DrawRect 绘制易于控制像素大小）
        float markerSize = 9f;
        EditorGUI.DrawRect(new Rect(start - Vector2.one * (markerSize / 2f), Vector2.one * markerSize), c);
        EditorGUI.DrawRect(new Rect(end - Vector2.one * (markerSize / 2f), Vector2.one * markerSize), c2);

        Handles.EndGUI();
    }

    private static Vector2 ClosestPointOnRectEdge(Rect r, Vector2 toPoint)
    {
        // 计算从矩形中心到目标点的直线与矩形四边的交点（取第一个合法交点）
        Vector2 c = r.center;
        Vector2 dir = toPoint - c;
        float dx = dir.x;
        float dy = dir.y;

        float bestT = float.MaxValue;
        Vector2 bestPoint = c;

        if (Mathf.Abs(dx) > 1e-6f)
        {
            // 左边
            float t = (r.xMin - c.x) / dx;
            if (t > 0f && t < bestT)
            {
                float y = c.y + dy * t;
                if (y >= r.yMin - 1e-6f && y <= r.yMax + 1e-6f)
                {
                    bestT = t;
                    bestPoint = new Vector2(r.xMin, y);
                }
            }

            // 右边
            t = (r.xMax - c.x) / dx;
            if (t > 0f && t < bestT)
            {
                float y = c.y + dy * t;
                if (y >= r.yMin - 1e-6f && y <= r.yMax + 1e-6f)
                {
                    bestT = t;
                    bestPoint = new Vector2(r.xMax, y);
                }
            }
        }

        if (Mathf.Abs(dy) > 1e-6f)
        {
            // 底边
            float t = (r.yMin - c.y) / dy;
            if (t > 0f && t < bestT)
            {
                float x = c.x + dx * t;
                if (x >= r.xMin - 1e-6f && x <= r.xMax + 1e-6f)
                {
                    bestT = t;
                    bestPoint = new Vector2(x, r.yMin);
                }
            }

            // 顶边
            t = (r.yMax - c.y) / dy;
            if (t > 0f && t < bestT)
            {
                float x = c.x + dx * t;
                if (x >= r.xMin - 1e-6f && x <= r.xMax + 1e-6f)
                {
                    bestT = t;
                    bestPoint = new Vector2(x, r.yMax);
                }
            }
        }

        if (bestT < float.MaxValue)
            return bestPoint;

        // 无交点（例如目标就在中心），退回到矩形最近的边（投影到边缘）
        Vector2 clamped = new Vector2(Mathf.Clamp(toPoint.x, r.xMin, r.xMax), Mathf.Clamp(toPoint.y, r.yMin, r.yMax));
        if (r.Contains(toPoint))
        {
            // 如果目标在矩形内部，把点投影到距离最小的边
            float left = Mathf.Abs(toPoint.x - r.xMin);
            float right = Mathf.Abs(r.xMax - toPoint.x);
            float top = Mathf.Abs(r.yMax - toPoint.y);
            float bottom = Mathf.Abs(toPoint.y - r.yMin);
            float min = Mathf.Min(Mathf.Min(left, right), Mathf.Min(top, bottom));
            if (min == left) clamped = new Vector2(r.xMin, toPoint.y);
            else if (min == right) clamped = new Vector2(r.xMax, toPoint.y);
            else if (min == top) clamped = new Vector2(toPoint.x, r.yMax);
            else clamped = new Vector2(toPoint.x, r.yMin);
        }

        return clamped;
    }

    public static bool DrawDeletedBtn(Rect rect)
    {
        var icon = EditorGUIUtility.IconContent("d_CollabDeleted Icon");
        icon.tooltip = "移除该元素";
        var clickBtn = GUI.Button(rect, icon);
        return clickBtn;
    }

    public static bool DrawCreateBtn(Rect rect)
    {
        var icon = EditorGUIUtility.IconContent("d_Collab.FileAdded");
        icon.tooltip = "添加一个元素";
        var clickBtn = GUI.Button(rect, icon);
        return clickBtn;
    }

    public static void DrawCreateBtn<T>(Rect rect, Action<T> action)
        where T : Enum
    {
        var icon = EditorGUIUtility.IconContent("d_Collab.FileAdded");
        icon.tooltip = $"选择一个 [{typeof(T)}] 元素添加";
        DrawEnum<T>(rect, icon, action);
    }

    public static void DrawAreaDelAndAddBtn(
        in Rect rect
        , int count
        , AiNodeRectInfo[] rectInfos
        , AiNodeRectIndexInfo indexInf
        , Action<int> delAction
        , Action addAction
    )
    {
        DrawAreaDelAndAddBtn(in rect, count, rectInfos, indexInf, delAction,
            btnRect =>
            {
                if (EditorGuiUtil.DrawCreateBtn(btnRect))
                {
                    addAction.Invoke();
                }
            });
    }

    private static void DrawAreaDelAndAddBtn(
        in Rect rect
        , int count
        , AiNodeRectInfo[] rectInfos
        , AiNodeRectIndexInfo indexInf
        , Action<int> delAction
        , Action<Rect> addAction
    )
    {
        var btnSize = new Vector2(20, 20);
        for (var i = 0; i < count; i++)
        {
            var subGenRect = rectInfos[indexInf.GetIndex(i)].GetRect(rect.position);

            var btnRect = new Rect(Vector2.zero, btnSize)
            {
                x = subGenRect.xMax - 5 - btnSize.x,
                y = subGenRect.yMin - btnSize.y,
            };
            if (i == count - 1)
            {
                addAction.Invoke(btnRect);
                btnRect.x -= btnRect.width + 2;
            }

            if (EditorGuiUtil.DrawDeletedBtn(btnRect))
            {
                delAction.Invoke(i);
                count--;
                i--;
            }
        }
    }

    public static void DrawAreaDelAndAddBtn<T>(
        in Rect rect
        , int count
        , AiNodeRectInfo[] rectInfos
        , AiNodeRectIndexInfo indexInf
        , Action<int> delAction
        , Action<T> addAction
    )
        where T : Enum

    {
        DrawAreaDelAndAddBtn(in rect, count, rectInfos, indexInf, delAction,
            btnRect => { EditorGuiUtil.DrawCreateBtn<T>(btnRect, addAction); });
    }
}