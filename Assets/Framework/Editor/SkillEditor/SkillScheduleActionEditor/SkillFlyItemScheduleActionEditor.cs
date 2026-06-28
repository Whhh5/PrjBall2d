using UnityEditor;
using UnityEngine;

public partial class SkillFlyItemScheduleActionEditor : SkillFlyItemScheduleAction, ISkillScheduleActionEditor
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
        _ScheduleInvoke = EditorGUILayout.Slider(_ScheduleInvoke, 0, 1);
        EditorGuiUtil.DrawEnum(_FlyType, select => _FlyType = select, nameof(_FlyType));
        _PosOffset = EditorGUILayout.Vector3Field(nameof(_PosOffset), _PosOffset);
        _RotOffset = EditorGUILayout.Vector3Field(nameof(_RotOffset), _RotOffset);

        EditorGuiUtil.DrawCfgField<ProjectileCfg>(_ProjectileCfgId, select => _ProjectileCfgId = select,
            SkillEditorDefine.FieldWidth);
        _MaxDistance =  EditorGUILayout.FloatField(nameof(_MaxDistance), _MaxDistance);
        _Speed =  EditorGUILayout.FloatField(nameof(_Speed), _Speed);
        
    }


    [EditorField(nameof(_FlyType))]
    private EnSkillFlyItemType _FlyType
    {
        get => this.GetFieldValue(nameof(_FlyType), EnSkillFlyItemType.None);
        set => this.SetFieldValue(nameof(_FlyType), value);
    }

    [EditorField(nameof(_ScheduleInvoke))]
    private float _ScheduleInvoke
    {
        get => this.GetFieldValue(nameof(_ScheduleInvoke), 0f);
        set => this.SetFieldValue(nameof(_ScheduleInvoke), value);
    }

    [EditorField(nameof(_PosOffset))]
    private Vector3 _PosOffset
    {
        get => this.GetFieldValue(nameof(_PosOffset), Vector3.zero);
        set => this.SetFieldValue(nameof(_PosOffset), value);
    }

    [EditorField(nameof(_RotOffset))]
    private Vector3 _RotOffset
    {
        get => this.GetFieldValue(nameof(_RotOffset), Vector3.zero);
        set => this.SetFieldValue(nameof(_RotOffset), value);
    }

    [EditorField(nameof(_ArrPosData))]
    private int[] _ArrPosData
    {
        get => this.GetFieldValue<int[]>(nameof(_ArrPosData));
    }

    [EditorField(nameof(_MaxDistance))]
    private float _MaxDistance
    {
        get => this.GetFieldValue<float>(nameof(_MaxDistance));
        set => this.SetFieldValue(nameof(_MaxDistance), value);
    }

    [EditorField(nameof(_Speed))]
    private float _Speed
    {
        get => this.GetFieldValue<float>(nameof(_Speed));
        set => this.SetFieldValue(nameof(_Speed), value);
    }

    [EditorField(nameof(_ProjectileCfgId))]
    private int _ProjectileCfgId
    {
        get => this.GetFieldValue<int>(nameof(_ProjectileCfgId));
        set => this.SetFieldValue(nameof(_ProjectileCfgId), value);
    }
}