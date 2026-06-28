
using UnityEditor;
using UnityEngine;

public partial class SkillEffectScheduleActionEditor : SkillEffectScheduleAction, ISkillScheduleActionEditor
{
    public void OnSkillEditorGUI()
    {
        EditorGUILayout.BeginVertical();
        {
            DrawBaseField();
        }
        EditorGUILayout.EndVertical();
    }
    private void DrawBaseField()
    {
        EditorGuiUtil.DrawCfgField<EffectCfg>(nameof(_EffectID), _EffectID, selectId => _EffectID = selectId, SkillEditorDefine.FieldWidth);
        _Schedule = EditorGUILayout.FloatField(new GUIContent("����ʱ��"), _Schedule);
        _OffsetPos = EditorGUILayout.Vector3Field(new GUIContent("λ��ƫ��"), _OffsetPos);
        _OffsetRot = EditorGUILayout.Vector3Field(new GUIContent("��תƫ��"), _OffsetRot);
    }

    [EditorField(nameof(_EffectID))]
    private int _EffectID
    {
        get => this.GetFieldValue(nameof(_EffectID), 0);
        set => this.SetFieldValue(nameof(_EffectID), value);
    }
    [EditorField(nameof(_Schedule))]
    private float _Schedule
    {
        get => this.GetFieldValue(nameof(_Schedule), 0f);
        set => this.SetFieldValue(nameof(_Schedule), value);
    }
    [EditorField(nameof(_OffsetPos))]
    private Vector3 _OffsetPos
    {
        get => this.GetFieldValue(nameof(_OffsetPos), Vector3.zero);
        set => this.SetFieldValue(nameof(_OffsetPos), value);
    }
    [EditorField(nameof(_OffsetRot))]
    private Vector3 _OffsetRot
    {
        get => this.GetFieldValue(nameof(_OffsetRot), Vector3.zero);
        set => this.SetFieldValue(nameof(_OffsetRot), value);
    }
    [EditorField(nameof(_OffsetScale))]
    private Vector3 _OffsetScale
    {
        get => this.GetFieldValue(nameof(_OffsetScale), Vector3.zero);
        set => this.SetFieldValue(nameof(_OffsetScale), value);
    }
}