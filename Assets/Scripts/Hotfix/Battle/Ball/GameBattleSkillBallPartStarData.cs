using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameBattleSkillBallPartStarUserData : ISerializeToIntArrayUserData
{
    public int startIndex { get; set; }
    public int count { get; set ; }
    public int[] arrParams { get ; set ; }

    public int partId;
    public int star;
    public void OnPoolDestroy()
    {
        partId
            = star
            = -1;
    }
}

public abstract class GameBattleSkillBallPartStarData : ISerializeToIntArray<GameBattleSkillBallPartStarUserData>, IDescription
{
    protected int _PartId;
    protected int _Star;
    public abstract void DeserializeFromIntArray(int[] datas, int start, int count, ISerializeToIntArrayUserData userData);

    public virtual void GetDescriptionList(ref List<DescriptionInfo> descInfos, EnDescriptionLevel level)
    {
        
    }

    public EnTypeId GetTypeDefineId()
    {
        throw new System.NotImplementedException();
    }

    public virtual void OnPoolDestroy()
    {
    }

    public virtual void OnPoolInit(GameBattleSkillBallPartStarUserData userData)
    {
        _PartId = userData.partId;
        _Star = userData.star;
        // DeserializeFromIntArray(userData.arrParams, userData.startIndex, userData.count, userData);
    }

    public abstract List<int> SerializeToIntArray();
}