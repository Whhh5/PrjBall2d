using System.Linq;
using UnityEditor;
using UnityEngine;

public class SkillPlayableAdapterDrawBuffEditor
{
    private int _DrawBuffInfoIndex = -1;
    private ISkillPlayableAdapterGetBuffEditor _DrawData = null;
    public void SetSkillBuffParamsEditor(ISkillPlayableAdapterGetBuffEditor drawEditor)
    {
        _DrawData = drawEditor;
    }    
    public void OnSkillEditorGUI()
    {
        var buffInfoList = _DrawData.GetSkillBuffParamsEditor();
        EditorGUILayout.BeginVertical();
        {
            EditorGuiUtil.DrawEnum<EnBuff>(selectBuff =>
            {
                var buffInfo = SkillEditorDefine.GetEntityBuffParamsEditor(selectBuff);
                if (buffInfo == null)
                    return;
                var tempList = buffInfoList?.ToList() ?? new();
                tempList.Add(buffInfo);
                buffInfoList = tempList.ToArray();
                _DrawData.SetSkillBuffParamsEditor(buffInfoList);
            }, "Add Buff",
                isShow: buff => SkillEditorUtil.BuffIsShowMenu(buff, buffInfoList));

            EditorGUILayout.BeginHorizontal();
            {
                GUILayout.Button("label", GUILayout.Width(3), GUILayout.ExpandHeight(true));
                GUILayout.Space(SkillEditorDefine.TabWidth);
                if ((buffInfoList?.Length ?? 0) == 0)
                {
                    EditorGUILayout.HelpBox("ѡ��һ��buff", MessageType.Warning);
                }
                else
                {
                    SkillEditorUtil.SkillShowBuffMenu(buffInfoList, _DrawBuffInfoIndex,
                        selectIndex => _DrawBuffInfoIndex = selectIndex, buffInfos => buffInfoList = buffInfos);

                    GUILayout.Space(10);
                    if (_DrawBuffInfoIndex >= 0 && _DrawBuffInfoIndex < buffInfoList?.Length)
                        buffInfoList[_DrawBuffInfoIndex].OnSkillEditorGUI();

                    GUILayout.FlexibleSpace();
                }
            }
            EditorGUILayout.EndHorizontal();

        }
        EditorGUILayout.EndVertical();
    }
}