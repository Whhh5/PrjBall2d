

using UnityEngine;

public class CoreEffectAttributePercentageAnalysist : CoreEffectAttributeAnalysist
{
    public new void ApplyEntity(int entityId, int coreId)
    {
        var attCom = SceneEntityMgr.Instance.GetEntityCom<SceneEntityAttributeComData>(entityId, EnSceneEntityComType.Attribute);
        var value = UICoreMgr.Instance.GetCoreEffectValue(coreId);
        var coreCfg = UICoreUtility.GetCoreCfg(coreId);
        var attId = (EnEntityAttributeId)coreCfg.arrEffectParams[0];
        attCom.AddAddtionalPercentage(attId, value / 100f);
    }
}