using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;



public class GameBattleSkillBallPart_1008EntityData : GameBattleSkillBallPartEntityData<GameBattleSkillBallPart_1008Entity, GameBattleSkillBallPart_1008StarData>
{
    public override EnLoadTarget LoadTarget => EnLoadTarget.Pre_GameBattleSkillBallPart_1008Entity;

    private CancellationTokenSource _Token;

    private HashSet<int> _RepelEntityIds = new();
    protected override void OnPoolInit(GameBattleSkillBallPartEntityUserData userData)
    {
        base.OnPoolInit(userData);
        _Token = new();
    }
    public override void OnPoolDestroy()
    {
        _RepelEntityIds.Clear();
        _Token.Cancel();
        base.OnPoolDestroy();
        _Token = null;
    }

    public override void GameBallOnCollision(in int ballEntityId, in GamePhysicsHit hit)
    {
        if (_RepelEntityIds.Contains(hit.entityId))
            return;
        _RepelEntityIds.Add(hit.entityId);
        PlayAnim(hit.entityId);
    }

    private async void PlayAnim(int entityId)
    {
        var starData = GetBallPartStarData();
        var monsterEntityData = EntityMgr.Instance.GetEntityData<GameBattleMonsterEntityData>(entityId);
        monsterEntityData.AddMonsterStatus(EnSceneEntityStatus.ForceMove);
        var lastPos = EntityMgr.Instance.GetEntityWorldPos2D(entityId);
        var endPosY = lastPos.y + starData.repelDistance;
        var lastY = lastPos.y;
        var isCencel = await DOTween.To(() => 0, slider =>
        {
            if(!GameMonsterUtility.EntityIsValid(entityId))
                return;

            var curPos = EntityMgr.Instance.GetEntityWorldPos2D(entityId);
            var disY = Mathf.Lerp(lastPos.y, endPosY, slider);
            curPos.y += disY - lastY;
            lastY = disY;

            GameMapUtility.CheckMoveBoundsTopY(ref curPos.y);
            EntityMgr.Instance.SetEntityWorldPos(entityId, curPos);
            GameMonsterMgr.Instance.CheckUpdateEntityGrid(entityId);

        }, 1f, 0.2f)
            .ToUniTask(cancellationToken: _Token.Token).SuppressCancellationThrow();
        if (!isCencel)
        {
            monsterEntityData.RemoveMonsterStatus(EnSceneEntityStatus.ForceMove);
            _RepelEntityIds.Remove(entityId);
        }

    }
}
public class GameBattleSkillBallPart_1008Entity : GameBattleSkillBallPartEntity
{

}
