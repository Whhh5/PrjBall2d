using UnityEngine;

public partial class PhysicsResolveCylinderEditor : IPhysicsResolveDebugModelEditor
{
    private GameObject _DebugModelGo = null;
    private Transform _DebugModelGoTran => _DebugModelGo.transform;
    private GameObject _DebugCylinder = null;
    private readonly float _ShowTime = 0.2f;
    private bool _LastState = false;
    public void OnDebugModelEnterEditor(GameObject go)
    {
        _DebugModelGo = go;
        _DebugCylinder = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        _DebugCylinder.GetComponent<MeshRenderer>().sharedMaterial.color = new Color(0, 1, 1, 0.2f);
        ResetTran();
    }

    public void OnDebugModelExitEditor()
    {
        GameObject.DestroyImmediate(_DebugCylinder);

        _DebugModelGo = null;
        _DebugCylinder = null;
        _LastState = false;
    }

    public void OnSceneUpdateEditor(in Rect stageRect, in Rect boxRect, in SkillDebugModelTimelineInfo timeInfo, in float invokeSchedule)
    {
        var curShowTime = timeInfo.skillTime - timeInfo.StageLocalSliderToSkillTime(invokeSchedule);
        var state = curShowTime >= 0 && timeInfo.stageLocalSlider >= invokeSchedule && curShowTime <= _ShowTime;
        _DebugCylinder.SetActive(state);
        if (state != _LastState)
        {
            _LastState = state;
            ResetTran();
        }
        if (!state)
            return;
        SetPos();
        SetScale();

        var sliderBg = new Rect()
        {
            x = stageRect.x + stageRect.width * invokeSchedule,
            y = boxRect.y + boxRect.height,
            width = _ShowTime / timeInfo.stageMaxTime * stageRect.width,
            height = 2,
        };
        EditorGuiUtil.DrawSlider(sliderBg, curShowTime / _ShowTime);
    }


    private void ResetTran()
    {
        _DebugCylinder.transform.position = GetPos();
        _DebugCylinder.transform.rotation = GetRot();
        _DebugCylinder.transform.localScale = GetScale();
    }
    private Vector3 GetPos()
    {
        var localPos = (_PosPoint + _PosPoint2) * 0.5f;
        var result = CalculateUtility.GetPos(_DebugModelGoTran, in localPos);
        return result;
    }
    private void SetPos()
    {
        var localPos = CalculateUtility.GetPosOffset(_DebugModelGoTran, _DebugCylinder.transform.position);
        var point1 = localPos - 0.5f * _DebugCylinder.transform.lossyScale.y * _DebugCylinder.transform.up;
        var point2 = localPos + 0.5f * _DebugCylinder.transform.lossyScale.y * _DebugCylinder.transform.up;
        if (Vector3.Distance(point1, _PosPoint) > 0.01f)
        {
            _PosPoint = point1;
        }
        if (Vector3.Distance(point2, _PosPoint2) > 0.01f)
        {
            _PosPoint2 = point2;
        }
    }
    private Quaternion GetRot()
    {
        var direction = (_PosPoint2 - _PosPoint).normalized; // 圆柱体主轴方向
        var rotation = Quaternion.FromToRotation(Vector3.up, direction);
        return rotation;
    }
    private Vector3 GetScale()
    {
        var scaleY = Vector3.Distance(_PosPoint, _PosPoint2);
        var result = new Vector3(_Radius * 2, scaleY, _Radius * 2);
        return result;
    }
    private void SetScale()
    {
        var radius = _DebugCylinder.transform.localScale.x * 0.5f;
        if (Mathf.Abs(radius - _Radius) > 0.01f)
        {
            _Radius = radius;
        }
    }
}