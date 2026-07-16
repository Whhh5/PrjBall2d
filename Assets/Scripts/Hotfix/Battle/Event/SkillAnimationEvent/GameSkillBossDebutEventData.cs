using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSkillBossDebutEventData : SkillEventScheduleData
{

    public override EnTypeId GetTypeDefineId() => EnTypeId.GameSkillBossDebutEventData;
    public override void DeserializeFromIntArray(int[] datas, int start, int count, ISerializeToIntArrayUserData userData)
    {
        
    }

    public override List<int> SerializeToIntArray()
    {
        return null;
    }
}
