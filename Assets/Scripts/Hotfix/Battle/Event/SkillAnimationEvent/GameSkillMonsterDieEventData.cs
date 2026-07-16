
using System.Collections.Generic;

public class GameSkillMonsterDieEventData : SkillEventScheduleData
{
    public override void DeserializeFromIntArray(int[] datas, int start, int count, ISerializeToIntArrayUserData userData)
    {
    }

    public override EnTypeId GetTypeDefineId() => EnTypeId.GameSkillMonsterDieEventData;

    public override List<int> SerializeToIntArray()
    {
        return null;
    }
}