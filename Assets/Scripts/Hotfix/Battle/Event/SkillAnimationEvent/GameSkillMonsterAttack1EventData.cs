using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class GameSkillMonsterAttack1EventData : SkillEventScheduleData
{
    public override void DeserializeFromIntArray(int[] datas, int start, int count, ISerializeToIntArrayUserData userData)
    {
    }

    public override EnTypeId GetTypeDefineId() => EnTypeId.GameSkillMonsterAttack1EventData;

    public override List<int> SerializeToIntArray()
    {
        var list = new List<int>();
        return list;
    }
}