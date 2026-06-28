using System;
using System.Collections.Generic;
using UnityEngine;

public class SkillTimelineAttackCylinderEvent : SkillTimelineAttackRangeEvent
{
    protected sealed override void DiserializeFromIntArray2(int[] datas, int start, int count, ISerializeToIntArrayUserData userData)
    {
        var localIndex = 0;
        ISerializeToIntArray.ParseToValue(in datas, in start, in count, ref localIndex, out _Height);
        ISerializeToIntArray.ParseToValue(in datas, in start, in count, ref localIndex, out _Radius);
    }
    protected sealed override List<int> SerializeToIntArray2()
    {
        var result = new List<int>();
        ISerializeToIntArray.SerializeToInt(ref result, in _Height);
        ISerializeToIntArray.SerializeToInt(ref result, in _Radius);
        return result;
    }
    private float _Height;
    private float _Radius;

    public override EnTypeId GetTypeDefineId() => EnTypeId.SkillTimelineAttackCylinderEvent;
    public override void OnPoolDestroy()
    {
        base.OnPoolDestroy();
        _Height
            = _Radius
            = -1;
    }

    protected override int CaculateCastEntityIdCount()
    {
        var worldPos = GetCaculateCastWorldPos();
        var rotation = GetCaculateCastRotation();
        ref readonly var layer = ref SceneEntityMgr.Instance.GetEntityEnemyLayer(in _EntityId);
        ref readonly var count = ref SkillPhysicsMgr.Instance.CylinderCast(in worldPos, Vector3.up * _Height, in _Radius, in layer);
        return count;
    }

}

