using System.Collections.Generic;
using System.Runtime.InteropServices.ComTypes;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

public class GameLeechRuntime : IClassPoolDestroy
{
    public int targetEntityId;
    public Vector2 targetCenterOffset;
    public float createTime;
    public float moveTime;
    public float lastAtkTime;
    public int curStoringValue;

    public int ballId;
    public int ballStar;
    public int ownerEntityId;
    public int damageValue;

    public void OnPoolDestroy()
    {
        createTime
            = moveTime
            = targetEntityId
            = curStoringValue
            = ownerEntityId
            = damageValue
            = ballId
            = ballStar
            = -1;
        targetCenterOffset = default;
    }
}

public class GameBattleLeechSystem : IBattleSystem
{
    private readonly Dictionary<int, GameLeechRuntime> _LeechRuntimes = new();
    private readonly List<int> _Keys = new();
    private readonly List<int> _EffectEntityIds = new();
    private CancellationTokenSource _EffectMoveTokenSource = null;

    public void OnDisable()
    {
        ClearLeechEntitys();
    }

    public void OnEnable()
    {
        _EffectMoveTokenSource = new();
    }

    public void OnReset()
    {
        ClearLeechEntitys();

        _EffectMoveTokenSource = new();
    }

    public void OnUpdate()
    {
        _Keys.Clear();
        _Keys.AddRange(_LeechRuntimes.Keys);
        var curTime = GameUtility.GetGameTime();
        foreach (var entityId in _Keys)
        {
            var rtData = _LeechRuntimes[entityId];
            var ballStarData = GameBattleBallMgr.Instance.GetBallStarData<GameBattleSkillBall10071012StarData>(rtData.ballId, rtData.ballStar);

            if (!GameMonsterUtility.EntityIsValid(rtData.targetEntityId))
            {
                FinishDestroyLeechEntity(entityId);
                continue;
            }
            // 移动
            var posInfo = EntityMgr.Instance.GetEntityData<IEntityGridPointInfo>(rtData.targetEntityId);
            var toPos = posInfo.GetCenterPointWorldPos() + rtData.targetCenterOffset;
            if (rtData.createTime + rtData.moveTime > curTime)
            {
                var curPos = EntityMgr.Instance.GetEntityWorldPos2D(entityId);
                var nextPos = Vector2.Lerp(curPos, toPos, Mathf.Clamp01((curTime - rtData.createTime) / rtData.moveTime));
                EntityMgr.Instance.SetEntityWorldPos(entityId, nextPos);
                SceneEntityMgr.Instance.AddCmd(entityId, EnEntityCmd.LeechMove);
                continue;
            }
            EntityMgr.Instance.SetEntityWorldPos(entityId, toPos);

            if (rtData.lastAtkTime + ballStarData.atkInterval < curTime)
            {
                SceneEntityMgr.Instance.AddCmd(entityId, EnEntityCmd.LeechAttack);
                rtData.lastAtkTime = curTime;
            }
        }
    }

    private void ClearLeechEntitys()
    {
        _EffectMoveTokenSource?.Cancel();
        foreach (var entityId in _EffectEntityIds)
        {
            SceneEntityMgr.Instance.DestroyEntityData(entityId);
        }
        foreach (var (entityId, rtData) in _LeechRuntimes)
        {
            DestroyLeechEntity(entityId);
            ClassPoolMgr.Instance.Push(rtData);
        }
        _LeechRuntimes.Clear();
        _EffectMoveTokenSource = null;
        _EffectEntityIds.Clear();
    }
    private void FinishDestroyLeechEntity(in int entityId)
    {
        var rtData = _LeechRuntimes[entityId];
        var ballStarData = GameBattleBallMgr.Instance.GetBallStarData<GameBattleSkillBall10071012StarData>(rtData.ballId, rtData.ballStar);

        var curPos = EntityMgr.Instance.GetEntityWorldPos2D(entityId);
        PlayRestoreAnim(curPos, rtData.ownerEntityId);
        var addValue = Mathf.CeilToInt(rtData.curStoringValue * ballStarData.addStoringPercentage01);
        GameEntityUtility.AddAttributeValue(rtData.ownerEntityId, rtData.ownerEntityId, ballStarData.addAttribute, addValue);

        var evUd = ClassPoolMgr.Instance.Pull<GameLeachFinishDieEventData>();
        evUd.leechEntityId = entityId;
        evUd.curStoringValue = rtData.curStoringValue;
        EventMgr.Instance.FireExecute(EnABBEvent.EVENT_BATTLE_LEECH_FINISH_DIE, entityId, 0, evUd);
        ClassPoolMgr.Instance.Push(evUd);

        DestroyLeechEntity(entityId);
        _LeechRuntimes.Remove(entityId);
        ClassPoolMgr.Instance.Push(rtData);
    }

    public void DestroyLeechEntity(in int entityId)
    {
        EventMgr.Instance.Unregister<GameSkillMonsterAttack1EventData>(EnABBEvent.EVENT_SKILL_EVENT_SCHEDULE, entityId, (int)EnSkillEventScheduleId.Attack, OnEvent_Attack);
        SceneEntityMgr.Instance.RemoveCom(entityId, EnSceneEntityComType.Animation);
        SceneEntityMgr.Instance.RemoveCom(entityId, EnSceneEntityComType.Cmd);
        SceneEntityMgr.Instance.DestroyEntityData(entityId);
    }
    public void CreateLeechEntity(in int ballId, in int ballStar, in int ownerEntityId, in int depEntityId, in Vector2 pos, in int damageValue)
    {
        var entityId = SceneEntityMgr.Instance.CreateEntityData<GameBattleLeechEntityData>();
        EntityMgr.Instance.SetEntityWorldPos(entityId, pos);
        EntityMgr.Instance.LoadEntity(entityId);

        var monsterEntityData = EntityMgr.Instance.GetEntityData<GameBattleMonsterEntityData>(depEntityId);
        var offset = new Vector2()
        {
            x = Random.Range(-0.3f, 0.3f),
            y = Random.Range(-0.3f, 0.3f),
        } * monsterEntityData.GetVolume();
        var dir = (monsterEntityData.GetCenterPointWorldPos() + offset - pos).normalized;
        EntityMgr.Instance.SetEntityRotation(entityId, new Vector3(0, 0, Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg + 180));


        SceneEntityMgr.Instance.AddCom(entityId, EnSceneEntityComType.Animation);
        SceneEntityMgr.Instance.AddCom(entityId, EnSceneEntityComType.Cmd);
        SceneEntityMgr.Instance.AddCmd(entityId, EnEntityCmd.LeechIdle);

        var rtData = ClassPoolMgr.Instance.Pull<GameLeechRuntime>();
        rtData.createTime = GameUtility.GetGameTime();
        rtData.targetEntityId = depEntityId;
        rtData.targetCenterOffset = offset;
        rtData.moveTime = 1f;
        rtData.ownerEntityId = ownerEntityId;
        rtData.damageValue = damageValue;

        rtData.ballId = ballId;
        rtData.ballStar = ballStar;

        _LeechRuntimes.Add(entityId, rtData);

        EventMgr.Instance.Register<GameSkillMonsterAttack1EventData>(EnABBEvent.EVENT_SKILL_EVENT_SCHEDULE, entityId, (int)EnSkillEventScheduleId.Attack, OnEvent_Attack);

    }

    private void OnEvent_Attack(in int entityId, in int enEventType, in GameSkillMonsterAttack1EventData data)
    {
        var rtData = _LeechRuntimes[entityId];

        var ballStarData = GameBattleBallMgr.Instance.GetBallStarData<GameBattleSkillBall10071012StarData>(rtData.ballId, rtData.ballStar);
        var damageValue = Mathf.CeilToInt(rtData.damageValue + ballStarData.storingPercentage01 * rtData.curStoringValue);

        var atkPoint = EntityMgr.Instance.GetEntityWorldPos2D(entityId);
        var damageResult = GameAttackMgr.Instance.AttackMonster(in rtData.ownerEntityId, in rtData.targetEntityId, in damageValue, in atkPoint, in ballStarData.leechDamageType);

        AudioMgr.Instance.Play(EnAudioId.BallPart1012_Effect1);

        rtData.curStoringValue = Mathf.Min(ballStarData.maxStoringValue, rtData.curStoringValue + damageResult.value);

        var curScale = EntityMgr.Instance.GetEntityScale(entityId);
        var addScale = Vector3.one * damageResult.value / 100;
        EntityMgr.Instance.SetEntityScale(entityId, curScale + addScale);

        if (ballStarData.star2TestStoringValue > 0)
        {
            if (GameMonsterUtility.EntityIsValid(in rtData.targetEntityId))
            {
                if (rtData.curStoringValue > ballStarData.star2TestStoringValue)
                {
                    BuffMgr.Instance.AddEntityBuff(rtData.ownerEntityId, rtData.targetEntityId, ballStarData.star2BuffData);
                }
            }
        }

        if (ballStarData.maxStoringValue == rtData.curStoringValue)
        {
            FinishDestroyLeechEntity(entityId);
        }
    }

    private async void PlayRestoreAnim(Vector2 startPos, int entityId)
    {
        var pointInfo = EntityMgr.Instance.GetEntityData<IEntityGridPointInfo>(entityId);
        var targetPos = pointInfo.GetCenterPointWorldPos();
        var offsetDis = Random.Range(-1f, 1f);
        var effectEntityId = GameEffectMgr.Instance.PlayEffect(EnGameEffectId.Eff_Hit_Xixue_trail, startPos);
        _EffectEntityIds.Add(effectEntityId);
        AudioMgr.Instance.Play(EnAudioId.BallPart1012_Effect1);

        var isCancel = await DOTween.To(() => 0f, slider =>
        {
            if (EntityMgr.Instance.IsValid(entityId))
                targetPos = pointInfo.GetCenterPointWorldPos();

            var toPos = Vector2.Lerp(startPos, targetPos, slider);
            var dir = (targetPos - startPos).normalized;
            var offset = Mathf.Sin(Mathf.Lerp(0, Mathf.PI, slider)) * offsetDis * new Vector2(-dir.y, dir.x);

            EntityMgr.Instance.SetEntityWorldPos(effectEntityId, toPos + offset);
        }, 1f, 0.5f)
            .ToUniTask(cancellationToken: _EffectMoveTokenSource.Token).SuppressCancellationThrow();
        if (!isCancel)
        {
            GameEffectMgr.Instance.PlayEffectOnce(EnGameEffectId.Eff_Hero_Hpadd, targetPos);
            AudioMgr.Instance.Play(EnAudioId.AddHp);
        }
        _EffectEntityIds.Remove(effectEntityId);
        GameEffectMgr.Instance.DestroyEffect(effectEntityId);
    }
}
