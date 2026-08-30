using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class GameBattleMonster1000301EntityData : GameBattleMonsterBossEntityData<GameBattleMonster1000301Entity>
{
    public override EnLoadTarget LoadTarget => EnLoadTarget.Pre_GameBattleMonster1000301Entity;

    private float _LastSkillTime = -1;
    private readonly float _Skill1Interval = 10;

    protected override Vector2 GetLocalCenterPointPos()
    {
        return new Vector2(1f, -1f);
    }

    protected override void OnPoolInit(GameBattleMonsterEntityUserData userData)
    {
        base.OnPoolInit(userData);
        EventMgr.Instance.Register<GameSkillBossSummonEventData>(EnABBEvent.EVENT_SKILL_EVENT_SCHEDULE, _EntityID, (int)EnSkillEventScheduleId.Summon, OnEvent_SkillBossSummon);

        _LastSkillTime = GetLocalTime();
    }
    public override void OnPoolDestroy()
    {
        EventMgr.Instance.Unregister<GameSkillBossSummonEventData>(EnABBEvent.EVENT_SKILL_EVENT_SCHEDULE, _EntityID, (int)EnSkillEventScheduleId.Summon, OnEvent_SkillBossSummon);

        base.OnPoolDestroy();
        _LastSkillTime = -1;
    }
    protected override void OnEvent_SkillMonsterAttack(in int nEntityId, in int nSkillEventId, in GameSkillMonsterAttack1EventData data)
    {
        base.OnEvent_SkillMonsterAttack(nEntityId, nSkillEventId, data);

        var curTime = GetLocalTime();
        if (_LastSkillTime + _Skill1Interval - curTime < 3)
        {
            _LastSkillTime = GetLocalTime() - _Skill1Interval + 3;
        }
    }

    public override void MonsterUpdate()
    {
        base.MonsterUpdate();
        if (!GameMonsterUtility.EntityIsAttack(_EntityID))
            return;

        var curTime = GetLocalTime();
        if (_LastSkillTime + _Skill1Interval > curTime)
            return;
        var monsterId = GetMonsterCfgId();
        var monsterCfg = GameSchedule.Instance.GetMonsterCfg0(monsterId);
        if (SceneEntityMgr.Instance.HasCmd(_EntityID, monsterCfg.nAttackCmdId))
            return;
        _LastSkillTime = curTime;

        SceneEntityMgr.Instance.AddCmd(_EntityID, EnEntityCmd.Boss1Skill1);

        if (_LastAttackTime + _AtkInterval - curTime < 3)
        {
            _LastSkillTime = GetLocalTime() - _AtkInterval + 3;
        }
    }

    private void OnEvent_SkillBossSummon(in int nEntityId, in int nSkillEventId, in GameSkillBossSummonEventData data)
    {
        ref readonly var mapRowCol = ref GameMapMgr.Instance.GetMapGridRowCol();

        var maxIndex = mapRowCol.x * mapRowCol.y;
        var validGridIndexs = new List<int>(maxIndex);
        for (int i = 0; i < maxIndex; i++)
        {
            if (!GameBattleGridMgr.Instance.GridStatusIs(i, EnGridStatus.Idle))
                continue;
            validGridIndexs.Add(i);
        }

        var createCount = 5;
        var validCount = Mathf.Min(createCount, validGridIndexs.Count);
        var summonEntityIds = new List<int> { 1000101, 1000102, 1000103, 1000104, 1000201 };
        summonEntityIds.Sort((item1, item2) =>
        {
            var itemCfg1 = GameSchedule.Instance.GetMonsterCfg0(item1);
            var itemCfg2 = GameSchedule.Instance.GetMonsterCfg0(item2);
            if (itemCfg1.arrGridLocalPos.Length > itemCfg2.arrGridLocalPos.Length)
                return 1;
            if (itemCfg1.arrGridLocalPos.Length < itemCfg2.arrGridLocalPos.Length)
                return -1;
            return 0;
        });
        for (int i = 0; i < validCount; i++)
        {
            if (validGridIndexs.Count == 0)
                break;
            var listIndex = Random.Range(0, validGridIndexs.Count);
            var gridIndex = validGridIndexs[listIndex];
            var (row, col) = GameMapUtility.IndexToRowCol(gridIndex);

            var testMonsterIndex = Random.Range(0, summonEntityIds.Count);
            for (int j = testMonsterIndex; j >= 0; j--)
            {
                var monsterId = summonEntityIds[j];
                if (!TestMonsterGridIndex(monsterId, row, col))
                    continue;
                var monsterCfg = GameSchedule.Instance.GetMonsterCfg0(monsterId);
                for (int k = 0; k < monsterCfg.arrGridLocalPos.Length; k += 2)
                {
                    var monsterRow = row + monsterCfg.arrGridLocalPos[k];
                    var monsterCol = col + monsterCfg.arrGridLocalPos[k + 1];
                    var monsterIndex = GameMapUtility.RowColToIndex(monsterRow, monsterCol);
                    if (!validGridIndexs.Remove(monsterIndex))
                    {
                        throw new System.Exception($"{monsterIndex}");
                    }
                }
                GameMonsterMgr.Instance.CreateMonsterEntity(monsterId, row, col, 2010101, 1);
                break;
            }
        }

    }
    private bool TestMonsterGridIndex(in int monsterId, in int row, in int col)
    {
        var monsterCfg = GameSchedule.Instance.GetMonsterCfg0(monsterId);
        for (int k = 0; k < monsterCfg.arrGridLocalPos.Length; k += 2)
        {
            var testRow = row + monsterCfg.arrGridLocalPos[k];
            var testCol = col + monsterCfg.arrGridLocalPos[k + 1];
            if (!GameMapUtility.RowColIsValid(testRow, testCol))
                return false;
            var testIndex = GameMapUtility.RowColToIndex(testRow, testCol);
            if (!GameBattleGridMgr.Instance.GridStatusIs(testIndex, EnGridStatus.Idle))
                return false;
        }
        return true;
    }

}
public class GameBattleMonster1000301Entity : GameBattleMonsterBossEntity
{
    public override void SetHpSliderValue(float value)
    {
        //EventMgr.Instance.FireExecute(EnABBEvent.EVENT_BATTLE_BOSS_HP_CHANGE,)
    }
}