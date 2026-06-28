

using UnityEngine;

public class SkillPhysicsMgr : Singleton<SkillPhysicsMgr>
{
    private int _LastCastEntityCount = -1;
    private readonly int[] _CastEntityIds = new int[50];
    private readonly int[] _EntityRaycastHitIndexs = new int[50];
    private readonly RaycastHit[] _RaycastHits = new RaycastHit[50];

    public ref int GetLastCastEntityId(in int index)
    {
        return ref _CastEntityIds[index];
    }
    public ref RaycastHit GetLastCastRaycastHit(in int index)
    {
        ref var hitIndex = ref _EntityRaycastHitIndexs[index];
        return ref _RaycastHits[hitIndex];
    }
    public ref readonly int SphereCast(in Vector3 origin, in float radius, in EnGameLayerInt layerMask)
    {
        var hitCount = Physics.SphereCastNonAlloc(origin, radius, Vector3.forward, _RaycastHits, 0, (int)layerMask, QueryTriggerInteraction.Ignore);

        ref readonly var entityIds = ref CaculateValidEntityId(in _RaycastHits, in hitCount);
        return ref _LastCastEntityCount;
    }
    public ref readonly int CylinderCast(in Vector3 origin, in Vector3 point2, in float radius, in EnGameLayerInt layerMask)
    {
        var hitCount = Physics.CapsuleCastNonAlloc(origin, point2, radius, Vector3.forward, _RaycastHits, 0, (int)layerMask, QueryTriggerInteraction.Ignore);

        ref readonly var entityIds = ref CaculateValidEntityId(in _RaycastHits, in hitCount);

        for (int i = 0; i < _LastCastEntityCount; i++)
        {
            ref readonly var entityId = ref entityIds[i];
            var entityData = EntityMgr.Instance.GetEntityData(in entityId);
            ref readonly var worldPos = ref entityData.GetWorldPos();
            if (worldPos.y <= point2.y && worldPos.y >= origin.y)
                continue;
            entityIds[i--] = entityIds[--_LastCastEntityCount];
        }
        return ref _LastCastEntityCount;
    }

    private ref readonly int[] CaculateValidEntityId(in RaycastHit[] hits, in int hitCount)
    {
        _LastCastEntityCount = 0;
        for (int i = 0; i < hitCount; i++)
        {
            ref var hit = ref hits[i];
            if (!hit.collider.TryGetComponent<GameEntity>(out var entity))
                continue;

            var entityId = (int)entity;
            if (!SceneEntityMgr.Instance.EntityIsValid(in entityId))
                continue;

            _EntityRaycastHitIndexs[_LastCastEntityCount] = i;
            _CastEntityIds[_LastCastEntityCount++] = entityId;

        }
        return ref _CastEntityIds;
    }
}
