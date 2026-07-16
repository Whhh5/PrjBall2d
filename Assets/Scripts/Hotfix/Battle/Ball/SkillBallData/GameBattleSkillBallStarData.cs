
using System.Collections.Generic;

public interface IBattleSkillBallPartInfo
{
    public int GetPartCount();
    public bool ContainsPart();
    public (int partId, int partStar) GetPartInfo(int index);
}

public abstract class GameBattleSkillBallStarData : ISerializeToIntArray<ISerializeToIntArrayUserData>, IBattleSkillBallPartInfo
{
    public EnTypeId GetTypeDefineId()
    {
        throw new System.NotImplementedException();
    }

    public List<GameBattleSkillBallPartInfo> partInfos;

    public virtual void OnPoolDestroy()
    {
        partInfos = null;
    }

    public virtual void OnPoolInit(ISerializeToIntArrayUserData userData)
    {
        DeserializeFromIntArray(userData.arrParams, userData.startIndex, userData.count, userData);
    }

    protected abstract List<int> SerializeToIntArray2();
    protected abstract void DeserializeFromIntArray2(int[] datas, int start, int count, ISerializeToIntArrayUserData userData);

    public List<int> SerializeToIntArray()
    {
        var list = new List<int>();
        ISerializeToIntArray.SerializeToInt(ref list, in partInfos);

        var childList = SerializeToIntArray2();
        list.AddRange(childList);

        return list;
    }
    public void DeserializeFromIntArray(int[] datas, int start, int count, ISerializeToIntArrayUserData userData)
    {
        var index = 0;
        ISerializeToIntArray.ParseToValue(in datas, in start, in count, ref index, out partInfos, in userData);
        DeserializeFromIntArray2(datas, start + index, count, userData);
    }
    public bool ContainsPart()
    {
        return partInfos.Count > 0;
    }


    public int GetPartCount()
    {
        return partInfos.Count;
    }

    public (int partId, int partStar) GetPartInfo(int index)
    {
        var info = partInfos[index];
        return (info.partId, info.partStar);
    }

}