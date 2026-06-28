using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;


public delegate void PhysicsColliderCallback(ref EntityPhysicsInfo[] entityIDs, ref int count, IPhysicsColliderCallbackCustomData cusomData);

public class PhysicsMgr : Singleton<PhysicsMgr>
{
    public void PhysicsOverlap<T>(in T data, in int entityID, in EnGameLayerInt layer, in PhysicsColliderCallback callback, in IPhysicsColliderCallbackCustomData cusomData)
        where T : IPhysicsResolve
    {
        data.Execute(in entityID, in layer, in callback, in cusomData);
    }
}
