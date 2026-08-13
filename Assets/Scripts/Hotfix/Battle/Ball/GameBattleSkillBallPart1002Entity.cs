using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

public class GameBattleSkillBallPart1002EntityData : GameBattleSkillBallPartEntityData<GameBattleSkillBallPart1002Entity, GameBattleSkillBallPart1002StarData>
{
    public override EnLoadTarget LoadTarget => EnLoadTarget.Pre_GameBattleSkillBallPart1002Entity;

    private GamePhysicsHit[] _Hits = new GamePhysicsHit[20];
    private CancellationTokenSource _Token;

    private readonly List<int> _TargetIndexPool = new(30);
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
        _TargetIndexPool.Clear();
    }
    public override void GameBallOnCollision(in int entityId, in GamePhysicsHit hit)
    {
        var starData = GetBallPartStarData();
        var testValue = Random.Range(0, 100);
        if (testValue > starData.fireProbability0100)
            return;

        var mapId = GameMapMgr.Instance.GetCurMapId();

        var rangeMax = 5;
        var rangeMin = 1;

        var pointRowCol = GameMapUtility.IndexToRowCol(mapId, hit.pointGridIndex);
        _TargetIndexPool.Clear();
        for (int row = pointRowCol.row - rangeMax; row <= pointRowCol.row + rangeMax; row++)
        {
            if (!GameMapUtility.RowIsValid(mapId, row))
                continue;
            if (row > pointRowCol.row - rangeMin && row <= pointRowCol.row + rangeMin)
                continue;
            for (int col = pointRowCol.col - rangeMax; col <= pointRowCol.col + rangeMax; col++)
            {
                if (!GameMapUtility.ColIsValid(mapId, col))
                    continue;
                if (col > pointRowCol.col - rangeMin && col <= pointRowCol.col + rangeMin)
                    continue;
                var testIndex = GameMapUtility.RowColToIndex(mapId, in row, in col);
                _TargetIndexPool.Add(testIndex);
            }
        }
        var index = _TargetIndexPool[Random.Range(0, _TargetIndexPool.Count)];
        var worldPos = GameBattleGridMgr.Instance.GetGridWorldPos(index);

        FlyAnim(hit.point, worldPos);

        GameEffectMgr.Instance.PlayEffectOnce(EnGameEffectId.Eff_ball_part_1002_2, WorldPos);
    }

    private async void FlyAnim(Vector2 startPoint, Vector2 endPoint)
    {
        var starData = GetBallPartStarData();
        var flyEffectId = GameEffectMgr.Instance.PlayEffect(EnGameEffectId.Eff_ball_part_1002_1, startPoint, starData.radius / 0.5f);
        var shadowEffectId = GameEffectMgr.Instance.PlayEffect(EnGameEffectId.Eff_ball_part_1002_4, endPoint, starData.radius / 0.5f);
        var time = Vector3.Distance(startPoint, endPoint) / 5;
        var lastPos = startPoint;
        AudioMgr.Instance.Play(EnAudioId.BallPart1002_Effect1);
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
        AudioMgr.Instance.Play(EnAudioId.BallPart1002_Effect2);
    }

    private void AtkSphereMonsters(in Vector2 point, in float radius)
    {
        var targetCount = GamePhysicsUtility.SphereCastAll(in point, in radius, ref _Hits);
        if (targetCount > 0)
        {
            var starData = GetBallPartStarData();
            var atkDamage = starData.atkDamage + Mathf.CeilToInt(GetOwnerEntityAttributeDamage(starData.attId) * starData.atkPercentage01);
            for (int i = 0; i < targetCount; i++)
            {
                ref readonly var hit = ref _Hits[i];
                var pointCom = EntityMgr.Instance.GetEntityData<IEntityGridPointInfo>(hit.entityId);
                var centerPos = pointCom.GetCenterPointWorldPos();
                GameAttackMgr.Instance.AttackMonster(_OwnerEntityId, hit.entityId, atkDamage, centerPos, starData.atkDamageType);
            }
        }
    }
}
public class GameBattleSkillBallPart1002Entity : GameBattleSkillBallPartEntity
{

}
