using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public interface ICameraTarget
{
    public bool IsCameraTargetValid();
    public Vector3 GetCameraTargetWorldPos();
    public Vector3 GetCameraTargetOffsetPos();
    public Vector3 GetCameraLookAtPos();
}
public class CameraMgr : SingletonMono<CameraMgr>
{
    [SerializeField]
    private Camera _MainCamera;
    [SerializeField]
    private Camera _UICamera;
    [SerializeField]
    private UniversalAdditionalCameraData m_URPCamera = null;
    //[SerializeField]
    //private CinemachineVirtualCamera m_VirtualCamera = null;

    private Transform _CameraTran = null;
    private ICameraTarget _CameraTarget = null;
    private Vector3 _WorldPos;
    private Quaternion _LookAt;
    private float _Depth;

    public UniversalAdditionalCameraData GetURPCamera()
    {
        return m_URPCamera;
    }
    protected override void Awake()
    {
        base.Awake();

        _CameraTran = _MainCamera.transform;
        _LookAt = _CameraTran.rotation;
    }
    protected override void OnDestroy()
    {
        _MainCamera = null;
        m_URPCamera = null;
        //m_VirtualCamera = null;
        base.OnDestroy();
    }

    public Camera GetMainCamera()
    {
        return _MainCamera;
    }
    public Camera GetUICamera()
    {
        return _UICamera;
    }
    public ref readonly Vector3 GetCameraWorldPos()
    {
        return ref _WorldPos;
    }
    public bool TryGetCameraWorldPos(out Vector3 pos)
    {
        if (_MainCamera == null)
        {
            pos = Vector3.zero;
            return false;
        }
        pos = _MainCamera.transform.position;
        return true;
    }
    public Vector3 GetCameraForward()
    {
        return _MainCamera.transform.forward;
    }
    public Vector3 GetCameraRight()
    {
        return _MainCamera.transform.right;
    }
    public Vector3 GetCameraUp()
    {
        return _MainCamera.transform.up;
    }

    public void SetCameraTarget(in int entityId)
    {
        var cameraTarget = EntityMgr.Instance.GetEntityData<ICameraTarget>(in entityId);
        if (cameraTarget == null)
        {
            ABBUtil.LogError($"SetCameraTarget failed, entityId:{entityId}, EntityData 没有实现接口: {typeof(ICameraTarget)}");
            return;
        }
        _CameraTarget = cameraTarget;
    }
    public void SetCameraPos(Vector3 pos)
    {
        _WorldPos = pos;
    }
    public void ForceSetCameraPos(Vector3 pos)
    {
        _MainCamera.transform.position
            = _WorldPos
            = pos;
        _Depth = pos.z;
    }
    public Vector3 GetCameraTargetPos()
    {
        if (_CameraTarget == null)
            return _WorldPos;
        var result = _CameraTarget.GetCameraTargetWorldPos() + _CameraTarget.GetCameraTargetOffsetPos();
        return result;
    }
    public void SetDepth(in float depth)
    {
        _Depth = depth;
    }

    protected override void Update()
    {
        base.Update();

        //var pos = new Vector3(_WorldPos.x, _WorldPos.y, _Depth);
        var lerpT = Time.deltaTime * 10;
        var cameraPos = Vector3.Lerp(_CameraTran.position, _WorldPos, lerpT);
        var cameraQua = Quaternion.Lerp(_CameraTran.rotation, _LookAt, lerpT);

        _CameraTran.SetPositionAndRotation(cameraPos, cameraQua);

        if (_CameraTarget != null && _CameraTarget.IsCameraTargetValid())
        {
            _WorldPos = GetCameraTargetPos();
            var lookAt = _CameraTarget.GetCameraLookAtPos();
            _LookAt = Quaternion.LookRotation(lookAt - _WorldPos);
        }
    }
}
