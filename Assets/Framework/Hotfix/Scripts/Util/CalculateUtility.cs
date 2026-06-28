


using UnityEngine;

public static class CalculateUtility
{
    public static Vector3 GetPosOffset(in Vector3 target, in Vector3 forward, in Vector3 up, in Vector3 right, in Vector3 target2)
    {
        var localPos = target2 - target;
        var offset = new Vector3(Vector3.Dot(localPos, right), Vector3.Dot(localPos, up), Vector3.Dot(localPos, forward));
        return offset;
    }
    public static Vector3 GetPosOffset(in Transform tran, in Vector3 target2)
    {
        var offset = GetPosOffset(tran.position, tran.forward, tran.up, tran.right, in target2);
        return offset;
    }
    public static Vector3 GetPos(in Vector3 target, in Vector3 forward, in Vector3 up, in Vector3 right, in Vector3 offset)
    {
        var pos = target + right * offset.x + up * offset.y + forward * offset.z;
        return pos;
    }
    public static Vector3 GetPos(in Transform tran, in Vector3 offset)
    {
        var pos = GetPos(tran.position, tran.forward, tran.up, tran.right, in offset);
        return pos;
    }
    public static Vector3 GetScaleOffset(in Vector3 target, in Vector3 target2)
    {
        var scale = new Vector3(target2.x / target.x, target2.y / target.y, target2.z / target.z);
        return scale - Vector3.one;
    }
    public static Vector3 GetScale(in Vector3 target, in Vector3 offset)
    {
        var scale = new Vector3(offset.x * target.x, offset.y * target.y, offset.z * target.z);
        return scale + target;
    }
    public static Quaternion GetRotationOffset(in Quaternion target, in Quaternion target2)
    {
        var rot = Quaternion.Inverse(target) * target2;
        return rot;
    }
    public static Quaternion GetRotation(in Quaternion target, in Quaternion offset)
    {
        var rot = target * offset;
        return rot;
    }
}