
using System.Collections.Generic;

public class GameBattleWaveMonsterInfo : ISerializeToIntArray<ISerializeToIntArrayUserData>
{
    public EnTypeId GetTypeDefineId() => EnTypeId.GameBattleWaveMonsterInfo;
    public int monsterId;
    public int backRow;
    public int col;
    public int attValuesId;

    public void DeserializeFromIntArray(int[] datas, int start, int count, ISerializeToIntArrayUserData userData)
    {
        var index = 0;
        ISerializeToIntArray.ParseToValue(in datas, in start, in count, ref index, out monsterId);
        ISerializeToIntArray.ParseToValue(in datas, in start, in count, ref index, out backRow);
        ISerializeToIntArray.ParseToValue(in datas, in start, in count, ref index, out col);
        ISerializeToIntArray.ParseToValue(in datas, in start, in count, ref index, out attValuesId);
    }

    public List<int> SerializeToIntArray()
    {
        var list = new List<int>();
        ISerializeToIntArray.SerializeToInt(ref list, in monsterId);
        ISerializeToIntArray.SerializeToInt(ref list, in backRow);
        ISerializeToIntArray.SerializeToInt(ref list, in col);
        ISerializeToIntArray.SerializeToInt(ref list, in attValuesId);

        return list;
    }


    public void OnPoolDestroy()
    {
    }

    public void OnPoolInit(ISerializeToIntArrayUserData userData)
    {
        DeserializeFromIntArray(userData.arrParams, userData.startIndex, userData.count, userData);
    }
}