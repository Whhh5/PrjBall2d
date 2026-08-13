using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameBallFireSystem : IBattleSystem
{
    private float _LastFireTime = 0;
    public void OnDisable()
    {
    }

    public void OnEnable()
    {
    }

    public void OnReset()
    {
    }

    public void OnUpdate()
    {
        var inputSystem = GameBattleMgr.Instance.GetSystem<GameBattleInputSystem>();
        if (!inputSystem.MouseIsValid())
            return;
        var playerEntityId = GamePlayerMgr.Instance.GetPlayerEntityId();
        var curTime = GameUtility.GetGameTime();
        var interval = GamePlayerMgr.Instance.GetBallFireInterval(playerEntityId);
        if (interval + _LastFireTime > curTime)
            return;
        if (!GameBattleBallMgr.Instance.TryGetNextFireBallId(out var ballId))
            return;
        _LastFireTime = curTime;
        var firePoint = inputSystem.GetFirePos();
        var dir = inputSystem.GetFireDirection();

        GameBattleBallMgr.Instance.CreateBallEntity(playerEntityId, ballId, firePoint, dir);
        var cfg = GameSchedule.Instance.GetSkillBallCfg0(ballId);
        GameEffectMgr.Instance.PlayEffectOnce(cfg.nFireEffectId, firePoint);

        var ballCfg = GameSchedule.Instance.GetSkillBallCfg0(ballId);
        AudioMgr.Instance.Play(ballCfg.nFireAudioId);
    }
}
