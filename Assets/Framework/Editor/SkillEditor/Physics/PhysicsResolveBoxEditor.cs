using UnityEditor;
using UnityEngine;

public partial class PhysicsResolveBoxEditor : PhysicsResolveBox, IPhysicsResolveEditor
{
    public void OnSkillEditorGUI()
    {
        EditorGUILayout.BeginVertical();
        {
            DrawBaseField();
            EditorGuiUtil.DrawEnum(_CenterType, selectValue => _CenterType = selectValue);
            DrawExecuteTypeField();
        }
        EditorGUILayout.EndVertical();
    }

    private void DrawBaseField()
    {
        _BoxSize = EditorGUILayout.Vector3Field("包围盒大小", _BoxSize);
        _RotOffset = EditorGUILayout.Vector3Field("旋转偏移", _RotOffset);
        _PosOffset = EditorGUILayout.Vector3Field("位置偏移", _PosOffset);
    }
    private void DrawExecuteTypeField()
    {
        EditorGuiUtil.DrawEnum(_ExecuteType, selectValue => _ExecuteType = selectValue);

        if (_ExecuteType == EnPhysicsBoxType.Successive)
        {
            EditorGUILayout.BeginHorizontal();
            {
                GUILayout.Button("", GUILayout.Width(SkillEditorDefine.LineSize), GUILayout.ExpandHeight(true));
                GUILayout.Space(SkillEditorDefine.DefineSpace);
                EditorGUILayout.BeginVertical();
                {
                    _ExecuteTime = EditorGUILayout.FloatField("执行时间", _ExecuteTime);
                    _UnitSizeZ = EditorGUILayout.FloatField("一次监测大小", _ExecuteTime);
                }
                EditorGUILayout.EndVertical();
            }
            EditorGUILayout.EndHorizontal();
        }
    }


    [EditorField(nameof(_CenterType))]
    private EnPhysicsBoxCenterType _CenterType
    {
        get => this.GetFieldValue(nameof(_CenterType), EnPhysicsBoxCenterType.None);
        set => this.SetFieldValue(nameof(_CenterType), value);
    }

    [EditorField(nameof(_ExecuteType))]
    private EnPhysicsBoxType _ExecuteType
    {
        get => this.GetFieldValue(nameof(_ExecuteType), EnPhysicsBoxType.None);
        set => this.SetFieldValue(nameof(_ExecuteType), value);
    }

    [EditorField(nameof(_ExecuteTime))]
    private float _ExecuteTime
    {
        get => this.GetFieldValue(nameof(_ExecuteTime), 0f);
        set => this.SetFieldValue(nameof(_ExecuteTime), value);
    }

    [EditorField(nameof(_UnitSizeZ))]
    private float _UnitSizeZ
    {
        get => this.GetFieldValue(nameof(_UnitSizeZ), 0f);
        set => this.SetFieldValue(nameof(_UnitSizeZ), value);
    }

    [EditorField(nameof(_BoxSize))]
    private Vector3 _BoxSize
    {
        get => this.GetFieldValue(nameof(_BoxSize), Vector3.zero);
        set => this.SetFieldValue(nameof(_BoxSize), value);
    }

    [EditorField(nameof(_RotOffset))]
    private Vector3 _RotOffset
    {
        get => this.GetFieldValue(nameof(_RotOffset), Vector3.zero);
        set => this.SetFieldValue(nameof(_RotOffset), value);
    }

    [EditorField(nameof(_PosOffset))]
    private Vector3 _PosOffset
    {
        get => this.GetFieldValue(nameof(_PosOffset), Vector3.zero);
        set => this.SetFieldValue(nameof(_PosOffset), value);
    }
}