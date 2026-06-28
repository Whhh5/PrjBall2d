using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SkillBehaviour : ISkillBehaviour
{
    public abstract void Execute(int entityID);
    public abstract EnSkillBehaviourType SkillBehavioueType();
    public abstract EnTypeId GetTypeDefineId();
    public abstract List<int> SerializeToIntArray();
    public abstract void DeserializeFromIntArray(int[] datas, int start, int count, ISerializeToIntArrayUserData userData);

    public virtual void OnPoolDestroy()
    {
    }
    public void OnPoolInit(ISerializeToIntArrayUserData userData)
    {
        DeserializeFromIntArray(userData.arrParams, userData.startIndex, userData.count, userData);
    }
}
