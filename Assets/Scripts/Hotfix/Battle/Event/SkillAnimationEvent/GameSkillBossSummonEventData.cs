using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSkillBossSummonEventData : SkillEventScheduleData
{

    public override EnTypeId GetTypeDefineId() => EnTypeId.GameSkillBossSummonEventData;
    public override void DeserializeFromIntArray(int[] datas, int start, int count, ISerializeToIntArrayUserData userData)
    {
    }

    public override List<int> SerializeToIntArray()
    {
        return null;
    }
}
