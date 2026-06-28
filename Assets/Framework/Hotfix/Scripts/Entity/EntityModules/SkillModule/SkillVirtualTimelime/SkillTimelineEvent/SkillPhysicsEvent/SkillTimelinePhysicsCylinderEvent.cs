using OfficeOpenXml.FormulaParsing.Excel.Functions.Math;
using System;
using System.Collections.Generic;
using UnityEngine;

public class SkillTimelinePhysicsCylinderEvent : SkillVirtualTimelinePhysicsEvent
{
    private Vector3 _PosOffset;
    private Vector3 _RotOffset;
    private float _Height;
    private float _Radius;
    public override EnTypeId GetTypeDefineId() => EnTypeId.SkillTimelinePhysicsCylinderEvent;
    public override EnSkillPhysicsType GetSkillPhysicsType() => EnSkillPhysicsType.Cylinder;



    public override void DeserializeFromIntArray(int[] datas, int start, int count, ISerializeToIntArrayUserData userData)
    {
        var localIndex = 0;
        ISerializeToIntArray.ParseToValue(in datas, in start, in count, ref localIndex, out _PosOffset.x);
        ISerializeToIntArray.ParseToValue(in datas, in start, in count, ref localIndex, out _PosOffset.y);
        ISerializeToIntArray.ParseToValue(in datas, in start, in count, ref localIndex, out _PosOffset.z);
        ISerializeToIntArray.ParseToValue(in datas, in start, in count, ref localIndex, out _RotOffset.x);
        ISerializeToIntArray.ParseToValue(in datas, in start, in count, ref localIndex, out _RotOffset.y);
        ISerializeToIntArray.ParseToValue(in datas, in start, in count, ref localIndex, out _RotOffset.z);
        ISerializeToIntArray.ParseToValue(in datas, in start, in count, ref localIndex, out _Height);
        ISerializeToIntArray.ParseToValue(in datas, in start, in count, ref localIndex, out _Radius);
    }
    public override List<int> SerializeToIntArray()
    {
        var resultArr = new List<int>();
        ISerializeToIntArray.SerializeToInt(ref resultArr, in _PosOffset.x);
        ISerializeToIntArray.SerializeToInt(ref resultArr, in _PosOffset.y);
        ISerializeToIntArray.SerializeToInt(ref resultArr, in _PosOffset.z);
        ISerializeToIntArray.SerializeToInt(ref resultArr, in _RotOffset.x);
        ISerializeToIntArray.SerializeToInt(ref resultArr, in _RotOffset.y);
        ISerializeToIntArray.SerializeToInt(ref resultArr, in _RotOffset.z);
        ISerializeToIntArray.SerializeToInt(ref resultArr, in _Height);
        ISerializeToIntArray.SerializeToInt(ref resultArr, in _Radius);
        return resultArr;
    }

    public override void OnVirtualTimelineEvent()
    {
        var worldPos = EntityMgr.Instance.GetEntityWorldPos(in _EntityId);
        var layer = SceneEntityMgr.Instance.GetEntityEnemyLayer(in _EntityId);
        ref readonly var count = ref SkillPhysicsMgr.Instance.CylinderCast(in worldPos, Vector3.up * _Height, in _Radius, in layer);

        if (count > 0)
        {
            var atkLog = $"attack target entityId:{_EntityId}, count:{count}, skill:{_SkillId}\n";
            for (int i = 0; i < count; i++)
            {
                ref readonly var entityId = ref SkillPhysicsMgr.Instance.GetLastCastEntityId(in i);

                atkLog += $"entityId:{entityId}, value:{1}\n";

                var eventData = ClassPoolMgr.Instance.Pull<SkillTimelinePhysicsCylinderdEventData>();
                eventData.attackerEntityId = _EntityId;
                eventData.hitEntityId = entityId;
                eventData.timelineEventId = _TimelineEventId;
                eventData.skillId = _SkillId;
                ABBEventMgr.Instance.FireExecute(EnABBEvent.EVENT_SKILL_TIMELINE_PHYSICS, _SkillId, _TimelineEventId, eventData);
                ClassPoolMgr.Instance.Push(eventData);
            }
            Debug.Log(atkLog);
        }
    }
}

