using UnityEngine;

public interface IEntityCameraCom : IEntity3DCom
{
    public void IncrementRotationOffset(Vector3 quaternion);
    public void IncrementRadiusOffset(float radius);
}
public class EntityCameraComData : Entity3DComDataGO<IEntityCameraCom>
{
    public override EnSceneEntityComType GetComType() => EnSceneEntityComType.Camera;
    private readonly float _RotYIncrement = 20f;
    private readonly float _ToEntityMaxDistance = 20;
    public override void OnCreateGO()
    {
        base.OnCreateGO();

        // var followTran = _GoCom.GetEntityCameraCom_FollowTran();
        // var lookAtTran = _GoCom.GetEntityCameraCom_LookAtTran();
        // CameraMgr.Instance.SetVirtualCameraTrackTran(followTran, lookAtTran);

        ABBInputMgr.Instance.AddListaner(KeyCode.Mouse0, KeyCode_Mouse0);
        ABBInputMgr.Instance.AddListaner(KeyCode.Mouse1, KeyCode_Mouse2);
        ABBInputMgr.Instance.AddListanerDown(KeyCode.E, OnClick_KeyCodeDownE);
        ABBInputMgr.Instance.AddListanerDown(KeyCode.Q, OnClick_KeyCodeDownQ);
    }

    public override void OnDestroyGO()
    {
        ABBInputMgr.Instance.RemoveListaner(KeyCode.Mouse0, KeyCode_Mouse0);
        ABBInputMgr.Instance.RemoveListaner(KeyCode.Mouse1, KeyCode_Mouse2);
        ABBInputMgr.Instance.RemoveListanerDown(KeyCode.E, OnClick_KeyCodeDownE);
        ABBInputMgr.Instance.RemoveListanerDown(KeyCode.Q, OnClick_KeyCodeDownQ);
        //CameraMgr.Instance.ClearLookAt();
        base.OnDestroyGO();

    }

    private void KeyCode_Mouse0()
    {
        if (!EntityUtility.IsValid(_EntityId))
            return;
        var mouseDelta = ABBUtil.GetMousePositionDelta();

        var rotY = mouseDelta.x / Screen.width * Mathf.PI * 2 * Mathf.Rad2Deg;

        var rotX = mouseDelta.y / Screen.height * Mathf.PI * 2 * Mathf.Rad2Deg;

        _GoCom.IncrementRotationOffset(new(rotX, rotY, 0));
    }
    private void KeyCode_Mouse2()
    {
        if (!EntityUtility.IsValid(_EntityId))
            return;
        var mouseDelta = ABBUtil.GetMousePositionDelta();

        var value = mouseDelta.y / Screen.height * _ToEntityMaxDistance;
        _GoCom.IncrementRadiusOffset(value);
    }

    private void OnClick_KeyCodeDownQ()
    {
        if (!EntityUtility.IsValid(_EntityId))
            return;
        _GoCom.IncrementRotationOffset(-_RotYIncrement * Vector3.up);
    }
    private void OnClick_KeyCodeDownE()
    {
        if (!EntityUtility.IsValid(_EntityId))
            return;
        _GoCom.IncrementRotationOffset(_RotYIncrement * Vector3.up);
    }
}
