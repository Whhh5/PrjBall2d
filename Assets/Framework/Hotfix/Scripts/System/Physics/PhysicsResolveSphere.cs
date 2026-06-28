using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PhysicsResolveSphere : IPhysicsResolve
{
    public EnSkillPhysicsType GetSkillPhysicsType() => EnSkillPhysicsType.Sphere;
    public EnTypeId GetTypeDefineId() => EnTypeId.PhysicsResolveSphere;


    public List<int> SerializeToIntArray()
    {
        var result = new List<int>();
        ISerializeToIntArray.SerializeToInt(ref result, in _Radius);
        ISerializeToIntArray.SerializeToInt(ref result, in _PosOffset);
        return result;
    }
    public void DeserializeFromIntArray(int[] datas, int start, int count, ISerializeToIntArrayUserData userData)
    {
        var localIndex = 0;
        ISerializeToIntArray.ParseToValue(in datas, in start, in count, ref localIndex, out _Radius);
        ISerializeToIntArray.ParseToValue(in datas, in start, in count, ref localIndex, out _PosOffset);
    }
    private float _Radius = 1;
    private Vector3 _PosOffset;

    public void OnPoolDestroy()
    {
    }
    public void OnPoolInit(ISerializeToIntArrayUserData userData)
    {
    }



    public void Execute(in int entityID, in EnGameLayerInt layer, in PhysicsColliderCallback callback, in IPhysicsColliderCallbackCustomData cusomData)
    {
        var entityData = EntityMgr.Instance.GetEntityData(entityID);
        var pos = CalculateUtility.GetPos(entityData.WorldPos, entityData.Forword, entityData.Up, entityData.Right, _PosOffset);


        ref var enittyIDs = ref EntityUtility.PhysicsOverlapSphere(out var count, pos, _Radius, (int)layer);
        //DebugDrawMgr.Instance.DrawSphere(pos, m_Radius, 0.5f);
        callback.Invoke(ref enittyIDs, ref count, cusomData);
    }
}