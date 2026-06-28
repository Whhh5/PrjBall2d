using UnityEngine;

public abstract class EntityRigidbody2DData : SceneEntityData
{
    private EntityRigidbody2D _EntityRigidbody = null;

    public Vector3 MoveRigibodyPos { get; private set; }

    public override void OnGOCreate()
    {
        base.OnGOCreate();
        _EntityRigidbody = _GameEntity as EntityRigidbody2D;
    }
    public override void OnGODestroy()
    {
        _EntityRigidbody = null;
        base.OnGODestroy();
    }

    public void AddForce(Vector2 force, ForceMode2D mode)
    {
        if (!_IsLoadSuccess)
            return;
        _EntityRigidbody.AddForce(force, mode);
    }
    public void Sleep()
    {
        if (!_IsLoadSuccess)
            return;
        _EntityRigidbody.Sleep();
    }
    public void MoveRigibody(Vector3 worldPos)
    {
        MoveRigibodyPos = worldPos;
        if (!_IsLoadSuccess)
        {
            SetPosition(worldPos);
            return;
        }
        _EntityRigidbody.MoveRigibody();
    }

}
public abstract class EntityRigidbody2D : SceneEntity
{
    [SerializeField] private Rigidbody2D _Rigidbody2D = null;

    private EntityRigidbody2DData _EntityRigidbodyData = null;

    public override void OnUnload()
    {
        _EntityRigidbodyData = null;
        base.OnUnload();
    }
    public override void LoadCompeletion()
    {
        base.LoadCompeletion();
        _EntityRigidbodyData = _EntityData as EntityRigidbody2DData;

        _Rigidbody2D.position = _EntityRigidbodyData.WorldPos;
    }

    public void AddForce(Vector2 force, ForceMode2D mode)
    {
        _Rigidbody2D.AddForce(force, mode);
    }
    public void Sleep()
    {
        _Rigidbody2D.Sleep();
    }
    public void MoveRigibody()
    {
        _Rigidbody2D.MovePosition(_EntityRigidbodyData.MoveRigibodyPos);
    }
    public override void SetPosition()
    {
        // _Rigidbody2D.MovePosition(_EntityRigidbodyData.MoveRigibodyPos);
    }
    protected override void Update()
    {
        base.Update();

        _EntityRigidbodyData.SetPosition(transform.position);
    }
}
