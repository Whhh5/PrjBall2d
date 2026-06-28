using System.Collections.Generic;
using UnityEngine;


public class PhysicsResolveCylinder : IPhysicsResolve
{
    public EnSkillPhysicsType GetSkillPhysicsType() => EnSkillPhysicsType.Cylinder;
    public List<int> SerializeToIntArray()
    {
        var result = new List<int>();
        ISerializeToIntArray.SerializeToInt(ref result, in _ExecuteTime);
        ISerializeToIntArray.SerializeToInt(ref result, in _Radius);
        ISerializeToIntArray.SerializeToInt(ref result, in _PosPoint);
        ISerializeToIntArray.SerializeToInt(ref result, in _PosPoint2);
        return result;
    }
    public void DeserializeFromIntArray(int[] datas, int start, int count, ISerializeToIntArrayUserData userData)
    {
        var localIndex = 0;
        ISerializeToIntArray.ParseToValue(in datas, in start, in count, ref localIndex, out _ExecuteTime);
        ISerializeToIntArray.ParseToValue(in datas, in start, in count, ref localIndex, out _Radius);
        ISerializeToIntArray.ParseToValue(in datas, in start, in count, ref localIndex, out _PosPoint);
        ISerializeToIntArray.ParseToValue(in datas, in start, in count, ref localIndex, out _PosPoint2);
    }
    private float _ExecuteTime;
    private float _Radius = 1;
    private Vector3 _PosPoint = new(0, -0.5f, 0);
    private Vector3 _PosPoint2 = new(0, 0.5f, 0);
    public EnTypeId GetTypeDefineId() => EnTypeId.PhysicsResolveCylinder;

    private EntityPhysicsInfo[] _TempEntityIDs = new EntityPhysicsInfo[20];

    public void OnPoolDestroy()
    {
        _ExecuteTime
            = _Radius
            = -1;
        _PosPoint
            = Vector3.zero;

    }
    public void OnPoolInit(ISerializeToIntArrayUserData userData)
    {
        DeserializeFromIntArray(userData.arrParams, userData.startIndex, userData.count, userData);
    }

    public void Execute(in int entityID, in EnGameLayerInt layer, in PhysicsColliderCallback callback, in IPhysicsColliderCallbackCustomData cusomData)
    {
        var entityData = EntityMgr.Instance.GetEntityData(entityID);
        var pos = CalculateUtility.GetPos(entityData.WorldPos, entityData.Forword, entityData.Up, entityData.Right, in _PosPoint);
        var pos2 = CalculateUtility.GetPos(entityData.WorldPos, entityData.Forword, entityData.Up, entityData.Right, in _PosPoint2);

        var idCount = SkillPhysicsMgr.Instance.CylinderCast(in pos, in pos2, in _Radius, in layer);

        callback(ref _TempEntityIDs, ref idCount, cusomData);
    }

}