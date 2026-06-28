
using UnityEditor;
using UnityEngine;


public partial class PhysicsResolveBoxEditor : IPhysicsResolveDebugModelEditor
{
    private GameObject _BoxGo = null;
    private GameObject _PlayerGo = null;
    private Transform _PlayerGoTran => _PlayerGo.transform;
    private float _ShowTime = 0.2f;

    private bool _LastState = false;
    public void OnDebugModelEnterEditor(GameObject go)
    {
        _PlayerGo = go;
        _BoxGo = GameObject.CreatePrimitive(PrimitiveType.Cube);
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
        SetRotOffset(_BoxGo.transform.rotation);
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
        _BoxGo.transform.rotation = GetBoxGoRotation();
        _BoxGo.transform.localScale = GetScale();
    }
    private Vector3 GetBoxGoPosition()
    {
        var pos = CalculateUtility.GetPos(_PlayerGoTran.position, _PlayerGoTran.forward, _PlayerGoTran.up, _PlayerGoTran.right, _PosOffset);
        return pos;
    }
    private void SetPosOffset(Vector3 worldPos)
    {
        var pos = CalculateUtility.GetPosOffset(_PlayerGoTran.position, _PlayerGoTran.forward, _PlayerGoTran.up, _PlayerGoTran.right, worldPos);
        if (Vector3.Distance(pos, _PosOffset) < 0.01f)
            return;
        _PosOffset = pos;
    }
    private Quaternion GetBoxGoRotation()
    {
        var qua = CalculateUtility.GetRotation(_PlayerGoTran.rotation, Quaternion.Euler(_RotOffset));
        return qua;
    }
    private void SetRotOffset(Quaternion worldRot)
    {
        var euler = CalculateUtility.GetRotationOffset(_PlayerGoTran.rotation, in worldRot);
        if (Vector3.Distance(euler.eulerAngles, _RotOffset) < 0.01f)
            return;
        _RotOffset = euler.eulerAngles;
    }
    private void SetScaleOffset(Vector3 worldScale)
    {
        var scale = CalculateUtility.GetScaleOffset(_PlayerGoTran.lossyScale, worldScale);
        if (Vector3.Distance(scale, _BoxSize) < 0.01f)
            return;
        _BoxSize = scale;
    }
    private Vector3 GetScale()
    {
        var scale = CalculateUtility.GetScale(_PlayerGoTran.lossyScale, _BoxSize);
        return scale;
    }
}
