using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class GamePlayerMgr : Singleton<GamePlayerMgr>, IGameMapLifecycle
{
    private int _PlayerEntityId = -1;
    private float _BallFireInterval;
    private float _BallSpeed;
    private bool _PlayerIsValid = false;
    public override async UniTask AwakeAsync()
    {
        await base.AwakeAsync();
        GameMapMgr.Instance.RegisterLifecycle(this);
    }
    public override void Destroy()
    {
        GameMapMgr.Instance.UnregisterLifecycle(this);
        base.Destroy();
    }
    public EnGameMapLifecycleOrder GetMapLifecycleOrder()
    {
        return EnGameMapLifecycleOrder.GamePlayerMgr;
    }
    public void EnterMap(int mapId)
    {
        CreatePlayer();
    }

    public void ExitMap(int mapId)
    {
        DestroyPlayer();
    }

    public void ReenterMap(int mapId)
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
        _PlayerIsValid = false;
    }

    private void CreatePlayer()
    {
        if (_PlayerEntityId > 0)
            return;
        var mapId = GameMapMgr.Instance.GetCurMapId();
        var pos = GameMapUtility.GetMapPlayerStartPosition(mapId);

        var playerId = 1;
        var cfg = GameSchedule.Instance.GetCharacterCfg0(playerId);
        _BallFireInterval = cfg.fBallFireInterval;
        _BallSpeed = cfg.fBallSpeed;
        _PlayerEntityId = SceneEntityMgr.Instance.CreateEntityData(cfg.nClassTypeId);

        SceneEntityMgr.Instance.AddCom(_PlayerEntityId, EnSceneEntityComType.Attribute);
        GameAttributeUtility.SetEntityAttributes(_PlayerEntityId, cfg.nAttributeValueCfgId);

        EntityMgr.Instance.SetEntityWorldPos(_PlayerEntityId, pos);
        EntityMgr.Instance.LoadEntity(_PlayerEntityId);
        _PlayerIsValid = true;

        var data = ClassPoolMgr.Instance.Pull<GameBattlePlayerCreateEventData>();
        data.entityId = _PlayerEntityId;
        EventMgr.Instance.FireExecute(EnABBEvent.EVENT_BATTLE_PLAYER_CREATE, 0, 0, data);
        ClassPoolMgr.Instance.Push(data);

        EventMgr.Instance.Register<GameBattleMouseClickEventData>(EnABBEvent.EVENT_BATTLE_MOUSE_CLICK, 0, 0, OnEvent_MouseClick);
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
        var userData = ClassPoolMgr.Instance.Pull<UIBattleEntityAttributeWindowUserData>();
        userData.title = "玩家";
        userData.targetEntityId = _PlayerEntityId;
        UIMgr.Instance.ShowWindow<UIBattleEntityAttributeWindowData>(userData);
        ClassPoolMgr.Instance.Push(userData);
    }
}
