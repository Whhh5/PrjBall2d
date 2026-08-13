using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GamePart10021006AttackData : IClassPoolDestroy
{
    public float atkTime;
    public HashSet<int> atkedEntityIds = new();
    public void OnPoolDestroy()
    {
        atkTime = -1;
        atkedEntityIds.Clear();
    }
}
public class GameBattleSkillBallPart10021006EntityData : GameBattleSkillBallPartEntityData<GameBattleSkillBallPart10021006Entity, GameBattleSkillBallPart10021006StarData>
{
    public override EnLoadTarget LoadTarget => EnLoadTarget.Pre_GameBattleSkillBallPart10021006Entity;

    private GamePhysicsHit[] _Hits = new GamePhysicsHit[10];
    private readonly Dictionary<int, GamePart10021006AttackData> _FireIndex2AtkedEntityIds = new();
    private readonly List<int> _Keys = new(10);
    public override void OnPoolDestroy()
    {
        foreach (var item in _FireIndex2AtkedEntityIds)
        {
            ClassPoolMgr.Instance.Push(item.Value);
        }
        EventMgr.Instance.Unregister<GameBattleBall10021006AttackEventData>(EnABBEvent.EVENT_BATTLE_BALLL10021006_ATTACK, 0, 0, OnEvent_Attack);
        base.OnPoolDestroy();
        _FireIndex2AtkedEntityIds.Clear();
    }
    protected override void OnPoolInit(GameBattleSkillBallPartEntityUserData userData)
    {
        base.OnPoolInit(userData);
        EventMgr.Instance.Register<GameBattleBall10021006AttackEventData>(EnABBEvent.EVENT_BATTLE_BALLL10021006_ATTACK, 0, 0, OnEvent_Attack);
    }
    public override void GameBallOnCollision(in int ballEntityId, in GamePhysicsHit hit)
    {
        _Keys.Clear();
        _Keys.AddRange(_FireIndex2AtkedEntityIds.Keys);
        var time = GameUtility.GetGameTime();
        foreach (var key in _Keys)
        {
            var atkData = _FireIndex2AtkedEntityIds[key];
            if (time - atkData.atkTime < 3f)
                continue;
            ClassPoolMgr.Instance.Push(atkData);
            _FireIndex2AtkedEntityIds.Remove(key);
            break;
        }
    }
    private void OnEvent_Attack(in int _, in int __, in GameBattleBall10021006AttackEventData data)
    {
        var starData = GetBallPartStarData();
        var testValue = Random.Range(0, 100);
        if (testValue >= starData.atkProbability0100)
            return;
        if (!_FireIndex2AtkedEntityIds.TryGetValue(data.fireIndex, out var atkData))
        {
            atkData = ClassPoolMgr.Instance.Pull<GamePart10021006AttackData>();
            _FireIndex2AtkedEntityIds.Add(data.fireIndex, atkData);
        }

        var damageValue = Mathf.CeilToInt(starData.damageProcentage01 * data.damageValue);

        var pointInfo = EntityMgr.Instance.GetEntityData<IEntityGridPointInfo>(data.targetEntityId);
        var monsterCenter = pointInfo.GetCenterPointWorldPos();
        var count = GamePhysicsUtility.SphereCastAll(in monsterCenter, in starData.radius, ref _Hits);
        for (int i = 0; i < count; i++)
        {
            ref readonly var hit = ref _Hits[i];
            var targetPointInfo = EntityMgr.Instance.GetEntityData<IEntityGridPointInfo>(hit.entityId);
            var targetCenterPos = targetPointInfo.GetCenterPointWorldPos();
            var atkPoint = (monsterCenter - targetCenterPos).normalized * Random.Range(0, 0.5f) + targetCenterPos;
            GameAttackMgr.Instance.AttackMonster(in data.ownerEntityId, in hit.entityId, in damageValue, in atkPoint, in starData.damageType);
            atkData.atkedEntityIds.Add(hit.entityId);
        }
        GameEffectMgr.Instance.PlayEffectOnce(EnGameEffectId.Eff_ball_part_10021006_1, monsterCenter, starData.radius * 2);
        if (count > 0)
        {
            atkData.atkTime = GameUtility.GetGameTime();
        }
    }
}

public class GameBattleSkillBallPart10021006Entity : GameBattleSkillBallPartEntity
{

}
