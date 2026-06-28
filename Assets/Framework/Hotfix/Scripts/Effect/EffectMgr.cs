using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;


public class EffectMgr : Singleton<EffectMgr>
{
    private class EffectInfo : IClassPoolDestroy
    {
        public int effectID = -1;
        public void OnPoolDestroy()
        {
            effectID = -1;
        }
    }
    private class DelayDestroyInfo : IClassPoolDestroy
    {
        public int effectEntityID;
        public float endTime;
        public void OnPoolDestroy()
        {
            endTime = -1;
        }
    }
    public override EnManagerFuncType FuncType => base.FuncType | EnManagerFuncType.Update;
    private Dictionary<int, EffectInfo> _EffectInfos = new();
    private List<DelayDestroyInfo> _DelayDestroyList = new();
    private HashSet<int> _DelayEntityID = new();
    private int _DelayListIndex = 0;

    public bool IsValid(int entityID)
    {
        if (!_EffectInfos.ContainsKey(entityID))
            return false;
        if (_DelayEntityID.Contains(entityID))
            return false;
        return true;
    }

    public int PlayEffectOnce(int effectID, Vector3 worldPos, Vector3 rot, Vector3 scale)
    {
        var entityID = PlayEffect(effectID);
        var entityData = EntityMgr.Instance.GetEntityData<EffectEntityData>(entityID);
        entityData.SetPosition(worldPos);
        entityData.SetLocalRotation(rot);
        entityData.SetLocalScale(scale);
        var time = entityData.GetMaxTime();
        AddDelayDesytroyList(entityID, time);
        return entityID;
    }
    public int PlayEffectOnce(int effectID, Vector3 worldPos)
    {
        var entityID = PlayEffect(effectID);
        var entityData = EntityMgr.Instance.GetEntityData<EffectEntityData>(entityID);
        entityData.SetPosition(worldPos);
        var time = entityData.GetMaxTime();
        AddDelayDesytroyList(entityID, time);
        return entityID;
    }
    public int PlayEffect(int effectID, Vector3 worldPos, Vector3 rot, Vector3 scale)
    {
        var entityID = PlayEffect(effectID);
        var entityData = EntityMgr.Instance.GetEntityData(entityID);
        entityData.SetPosition(worldPos);
        entityData.SetLocalRotation(rot);
        entityData.SetLocalScale(scale);
        return entityID;
    }
    public int PlayEffect(int effectID, Vector3 worldPos)
    {
        var entityID = PlayEffect(effectID);
        var entityData = EntityMgr.Instance.GetEntityData(entityID);
        entityData.SetPosition(worldPos);
        return entityID;
    }
    public int PlayEffect(int effectID)
    {
        var userData = ClassPoolMgr.Instance.Pull<EffectDataUserData>();
        userData.effectCfgID = effectID;
        var entityID = effectID switch
        {
            //1 => EntityMgr.Instance.CreateEntityData<EffectEntityDefaultData>(userData),
            //2 => 
            _ => EntityMgr.Instance.CreateEntityData<EffectEntityDefaultData>(userData),
        };
        ClassPoolMgr.Instance.Push(userData);

        var effectData = EntityMgr.Instance.GetEntityData<EffectEntityData>(entityID);
        effectData.Play();

        EntityMgr.Instance.LoadEntity(entityID);

        var effectInfo = ClassPoolMgr.Instance.Pull<EffectInfo>();
        effectInfo.effectID = effectID;
        _EffectInfos.Add(entityID, effectInfo);
        return entityID;
    }
    public void DestroyEffect(int entityID)
    {
        if (!_EffectInfos.TryGetValue(entityID, out var _))
            return;
        var effectData = EntityMgr.Instance.GetEntityData<EffectEntityData>(entityID);
        effectData.Stop();
        var time = effectData.GetMaxTime();
        AddDelayDesytroyList(entityID, time);
    }
    private void AddDelayDesytroyList(int entityID, float time)
    {
        var desInfo = ClassPoolMgr.Instance.Pull<DelayDestroyInfo>();
        desInfo.effectEntityID = entityID;
        desInfo.endTime = ABBUtil.GetGameTimeSeconds() + time;
        _DelayDestroyList.Add(desInfo);
        _DelayEntityID.Add(entityID);
    }
    public void KillEffect(int entityId)
    {
        if (_DelayEntityID.Contains(entityId))
        {
            // �Ż�
            var desInfo = _DelayDestroyList.Find(item => item.effectEntityID == entityId);
            _DelayDestroyList.Remove(desInfo);
            _DelayEntityID.Remove(entityId);
            ClassPoolMgr.Instance.Push(desInfo);
        }
        InnerKillEffect(entityId);
    }
    private void InnerKillEffect(int entityID)
    {
        if (!_EffectInfos.TryGetValue(entityID, out var effectInfo))
            return;
        _EffectInfos.Remove(entityID);
        EntityMgr.Instance.RecycleEntityData(entityID);
        ClassPoolMgr.Instance.Push(effectInfo);
    }

    public override void Update()
    {
        base.Update();

        var count = _DelayDestroyList.Count;
        if (count > 0)
        {
            _DelayListIndex %= count;
            var desInfo = _DelayDestroyList[_DelayListIndex++];
            if (desInfo.endTime < ABBUtil.GetGameTimeSeconds())
            {
                _DelayDestroyList.RemoveAt(--_DelayListIndex);
                _DelayEntityID.Remove(desInfo.effectEntityID);
                InnerKillEffect(desInfo.effectEntityID);
                ClassPoolMgr.Instance.Push(desInfo);
            }
        }
    }
}
