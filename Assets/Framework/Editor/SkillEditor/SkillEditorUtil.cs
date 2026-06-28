using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;


public static class SkillEditorUtil
{
    public static readonly GUIStyle LabelCenterStyle = new GUIStyle(EditorStyles.label)
    { alignment = TextAnchor.MiddleCenter };

    private static Dictionary<ISkillEditor, Color> _DrawSkillColorInfo = new();

    public static Color GetDrawSkillColor(ISkillEditor skillEditor)
    {
        if (!_DrawSkillColorInfo.TryGetValue(skillEditor, out var color))
        {
            var r = UnityEngine.Random.Range(0.2f, 0.8f);
            var g = UnityEngine.Random.Range(0.2f, 0.8f);
            var b = UnityEngine.Random.Range(0.2f, 0.8f);
            color = new Color(r, g, b, 0.5f);
            _DrawSkillColorInfo.Add(skillEditor, color);
        }
        return color;
    }

    public static bool BuffIsShowMenu(EnBuff buff, IEntityBuffParamsEditor[] buffInfos)
    {
        if (buffInfos == null)
            return true;
        if (buffInfos.All(item => item.GetBuff() != buff))
            return true;
        return false;
    }
    
    public static void SkillShowBuffMenu(IEntityBuffParamsEditor[] buffList, int curSelectIndex,
        Action<int> setSelectIndex, Action<IEntityBuffParamsEditor[]> setBuffValue)
    {
        EditorGUILayout.BeginVertical();
        {
            for (var i = 0; i < buffList?.Length; i++)
            {
                var index = i;
                var buffInfo = buffList[index];
                var buffType = buffInfo.GetBuff();
                var style = curSelectIndex == index
                    ? new GUIStyle(EditorStyles.helpBox) { alignment = TextAnchor.UpperCenter }
                    : GUI.skin.button;
                if (!GUILayout.Button($"{buffType}", style, GUILayout.Width(SkillEditorDefine.MenuWidth)))
                    continue;
                switch (Event.current.button)
                {
                    case 0:
                        // curSelectIndex = index;
                        setSelectIndex.Invoke(index);
                        break;
                    case 1:
                        {
                            var menu = new GenericMenu();
                            menu.AddItem(new GUIContent($"移除 {buffType}"), false, () =>
                            {
                                if (!EditorUtility.DisplayDialog("提示", $"移除buff {buffType}？", "ok", "cancel"))
                                    return;
                                if (curSelectIndex >= index)
                                {
                                    if (curSelectIndex >= buffList.Length)
                                    {
                                        // curSelectIndex = buffList.Length - 1;
                                        setSelectIndex.Invoke(buffList.Length - 1);
                                    }
                                }

                                var tempList = buffList.ToList();
                                SkillEditorDefine.RecycleEditorInstance(tempList[index]);
                                tempList.RemoveAt(index);
                                buffList = tempList.ToArray();

                                setBuffValue.Invoke(buffList);
                            });
                            menu.ShowAsContext();
                        }
                        break;
                }
            }
        }
        EditorGUILayout.EndVertical();
    }

    public static GUIStyle GetButtonStyle(bool state)
    {
        var style = state
            ? new GUIStyle(EditorStyles.helpBox) { alignment = TextAnchor.MiddleCenter }
            : new GUIStyle(GUI.skin.button);
        return style;
    }

    public static void DrawStageSelectBox(in Rect mainRect)
    {
        var selectLineLength = Mathf.Min(Mathf.Min(mainRect.width, mainRect.height) * 0.25f,
            SkillEditorDefine.SelectLineLength);
        Handles.DrawLines(new Vector3[]
        {
            mainRect.position,
            new Vector2(mainRect.xMin + selectLineLength, mainRect.y),
            new Vector2(mainRect.x, mainRect.yMin + selectLineLength),

            new Vector2(mainRect.xMax, mainRect.yMin),
            new Vector2(mainRect.xMax - selectLineLength, mainRect.y),
            new Vector2(mainRect.xMax, mainRect.yMin + selectLineLength),

            mainRect.position + mainRect.size,
            new Vector2(mainRect.xMax - selectLineLength, mainRect.yMax),
            new Vector2(mainRect.xMax, mainRect.yMax - selectLineLength),

            mainRect.position + new Vector2(0, mainRect.size.y),
            new Vector2(mainRect.xMin + selectLineLength, mainRect.yMax),
            new Vector2(mainRect.x, mainRect.yMax - selectLineLength),
        }, new[]
        {
            0, 1, 0, 2,
            3, 4, 3, 5,
            6, 7, 6, 8,
            9, 10, 9, 11,
        });
    }
}