using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public partial class SkillBuffScheduleActionEditor : SkillBuffScheduleAction, ISkillScheduleActionEditor
{
    public void OnSkillEditorGUI()
    {
        EditorGUILayout.BeginVertical();
        {
            EditorGuiUtil.DrawEnum(_BuffDataParams?.GetBuff() ?? EnBuff.None, select =>
            {
                if (_BuffDataParams != null && select == _BuffDataParams.GetBuff())
                    return;
                var newBuffData = SkillEditorDefine.GetEntityBuffParamsEditor(select);
                if (newBuffData == null)
                    return;
                if (_BuffDataParams != null)
                {
                    if (!EditorUtility.DisplayDialog("hit", "change buff？", "ok", "cancel"))
                        return;
                    SkillEditorDefine.RecycleEditorInstance(_BuffDataParams);
                }

                _BuffDataParams = newBuffData;
            });
            if (_BuffDataParams == null)
            {
                EditorGUILayout.HelpBox("add buff", MessageType.Error);
            }
            else
            {
                EditorGUILayout.BeginHorizontal();
                {
                    GUILayout.Button("", GUILayout.Width(SkillEditorDefine.LineSize), GUILayout.ExpandHeight(true));
                    GUILayout.Space(SkillEditorDefine.DefineSpace);
                    _BuffDataParams.OnSkillEditorGUI();
                }
                EditorGUILayout.EndHorizontal();
            }

            _StartSchedule = EditorGUILayout.FloatField("开始时间点", _StartSchedule);
            _IsSectionBuff = EditorGUILayout.Toggle("是否区间Buff", _IsSectionBuff);
            if (_IsSectionBuff)
            {
                EditorGUILayout.BeginHorizontal();
                {
                    GUILayout.Button("", GUILayout.Width(SkillEditorDefine.LineSize), GUILayout.ExpandHeight(true));
                    GUILayout.Space(SkillEditorDefine.DefineSpace);
                    EditorGUILayout.BeginVertical();
                    {
                        _EndSchedule = EditorGUILayout.FloatField("结束时间点", _EndSchedule);
                        if (_EndSchedule < _StartSchedule)
                            _EndSchedule = _StartSchedule;
                    }
                    EditorGUILayout.EndVertical();
                }
                EditorGUILayout.EndHorizontal();
            }
        }
        EditorGUILayout.EndVertical();
    }


    [EditorField(nameof(_StartSchedule))]
    private float _StartSchedule
    {
        get => this.GetFieldValue(nameof(_StartSchedule), 0f);
        set => this.SetFieldValue(nameof(_StartSchedule), value);
    }

    [EditorField(nameof(_IsSectionBuff))]
    private bool _IsSectionBuff
    {
        get => this.GetFieldValue(nameof(_IsSectionBuff), true);
        set => this.SetFieldValue(nameof(_IsSectionBuff), value);
    }

    [EditorField(nameof(_EndSchedule))]
    private float _EndSchedule
    {
        get => this.GetFieldValue(nameof(_EndSchedule), 0f);
        set => this.SetFieldValue(nameof(_EndSchedule), value);
    }

    [EditorField(nameof(_BuffDataParams))]
    private IEntityBuffParamsEditor _BuffDataParams
    {
        get => this.GetFieldValue(nameof(_BuffDataParams), default(IEntityBuffParamsEditor));
        set => this.SetFieldValue(nameof(_BuffDataParams), value);
    }
}