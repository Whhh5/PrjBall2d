public static class ProjectileUtility
{

    public static int CreateProjectileEntity(in int projectileCfgId)
    {
        var userData = ClassPoolMgr.Instance.Pull<SceneProjectileEntityDataUserData>();
        var entityDataId = CreateProjectileEntity(in projectileCfgId, userData);
        
        ClassPoolMgr.Instance.Push(userData);
        return entityDataId;
    }
    
    public static int CreateProjectileEntity(in int projectileCfgId, SceneProjectileEntityDataUserData userData)
    {
        var projectileCfg = GameSchedule.Instance.GetProjectileCfg0(projectileCfgId);
        userData.projectileCfgId = projectileCfgId;
        var entityDataId = EntityMgr.Instance.CreateEntityData(in projectileCfg.nDataTypeId, userData);
        EntityMgr.Instance.LoadEntity(entityDataId);
        return entityDataId;
    }

    public static void DestroyProjectileEntity(in int entityId)
    {
        EntityMgr.Instance.RecycleEntityData(in entityId);
    }
}