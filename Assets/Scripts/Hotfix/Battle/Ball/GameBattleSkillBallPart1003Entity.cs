using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class GameBallPart1003GridData : IClassPoolDestroy
{
    public int effectEntityId;
    public float desTime;
    public void OnPoolDestroy()
    {
        effectEntityId = -1;
        desTime = -1;
    }
}
public class GameBattleSkillBallPart1003EntityData : GameBattleSkillBallPartEntityData<GameBattleSkillBallPart1003Entity, GameBattleSkillBallPart1003StarData>, IUpdate
{
    public override EnLoadTarget LoadTarget => EnLoadTarget.Pre_GameBattleSkillBallPart1003Entity;

    private readonly Dictionary<int, GameBallPart1003GridData> _EffectGridIndex = new();
    private float _LastAtkTime = -1;
    protected override void OnPoolInit(GameBattleSkillBallPartEntityUserData userData)
    {
        base.OnPoolInit(userData);
        var starData = GetBallPartStarData();
        EventMgr.Instance.Register<EntityBuffAddEventBuff>(EnABBEvent.EVENT_ENTITY_BUFF_Add, (int)starData.buffId, 0, OnEvent_EntityBuffAdd);

        UpdateMgr.Instance.Registener(this);
    }
    public override void OnPoolDestroy()
    {
        UpdateMgr.Instance.Unregistener(this);
        foreach (var (_, data) in _EffectGridIndex)
        {
            GameEffectMgr.Instance.DestroyEffect(data.effectEntityId);
            ClassPoolMgr.Instance.Push(data);
        }
        var starData = GetBallPartStarData();
        EventMgr.Instance.Register<EntityBuffAddEventBuff>(EnABBEvent.EVENT_ENTITY_BUFF_Add, (int)starData.buffId, 0, OnEvent_EntityBuffAdd);
        base.OnPoolDestroy();
        _EffectGridIndex.Clear();
        _LastAtkTime = -1;
    }
    public override void GameBallOnCollision(in int entityId, in GamePhysicsHit hit)
    {
    }

    private void OnEvent_EntityBuffAdd(in int nBuffId, in int enityId, in EntityBuffAddEventBuff userData)
    {
        var starData = GetBallPartStarData();
        if (userData.count < starData.buffCount)
            return;
        var entityPointInfo = EntityMgr.Instance.GetEntityData<IEntityGridPointInfo>(in enityId);
        var mainIndex = entityPointInfo.GetMainGridIndex();
        var gridPoss = entityPointInfo.GetGridIndexs();
        for (int i = 0; i < gridPoss.Length; i += 2)
        {
            var index = GameMapUtility.RowColToIndex( mainIndex.row + gridPoss[i], mainIndex.col + gridPoss[i + 1]);
            if (!_EffectGridIndex.TryGetValue(index, out var data))
            {
                var gridWorldPos = GameBattleGridMgr.Instance.GetGridWorldPos(index);
                var effectEntityId = GameEffectMgr.Instance.PlayEffect(EnGameEffectId.Eff_ball_part_1003_2, in gridWorldPos);
                data = ClassPoolMgr.Instance.Pull<GameBallPart1003GridData>();
                data.effectEntityId = effectEntityId;
                _EffectGridIndex.Add(index, data);
            }
            data.desTime = GameUtility.GetGameTime() + starData.atkDuration;
        }
    }

    public void Update()
    {
        var starData = GetBallPartStarData();
        var curTime = GameUtility.GetGameTime();
        if (_LastAtkTime + starData.atkInterval < curTime)
        {
            // 攻击一次
            Attack();
            _LastAtkTime = curTime;
        }

        var keys = _EffectGridIndex.Keys.ToList();
        for (int i = 0; i < keys.Count; i++)
        {
            var gridIndex = keys[i];
            var data = _EffectGridIndex[gridIndex];
            if (data.desTime > curTime)
                continue;
            GameEffectMgr.Instance.DestroyEffect(data.effectEntityId);
            ClassPoolMgr.Instance.Push(data);
            _EffectGridIndex.Remove(gridIndex);
        }
    }
    private void Attack()
    {
        var entitys = new HashSet<int>(_EffectGridIndex.Count * 2);
        var list = new List<int>(3);
        foreach (var (gridIndex, data) in _EffectGridIndex)
        {
            GameBattleGridMgr.Instance.GetGridEntityIds(gridIndex, ref list);
            foreach (var enttiyId in list)
                entitys.Add(enttiyId);
        }
        if (entitys.Count == 0)
            return;

        var starData = GetBallPartStarData();
        var ownerDamage = Mathf.CeilToInt(GetOwnerEntityAttributeDamage(starData.attId) * starData.attPercentage01);
        var damage = starData.atkDamage + ownerDamage;
        foreach (var entityId in entitys)
        {
            var pointCom = EntityMgr.Instance.GetEntityData<IEntityGridPointInfo>(entityId);
            var centerPos = pointCom.GetCenterPointWorldPos();
            GameAttackMgr.Instance.AttackMonster(in _OwnerEntityId, in entityId, in damage, in centerPos, starData.atkDamageType);

            GameEffectMgr.Instance.PlayEffectOnce(EnGameEffectId.Eff_ball_part_1003_3, in centerPos);
        }
        GameEffectMgr.Instance.PlayEffectOnce(EnGameEffectId.Eff_ball_part_1003_1, WorldPos);
        AudioMgr.Instance.Play(EnAudioId.BallPart1003_Effect1);
    }
}
public class GameBattleSkillBallPart1003Entity : GameBattleSkillBallPartEntity
{

}
