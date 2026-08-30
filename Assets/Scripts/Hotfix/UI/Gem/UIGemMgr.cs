using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

public enum EnGemTypeId
{
    None,
    Box,
    Sphere,
    Triangle,
    EnumCount,
}
public enum EnGemEffectTypeId
{
    Attribute,
    AttributePercentage,
}
public enum EnGemQuality
{
    None,
    Quality1,
    Quality2,
    Quality3,
    Quality4,
    Quality5,
    Quality6,
    EnumCount,
}
public class UIGemMgr : Singleton<UIGemMgr>
{

    public override void Destroy()
    {
        EventMgr.Instance.Unregister<GameBattlePlayerCreateEventData>(EnABBEvent.EVENT_BATTLE_PLAYER_CREATE, 0, 0, OnEvent_PlayerCreate);
        base.Destroy();
    }
    public override async UniTask AwakeAsync()
    {
        await base.AwakeAsync();

        EventMgr.Instance.Register<GameBattlePlayerCreateEventData>(EnABBEvent.EVENT_BATTLE_PLAYER_CREATE, 0, 0, OnEvent_PlayerCreate);
    }

    public int AddGem(int goodsCfgId)
    {
        var perData = PersistentMgr.Instance.GetData<PersistentGemData>(EnPersistentId.Gem);
        var quality = (EnGemQuality)Random.Range((int)EnGemQuality.None + 1, (int)EnGemQuality.EnumCount);
        var effectValue = UIGemUtility.CalculateEffectValue(goodsCfgId, quality);
        var gemId = perData.AddGemData(goodsCfgId, effectValue, quality);

        EventMgr.Instance.FireExecute(EnABBEvent.EVENT_GEM_ADD, goodsCfgId, gemId);
        return gemId;
    }
    public void RemoveGem(int gemId)
    {
        var goodsCfgId = UIGemUtility.GetGemGoodsCfgId(gemId);
        EventMgr.Instance.FireExecute(EnABBEvent.EVENT_GEM_REMOVE, goodsCfgId, gemId);

        var perData = PersistentMgr.Instance.GetData<PersistentGemData>(EnPersistentId.Gem);
        perData.RemoveGemData(gemId);
    }
    public void GetAleadyGemIds(ref List<int> ids)
    {
        var perData = PersistentMgr.Instance.GetData<PersistentGemData>(EnPersistentId.Gem);
        perData.GetGemIds(ref ids);
    }
    public void SetGemInlayStatus(int gemId, bool isInlay)
    {
        var perData = PersistentMgr.Instance.GetData<PersistentGemData>(EnPersistentId.Gem);
        perData.SetIsInlay(gemId, isInlay);
    }
    public bool GetGemInlayStatus(int gemId)
    {
        var perData = PersistentMgr.Instance.GetData<PersistentGemData>(EnPersistentId.Gem);
        var isInlay = perData.GetIsInlay(gemId);
        return isInlay;
    }

    public void ApplyEntity(int entityId)
    {
        var list = new List<int>(100);
        GetAleadyGemIds(ref list);
        for (int i = 0; i < list.Count; i++)
        {
            var gemId = list[i];
            if (!GetGemInlayStatus(gemId))
                continue;
            UIGemUtility.ApplyEntity(entityId, gemId);
        }
    }

    private void OnEvent_PlayerCreate(in int _, in int __, in GameBattlePlayerCreateEventData data)
    {
        ApplyEntity(data.entityId);
    }

}
