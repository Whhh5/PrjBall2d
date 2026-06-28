using System;
using System.Collections.Generic;

public class SkillVirtualTimelineAnimtionPlayableNodeUserData : VirtualTimelineUserData
{
    public int entityId;
}
public class SkillVirtualTimelineAnimationPlayableNode : ISkillVirtualTimelinePlayableNode
{
    public List<int> SerializeToIntArray()
    {
        throw new NotImplementedException();
    }
    public void DiserializeFromIntArray(int[] datas, int start, int count, VirtualTimelineUserData userData)
    {
        throw new NotImplementedException();
    }

    // private int _AnimationId = -1;
    public EnTypeId GetTypeDefineId() => EnTypeId.SkillVirtualTimelineAnimationPlayableNode;

    // private int _EntityId = -1;
    public void OnPoolDestroy()
    {
        // _EntityId
        //     = _AnimationId
        //     = -1;
    }

    public void OnPoolInit(SkillVirtualTimelineAnimtionPlayableNodeUserData userData)
    {

        //var entityData = EntityMgr.Instance.GetEntityData<SceneEntityData>(userData.entityId);

        //var animCom = entityData.GetEntityComponent<EntityAnimComData>();
        //animCom.AddCmd();
    }

    public float GetVirtualTimelineNodeDurationTime()
    {
        throw new NotImplementedException();
    }

    public bool IsVirtualTimelineNodeEnd(in float nodeLocalTime, in float deltaTime, in float nodeProgress01)
    {
        throw new NotImplementedException();
    }


    public void OnVirtualTimelineNodeEnd()
    {
        throw new NotImplementedException();
    }

    public void OnVirtualTimelineNodeStart()
    {
        throw new NotImplementedException();
    }

    public void OnVirtualTimelineNodeUpdate(in float nodeLocalTime, in float deltaTime, in float nodeProgress01)
    {
        throw new NotImplementedException();
    }

    public void DeserializeFromIntArray(int[] datas, int start, int count, ISerializeToIntArrayUserData userData)
    {
        throw new NotImplementedException();
    }
}

