using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class GamePlayerMgr : Singleton<GamePlayerMgr>, IGameMapLifecycle
{
    private int _PlayerEntityId = -1;
    private float _BallFireInterval;
    private float _BallSpeed;

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

    }
    private void DestroyPlayer()
    {
        SceneEntityMgr.Instance.RemoveCom(_PlayerEntityId, EnSceneEntityComType.Attribute);
        SceneEntityMgr.Instance.DestroyEntityData(_PlayerEntityId);
        _PlayerEntityId = -1;
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
}
