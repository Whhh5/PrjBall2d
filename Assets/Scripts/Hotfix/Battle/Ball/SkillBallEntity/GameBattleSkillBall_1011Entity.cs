

using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;


public class GameBattleBallLightningEventData : IClassPoolDestroy
{
    public int targetEntityId;
    public int damage;
    public void OnPoolDestroy()
    {
        targetEntityId
            = damage
            = -1;
    }
}
public class GameBattleSkillBall_1011EntityData : GameBattleSkillBallEntityData<GameBattleSkillBall_1011Entity, GameBattleSkillBall_1011StarData>
{
    public override EnLoadTarget LoadTarget => EnLoadTarget.Pre_BattleSkillBall_1011;


    private List<int> _TempEntityIds = new(20);
    private readonly List<int> _TempEntityIds2 = new(10);
    private readonly HashSet<int> _TempEntityIdHas = new(20);

    private CancellationTokenSource _Token;
    protected override void OnPoolInit(GameBattleSkillBallEntityUserData userData)
    {
        base.OnPoolInit(userData);
        _Token = new();
    }

    public override void OnPoolDestroy()
    {
        _Token.Cancel();
        base.OnPoolDestroy();
        _Token = null;
    }
    protected override void OnCollision(in GamePhysicsHit hit)
    {
        var starData = GetStarUserData();
        var ownerAtk = Mathf.CeilToInt(GetOwnerEntityAttackDamage() * starData.atkPercentage);
        var atkValue = ownerAtk + starData.damageValue;
        GameAttackMgr.Instance.Attack(_OwnerEntityId, hit.entityId, atkValue, hit.point, starData.damageType);

        var mapId = GameMapMgr.Instance.GetCurMapId();
        var (row, col) = GameMapUtility.IndexToRowCol(mapId, hit.pointGridIndex);
        var changeRow = (starData.effectRowCount - 1) / 2;
        var changeCol = (starData.effectColCount - 1) / 2;
        _TempEntityIds.Clear();
        _TempEntityIdHas.Clear();
        _TempEntityIds2.Clear();
        for (int i = row - changeRow; i <= row + changeRow; i++)
        {
            for (int j = col - changeCol; j <= col + changeCol; j++)
            {
                if (!GameMapUtility.RowColIsValid(mapId, i, j))
                    continue;
                var gridIndex = GameMapUtility.RowColToIndex(mapId, i, j);
                GameBattleGridMgr.Instance.GetGridEntityIds(gridIndex, ref _TempEntityIds);
            }
        }
        for (int i = 0; i < _TempEntityIds.Count; i++)
        {
            var entityId = _TempEntityIds[i];
            if (entityId == hit.entityId)
                continue;
            if (!_TempEntityIdHas.Add(entityId))
                continue;
            _TempEntityIds2.Add(entityId);
        }

        var atkCount = Mathf.Min(starData.effectAtkCount - 1, _TempEntityIds2.Count);
        var list = new List<int>(atkCount + 1)
        {
            hit.entityId
        };
        for (int i = 0; i < atkCount; i++)
        {
            var index = Random.Range(0, _TempEntityIds2.Count);
            var entityId = _TempEntityIds2[index];
            _TempEntityIds2.RemoveAt(index);
            list.Add(entityId);
        }
        var effectAtkDamage = starData.effectAtkDamage + Mathf.CeilToInt(GetOwnerEntityAttackDamage() * starData.effectAtkPercentage);
        AttackEntitys(list, effectAtkDamage, starData.effectAtkDamageType, starData.effectInterval);
    }

    private async void AttackEntitys(List<int> entityIds, int damage, EnDamageType damageType, float interval)
    {
        for (int i = 0; i < entityIds.Count; i++)
        {
            var entityId = entityIds[i];
            if (!GameMonsterUtility.EntityIsValid(entityId))
                continue;
            var entityData = EntityMgr.Instance.GetEntityData<GameBattleMonsterEntityData>(in entityId);
            var centerPos = entityData.GetCenterPointWorldPos();

            var worldPosXOffset = Random.Range(-0.3f, 0.3f);
            var worldPosYOffset = Random.Range(-0.3f, 0.3f);
            var pos = centerPos + new Vector2(worldPosXOffset, worldPosYOffset);
            GameEffectMgr.Instance.PlayEffectOnce(EnGameEffectId.Eff_buff_Lightning, in pos);
            GameAttackMgr.Instance.Attack(_OwnerEntityId, entityId, damage, in pos, damageType);

            var evUd = ClassPoolMgr.Instance.Pull<GameBattleBallLightningEventData>();
            evUd.damage = damage;
            evUd.targetEntityId = entityId;
            EventMgr.Instance.FireExecute(EnABBEvent.EVENT_BATTLE_BALL_LEIGHTNING, evUd);
            ClassPoolMgr.Instance.Push(evUd);
            var isCancel = await UniTask.WaitForSeconds(interval, cancelImmediately: true, cancellationToken: _Token.Token)
                .SuppressCancellationThrow();
            if (isCancel)
                break;
        }
    }
}
public class GameBattleSkillBall_1011Entity : GameBattleSkillBallEntity
{

}