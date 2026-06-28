using UnityEngine;
public class EntityNoMovementBuff : SkillEntityBuff<EntityNoMovementBuffData>
{

    protected override void OnEnable(int addKey, EntityNoMovementBuffData buffParams)
    {
        base.OnEnable(addKey, buffParams);
        var proCom = SceneEntityMgr.Instance.GetEntityCom<SceneEntityPropertyComData>(_TargetEntityID, EnSceneEntityComType.Property);
        proCom.SetIsCanMove(false);
    }

    protected override void ReOnEnable(int addKey, EntityNoMovementBuffData buffParams)
    {
        base.ReOnEnable(addKey, buffParams);
        var proCom = SceneEntityMgr.Instance.GetEntityCom<SceneEntityPropertyComData>(_TargetEntityID, EnSceneEntityComType.Property);
        proCom.SetIsCanMove(false);
    }

    public override bool OnDisable(int addKey)
    {
        var proCom = SceneEntityMgr.Instance.GetEntityCom<SceneEntityPropertyComData>(_TargetEntityID, EnSceneEntityComType.Property);
        proCom.SetIsCanMove(true);
        return base.OnDisable(addKey);
    }
}
