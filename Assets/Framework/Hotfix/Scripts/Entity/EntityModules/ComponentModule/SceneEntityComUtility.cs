

public static class SceneEntityComUtility
{
    public static IEntity3DComData CreateSceneEntityComData(in EnSceneEntityComType comType, in Entity3DComDataUserData userData)
    {
        return comType switch
        {
            EnSceneEntityComType.Camera => ClassPoolMgr.Instance.Pull<EntityCameraComData>(userData),
            EnSceneEntityComType.Rigidbody => ClassPoolMgr.Instance.Pull<SceneEntityRigidbodyComData>(userData),
            EnSceneEntityComType.Property => ClassPoolMgr.Instance.Pull<SceneEntityPropertyComData>(userData),
            EnSceneEntityComType.AI => ClassPoolMgr.Instance.Pull<EntityAIComData>(userData),
            EnSceneEntityComType.Animation => ClassPoolMgr.Instance.Pull<EntityAnimComData>(userData),
            EnSceneEntityComType.Buff => ClassPoolMgr.Instance.Pull<EntityBuffComData>(userData),
            EnSceneEntityComType.Cmd => ClassPoolMgr.Instance.Pull<EntityCmdComData>(userData),
            _ => null,
        };
    }

    public static void DestroySceneEntityComData(IEntity3DComData comData)
    {
        ClassPoolMgr.Instance.Push(comData);
    }
}