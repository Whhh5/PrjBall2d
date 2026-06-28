
using UnityEngine;


public struct EntityPhysicsInfo
{
    public Vector3 closestPoint;
    public int entityID;
}
public static class EntityUtility
{
    public static float GetA2bDistance(in int entityId1, in int entityId2)
    {
        var pos1 = EntityMgr.Instance.GetEntityWorldPos(in entityId1);
        var pos2 = EntityMgr.Instance.GetEntityWorldPos(in entityId2);
        var dis = Vector3.Distance(pos1, pos2);
        return dis;
    }
    public static float GetA2bDisSqr(in int entityId1, in int entityId2)
    {
        var pos1 = EntityMgr.Instance.GetEntityWorldPos(in entityId1);
        var pos2 = EntityMgr.Instance.GetEntityWorldPos(in entityId2);
        var sqrDis = Vector3.SqrMagnitude(pos1 - pos2);
        return sqrDis;
    }

    public static Vector3 GetDirection(in int entityId1, in int entityId2)
    {
        var pos1 = EntityMgr.Instance.GetEntityWorldPos(in entityId1);
        var pos2 = EntityMgr.Instance.GetEntityWorldPos(in entityId2);
        var dir = (pos2 - pos1).normalized;
        return dir;
    }

    public static float GetLocalForwardAngle(in int entityId1, in int entityId2)
    {
        var entityData1 = EntityMgr.Instance.GetEntityData(in entityId1);
        var pos1 = EntityMgr.Instance.GetEntityWorldPos(in entityId1);
        var pos2 = EntityMgr.Instance.GetEntityWorldPos(in entityId2);
        var dir = (pos2 - pos1).normalized;
        var angle = Mathf.Acos(Vector3.Dot(entityData1.Forword, dir) / 1f);
        return angle;
    }

    public static void MoveEntity(in int entityId, in Vector3 moveDir, in float length, in bool force = false)
    {
        var proCom = SceneEntityMgr.Instance.GetEntityCom<SceneEntityPropertyComData>(in entityId, EnSceneEntityComType.Property);
        if (!force && !proCom.GetIsCanMove())
            return;
        ref var speed = ref proCom.GetMoveSpeed();

        var rigidCom = SceneEntityMgr.Instance.GetEntityCom<SceneEntityRigidbodyComData>(in entityId, EnSceneEntityComType.Rigidbody);
        rigidCom.MoveIncrement(speed * ABBUtil.GetTimeDelta() * moveDir.normalized * length);

        var monsterCfg = MonsterUtility.GetMonsterCfg(in entityId);
        var cmd = length > 0.7f ? monsterCfg.nRunCmdId : monsterCfg.nWalkCmdId;
        if (!SceneEntityMgr.Instance.HasCmd(in entityId, in cmd))
            SceneEntityMgr.Instance.AddCmd(in entityId, in cmd);
    }
    public static void RotationEntity(in int entityId, in Vector3 lookAt, in bool force = false)
    {
        var proCom = SceneEntityMgr.Instance.GetEntityCom<SceneEntityPropertyComData>(in entityId, EnSceneEntityComType.Property);
        if (!force && !proCom.GetIsCanRotate())
            return;
        var rotation = EntityMgr.Instance.GetEntityRotation(in entityId);

        var rot = Quaternion.LookRotation(lookAt);
        ref readonly var speed = ref proCom.GetRotationSpeed();
        var lerpRot = Quaternion.SlerpUnclamped(Quaternion.Euler(rotation), rot, speed * ABBUtil.GetTimeDelta());

        EntityMgr.Instance.SetEntityRotation(in entityId, lerpRot.eulerAngles);
    }

    public static Vector3 CalculateMoveDir(in int entityId, in Vector3 entityDir, in Vector3 forward)
    {
        var dir = entityDir.normalized;
        var myUp = Vector3.up;
        var myForward = new Vector3(forward.x, 0, forward.z).normalized;
        var myRight = Vector3.Cross(myUp, myForward).normalized;
        var result = dir.x * myRight + dir.z * myForward + dir.y * myUp;
        return result.normalized;
    }
    public static void MoveEntityByEntityDir(in int entityId, in Vector3 entityDir, in float length)
    {
        var entityData = EntityMgr.Instance.GetEntityData(in entityId);
        var dir = CalculateMoveDir(in entityId, in entityDir, entityData.Forword);
        EntityUtility.MoveEntity(in entityId, in dir, in length);
    }
    public static void RotationEntityByEntityDir(in int entityId, in Vector3 entityDir)
    {
        var entityData = EntityMgr.Instance.GetEntityData(in entityId);
        var moveDir = CalculateMoveDir(in entityId, in entityDir, entityData.Forword);
        EntityUtility.RotationEntity(in entityId, in moveDir);
    }

    static public bool IsValid(int entityID)
    {
        if (!SceneEntityMgr.Instance.EntityIsValid(entityID))
            return false;
        if (SceneEntityMgr.Instance.HasEntityCom(entityID, EnSceneEntityComType.Life))
        {
            var lifeCom = SceneEntityMgr.Instance.GetEntityCom<EntityLifeComData>(entityID, EnSceneEntityComType.Life);
            if (lifeCom.IsDie())
                return false;
        }
        return true;
    }

    private static readonly Collider[] _TempCollider = new Collider[100];
    private static EntityPhysicsInfo[] _TempEntityID = new EntityPhysicsInfo[100];
    public static bool PhysicsOverlapSphere1(Vector3 worldPos, float radius, int layer, out int entityID)
    {
        var count = Physics.OverlapSphereNonAlloc(worldPos, radius, _TempCollider, layer);
        if (count > 0)
        {
            for (int i = 0; i < count; i++)
            {
                var col = _TempCollider[i];
                if (!col.TryGetComponent<SceneEntity>(out var entityCom))
                    continue;
                if (!IsValid(entityCom))
                    continue;
                entityID = entityCom;
                return true;
            }
        }
        entityID = -1;
        return false;
    }
    static public ref EntityPhysicsInfo[] PhysicsOverlapSphere(out int count, Vector3 worldPos, float radius, int layer)
    {
        var colliderCount = Physics.OverlapSphereNonAlloc(worldPos, radius, _TempCollider, layer);
        count = 0;
        for (int i = 0; i < colliderCount; i++)
        {
            var col = _TempCollider[i];
            if (!col.TryGetComponent<SceneEntity>(out var entityCom))
                continue;
            if (!IsValid(entityCom))
                continue;

            ref var element = ref _TempEntityID[count++];
            element.entityID = entityCom;
            element.closestPoint = col.ClosestPoint(worldPos);
        }
        return ref _TempEntityID;
    }
    //static public ref int[] PhysicsOverlapBox(out int count, Vector3 worldPos, Vector3 halfSize, Quaternion qua, int layer)
    //{
    //    count = 0;
    //    return ref _TempEntityID;
    //}
    static public int PhysicsOverlapBox(ref EntityPhysicsInfo[] entityIDs, Vector3 worldPos, Vector3 halfSize, Quaternion qua, int layer)
    {
        var count = Physics.OverlapBoxNonAlloc(worldPos, halfSize, _TempCollider, qua, layer);
        var idCount = 0;
        for (int i = 0; i < count; i++)
        {
            var col = _TempCollider[i];
            if (!col.TryGetComponent<SceneEntity>(out var entityCom))
                continue;
            if (!IsValid(entityCom))
                continue;

            ref var element = ref entityIDs[idCount++];
            element.entityID = entityCom;
            element.closestPoint = col.ClosestPoint(worldPos);
        }
        return idCount;
    }
}