using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class GameBattleSkillBallPart10071011EntityData : GameBattleSkillBallPartEntityData<GameBattleSkillBallPart10071011Entity, GameBattleSkillBallPart10071011StarData>
{
    public override EnLoadTarget LoadTarget => EnLoadTarget.Pre_GameBattleSkillBallPart10071011Entity;

    private CancellationTokenSource _TokenSource;

    public override void OnPoolDestroy()
    {
        _TokenSource.Cancel();
        base.OnPoolDestroy();
        EventMgr.Instance.Unregister<GameBattleSkillBall_10071011AttackEventData>(EnABBEvent.EVENT_BATTLE_BALLPART10071011_ATTACK, 0, 0, OnEvent_Attack);
        _TokenSource = null;
    }
    protected override void OnPoolInit(GameBattleSkillBallPartEntityUserData userData)
    {
        base.OnPoolInit(userData);
        _TokenSource = new();
        EventMgr.Instance.Register<GameBattleSkillBall_10071011AttackEventData>(EnABBEvent.EVENT_BATTLE_BALLPART10071011_ATTACK, 0, 0, OnEvent_Attack);
    }
    public override void GameBallOnCollision(in int ballEntityId, in GamePhysicsHit hit)
    {

    }

    private void OnEvent_Attack(in int _, in int __, in GameBattleSkillBall_10071011AttackEventData data)
    {
        Attack(data.entityId, data.damageValue, data.damageType);
    }

    private async void Attack(int monsterEntityId, int damageValue, EnDamageType damageType)
    {
        var starData = GetBallPartStarData();
        var pointInfo = EntityMgr.Instance.GetEntityData<IEntityGridPointInfo>(monsterEntityId);
        var center = pointInfo.GetCenterPointWorldPos();
        var atkPos = center + new Vector2()
        {
            x = Random.Range(-0.3f, 0.3f),
            y = Random.Range(-0.3f, 0.3f),
        };
        //GameEffectMgr.Instance.PlayEffectOnce(EnGameEffectId.Eff_ball_part_10071011_2, atkPos);
        var isCancel = await UniTask.Delay((int)(starData.delayTime * 1000), cancellationToken: _TokenSource.Token, cancelImmediately: true)
            .SuppressCancellationThrow();

        if (!isCancel)
        {
            if (GameMonsterUtility.EntityIsValid(monsterEntityId))
            {
                GameAttackMgr.Instance.AttackMonster(_OwnerEntityId, monsterEntityId, damageValue, atkPos, damageType);
                GameEffectMgr.Instance.PlayEffectOnce(EnGameEffectId.Eff_buff_Lightning, atkPos);
                AudioMgr.Instance.Play(EnAudioId.Lightning);
            }
        }
    }
}
public class GameBattleSkillBallPart10071011Entity : GameBattleSkillBallPartEntity
{

}
