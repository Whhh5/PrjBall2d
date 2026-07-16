using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

public class GameBattleSkillBallPart_1002EntityData : GameBattleSkillBallPartEntityData<GameBattleSkillBallPart_1002Entity, GameBattleSkillBallPart_1002StarData>
{
    public override EnLoadTarget LoadTarget => EnLoadTarget.Pre_GameBattleSkillBallPart_1002Entity;

    private GamePhysicsHit[] _Hits = new GamePhysicsHit[20];
    private CancellationTokenSource _Token;

    protected override void OnPoolInit(GameBattleSkillBallPartEntityUserData userData)
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
    public override void GameBallOnCollision(in int entityId, in GamePhysicsHit hit)
    {
        var starData = GetBallPartStarData();
        var testValue = Random.Range(0, 100);
        if (testValue > starData.fireProbability)
            return;

        var mapId = GameMapMgr.Instance.GetCurMapId();
        var mapLb = GameMapUtility.GetGridLbPosition(mapId);
        var mapRu = GameMapUtility.GetGridRuPosition(mapId);
        var endPoint = new Vector2(Random.Range(mapLb.x, mapRu.x), Random.Range(mapLb.y, mapRu.y));

        FlyAnim(hit.point, endPoint);

        GameEffectMgr.Instance.PlayEffectOnce(EnGameEffectId.Eff_ball_part_1002_2, WorldPos);
    }

    private async void FlyAnim(Vector2 startPoint, Vector2 endPoint)
    {
        var starData = GetBallPartStarData();
        var flyEffectId = GameEffectMgr.Instance.PlayEffect(EnGameEffectId.Eff_ball_part_1002_1, startPoint, starData.radius / 0.5f);
        var shadowEffectId = GameEffectMgr.Instance.PlayEffect(EnGameEffectId.Eff_ball_part_1002_4, endPoint, starData.radius / 0.5f);
        var time = Vector3.Distance(startPoint, endPoint) / 5;
        var lastPos = startPoint;
        var isCancel = await DOTween.To(() => 0f, slider =>
        {
            var pos = Vector2.Lerp(startPoint, endPoint, slider);
            var dir = (pos - lastPos).normalized;
            EntityMgr.Instance.SetEntityWorldPos(flyEffectId, pos);

            var angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            EntityMgr.Instance.SetEntityRotation(flyEffectId, new Vector3(0, 0, angle));
            
        }, 1f, time)
            .SetEase(Ease.Linear).ToUniTask(cancellationToken: _Token.Token)
            .SuppressCancellationThrow();

        GameEffectMgr.Instance.DestroyEffect(flyEffectId);
        GameEffectMgr.Instance.DestroyEffect(shadowEffectId);
        if (isCancel)
            return;

        GameEffectMgr.Instance.PlayEffectOnce(EnGameEffectId.Eff_ball_part_1002_3, endPoint, starData.radius / 0.5f);
        AtkSphereMonsters(endPoint, starData.radius);
    }

    private void AtkSphereMonsters(in Vector2 point, in float radius)
    {
        var targetCount = GamePhysicsUtility.SphereCastAll(in point, in radius, ref _Hits);
        if (targetCount > 0)
        {
            var starData = GetBallPartStarData();
            var atkDamage = starData.atkDamage + Mathf.CeilToInt(GetOwnerEntityAttackDamage() * starData.atkPercentage);
            for (int i = 0; i < targetCount; i++)
            {
                ref readonly var hit = ref _Hits[i];
                var pointCom = EntityMgr.Instance.GetEntityData<IEntityGridPointInfo>(hit.entityId);
                var centerPos = pointCom.GetCenterPointWorldPos();
                GameAttackMgr.Instance.Attack(_OwnerEntityId, hit.entityId, atkDamage, centerPos, starData.atkDamageType);
            }
        }
    }
}
public class GameBattleSkillBallPart_1002Entity : GameBattleSkillBallPartEntity
{

}
