using System.Collections.Generic;
using UnityEngine;


public class GameSwordRuntimeData : IClassPoolDestroy
{
    public float startTime;
    public float moveTime;
    public Vector2 point1;
    public Vector2 point2;
    public HashSet<int> atkedEntityIds = new();
    public int damageValue;

    public int ballId;
    public int star;
    public int ownerEntityId;

    public void OnPoolDestroy()
    {
        atkedEntityIds.Clear();
        point1
            = point2
            = default;
        startTime
            = moveTime
            = ballId
            = star
            = ownerEntityId
            = -1;
    }
}
public class GameBattleSwordSystem : IBattleSystem
{
    private readonly Dictionary<int, GameSwordRuntimeData> _SwordRuntimeDatas = new();
    private readonly List<int> _Keys = new();
    private GamePhysicsHit[] _Hits = new GamePhysicsHit[5];
    public void OnDisable()
    {
        ClearSwordEntitys();
    }

    public void OnEnable()
    {

    }

    public void OnReset()
    {
        ClearSwordEntitys();
    }

    public void OnUpdate()
    {
        _Keys.Clear();
        _Keys.AddRange(_SwordRuntimeDatas.Keys);

        var curTime = GameUtility.GetGameTime();
        foreach (var entityId in _Keys)
        {
            var rtData = _SwordRuntimeDatas[entityId];
            var slider = Mathf.Clamp01((curTime - rtData.startTime) / rtData.moveTime);
            var pos = Vector2.Lerp(rtData.point1, rtData.point2, slider);
            EntityMgr.Instance.SetEntityWorldPos(entityId, pos);

            Attack(in entityId, in pos);

            if (Mathf.Approximately(slider, 1))
            {
                DestroySwordEntity(entityId);
            }
        }
    }

    private void ClearSwordEntitys()
    {
        foreach (var (entityId, rtData) in _SwordRuntimeDatas)
        {
            SceneEntityMgr.Instance.DestroyEntityData(entityId);
            ClassPoolMgr.Instance.Push(rtData);
        }
        _SwordRuntimeDatas.Clear();
    }

    public void DestroySwordEntity(in int entityId)
    {
        var rtData = _SwordRuntimeDatas[entityId];
        SceneEntityMgr.Instance.DestroyEntityData(entityId);
        _SwordRuntimeDatas.Remove(entityId);
        ClassPoolMgr.Instance.Push(rtData);
    }

    public void CreateSwordEntity(in int ballId, in int star, in int damageValue, in int ownerEntityId, in Vector2 point1, in Vector2 point2)
    {
        var swordEntityId = SceneEntityMgr.Instance.CreateEntityData<GameBattleSwordEntityData>();
        EntityMgr.Instance.LoadEntity(swordEntityId);

        var dir = (point2 - point1).normalized;

        var angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        EntityMgr.Instance.SetEntityRotation(swordEntityId, new Vector3(0, 0, angle - 90));
        EntityMgr.Instance.SetEntityWorldPos(swordEntityId, point1);

        var rtData = ClassPoolMgr.Instance.Pull<GameSwordRuntimeData>();
        rtData.ballId = ballId;
        rtData.star = star;
        rtData.point1 = point1;
        rtData.point2 = point2;
        rtData.ownerEntityId = ownerEntityId;
        rtData.startTime = GameUtility.GetGameTime();
        rtData.moveTime = Vector2.Distance(point2, point1) / 50;
        rtData.damageValue = damageValue;

        _SwordRuntimeDatas.Add(swordEntityId, rtData);
    }
    public void Attack(in int entityId, in Vector2 pos)
    {
        var rtData = _SwordRuntimeDatas[entityId];
        var ballStarData = GameBattleBallMgr.Instance.GetBallStarData<GameBattleSkillBall10011008StarData>(rtData.ballId, rtData.star);
        var count = GamePhysicsUtility.SphereCastAll(in pos, in ballStarData.atkRadius, ref _Hits);
        if (count == 0)
            return;

        for (int i = 0; i < count; i++)
        {
            ref readonly var hit = ref _Hits[i];
            if (rtData.atkedEntityIds.Contains(hit.entityId))
                continue;
            rtData.atkedEntityIds.Add(hit.entityId);
            var targetEntityPointInfo = EntityMgr.Instance.GetEntityData<IEntityGridPointInfo>(hit.entityId);
            var center = targetEntityPointInfo.GetCenterPointWorldPos();
            var atkPoint = ballStarData.atkRadius * Random.Range(0.2f, 1f) * (center - pos).normalized + pos;
            GameAttackMgr.Instance.AttackMonster(rtData.ownerEntityId, hit.entityId, rtData.damageValue, in atkPoint, ballStarData.swordDamageType);

            if (ballStarData.star2BuffProbability0100 > 0)
            {
                var testValue = Random.Range(0, 100);
                if (testValue < ballStarData.star2BuffProbability0100)
                {
                    BuffMgr.Instance.AddEntityBuff(rtData.ownerEntityId, hit.entityId, ballStarData.star2BuffData);
                }
            }
        }
    }
}
