using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class GameBattleSkillBallPart10031004EntityData : GameBattleSkillBallPartEntityData<GameBattleSkillBallPartEntity, GameBattleSkillBallPart10031004StarData>, IUpdate
{
    private class RuntimeData : IClassPoolDestroy
    {
        public float startTime;
        public int entityId;

        public void OnPoolDestroy()
        {
            startTime
                = entityId
                = -1;
        }
    }
    public override EnLoadTarget LoadTarget => EnLoadTarget.Pre_GameBattleSkillBallPart10031004Entity;

    private readonly Dictionary<int, RuntimeData> _GridIndex2EntityId = new();
    private readonly List<int> _Keys = new();
    private readonly HashSet<int> _AtkEntityIds = new();
    private List<int> _AtkEntityIdList = new();
    private float _LastAtkTime;

    public override void OnPoolDestroy()
    {
        UpdateMgr.Instance.Unregistener(this);
        foreach (var item in _GridIndex2EntityId)
        {
            GameEffectMgr.Instance.DestroyEffect(item.Value.entityId);
            ClassPoolMgr.Instance.Push(item.Value);
        }
        base.OnPoolDestroy();
        _GridIndex2EntityId.Clear();
        _LastAtkTime = -1;
    }
    protected override void OnPoolInit(GameBattleSkillBallPartEntityUserData userData)
    {
        base.OnPoolInit(userData);
        UpdateMgr.Instance.Registener(this);
    }

    public override void GameBallOnCollision(in int ballEntityId, in GamePhysicsHit hit)
    {

    }

    public void Update()
    {
        CheckEntitys();

        _Keys.Clear();
        _AtkEntityIds.Clear();
        _Keys.AddRange(_GridIndex2EntityId.Keys);
        var starData = GetBallPartStarData();
        var curTime = GameUtility.GetGameTime();


        if (_GridIndex2EntityId.Count > 0)
        {
            if (_LastAtkTime + starData.interval < curTime)
            {
                var ownerDamage = Mathf.CeilToInt(GetOwnerEntityAttributeDamage(starData.attId) * starData.atkPercentage01);
                var damageValue = starData.atkDamage + ownerDamage;
                _LastAtkTime = curTime;
                foreach (var index in _Keys)
                {
                    var rtData = _GridIndex2EntityId[index];
                    _AtkEntityIdList.Clear();
                    GameBattleGridMgr.Instance.GetGridEntityIds(index, ref _AtkEntityIdList);
                    foreach (var entityId in _AtkEntityIdList)
                    {
                        if (_AtkEntityIds.Contains(entityId))
                            continue;
                        _AtkEntityIds.Add(entityId);
                        var pointInfo = EntityMgr.Instance.GetEntityData<GameBattleMonsterEntityData>(entityId);
                        var volume = pointInfo.GetVolume();
                        var center = pointInfo.GetCenterPointWorldPos();
                        var offset = new Vector2()
                        {
                            x = Random.Range(-0.3f, 0.3f),
                            y = Random.Range(-0.3f, 0.3f),
                        } * volume;
                        GameAttackMgr.Instance.AttackMonster(_OwnerEntityId, entityId, damageValue, center + offset, starData.atkDamageType);
                        GameEffectMgr.Instance.PlayEffectOnce(EnGameEffectId.Eff_ball_part_10031004_3, center, volume);
                        AudioMgr.Instance.Play(EnAudioId.Burn);
                    }
                }
            }
        }
        foreach (var index in _Keys)
        {
            var rtData = _GridIndex2EntityId[index];
            if (curTime - rtData.startTime < starData.duration)
                continue;
            GameEffectMgr.Instance.DestroyEffect(rtData.entityId);
            ClassPoolMgr.Instance.Push(rtData);
            _GridIndex2EntityId.Remove(index);
            continue;
        }
    }

    private void CheckEntitys()
    {
        var sys = GameBattleMgr.Instance.GetSystem<GameBattleFireSpinSystem>();
        var entityIds = sys.GetEntityKeys();
        var curTime = GameUtility.GetGameTime();
        foreach (var fireSpinEntityId in entityIds)
        {
            var worldPos = EntityMgr.Instance.GetEntityWorldPos2D(fireSpinEntityId);
            if (!GameMapUtility.WorldPosToGridRowCol(in worldPos, out var fireSpinRow, out var fireSpinCol))
                continue;
            var radius = sys.GetEntityRadius(fireSpinEntityId);
            var offset = Mathf.FloorToInt(radius);
            for (int i = -offset; i < offset; i++)
            {
                var row = fireSpinRow + i;
                if (!GameMapUtility.RowIsValid(row))
                    continue;
                for (int j = -offset; j < offset; j++)
                {
                    var col = fireSpinCol + j;
                    if (!GameMapUtility.ColIsValid(col))
                        continue;
                    var index = GameMapUtility.RowColToIndex(row, col);
                    var indexPos = GameBattleGridMgr.Instance.GetGridWorldPos(index);
                    if (Vector2.SqrMagnitude(indexPos - worldPos) > radius * radius)
                        continue;
                    if (!_GridIndex2EntityId.TryGetValue(index, out var rtData))
                    {
                        var gridPos = GameBattleGridMgr.Instance.GetGridWorldPos(index);
                        rtData = ClassPoolMgr.Instance.Pull<RuntimeData>();
                        var entityId = GameEffectMgr.Instance.PlayEffect(EnGameEffectId.Eff_ball_part_10031004_2, gridPos);
                        rtData.entityId = entityId;
                        _GridIndex2EntityId.Add(index, rtData);
                    }
                    rtData.startTime = curTime;
                }
            }
        }
    }

}
public class GameBattleSkillBallPart10031004Entity : GameBattleSkillBallPartEntity
{

}
