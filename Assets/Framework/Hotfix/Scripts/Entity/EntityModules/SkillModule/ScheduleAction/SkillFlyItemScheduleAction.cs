using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

public enum EnSkillFlyItemType
{
    None,
    Line,
    Sin,
    Bezier,
    Forward,
}

public class SkillFlyItemScheduleAction : ISkillScheduleAction
{
    public List<int> SerializeToIntArray()
    {
        var result = new List<int>();
        ISerializeToIntArray.SerializeToInt(ref result, _ScheduleInvoke);
        ISerializeToIntArray.SerializeToInt(ref result, _FlyType);
        ISerializeToIntArray.SerializeToInt(ref result, _MaxDistance);
        ISerializeToIntArray.SerializeToInt(ref result, _PosOffset);
        ISerializeToIntArray.SerializeToInt(ref result, _RotOffset);
        ISerializeToIntArray.SerializeToInt(ref result, _ArrPosData);
        ISerializeToIntArray.SerializeToInt(ref result, _Speed);
        ISerializeToIntArray.SerializeToInt(ref result, _ProjectileCfgId);
        return result;
    }

    public void DeserializeFromIntArray(int[] datas, int start, int count, ISerializeToIntArrayUserData userData)
    {
        var localCount = 0;
        ISerializeToIntArray.ParseToValue(in datas, in start, in count, ref localCount, out _ScheduleInvoke);
        ISerializeToIntArray.ParseToValue(in datas, in start, in count, ref localCount, out _FlyType);
        ISerializeToIntArray.ParseToValue(in datas, in start, in count, ref localCount, out _MaxDistance);
        ISerializeToIntArray.ParseToValue(in datas, in start, in count, ref localCount, out _PosOffset);
        ISerializeToIntArray.ParseToValue(in datas, in start, in count, ref localCount, out _RotOffset);
        ISerializeToIntArray.ParseToValue(in datas, in start, in count, ref localCount, out _ArrPosData);
        ISerializeToIntArray.ParseToValue(in datas, in start, in count, ref localCount, out _Speed);
        ISerializeToIntArray.ParseToValue(in datas, in start, in count, ref localCount, out _ProjectileCfgId);
    }


    private EnSkillFlyItemType _FlyType = EnSkillFlyItemType.None;
    private float _ScheduleInvoke;
    private Vector3 _PosOffset;
    private Vector3 _RotOffset;
    private int[] _ArrPosData;
    private float _MaxDistance = -1;
    private float _Speed = -1;
    private int _ProjectileCfgId;
    public EnTypeId GetTypeDefineId() => EnTypeId.SkillFlyItemScheduleAction;
    public EnAtkLinkScheculeType GetScheduleType() => EnAtkLinkScheculeType.FlyItem;


    public void OnPoolDestroy()
    {
        _FlyType = EnSkillFlyItemType.None;
        _PosOffset = Vector3.zero;
        _ArrPosData = null;
        _MaxDistance
            = _ScheduleInvoke
                = _ProjectileCfgId
                    = -1;
    }

    public void OnPoolInit(ISerializeToIntArrayUserData userData)
    {
        DeserializeFromIntArray(userData.arrParams, userData.startIndex, userData.count, userData);
    }

    public void Reset()
    {
    }

    private async void ScheduleInvoke(int entityId, IClassPoolUserData userData)
    {
        var entityData = EntityMgr.Instance.GetEntityData(in entityId);
        var startPos = CalculateUtility.GetPos(entityData.WorldPos, entityData.Forword, entityData.Up, entityData.Right,
            in _PosOffset);
        var startRot =
            CalculateUtility.GetRotation(Quaternion.Euler(entityData.LocalRotation), Quaternion.Euler(_RotOffset));

        var flyEntityDataId = ProjectileUtility.CreateProjectileEntity(_ProjectileCfgId);

        EntityMgr.Instance.SetEntityWorldPos(in flyEntityDataId, in startPos);
        EntityMgr.Instance.SetEntityRotation(in flyEntityDataId, startRot.eulerAngles);

        var dir = startRot * Vector3.forward;
        var endLocalPos = startPos + _MaxDistance * dir;
        await DOTween.To(() => 0f, slider =>
            {
                var nextPos = GetFlyItemPosition(in slider, in startPos, in endLocalPos);
                EntityMgr.Instance.SetEntityWorldPos(in flyEntityDataId, in nextPos);
            }, 1f, _MaxDistance / _Speed)
            .SetEase(Ease.Linear);

        ProjectileUtility.DestroyProjectileEntity(in flyEntityDataId);
    }

    public Vector3 GetFlyItemPosition(in float slider, in Vector3 startPos, in Vector3 endPos)
    {
        var nextPos = Vector3.Lerp(startPos, endPos, slider);
        return nextPos;
    }    
    // public Quaternion GetFlyItemRotation(in float slider)
    // {
    //     
    // }

    public void GetEventList(ref List<SkillItemEventInfo> eventList)
    {
        var skillEventInfo = ClassPoolMgr.Instance.Pull<SkillItemEventInfo>();
        skillEventInfo.schedule = _ScheduleInvoke;
        skillEventInfo.onEvent = ScheduleInvoke;
        eventList.Add(skillEventInfo);
    }
}