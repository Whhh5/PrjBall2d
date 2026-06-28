using System.Collections.Generic;
using UnityEngine;


public class PhysicsResolveBox : IPhysicsResolve
{
    public EnSkillPhysicsType GetSkillPhysicsType() => EnSkillPhysicsType.Box;
    public List<int> SerializeToIntArray()
    {
        var result = new List<int>();
        ISerializeToIntArray.SerializeToInt(ref result, _CenterType);
        ISerializeToIntArray.SerializeToInt(ref result, _ExecuteType);
        ISerializeToIntArray.SerializeToInt(ref result, in _ExecuteTime);
        ISerializeToIntArray.SerializeToInt(ref result, in _UnitSizeZ);
        ISerializeToIntArray.SerializeToInt(ref result, in _BoxSize);
        ISerializeToIntArray.SerializeToInt(ref result, in _RotOffset);
        ISerializeToIntArray.SerializeToInt(ref result, in _PosOffset);
        return result;
    }
    public void DeserializeFromIntArray(int[] datas, int start, int count, ISerializeToIntArrayUserData userData)
    {
        var localIndex = 0;
        ISerializeToIntArray.ParseToValue(in datas, in start, in count, ref localIndex, out _CenterType);
        ISerializeToIntArray.ParseToValue(in datas, in start, in count, ref localIndex, out _ExecuteType);
        ISerializeToIntArray.ParseToValue(in datas, in start, in count, ref localIndex, out _ExecuteTime);
        ISerializeToIntArray.ParseToValue(in datas, in start, in count, ref localIndex, out _UnitSizeZ);
        ISerializeToIntArray.ParseToValue(in datas, in start, in count, ref localIndex, out _BoxSize);
        ISerializeToIntArray.ParseToValue(in datas, in start, in count, ref localIndex, out _RotOffset);
        ISerializeToIntArray.ParseToValue(in datas, in start, in count, ref localIndex, out _PosOffset);
    }
    private EnPhysicsBoxCenterType _CenterType;
    private EnPhysicsBoxType _ExecuteType;
    private float _ExecuteTime;
    private float _UnitSizeZ;
    private Vector3 _BoxSize = Vector3.one;
    private Vector3 _RotOffset;
    private Vector3 _PosOffset;
    public EnTypeId GetTypeDefineId() => EnTypeId.PhysicsResolveBox;

    private EntityPhysicsInfo[] _TempEntityIDs = new EntityPhysicsInfo[20];

    public void OnPoolDestroy()
    {
        _ExecuteTime
            = _UnitSizeZ
            = -1;
        _RotOffset
            = _PosOffset
            = Vector3.zero;
        _BoxSize = Vector3.one;
        _CenterType = EnPhysicsBoxCenterType.None;
        _ExecuteType = EnPhysicsBoxType.None;

    }
    public void OnPoolInit(ISerializeToIntArrayUserData userData)
    {
        DeserializeFromIntArray(userData.arrParams, userData.startIndex, userData.count, userData);
    }

    public void Execute(in int entityID, in EnGameLayerInt layer, in PhysicsColliderCallback callback, in IPhysicsColliderCallbackCustomData cusomData)
    {
        var entityData = EntityMgr.Instance.GetEntityData(entityID);
        var entityRot = entityData.GetLocalRotation();
        var ebtityScale = entityData.GetLocalScale();

        var pos = CalculateUtility.GetPos(entityData.WorldPos, entityData.Forword, entityData.Up, entityData.Right, _PosOffset);

        var targetRot = CalculateUtility.GetRotation(Quaternion.Euler(entityRot), Quaternion.Euler(_RotOffset));
        var unitHalfSize = CalculateUtility.GetScale(in ebtityScale, in _BoxSize) * 0.5f;

        var idCount = EntityUtility.PhysicsOverlapBox(ref _TempEntityIDs, pos, unitHalfSize, targetRot, (int)layer);
        // DebugDrawMgr.Instance.DrawBox(pos, unitHalfSize, targetRot, 0.5f);
        callback(ref _TempEntityIDs, ref idCount, cusomData);
    }

}