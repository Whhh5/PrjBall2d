using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class GamePlayerMgr : Singleton<GamePlayerMgr>, IGameMapLifecycle
{
    private int _CharacterId = -1;
    private int _PlayerEntityId = -1;
    private bool _PlayerIsValid = false;
    public override void Destroy()
    {
        GameMapMgr.Instance.UnregisterLifecycle(this);
        base.Destroy();
        _CharacterId
            = _PlayerEntityId
            = -1;
        _PlayerIsValid = false;
    }
    public override async UniTask AwakeAsync()
    {
        await base.AwakeAsync();
        GameMapMgr.Instance.RegisterLifecycle(this);
    }
    public EnGameMapLifecycleOrder GetMapLifecycleOrder()
    {
        return EnGameMapLifecycleOrder.GamePlayerMgr;
    }
    public void EnterMap(int mapId, int chapterId)
    {
        CreatePlayer();
    }

    public void ExitMap()
    {
        DestroyPlayer();
    }

    public void ReenterMap()
    {
        DestroyPlayer();
        CreatePlayer();
    }
    private void DestroyPlayer()
    {
        var data = ClassPoolMgr.Instance.Pull<GameBattlePlayerDestroyEventData>();
        data.entityId = _PlayerEntityId;
        EventMgr.Instance.FireExecute(EnABBEvent.EVENT_BATTLE_PLAYER_DESTROY, 0, 0, data);
        ClassPoolMgr.Instance.Push(data);

        SceneEntityMgr.Instance.RemoveCom(_PlayerEntityId, EnSceneEntityComType.Misc);
        SceneEntityMgr.Instance.RemoveCom(_PlayerEntityId, EnSceneEntityComType.Attribute);
        SceneEntityMgr.Instance.RemoveCom(_PlayerEntityId, EnSceneEntityComType.Animation);
        SceneEntityMgr.Instance.RemoveCom(_PlayerEntityId, EnSceneEntityComType.Cmd);
        SceneEntityMgr.Instance.DestroyEntityData(_PlayerEntityId);
        _PlayerEntityId = -1;
        _CharacterId = -1;
        _PlayerIsValid = false;
    }

    private void CreatePlayer()
    {
        if (_PlayerEntityId > 0)
            return;
        ref readonly var pos = ref GameMapMgr.Instance.GetMapPlayerStartPosition();

        _CharacterId = UICharacterUtility.GetActiveCharacterId();
        var cfg = GameSchedule.Instance.GetCharacterCfg0(_CharacterId);
        var monsterCfg = GameSchedule.Instance.GetMonsterCfg0(cfg.nMonsterId);
        var userData = ClassPoolMgr.Instance.Pull<GameBattleCharacterEntityUserData>();
        userData.characterId = _CharacterId;
        _PlayerEntityId = SceneEntityMgr.Instance.CreateEntityData(monsterCfg.nEntityTypeId, userData);
        ClassPoolMgr.Instance.Push(userData);

        SceneEntityMgr.Instance.AddCom(_PlayerEntityId, EnSceneEntityComType.Attribute);
        SceneEntityMgr.Instance.AddCom(_PlayerEntityId, EnSceneEntityComType.Misc);
        SceneEntityMgr.Instance.AddCom(_PlayerEntityId, EnSceneEntityComType.Animation);
        SceneEntityMgr.Instance.AddCom(_PlayerEntityId, EnSceneEntityComType.Cmd);
        //GameAttributeUtility.SetEntityAttributes(_PlayerEntityId, cfg.nBaseAttributeValueId);
        SetEntityBaseAttributes(_PlayerEntityId, _CharacterId);

        SceneEntityMgr.Instance.AddCmd(_PlayerEntityId, monsterCfg.nIdleCmdID);

        var miscCom = SceneEntityMgr.Instance.GetEntityCom<EntityMiscComData>(_PlayerEntityId, EnSceneEntityComType.Misc);
        miscCom.SetValue(EnMiscId.BallSpeed, cfg.fBallSpeed);
        miscCom.SetValue(EnMiscId.BallFireInterval, cfg.fBallFireInterval);

        EntityMgr.Instance.SetEntityWorldPos(_PlayerEntityId, pos);
        EntityMgr.Instance.LoadEntity(_PlayerEntityId);
        _PlayerIsValid = true;

        var data = ClassPoolMgr.Instance.Pull<GameBattlePlayerCreateEventData>();
        data.entityId = _PlayerEntityId;
        EventMgr.Instance.FireExecute(EnABBEvent.EVENT_BATTLE_PLAYER_CREATE, _PlayerEntityId, 0, data);
        ClassPoolMgr.Instance.Push(data);

        EventMgr.Instance.Register<GameBattleMouseClickEventData>(EnABBEvent.EVENT_BATTLE_MOUSE_CLICK, 0, 0, OnEvent_MouseClick);
    }
    private void SetEntityBaseAttributes(int entityId, int characterId)
    {
        var attInfos = UICharacterUtility.GetAttributeValues(characterId);
        for (int i = 0; i < attInfos.Count; i++)
        {
            var attInfo = attInfos[i];
            GameAttributeUtility.SetBaseValue(entityId, attInfo.attId, attInfo.value);
            if (attInfo.attId == EnEntityAttributeId.LimitHp)
            {
                GameAttributeUtility.SetBaseValue(entityId, EnEntityAttributeId.Hp, attInfo.value);
            }
        }
    }

    public int GetCharacterId()
    {
        return _CharacterId;
    }
    public int GetPlayerEntityId()
    {
        return _PlayerEntityId;
    }
    public float GetBallFireInterval(int entityId)
    {
        var interval = GameAttributeUtility.GetMiscValue(entityId, EnMiscId.BallFireInterval);
        return interval;
    }
    public float GetBallSpeed(int entityId)
    {
        var speed = GameAttributeUtility.GetMiscValue(entityId, EnMiscId.BallSpeed);
        var speedPercentage = GameAttributeUtility.GetMiscValue(entityId, EnMiscId.BallSpeedPercentage);
        var value = speed * (1 + speedPercentage / 100);
        return value;
    }
    public bool IsValid()
    {
        if (_PlayerEntityId <= 0)
            return false;
        return _PlayerIsValid;
    }
    public void SetValid(bool valid)
    {
        _PlayerIsValid = valid;
        var characterCfg = GameSchedule.Instance.GetCharacterCfg0(_CharacterId);
        var monsterCfg = GameSchedule.Instance.GetMonsterCfg0(characterCfg.nMonsterId);
        if (valid)
        {
            SceneEntityMgr.Instance.RemoveCmd(_PlayerEntityId, monsterCfg.nDieCmdID);
        }
        else
        {
            SceneEntityMgr.Instance.AddCmd(_PlayerEntityId, monsterCfg.nDieCmdID);
        }
    }

    private void OnEvent_MouseClick(in int _, in int __, in GameBattleMouseClickEventData data)
    {
        var entityWoldPos = EntityMgr.Instance.GetEntityWorldPos2D(_PlayerEntityId);
        var disSqr = Vector3.SqrMagnitude(data.worldPos - entityWoldPos);
        if (disSqr > 1)
            return;
        var characterCfg = GameSchedule.Instance.GetCharacterCfg0(_CharacterId);
        var monsterCfg = GameSchedule.Instance.GetMonsterCfg0(characterCfg.nMonsterId);
        var userData = ClassPoolMgr.Instance.Pull<UIBattleEntityAttributeTipsUserData>();
        userData.title = LanguageUtility.GetLanguage(monsterCfg.nNameLangId);
        userData.targetEntityId = _PlayerEntityId;
        UIMgr.Instance.ShowWindow<UIBattleEntityAttributeTipsData>(userData);
        ClassPoolMgr.Instance.Push(userData);
    }

    public void AddHitCmd()
    {
        var characterCfgId = GamePlayerMgr.Instance.GetCharacterId();
        var entityId = GamePlayerMgr.Instance.GetPlayerEntityId();
        var characterCfg = GameSchedule.Instance.GetCharacterCfg0(characterCfgId);
        var monstercfg = GameSchedule.Instance.GetMonsterCfg0(characterCfg.nMonsterId);
        SceneEntityMgr.Instance.AddCmd(entityId, monstercfg.nCmdHitLeft);
    }
    public void AddAttackCmd()
    {
        var characterCfgId = GamePlayerMgr.Instance.GetCharacterId();
        var entityId = GamePlayerMgr.Instance.GetPlayerEntityId();
        var characterCfg = GameSchedule.Instance.GetCharacterCfg0(characterCfgId);
        var monstercfg = GameSchedule.Instance.GetMonsterCfg0(characterCfg.nMonsterId);
        SceneEntityMgr.Instance.AddCmd(entityId, monstercfg.nAttackCmdId);
    }
}
