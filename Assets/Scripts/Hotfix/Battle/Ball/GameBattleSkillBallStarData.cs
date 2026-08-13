
using System.Collections.Generic;

public interface IBattleSkillBallPartInfo
{
    public int GetPartCount();
    public bool ContainsPart();
    public (int partId, int partStar) GetPartInfo(int index);
}

public abstract class GameBattleSkillBallStarData : ISerializeToIntArray<GameBattleSkillBallStarUserData>, IBattleSkillBallPartInfo, IDescription
{
    protected int _BallId;
    protected int _Star;
    public EnTypeId GetTypeDefineId()
    {
        throw new System.Exception();
    }

    public List<GameBattleSkillBallPartInfo> partInfos;

    public virtual void OnPoolDestroy()
    {
        partInfos = null;
        _BallId
            = _Star
            = -1;
    }

    public virtual void OnPoolInit(GameBattleSkillBallStarUserData userData)
    {
        _BallId = userData.ballId;
        _Star = userData.star;
        //DeserializeFromIntArray(userData.arrParams, userData.startIndex, userData.count, userData);
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

    public virtual void GetDescriptionList(ref List<DescriptionInfo> descInfos, EnDescriptionLevel level)
    {
        if (partInfos.Count == 0)
            return;
        for (int i = 0; i < partInfos.Count; i++)
        {
            var partInfo = partInfos[i];

            var info = ClassPoolMgr.Instance.Pull<DescriptionInfo>();
            info.level = level;
            info.title = LanguageUtility.GetLanguage_10002();
            info.content = LanguageUtility.GetLanguage_BallPartStyleName(partInfo.partId);
            descInfos.Add(info);

            var partStarData = GameBattleBallPartsMgr.Instance.GetBallPartStarData(partInfo.partId, partInfo.partStar);
            partStarData.GetDescriptionList(ref descInfos, level + 1);

        }
    }
}