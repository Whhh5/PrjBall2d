using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;


public class GameLciclcRuntimeData : SceneEntityUserData
{
    public Vector2 dir;
    public int fireIndex;
    public float speed;
    public int damageValue;
    public int ballId;
    public int ballStar;
    public int ownerEntityId;
    public float radius;

    public readonly HashSet<int> ignoreEntityId = new(10);

    public override void OnPoolDestroy()
    {
        base.OnPoolDestroy();
        dir = default;
        speed
            = radius
            = damageValue
            = ownerEntityId
            = ballStar
            = ballId
            = fireIndex
            = -1;
        ignoreEntityId.Clear();
    }
}

public class GameBattleLciclcSystem : IBattleSystem
{
    private readonly Dictionary<int, GameLciclcRuntimeData> _LciclcEntityId2RuntimeData = new();
    private GamePhysicsHit[] _Hits = new GamePhysicsHit[10];
    private readonly List<int> _CheckEntityIds = new(30);

    public void OnEnable()
    {
    }
    public void OnDisable()
    {
        ClearEntity();
    }
    public void OnReset()
    {
        ClearEntity();
    }

    private void ClearEntity()
    {
        foreach (var (entityId, _) in _LciclcEntityId2RuntimeData)
        {
            SceneEntityMgr.Instance.DestroyEntityData(entityId);
        }
        _LciclcEntityId2RuntimeData.Clear();
    }

    public void OnUpdate()
    {
        _CheckEntityIds.Clear();
        _CheckEntityIds.AddRange(_LciclcEntityId2RuntimeData.Keys);
        for (int i = 0; i < _CheckEntityIds.Count; i++)
        {
            var entityId = _CheckEntityIds[i];
            Move(in entityId);
        }
    }

    private void Move(in int entityId)
    {
        var rtData = _LciclcEntityId2RuntimeData[entityId];

        ref readonly var mapLb = ref GameMapMgr.Instance.GetGridLbPosition();
        ref readonly var mapRu = ref GameMapMgr.Instance.GetGridRuPosition();

        var curPos = EntityMgr.Instance.GetEntityWorldPos2D(entityId);
        var nextPos = rtData.speed * GameUtility.GetGameTimeDelta() * rtData.dir + curPos;

        if (nextPos.x < mapLb.x || nextPos.x > mapRu.x || nextPos.y < mapLb.y || nextPos.y > mapRu.y)
        {
            DestroyLciclcEntity(entityId);
            return;
        }

        EntityMgr.Instance.SetEntityWorldPos(entityId, nextPos);

        var count = GamePhysicsUtility.SphereCastAll(nextPos, rtData.radius, ref _Hits);
        if (count > 0)
        {
            for (int i = 0; i < count; i++)
            {
                ref readonly var hit = ref _Hits[i];
                if (rtData.ignoreEntityId.Contains(hit.entityId))
                    continue;
                rtData.ignoreEntityId.Add(hit.entityId);

                Attack(rtData.fireIndex, rtData.ballId, rtData.ballStar, rtData.ownerEntityId, hit.entityId, entityId, rtData.damageValue);
            }
        }
    }

    private void DestroyLciclcEntity(int entityId)
    {
        var worldPos = EntityMgr.Instance.GetEntityWorldPos2D(entityId);
        GameEffectMgr.Instance.PlayEffectOnce(EnGameEffectId.Pre_Eff_Hit_ball10021006_destroy, worldPos);
        SceneEntityMgr.Instance.DestroyEntityData(entityId);
        var rtData = _LciclcEntityId2RuntimeData[entityId];
        ClassPoolMgr.Instance.Push(rtData);
        _LciclcEntityId2RuntimeData.Remove(entityId);
    }
    public int CreateLciclcCreate(in int fireIndex, in int damageValue, in int ballId, in int ballStar, in int ownerEnttyId, in float radius, in Vector2 pos)
    {
        var angle = Mathf.PI * 2 * Random.Range(0, 1f);
        var rtData = ClassPoolMgr.Instance.Pull<GameLciclcRuntimeData>();
        rtData.dir = new Vector2()
        {
            x = Mathf.Cos(angle),
            y = Mathf.Sin(angle),
        };
        rtData.speed = 10f;
        rtData.fireIndex = fireIndex;
        rtData.damageValue = damageValue;
        rtData.ballId = ballId;
        rtData.ballStar = ballStar;
        rtData.ownerEntityId = ownerEnttyId;
        rtData.radius = radius;
        var lciclcEntityId = SceneEntityMgr.Instance.CreateEntityData<GameBattleLciclcEntityData>();
        EntityMgr.Instance.SetEntityWorldPos(lciclcEntityId, pos);
        var rot = Quaternion.AngleAxis((angle - Mathf.PI / 2) * Mathf.Rad2Deg, Vector3.forward).eulerAngles;
        EntityMgr.Instance.SetEntityRotation(lciclcEntityId, rot);
        EntityMgr.Instance.LoadEntity(lciclcEntityId);

        _LciclcEntityId2RuntimeData.Add(lciclcEntityId, rtData);
        return lciclcEntityId;
    }

    private static void Attack(in int fireIndex, in int ballId, in int ballStar, in int ownerEntityId, in int targetEntityId, in int lcicleEntityId, in int damageValue)
    {
        var starData = GameBattleBallMgr.Instance.GetBallStarData<GameBattleSkillBall10021006StarData>(ballId, ballStar);
        var pos = EntityMgr.Instance.GetEntityWorldPos2D(lcicleEntityId);
        var pointInfo = EntityMgr.Instance.GetEntityData<IEntityGridPointInfo>(targetEntityId);
        var entityPos = pointInfo.GetCenterPointWorldPos();
        var dir = entityPos - pos;
        var dis = Vector2.SqrMagnitude(dir);
        var atkPoint = dis > starData.radius * starData.radius
            ? dir.normalized * starData.radius + pos
            : entityPos;
        GameAttackMgr.Instance.AttackMonster(ownerEntityId, targetEntityId, damageValue, atkPoint, starData.lciclcDamageType);
        AudioMgr.Instance.Play(EnAudioId.Frozen);

        if (starData.star2BuffData != null)
        {
            BuffMgr.Instance.AddEntityBuff(ownerEntityId, targetEntityId, starData.star2BuffData);
        }
        if (starData.star3BuffProbability0100 > 0)
        {
            var testValue = Random.Range(0, 100);
            if (testValue < starData.star3BuffProbability0100)
            {
                BuffMgr.Instance.AddEntityBuff(ownerEntityId, targetEntityId, starData.star3BuffData);
            }
        }

        var evUd = ClassPoolMgr.Instance.Pull<GameBattleBall10021006AttackEventData>();
        evUd.targetEntityId = targetEntityId;
        evUd.lciclcEntityId = lcicleEntityId;
        evUd.ownerEntityId = ownerEntityId;
        evUd.damageValue = damageValue;
        evUd.fireIndex = fireIndex;
        EventMgr.Instance.FireExecute(EnABBEvent.EVENT_BATTLE_BALLL10021006_ATTACK, 0, 0, evUd);
        ClassPoolMgr.Instance.Push(evUd);
    }
}
