using UnityEngine;

public class GameEffectMgr : Singleton<GameEffectMgr>
{
    public int PlayEffectOnce(int effectId, in Vector2 worldPos, in float scale)
    {
        var entityId = EffectMgr.Instance.PlayEffectOnce<GameEffectEntityData>(effectId, worldPos, Vector3.zero, Vector3.one * scale);
        return entityId;
    }
    public int PlayEffectOnce(EnGameEffectId effectId, in Vector2 worldPos, float scale)
    {
        var entityId = PlayEffectOnce((int)effectId, in worldPos, in scale);
        return entityId;
    }
    public int PlayEffectOnce(EnGameEffectId effectId, in Vector2 worldPos)
    {
        var entityId = PlayEffectOnce((int)effectId, in worldPos);
        return entityId;
    }
    public int PlayEffectOnce(int effectId, in Vector2 worldPos)
    {
        var entityId = PlayEffectOnce(effectId, in worldPos, 1);
        return entityId;
    }
    public int PlayEffect(EnGameEffectId effectId, in Vector2 worldPos)
    {
        var entityId = PlayEffect(effectId, in worldPos, 1);
        return entityId;
    }
    public int PlayEffect(EnGameEffectId effectId, in Vector2 worldPos, float scale)
    {
        var entityId = EffectMgr.Instance.PlayEffect<GameEffectEntityData>((int)effectId, worldPos, Vector3.zero, Vector3.one * scale);
        return entityId;
    }
    public void DestroyEffect(int entityId)
    {
        EffectMgr.Instance.DestroyEffect(entityId);
    }
}
