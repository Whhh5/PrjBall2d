using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class GamePlayerMgr : Singleton<GamePlayerMgr>, IGameMapLifecycle
{
    private int _CharacterId = -1;
    private int _PlayerEntityId = -1;
    private float _BallFireInterval;
    private float _BallSpeed;
    private bool _PlayerIsValid = false;
    public override void Destroy()
    {
        GameMapMgr.Instance.UnregisterLifecycle(this);
        base.Destroy();
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

        SceneEntityMgr.Instance.RemoveCom(_PlayerEntityId, EnSceneEntityComType.Attribute);
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
        _BallFireInterval = cfg.fBallFireInterval;
        _BallSpeed = cfg.fBallSpeed;
        _PlayerEntityId = SceneEntityMgr.Instance.CreateEntityData(cfg.nClassTypeId);

        SceneEntityMgr.Instance.AddCom(_PlayerEntityId, EnSceneEntityComType.Attribute);
        //GameAttributeUtility.SetEntityAttributes(_PlayerEntityId, cfg.nBaseAttributeValueId);
        SetEntityBaseAttributes(_PlayerEntityId, _CharacterId);

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
        var attCom = SceneEntityMgr.Instance.GetEntityCom<SceneEntityAttributeComData>(entityId, EnSceneEntityComType.Attribute);
        for (int i = 0; i < attInfos.Count; i++)
        {
            var attInfo = attInfos[i];
            attCom.AddAddtionalValue(attInfo.attId, attInfo.value);
            if (attInfo.attId == EnEntityAttributeId.LimitHp)
            {
                attCom.AddAddtionalValue(EnEntityAttributeId.Hp, attInfo.value);
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
        return _BallFireInterval;
    }
    public float GetBallSpeed(int entityId)
    {
        return _BallSpeed;
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
    }

    private void OnEvent_MouseClick(in int _, in int __, in GameBattleMouseClickEventData data)
    {
        var entityWoldPos = EntityMgr.Instance.GetEntityWorldPos2D(_PlayerEntityId);
        var disSqr = Vector3.SqrMagnitude(data.worldPos - entityWoldPos);
        if (disSqr > 1)
            return;
        var characterCfg = GameSchedule.Instance.GetCharacterCfg0(_CharacterId);
        var userData = ClassPoolMgr.Instance.Pull<UIBattleEntityAttributeTipsUserData>();
        userData.title = LanguageUtility.GetLanguage(characterCfg.nNameLangId);
        userData.targetEntityId = _PlayerEntityId;
        UIMgr.Instance.ShowWindow<UIBattleEntityAttributeTipsData>(userData);
        ClassPoolMgr.Instance.Push(userData);
    }
}
