


using UnityEngine;

public interface ISceneEntityRigidbodyCom : IEntity3DCom
{
    public Rigidbody GetRigidbody();
}
public sealed class SceneEntityRigidbodyComData : Entity3DComDataGO<ISceneEntityRigidbodyCom>
{
    public override EnSceneEntityComType GetComType() => EnSceneEntityComType.Rigidbody;

    private Rigidbody _Rigidbody = null;
    private bool _IsCreateGO = false;

    public override void OnDestroyGO()
    {
        base.OnDestroyGO();
        _Rigidbody = null;
        _IsCreateGO = false;
    }
    public override void OnCreateGO()
    {
        base.OnCreateGO();
        _Rigidbody = _GoCom.GetRigidbody();
        _IsCreateGO = true;
    }
    public bool IsGround()
    {
        return true;
    }

    public void MoveTo(in Vector3 targetPos)
    {
        if (!_IsCreateGO)
        {
            EntityMgr.Instance.SetEntityWorldPos(in _EntityId, in targetPos);
            return;
        }
        else
        {

            _Rigidbody.MovePosition(targetPos);
            EntityMgr.Instance.SetEntityWorldPos(in _EntityId, _Rigidbody.position);
        }
    }

    public void MoveIncrement(in Vector3 increment)
    {
        var pos = EntityMgr.Instance.GetEntityWorldPos(in _EntityId);
        var toPos = pos + increment;
        MoveTo(in toPos);

    }
}