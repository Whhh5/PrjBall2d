using System.Collections.Generic;

public class GameBattleSkillBallPartInfo : ISerializeToIntArray<ISerializeToIntArrayUserData>
{
    public int partId;
    public int partStar;
    public void DeserializeFromIntArray(int[] datas, int start, int count, ISerializeToIntArrayUserData userData)
    {
        var index = 0;
        ISerializeToIntArray.ParseToValue(in datas, in start,in count, ref index, out partId);
        ISerializeToIntArray.ParseToValue(in datas, in start,in count, ref index, out partStar);
    }

    public EnTypeId GetTypeDefineId() => EnTypeId.GameBattleSkillBallPartInfo;

    public void OnPoolDestroy()
    {
    }

    public void OnPoolInit(ISerializeToIntArrayUserData userData)
    {
        DeserializeFromIntArray(userData.arrParams, userData.startIndex, userData.count, userData);
    }

    public List<int> SerializeToIntArray()
    {
        var list = new List<int>();
        ISerializeToIntArray.SerializeToInt(ref list, in partId);
        ISerializeToIntArray.SerializeToInt(ref list, in partStar);
        return list;
    }
}
