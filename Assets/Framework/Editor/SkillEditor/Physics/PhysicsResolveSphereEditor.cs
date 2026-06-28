using UnityEditor;
using UnityEngine;

public partial class PhysicsResolveSphereEditor : PhysicsResolveSphere, IPhysicsResolveEditor
{
    public void OnSkillEditorGUI()
    {
        EditorGUILayout.BeginVertical();
        {
            _Radius = EditorGUILayout.FloatField("半径", _Radius, GUILayout.Width(SkillEditorDefine.FieldWidth));
            _PosOffset = EditorGUILayout.Vector3Field("位置偏移", _PosOffset, GUILayout.Width(SkillEditorDefine.FieldWidth));
        }
        EditorGUILayout.EndVertical();
    }

    [EditorField(nameof(_Radius))]
    private float _Radius
    {
        get => this.GetFieldValue(nameof(_Radius), 0f);
        set => this.SetFieldValue(nameof(_Radius), value);
    }

    [EditorField(nameof(_PosOffset))]
    private Vector3 _PosOffset
    {
        get => this.GetFieldValue(nameof(_PosOffset), Vector3.zero);
        set => this.SetFieldValue(nameof(_PosOffset), value);
    }
}