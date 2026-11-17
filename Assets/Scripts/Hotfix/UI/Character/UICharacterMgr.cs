using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.TextCore.Text;

public class UICharacterMgr : Singleton<UICharacterMgr>
{
    public override EnSingletonOrder SingletonOrder => EnSingletonOrder.UICharacterMgr;

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
    private void OnEvent_PlayerCreate(in int entityId, in int _, in GameBattlePlayerCreateEventData data)
    {
    }

    public void RefreshAptitude(int charaId, bool notify = true)
    {
        var data = PersistentMgr.Instance.GetData<PersistentCharacterData>(EnPersistentId.Character);
        var charaCfg = GameSchedule.Instance.GetCharacterCfg0(charaId);
        var list = GameAttributeUtility.GetAttributeValueList(charaCfg.nBaseAttributeValueId);
        foreach (var item in list)
        {
            var value = Random.Range(1, 100 + 1);
            data.SetAptitudeValue(charaId, item.attId, value);
        }
        if (notify)
            EventMgr.Instance.FireExecute(EnABBEvent.EVENT_CHARACTER_APTITUDE_REFRESH, charaId);
    }
    public void SetIsLock(int charaId, bool isLock, bool notify = true)
    {
        var data = PersistentMgr.Instance.GetData<PersistentCharacterData>(EnPersistentId.Character);
        data.SetIsLock(charaId, isLock);

        if (!isLock && notify)
        {
            RefreshAptitude(charaId, false);
            EventMgr.Instance.FireExecute(EnABBEvent.EVENT_CHARACTER_UNLOCK, charaId);
        }
    }
    public void SetLevel(int charaId, int level)
    {
        var data = PersistentMgr.Instance.GetData<PersistentCharacterData>(EnPersistentId.Character);
        var lastLevel = data.GetLevel(charaId);
        if (lastLevel == level)
            return;
        data.SetLevel(charaId, level);

        var evUd = ClassPoolMgr.Instance.Pull<GameCharacterUplevelEventData>();
        evUd.Setting(charaId, lastLevel, level);
        EventMgr.Instance.FireExecute(EnABBEvent.EVENT_CHARACTER_UPLEVEL, charaId, 0, evUd);
        ClassPoolMgr.Instance.Push(evUd);
    }
    public void SetActiveCharacterId(int characterId)
    {
        var data = PersistentMgr.Instance.GetData<PersistentCharacterData>(EnPersistentId.Character);
        data.SetActiveCharacterId(characterId);

        EventMgr.Instance.FireExecute(EnABBEvent.EVENT_CHARACTER_ACTIVE, characterId);
    }
    public void SetAptitudeValue(int charaId, EnEntityAttributeId attId, int value)
    {
        var data = PersistentMgr.Instance.GetData<PersistentCharacterData>(EnPersistentId.Character);
        var lastValue = data.GetAptitudeValue(charaId, attId);
        if (lastValue == value)
            return;
        data.SetAptitudeValue(charaId, attId, value);

        var evUd = ClassPoolMgr.Instance.Pull<GameCharacterAptitudeChangeEventData>();
        evUd.Setting(lastValue, value);
        EventMgr.Instance.FireExecute(EnABBEvent.EVENT_CHARACTER_APTITUDE_CHANGE, charaId, (int)attId, evUd);
        ClassPoolMgr.Instance.Push(evUd);
    }
}
