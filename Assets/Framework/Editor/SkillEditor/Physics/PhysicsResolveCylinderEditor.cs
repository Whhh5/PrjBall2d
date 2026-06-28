
using UnityEditor;
using UnityEngine;

public partial class PhysicsResolveCylinderEditor : PhysicsResolveCylinder, IPhysicsResolveEditor
{
    public void OnSkillEditorGUI()
    {
        EditorGUILayout.BeginVertical();
        {
            _ExecuteTime = EditorGUILayout.FloatField("执行时间", _ExecuteTime);
            _Radius = EditorGUILayout.FloatField("半径", _Radius);
            _PosPoint = EditorGUILayout.Vector3Field("起始点", _PosPoint);
            _PosPoint2 = EditorGUILayout.Vector3Field("结束点", _PosPoint2);
        }
        EditorGUILayout.EndVertical();
    }

    [EditorField(nameof(_ExecuteTime))]
    private float _ExecuteTime
    {
        get => this.GetFieldValue(nameof(_ExecuteTime), 0f);
        set => this.SetFieldValue(nameof(_ExecuteTime), value);
    }
    [EditorField(nameof(_Radius))]
    private float _Radius
    {
        get => this.GetFieldValue(nameof(_Radius), 0f);
        set => this.SetFieldValue(nameof(_Radius), value);
    }
    [EditorField(nameof(_PosPoint))]
    private Vector3 _PosPoint
    {
        get => this.GetFieldValue(nameof(_PosPoint), Vector3.zero);
        set => this.SetFieldValue(nameof(_PosPoint), value);
    }
    [EditorField(nameof(_PosPoint2))]
    private Vector3 _PosPoint2
    {
        get => this.GetFieldValue(nameof(_PosPoint2), Vector3.zero);
        set => this.SetFieldValue(nameof(_PosPoint2), value);
    }

}
