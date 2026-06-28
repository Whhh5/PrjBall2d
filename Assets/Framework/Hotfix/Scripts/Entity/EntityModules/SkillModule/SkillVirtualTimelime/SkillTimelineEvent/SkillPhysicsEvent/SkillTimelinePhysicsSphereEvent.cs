using System.Collections.Generic;
using UnityEngine;

public class SkillTimelinePhysicsSphereEvent : SkillVirtualTimelinePhysicsEvent
{
    public override EnSkillPhysicsType GetSkillPhysicsType() => EnSkillPhysicsType.Sphere;
    public override EnTypeId GetTypeDefineId() => EnTypeId.SkillTimelinePhysicsSphereEvent;

    private Vector3 _OffsetPos;
    private float _Radius;
    private int _Count;
    private EnSkillPhysicsRayType _RayType;


    public override void DeserializeFromIntArray(int[] datas, int start, int count, ISerializeToIntArrayUserData userData)
    {
        var index = 0;
        ISerializeToIntArray.ParseToValue(in datas, in start, in count, ref index, out _OffsetPos.x);
        ISerializeToIntArray.ParseToValue(in datas, in start, in count, ref index, out _OffsetPos.y);
        ISerializeToIntArray.ParseToValue(in datas, in start, in count, ref index, out _OffsetPos.z);
        ISerializeToIntArray.ParseToValue(in datas, in start, in count, ref index, out _Radius);
        ISerializeToIntArray.ParseToValue(in datas, in start, in count, ref index, out _Count);
        ISerializeToIntArray.ParseToValue(in datas, in start, in count, ref index, out int rayType);
        _RayType = (EnSkillPhysicsRayType)rayType;
    }


    public override void OnVirtualTimelineEvent()
    {
        ref readonly var entityPos = ref EntityMgr.Instance.GetEntityWorldPos(in _EntityId);
        ref readonly var entityEnemyLayer = ref SceneEntityMgr.Instance.GetEntityEnemyLayer(in _EntityId);
        var pos = entityPos + _OffsetPos;
        ref readonly var entityListCount = ref SkillPhysicsMgr.Instance.SphereCast(in pos, _Radius, in entityEnemyLayer);

        Debug.Log($"sphere target entity posOffset:{_OffsetPos}, radius:{_Radius}, count:{_Count}, rayType:{_RayType} --");
        for (int i = 0; i < entityListCount; i++)
        {
            ref readonly var entityId = ref SkillPhysicsMgr.Instance.GetLastCastEntityId(in i);
            Debug.Log($"sphere target entityId = {entityId}");
        }
        Debug.Log($"sphere target entity count = {entityListCount} --------------");
    }

    public override List<int> SerializeToIntArray()
    {
        var result = new List<int>();

        ISerializeToIntArray.SerializeToInt(ref result, in _OffsetPos.x);
        ISerializeToIntArray.SerializeToInt(ref result, in _OffsetPos.y);
        ISerializeToIntArray.SerializeToInt(ref result, in _OffsetPos.z);
        ISerializeToIntArray.SerializeToInt(ref result, in _Radius);
        ISerializeToIntArray.SerializeToInt(ref result, in _Count);
        ISerializeToIntArray.SerializeToInt(ref result, (int)_RayType);

        return result;
    }
}