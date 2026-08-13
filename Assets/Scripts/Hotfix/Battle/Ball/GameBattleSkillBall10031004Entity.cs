using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameBattleSkillBall10031004EntityData : GameBattleSkillBallEntityData<GameBattleSkillBall10031004Entity, GameBattleSkillBall10031004StarData>
{
    public override EnLoadTarget LoadTarget => EnLoadTarget.Pre_GameBattleSkillBall10031004Entity;

    protected override void OnCollision(in GamePhysicsHit hit)
    {
        var sys = GameBattleMgr.Instance.GetSystem<GameBattleFireSpinSystem>();
        var star = GameBattleBallMgr.Instance.GetBallStar(_SkillBallId);
        var angle = Random.Range(0, Mathf.PI * 2);
        var dir = new Vector2()
        {
            x = Mathf.Cos(angle),
            y = Mathf.Sin(angle),
        };
        sys.CreateFireSpinEntity(_OwnerEntityId, _SkillBallId, star, hit.point, dir);

        AudioMgr.Instance.Play(EnAudioId.Dark);

        GameBattleBallMgr.Instance.DestroyBallEntity(_EntityID);
    }
}

public class GameBattleSkillBall10031004Entity : GameBattleSkillBallEntity
{

}
