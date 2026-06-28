

using UnityEngine;

public static class MonsterUtility
{
    public static int CreateMonster(in int monsterId)
    {
        var userData = ClassPoolMgr.Instance.Pull<MonsterEntityDataUserData>();
        var entityId = CreateMonster(in monsterId, userData);

        ClassPoolMgr.Instance.Push(userData);
        return entityId;
    }
    public static int CreateMonster(in int monsterId, MonsterEntityDataUserData userData)
    {
        var monsterCfg = GameSchedule.Instance.GetMonsterCfg0(monsterId);
        userData.monsterCfgId = monsterId;
        var entityId = SceneEntityMgr.Instance.CreateEntityData(in monsterCfg.nEntityTypeId, userData);
        
        SceneEntityMgr.Instance.AddCom(in entityId, EnSceneEntityComType.Camera);
        SceneEntityMgr.Instance.AddCom(in entityId, EnSceneEntityComType.Rigidbody);
        SceneEntityMgr.Instance.AddCom(in entityId, EnSceneEntityComType.Animation);
        SceneEntityMgr.Instance.AddCom(in entityId, EnSceneEntityComType.Property);
        SceneEntityMgr.Instance.AddCom(in entityId, EnSceneEntityComType.Buff);
        SceneEntityMgr.Instance.AddCom(in entityId, EnSceneEntityComType.AI);
        SceneEntityMgr.Instance.AddCom(in entityId, EnSceneEntityComType.Cmd);

        if (monsterCfg.nIdleCmdID > 0)
        {
            SceneEntityMgr.Instance.AddCmd(in entityId, in monsterCfg.nIdleCmdID);
        }
        
        var proCom = SceneEntityMgr.Instance.GetEntityCom<SceneEntityPropertyComData>(in entityId, EnSceneEntityComType.Property);
        proCom.SetBaseMoveSpeed(3);
        proCom.SetBaseRotationSpeed(5);
        
        return entityId;
    }

    public static void DestroyMonster(in int monsterId)
    {
        SceneEntityMgr.Instance.RemoveCom(in monsterId, EnSceneEntityComType.Camera);
        SceneEntityMgr.Instance.RemoveCom(in monsterId, EnSceneEntityComType.Rigidbody);
        SceneEntityMgr.Instance.RemoveCom(in monsterId, EnSceneEntityComType.Animation);
        SceneEntityMgr.Instance.RemoveCom(in monsterId, EnSceneEntityComType.Property);
        SceneEntityMgr.Instance.RemoveCom(in monsterId, EnSceneEntityComType.Buff);
        SceneEntityMgr.Instance.RemoveCom(in monsterId, EnSceneEntityComType.AI);
        SceneEntityMgr.Instance.RemoveCom(in monsterId, EnSceneEntityComType.Cmd);
        SceneEntityMgr.Instance.DestroyEntityData(monsterId);
    }

    public static MonsterCfg GetMonsterCfg(in int entityId)
    {
        var monsterEntityData = EntityMgr.Instance.GetEntityData<SceneEntityData>(in entityId);
        ref readonly var monsterId = ref monsterEntityData.GetMonsterCfgId();
        var monsterCfg = GameSchedule.Instance.GetMonsterCfg0(monsterId);
        return monsterCfg;
    }
}