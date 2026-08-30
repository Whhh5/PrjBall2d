using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameBattleFireSpinSystem : IBattleSystem
{
    private class FireSpinRuntimeData : IClassPoolDestroy
    {
        public float startTime;
        public float lastAtkTime;
        public float extraRadius;
        public int damageValue;

        public int ballId;
        public int ballStar;
        public int ownerEntityId;
        public Vector2 moveDirection;
        public float moveTime;
        public float delayTime;

        public void OnPoolDestroy()
        {
            startTime
                = moveTime
                = extraRadius
                = lastAtkTime
                = delayTime
                = ballId
                = ballStar
                = ownerEntityId
                = damageValue
                = -1;
            moveDirection = default;
        }
    }
    private readonly Dictionary<int, FireSpinRuntimeData> _FireSpinRuntimeData = new();
    private readonly List<int> _Keys = new();
    private GamePhysicsHit[] _Hits = new GamePhysicsHit[30];
    private float _LastAudioTime = 0f;
    public void OnDisable()
    {
        ClearFireSpinEntitys();
    }

    public void OnEnable()
    {

    }

    public void OnReset()
    {
        ClearFireSpinEntitys();
    }

    public void OnUpdate()
    {
        _Keys.Clear();
        _Keys.AddRange(_FireSpinRuntimeData.Keys);

        var curTime = GameUtility.GetGameTime();
        foreach (var entityId in _Keys)
        {
            var rtData = _FireSpinRuntimeData[entityId];
            var ballStarData = GameBattleBallMgr.Instance.GetBallStarData<GameBattleSkillBall10031004StarData>(rtData.ballId, rtData.ballStar);
            var liftTime = curTime - rtData.startTime;
            if (liftTime > rtData.delayTime)
            {
                var curPos = EntityMgr.Instance.GetEntityWorldPos2D(entityId);
                var nextPos = curPos + rtData.moveDirection * GameUtility.GetGameTimeDelta();
                EntityMgr.Instance.SetEntityWorldPos(entityId, nextPos);
            }

            if (rtData.lastAtkTime + ballStarData.atkInterval < curTime)
            {
                Attack(entityId);
                rtData.lastAtkTime = curTime;
            }

            //var scale = EntityMgr.Instance.GetEntityScale(entityId);
            rtData.extraRadius = liftTime / 3;
            var scale = (ballStarData.atkRadius + rtData.extraRadius) * 2 * Vector3.one;
            EntityMgr.Instance.SetEntityScale(entityId, scale);
            if (liftTime > rtData.moveTime)
            {
                DestroyFireSpinEntity(entityId);
            }
        }
    }

    public List<int> GetEntityKeys()
    {
        return _FireSpinRuntimeData.Keys.ToList();
    }
    public float GetEntityRadius(int entityId)
    {
        var rtData = _FireSpinRuntimeData[entityId];
        var ballStarData = GameBattleBallMgr.Instance.GetBallStarData<GameBattleSkillBall10031004StarData>(rtData.ballId, rtData.ballStar);
        return ballStarData.atkRadius + rtData.extraRadius;
    }

    private void ClearFireSpinEntitys()
    {
        foreach (var (entityId, rtData) in _FireSpinRuntimeData)
        {
            GameEffectMgr.Instance.DestroyEffect(entityId);
            ClassPoolMgr.Instance.Push(rtData);
        }
        _FireSpinRuntimeData.Clear();
    }

    private void DestroyFireSpinEntity(in int entityId)
    {
        var rtData = _FireSpinRuntimeData[entityId];
        GameEffectMgr.Instance.DestroyEffect(entityId);
        ClassPoolMgr.Instance.Push(rtData);
        _FireSpinRuntimeData.Remove(entityId);
    }

    public void CreateFireSpinEntity(in int ownerEntityId, in int ballId, in int ballStar, in Vector2 startPos, in Vector2 moveDir, in int damageValue)
    {
        var ballStarData = GameBattleBallMgr.Instance.GetBallStarData<GameBattleSkillBall10031004StarData>(ballId, ballStar);
        var entityId = GameEffectMgr.Instance.PlayEffect(EnGameEffectId.Eff_buff_huoyanlongjuanfeng, startPos, ballStarData.atkRadius * 2);
        EntityMgr.Instance.LoadEntity(entityId);
        //EntityMgr.Instance.SetEntityWorldPos(entityId, startPos);

        var rtData = ClassPoolMgr.Instance.Pull<FireSpinRuntimeData>();
        rtData.ballId = ballId;
        rtData.ballStar = ballStar;
        rtData.moveDirection = moveDir;
        rtData.moveTime = ballStarData.moveTime;
        rtData.delayTime = 2f;
        rtData.startTime = GameUtility.GetGameTime();
        rtData.extraRadius = 0;
        rtData.ownerEntityId = ownerEntityId;
        rtData.damageValue = damageValue;
        _FireSpinRuntimeData.Add(entityId, rtData);
    }

    private void Attack(in int entityId)
    {
        var curTime = GameUtility.GetGameTime();
        if (curTime - _LastAudioTime > 1f)
        {
            _LastAudioTime = curTime;
            AudioMgr.Instance.Play(EnAudioId.BallPart1003_Effect1);
        }

        var rtData = _FireSpinRuntimeData[entityId];
        var ballStarData = GameBattleBallMgr.Instance.GetBallStarData<GameBattleSkillBall10031004StarData>(rtData.ballId, rtData.ballStar);
        var radius = rtData.extraRadius + ballStarData.atkRadius;
        var pos = EntityMgr.Instance.GetEntityWorldPos2D(entityId);
        var count = GamePhysicsUtility.SphereCastAll(in pos, in radius, ref _Hits);
        if (count == 0)
            return;
        for (int i = 0; i < count; i++)
        {
            ref readonly var hit = ref _Hits[i];
            var pointInfo = EntityMgr.Instance.GetEntityData<IEntityGridPointInfo>(hit.entityId);
            var center = pointInfo.GetCenterPointWorldPos();
            var atkPoint1 = center + new Vector2()
            {
                x = Random.Range(-0.3f, 0.3f),
                y = Random.Range(-0.3f, 0.3f),
            };
            var damageResult = GameAttackMgr.Instance.AttackMonster(rtData.ownerEntityId, hit.entityId, rtData.damageValue, atkPoint1, ballStarData.damageType);
            if (ballStarData.star2DamagePercentage01 > 0)
            {
                var atkPoint2 = center + new Vector2()
                {
                    x = Random.Range(-0.3f, 0.3f),
                    y = Random.Range(-0.3f, 0.3f),
                };
                var value = Mathf.CeilToInt(damageResult.value * ballStarData.star2DamagePercentage01);
                GameAttackMgr.Instance.AttackMonster(rtData.ownerEntityId, hit.entityId, value, atkPoint2, ballStarData.star2DamageType);
            }
        }
    }
}
