using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraLookAtData : MonoBehaviour, IEntityCameraCom
{
    [SerializeField]
    private Transform followTran;
    [SerializeField]
    private Transform lookAtTran;


    private Vector3 _QuaOffset = Vector3.zero;
    private float _RadiusOffset = 0;
    private Quaternion _Qua = Quaternion.identity;
    private float _Radius = 0;

    public Transform GetEntityCameraCom_FollowTran()
    {
        return followTran;
    }

    public Transform GetEntityCameraCom_LookAtTran()
    {
        return lookAtTran;
    }

    public void IncrementRotationOffset(Vector3 quaternion)
    {
        var minX = -70f;
        var maxX = 30f;

        var angle = _Qua.eulerAngles.x % 360f;
        if (angle > 180)
        {
            angle = angle % 180f - 180f;
        }

        var offsetX = Mathf.Clamp(_QuaOffset.x + quaternion.x, minX - angle, maxX - angle);

        _QuaOffset += quaternion;
        _QuaOffset.x = offsetX;
    }
    public void IncrementRadiusOffset(float radius)
    {
        var min = 1f;
        var max = 20;
        _RadiusOffset = Mathf.Clamp(_RadiusOffset + radius, min - _Radius, max - _Radius);
    }


    private void Awake()
    {
        _Radius = Vector3.Distance( followTran.position, lookAtTran.position);

        _Qua = Quaternion.LookRotation((followTran.position - lookAtTran.position).normalized);

    }
    private void Update()
    {
        var rot = Quaternion.Euler(_Qua.eulerAngles + _QuaOffset);
        var posOffset = rot * Vector3.forward * (_Radius + _RadiusOffset);
        followTran.position = lookAtTran.position + posOffset;
    }
}
