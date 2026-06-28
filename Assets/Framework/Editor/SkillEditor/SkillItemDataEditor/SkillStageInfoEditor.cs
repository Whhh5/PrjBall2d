using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public partial class SkillStageInfoEditor : SkillStageInfo, ISkillEditor
{
    private readonly bool[] _BuffFoldoutList = new bool[10];
    private int _CurSelectBuffInfoIndex = -1;
    private int _DrawAtkLinkScheduleIndex = -1;

    public void OnSkillEditorGUI()
    {
        EditorGUILayout.BeginVertical();
        {
            DrawBaseField();

            _BuffFoldoutList[0] = EditorGUILayout.BeginFoldoutHeaderGroup(_BuffFoldoutList[0], "Buff");
            if (_BuffFoldoutList[0])
            {
                DrawBuffInfoList();
            }

            EditorGUILayout.EndFoldoutHeaderGroup();

            _BuffFoldoutList[1] = EditorGUILayout.BeginFoldoutHeaderGroup(_BuffFoldoutList[1], "AtkLinkSchedule");
            if (_BuffFoldoutList[1])
            {
                DrawAtkLinkSchedule();
            }

            EditorGUILayout.EndFoldoutHeaderGroup();
        }
        EditorGUILayout.EndVertical();
    }

    private void DrawBaseField()
    {
        EditorGuiUtil.DrawCfgField<ClipCfg>("clipId", _ClipId, selectId =>
        {
            if (_ClipId != selectId)
                _ClipId = selectId;
        }, 200);
        _CanNextTime = EditorGUILayout.FloatField("可以进行下一个动画的时间", _CanNextTime, GUILayout.Width(200));
        _AtkEndTime = EditorGUILayout.FloatField("攻击结束", _AtkEndTime, GUILayout.Width(200));
        _IsAutoRemove = EditorGUILayout.Toggle("播放完成是否自动移除", _IsAutoRemove, GUILayout.Width(200));
    }


    private void DrawBuffInfoList()
    {
        EditorGUILayout.BeginVertical();
        {
            EditorGuiUtil.DrawEnum<EnBuff>(buff =>
                {
                    var tempList = _BuffInfoList?.ToList() ?? new List<IEntityBuffParamsEditor>();
                    var item = SkillEditorDefine.GetEntityBuffParamsEditor(buff);
                    if (item == null)
                        throw new Exception($"buff is not exist => {buff}");
                    tempList.Add(item);
                    _BuffInfoList = tempList.ToArray();
                }, "Add Buff",
                isShow: buff => SkillEditorUtil.BuffIsShowMenu(buff, _BuffInfoList));

            if ((_BuffInfoList?.Length ?? 0) == 0)
            {
                EditorGUILayout.HelpBox("选择一个 buff", MessageType.Warning);
            }
            else
            {
                EditorGUILayout.BeginHorizontal();
                {
                    GUILayout.Button("", GUILayout.Width(3), GUILayout.ExpandHeight(true));
                    GUILayout.Space(SkillEditorDefine.TabWidth);

                    SkillEditorUtil.SkillShowBuffMenu(_BuffInfoList, _CurSelectBuffInfoIndex, index => _CurSelectBuffInfoIndex = index, newBuffList => _BuffInfoList = newBuffList);

                    GUILayout.Space(10);
                    if (_CurSelectBuffInfoIndex >= 0 && _CurSelectBuffInfoIndex < _BuffInfoList?.Length)
                        _BuffInfoList[_CurSelectBuffInfoIndex].OnSkillEditorGUI();

                    GUILayout.FlexibleSpace();
                }
                EditorGUILayout.EndHorizontal();
            }
        }
        EditorGUILayout.EndVertical();
    }


    private void DrawAtkLinkSchedule()
    {
        EditorGuiUtil.DrawEnum<EnAtkLinkScheculeType>(actionType =>
        {
            var tempList = _ArrAtkLinkSchedule?.ToList() ?? new List<ISkillScheduleActionEditor>();
            var item = SkillEditorDefine.GetSkillScheduleActionEditor(actionType);
            if (item == null)
                throw new Exception($"actionType is not exist => {actionType}");
            tempList.Add(item);
            _ArrAtkLinkSchedule = tempList.ToArray();
        }, "Add Schedule");

        if ((_ArrAtkLinkSchedule?.Length ?? 0) == 0)
        {
            EditorGUILayout.HelpBox("选择一个进度事件", MessageType.Warning);
        }
        else
        {
            EditorGUILayout.BeginHorizontal();
            {
                GUILayout.Button("", GUILayout.Width(3), GUILayout.ExpandHeight(true));
                GUILayout.Space(SkillEditorDefine.TabWidth);
                DrawAtkLinkScheduleMenuList();
            
                GUILayout.Space(10);
                if (_DrawAtkLinkScheduleIndex >= 0 && _DrawAtkLinkScheduleIndex < _ArrAtkLinkSchedule?.Length)
                    _ArrAtkLinkSchedule[_DrawAtkLinkScheduleIndex].OnSkillEditorGUI();

                GUILayout.FlexibleSpace();
            }
            EditorGUILayout.EndHorizontal();
        }
    }

    private void DrawAtkLinkScheduleMenuList()
    {
        EditorGUILayout.BeginVertical();
        {
            for (var i = 0; i < _ArrAtkLinkSchedule?.Length; i++)
            {
                var index = i;
                var style = index == _DrawAtkLinkScheduleIndex
                    ? new GUIStyle(EditorStyles.helpBox) { alignment = TextAnchor.MiddleCenter }
                    : GUI.skin.button;
                if (!GUILayout.Button($"{index}", style, GUILayout.Width(SkillEditorDefine.MenuWidth)))
                    continue;
                switch (Event.current.button)
                {
                    case 0:
                        _DrawAtkLinkScheduleIndex = index;
                        break;
                    case 1: 
                    {
                        var menu = new GenericMenu();

                        menu.AddItem(new GUIContent("移除"), false, () =>
                        {
                            if (!EditorUtility.DisplayDialog("提示", $"移除进度事件 {index}？", "ok", "cancel"))
                                return;
                            if (index <= _DrawAtkLinkScheduleIndex)
                                _DrawAtkLinkScheduleIndex--;
                            var tempList = _ArrAtkLinkSchedule.ToList();
                            SkillEditorDefine.RecycleEditorInstance(tempList[index]);
                            tempList.RemoveAt(index);
                            _ArrAtkLinkSchedule = tempList.ToArray();
                        });
                        menu.ShowAsContext();
                        break;
                    }
                }
            }
        }
        EditorGUILayout.EndVertical();
    }

    [EditorField(nameof(_ClipId))]
    private int _ClipId
    {
        get => this.GetFieldValue(nameof(_ClipId), 0);
        set => this.SetFieldValue(nameof(_ClipId), value);
    }

    [EditorField(nameof(_CanNextTime))]
    private float _CanNextTime
    {
        get => this.GetFieldValue(nameof(_CanNextTime), 0.0f);
        set => this.SetFieldValue(nameof(_CanNextTime), value);
    }

    [EditorField(nameof(_AtkEndTime))]
    private float _AtkEndTime
    {
        get => this.GetFieldValue(nameof(_AtkEndTime), 0.0f);
        set => this.SetFieldValue(nameof(_AtkEndTime), value);
    }

    [EditorField(nameof(_IsAutoRemove))]
    private bool _IsAutoRemove
    {
        get => this.GetFieldValue(nameof(_IsAutoRemove), false);
        set => this.SetFieldValue(nameof(_IsAutoRemove), value);
    }

    [EditorField(nameof(_BuffInfoList))]
    private IEntityBuffParamsEditor[] _BuffInfoList
    {
        get => this.GetFieldValue(nameof(_BuffInfoList), default(IEntityBuffParamsEditor[]));
        set => this.SetFieldValue(nameof(_BuffInfoList), value);
    }

    [EditorField(nameof(_ArrAtkLinkSchedule))]
    private ISkillScheduleActionEditor[] _ArrAtkLinkSchedule
    {
        get => this.GetFieldValue(nameof(_ArrAtkLinkSchedule), default(ISkillScheduleActionEditor[]));
        set => this.SetFieldValue(nameof(_ArrAtkLinkSchedule), value);
    }
}