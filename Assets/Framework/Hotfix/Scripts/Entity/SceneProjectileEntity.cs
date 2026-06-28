public class SceneProjectileEntityDataUserData : GameEntityDataUserData
{
    public int projectileCfgId;

    public override void OnPoolDestroy()
    {
        base.OnPoolDestroy();
        projectileCfgId
            = -1;
    }
}

public abstract class SceneProjectileEntityData : GameEntityData<SceneProjectileEntity, SceneProjectileEntityDataUserData>
{
    public override EnLoadTarget LoadTarget
    {
        get
        {
            var projectileCfg = GameSchedule.Instance.GetProjectileCfg0(_ProjectileCfgId);
            return (EnLoadTarget)projectileCfg.nAssetID;
        }
    }

    private int _ProjectileCfgId;
    public override void OnPoolDestroy()
    {
        base.OnPoolDestroy();
        _ProjectileCfgId
            = -1;
    }

    protected override void OnPoolInit(SceneProjectileEntityDataUserData userData)
    {
        _ProjectileCfgId = userData.projectileCfgId;
    }
}
public abstract class SceneProjectileEntity : GameEntity
{
}