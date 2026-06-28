

using UnityEngine;

public partial class PhysicsResolveSphereEditor : IPhysicsResolveDebugModelEditor
{
    private GameObject _BoxGo = null;
    private GameObject _PlayerGo = null;
    private Transform _PlayerGoTran => _PlayerGo.transform;
    private float _ShowTime = 0.2f;

    private bool _LastState = false;
    public void OnDebugModelEnterEditor(GameObject go)
    {
        _PlayerGo = go;
        _BoxGo = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        ResetTran();
    }

    public void OnDebugModelExitEditor()
    {
        GameObject.DestroyImmediate(_BoxGo);
        _BoxGo = null;
        _PlayerGo = null;
        _LastState = false;
    }

    public void OnSceneUpdateEditor(in Rect stageRect, in Rect boxRect, in SkillDebugModelTimelineInfo timeInfo, in float invokeSchedule)
    {
        var curShowTime = timeInfo.skillTime - timeInfo.StageLocalSliderToSkillTime(invokeSchedule);
        var state = curShowTime >= 0 && timeInfo.stageLocalSlider >= invokeSchedule && curShowTime <= _ShowTime;
        _BoxGo.SetActive(state);
        if (state != _LastState)
        {
            _LastState = state;
            ResetTran();
        }
        if (!state)
            return;
        SetPosOffset(_BoxGo.transform.position);
        SetScaleOffset(_BoxGo.transform.lossyScale);

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
        _BoxGo.transform.position = GetBoxGoPosition();
        _BoxGo.transform.localScale = _Radius * Vector3.one;
    }

    private Vector3 GetBoxGoPosition()
    {
        var result = CalculateUtility.GetPos(_PlayerGoTran.position, _PlayerGoTran.forward, _PlayerGoTran.up, _PlayerGoTran.right, _PosOffset);
        return result;
    }
    private void SetPosOffset(Vector3 pos)
    {
        var localPos = CalculateUtility.GetPosOffset(_PlayerGoTran.position, _PlayerGoTran.forward, _PlayerGoTran.up, _PlayerGoTran.right, in pos);
        if (Vector3.Distance(localPos, _PosOffset) < 0.01f)
            return;
        _PosOffset = localPos;
    }

    public void SetScaleOffset(Vector3 scale)
    {
        if (Mathf.Abs(scale.x - _Radius) < 0.01f)
            return;
        _Radius = scale.x;
    }

}