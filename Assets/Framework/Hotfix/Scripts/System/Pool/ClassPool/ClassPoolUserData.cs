
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class PoolNaNUserData : IClassPoolUserData
{
    public void OnPoolDestroy()
    {
    }
}
public class ABBEventDataUserData : IClassPoolUserData
{
    public EnABBEvent abbevent;

    public void OnPoolDestroy()
    {
        abbevent = EnABBEvent.NONE;
    }
}
public class Entity3DComDataUserData : IClassPoolUserData
{
    public int entityID;
    public void OnPoolDestroy()
    {
        entityID = -1;
    }
}
public class EntityBuffDataUserData : IClassPoolUserData
{
    public int targetEntityID;
    public int sourceEntityID;
    public EnBuff buff;
    public void OnPoolDestroy()
    {
        targetEntityID = -1;
        buff = EnBuff.None;
    }
}
public class LayerMixerInfoUserData : IClassPoolUserData
{
    public EnAnimLayer layer;
    public ScriptPlayable<BridgePlayableAdapter> layerAdapter;
    public void OnPoolDestroy()
    {
        layer = EnAnimLayer.None;
        layerAdapter = ScriptPlayable<BridgePlayableAdapter>.Null;
    }
}
public class PlayableAdapterUserData : IClassPoolUserData
{
    public PlayableGraphAdapter graph;
    public Playable rootPlayable;

    public virtual void OnPoolDestroy()
    {
        rootPlayable = default;
        graph = null;
    }
}
public class PlayableGraphAdapterUserData : IClassPoolUserData
{
    public int entityID;
    public Animator anim;

    public void OnPoolDestroy()
    {
        entityID = -1;
        anim = null;
    }
}

public class EntityNoMovementBuffData : IEntityBuffParams
{
    public void OnPoolDestroy()
    {
    }

    public void OnPoolInit(ISerializeToIntArrayUserData userData)
    {
        DeserializeFromIntArray(userData.arrParams, userData.startIndex, userData.count, userData);
    }

    public EnBuff GetBuff() => EnBuff.NoMovement;
    public EnTypeId GetTypeDefineId() => EnTypeId.EntityNoMovementBuffData;

    public List<int> SerializeToIntArray()
    {
        return null;
    }

    public void DeserializeFromIntArray(int[] datas, int start, int count, ISerializeToIntArrayUserData userData)
    {
        
    }

}

public abstract class AttackLinkUserData : IClassPoolUserData
{
    public abstract void OnPoolDestroy();
}

public class MonsterBaseDataUserData: MonsterEntityDataUserData
{
    public int monsterID;
}
public abstract class PlayableBehaviourAdapterUserData : IClassPoolUserData
{
    public virtual void OnPoolDestroy()
    {

    }
}
public abstract class CmdPlayableAdapterUserData : PlayableAdapterUserData
{

}

public class SkillBehaviourUserData : IClassPoolUserData
{
    public int[] arrValue;
    public void OnPoolDestroy()
    {
        arrValue = null;
    }
}
public sealed class GeneralIntUserData : IClassPoolUserData
{
    public int intValue;
    public void OnPoolDestroy()
    {
        intValue = -1;
    }
}
public sealed class GeneralInt2UserData : IClassPoolUserData
{
    public int intValue;
    public int intValue2;
    public void OnPoolDestroy()
    {
        intValue
            = intValue2
            = -1;
    }
}

public class AIModuleUserData : IClassPoolUserData
{
    public int entityID;
    public int aiModuleCfgID;
    public int moduleDataID;
    public virtual void OnPoolDestroy()
    {
        aiModuleCfgID
            = entityID
            = moduleDataID
            = -1;
    }
}

public class EffectDataUserData : GameEntityDataUserData
{
    public int effectCfgID;
    public override void OnPoolDestroy()
    {
        base.OnPoolDestroy();
        effectCfgID
            = -1;
    }
}

