using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class GameBattleMonsterEliteEntityData<T> : GameBattleMonsterEntityData<T>
    where T: GameBattleMonsterEliteEntity
{
    private float _LastAttackTime;
    private readonly float _AtkInterval = 3f;
    protected override void OnPoolInit(GameBattleMonsterEntityUserData userData)
    {
        base.OnPoolInit(userData);

        EventMgr.Instance.Register<GameSkillMonsterAttack1EventData>(EnABBEvent.EVENT_SKILL_EVENT_SCHEDULE, _EntityID, (int)EnSkillEventScheduleId.Attack, OnEvent_SkillMonsterAttack);

    }
    public override void OnPoolDestroy()
    {
        EventMgr.Instance.Unregister<GameSkillMonsterAttack1EventData>(EnABBEvent.EVENT_SKILL_EVENT_SCHEDULE, _EntityID, (int)EnSkillEventScheduleId.Attack, OnEvent_SkillMonsterAttack);

        base.OnPoolDestroy();
        _LastAttackTime = -1;
    }
    public override void EnterMapDown()
    {

    }
    public override void UpdateMapDown()
    {
        if (!GamePlayerMgr.Instance.IsValid())
            return;
        var curTime = GetLocalTime();
        if (_LastAttackTime + _AtkInterval > curTime)
            return;
        var monsterId = GetMonsterCfgId();
        var monsterCfg = GameSchedule.Instance.GetMonsterCfg0(monsterId);
        if (!SceneEntityMgr.Instance.IsAddCmd(_EntityID, monsterCfg.nAttackCmdId))
            return;
        SceneEntityMgr.Instance.AddCmd(_EntityID, monsterCfg.nAttackCmdId);

        _LastAttackTime = curTime;
    }
    public override void ExitMapDown()
    {

    }

    private void OnEvent_SkillMonsterAttack(in int nEntityId, in int nSkillEventId, in GameSkillMonsterAttack1EventData data)
    {
        var entityAttCom = SceneEntityMgr.Instance.GetEntityCom<SceneEntityAttributeComData>(nEntityId, EnSceneEntityComType.Attribute);
        var atkDamage = entityAttCom.GetValueInt(EnEntityAttributeId.Attack);
        var playerEntityId = GamePlayerMgr.Instance.GetPlayerEntityId();
        var pointInfo = EntityMgr.Instance.GetEntityData<IEntityGridPointInfo>(playerEntityId);
        var atkPoint = pointInfo.GetCenterPointWorldPos();
        GameAttackMgr.Instance.AttackPlayer(nEntityId, playerEntityId, atkDamage, atkPoint, EnDamageType.Physic);
    }
}
public abstract class GameBattleMonsterEliteEntity : GameBattleMonsterEntity
{
    [SerializeField]
    private SpriteRenderer _HpSlider;

    [SerializeField]
    private float _HpMaxSliderValue;


    public override void SetHpSliderValue(float value)
    {
        _HpSlider.size = new Vector2(_HpMaxSliderValue * value, _HpSlider.size.y);
    }
}
