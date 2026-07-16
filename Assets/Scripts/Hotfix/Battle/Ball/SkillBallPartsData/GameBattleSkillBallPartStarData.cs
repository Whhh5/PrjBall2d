using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class GameBattleSkillBallPartStarData : ISerializeToIntArray<ISerializeToIntArrayUserData>
{
    public abstract void DeserializeFromIntArray(int[] datas, int start, int count, ISerializeToIntArrayUserData userData);

    public EnTypeId GetTypeDefineId()
    {
        throw new System.NotImplementedException();
    }

    public virtual void OnPoolDestroy()
    {
    }

    public virtual void OnPoolInit(ISerializeToIntArrayUserData userData)
    {
        DeserializeFromIntArray(userData.arrParams, userData.startIndex, userData.count, userData);
    }

    public abstract List<int> SerializeToIntArray();
}