using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class GameBattleMonitorMgr : Singleton<GameBattleMonitorMgr>, IGameMapLifecycle
{
    private int _MonsterDieCount = 0;
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
    public EnGameMapLifecycleOrder GetMapLifecycleOrder() => EnGameMapLifecycleOrder.GameBattleMonitorMgr;
    public void EnterMap(int mapId, int chapterId)
    {
        _MonsterDieCount = 0;
        EventMgr.Instance.Register<GameBattleMonsterDieEventData>(EnABBEvent.EVENT_BATTLE_MONSTER_DIE, 0, 0, OnEvent_MonsterDie);
    }

    public void ExitMap()
    {
        EventMgr.Instance.Unregister<GameBattleMonsterDieEventData>(EnABBEvent.EVENT_BATTLE_MONSTER_DIE, 0, 0, OnEvent_MonsterDie);

    }


    public void ReenterMap()
    {
        _MonsterDieCount = 0;
    }
    private void OnEvent_MonsterDie(in int entityId, in int _, in GameBattleMonsterDieEventData data)
    {
        _MonsterDieCount++;
    }
    public int GetMonsterDieCount()
    {
        return _MonsterDieCount;
    }
}
