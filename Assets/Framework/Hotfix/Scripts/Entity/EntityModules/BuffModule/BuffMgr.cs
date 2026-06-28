using System.Collections.Generic;
using UnityEngine;



public class BuffMgr : Singleton<BuffMgr>
{
    #region struct
    private class AddBuffInfo : IClassPoolNone
    {
        public int buffDataID;
        public int entityID;
        public EnBuff buff;
    }
    private class EntityBuffInfo : IClassPoolDestroy
    {
        //private Dictionary<EnBuffType, List<EnBuff>> _BuffInfo = new();
        private readonly Dictionary<EnBuff, int> _Buff2DataIDDic = new();
        public int Count => _Buff2DataIDDic.Count;

        public void OnPoolDestroy()
        {
            _Buff2DataIDDic.Clear();
        }
        public bool Contains(EnBuff buff)
        {
            if (!_Buff2DataIDDic.ContainsKey(buff))
                return false;
            return true;
        }
        public int GetBuffDataID(EnBuff buff)
        {
            return _Buff2DataIDDic[buff];
        }
        public bool TryGetBuffDataID(EnBuff buff, out int buffDataID)
        {
            if (!_Buff2DataIDDic.TryGetValue(buff, out buffDataID))
                return false;
            return true;
        }
        public void AddBuff(EnBuff buff, int buffDataID)
        {
            _Buff2DataIDDic.Add(buff, buffDataID);
        }
        public void RemoveBuff(EnBuff buff)
        {
            _Buff2DataIDDic.Remove(buff);
        }
        public void GetAllEntityBuffDataID(ref List<int> ids)
        {
            foreach (var item in _Buff2DataIDDic)
                ids.Add(item.Value);
        }
    }
    #endregion

    public override EnManagerFuncType FuncType => base.FuncType | EnManagerFuncType.Update;
    private Dictionary<int, AddBuffInfo> _AddKeyData = new();
    private Dictionary<int, HashSet<int>> _BuffDataID2AddKey = new();

    private Dictionary<int, EntityBuffInfo> _Entity2BuffInfo = new();
    private Dictionary<int, ISkillEntityBuff> _ID2BuffDataDic = new();

    //private List<int> _TimeBuffKeys = new(10);
    private Dictionary<EnBuffType, Dictionary<int, IBuffInfo>> _BuffTypeList = new();

    public EnBuffType GetBuffType(EnBuff buff)
    {
        var buffCfg = GameSchedule.Instance.GetBuffCfg0((int)buff);
        return (EnBuffType)buffCfg.nBuffType;
    }
    public bool IsValidAddKey(int addKey)
    {
        if (!_AddKeyData.ContainsKey(addKey))
            return false;
        return true;
    }
    public int AddEntityBuff(int sourceEntityID, int targetEntityID, EnBuff buff)
    {
        throw new System.NotImplementedException();
    }
    public int AddEntityBuff(int sourceEntityID, int targetEntityID, IEntityBuffParams buffParams)
    {
        var buff = buffParams.GetBuff();
        if (!SceneEntityMgr.Instance.HasEntityCom(targetEntityID, EnSceneEntityComType.Buff))
            return -1;
        var addKey = ABBUtil.GetTempKey();

        if (!_Entity2BuffInfo.TryGetValue(targetEntityID, out var entityBuffInfo))
        {
            entityBuffInfo = ClassPoolMgr.Instance.Pull<EntityBuffInfo>();
            _Entity2BuffInfo.Add(targetEntityID, entityBuffInfo);
        }

        if (!entityBuffInfo.TryGetBuffDataID(buff, out var buffDataID))
        {
            buffDataID = CreateEntityBuffData(sourceEntityID, targetEntityID, buff);
            var buffData = GetBuffData(buffDataID);
            buffData.OnEnable(addKey, buffParams);
            entityBuffInfo.AddBuff(buff, buffDataID);
        }
        else
        {
            var buffData = GetBuffData(buffDataID);
            buffData.ReOnEnable(addKey, buffParams);
        }

        var buffInfo = ClassPoolMgr.Instance.Pull<AddBuffInfo>();
        buffInfo.buff = buff;
        buffInfo.entityID = targetEntityID;
        buffInfo.buffDataID = buffDataID;
        _AddKeyData.Add(addKey, buffInfo);

        if (!_BuffDataID2AddKey.TryGetValue(buffDataID, out var addKeyList))
        {
            addKeyList = new();
            _BuffDataID2AddKey.Add(buffDataID, addKeyList);
        }
        addKeyList.Add(addKey);

        UpdateBuffTypeInfo(buffDataID, buffParams);
        //var buffCom = Entity3DMgr.Instance.GetEntityCom<EntityBuffComData>(targetEntityID);
        //buffCom.AddBuff(addKey);

        return addKey;
    }
    private List<int> _TempList = new(10);
    public void RemoveEntityBuffByEntityID(int entityID)
    {
        if (!_Entity2BuffInfo.TryGetValue(entityID, out var buffInfo))
            return;
        _TempList.Clear();
        buffInfo.GetAllEntityBuffDataID(ref _TempList);
        for (int i = 0; i < _TempList.Count; i++)
        {
            var dataID = _TempList[i];
            var list = _BuffDataID2AddKey[dataID];
            foreach (var addKey in list)
                RemoveEntityBuff2(addKey);
            _BuffDataID2AddKey.Remove(dataID);
        }
    }
    public void RemoveEntityBuff(int addBuffKey)
    {
        if (!_AddKeyData.TryGetValue(addBuffKey, out var addInfo))
            return;
        if (_BuffDataID2AddKey.TryGetValue(addInfo.buffDataID, out var list))
            if (list.Remove(addBuffKey))
                if (list.Count == 0)
                    _BuffDataID2AddKey.Remove(addInfo.buffDataID);
        RemoveEntityBuff2(addBuffKey);
    }

    private void RemoveEntityBuff2(int addBuffKey)
    {
        if (!_AddKeyData.TryGetValue(addBuffKey, out var addInfo))
            return;

        //var animCom = Entity3DMgr.Instance.GetEntityCom<EntityBuffComData>(addInfo.entityID);
        //animCom.RemoveBuff(addBuffKey);
        _AddKeyData.Remove(addBuffKey);


        if (!_Entity2BuffInfo.TryGetValue(addInfo.entityID, out var entityBuffInfo))
            return;

        if (!entityBuffInfo.TryGetBuffDataID(addInfo.buff, out var buffDataID))
            return;

        var buffData = GetBuffData(buffDataID);

        buffData.OnDisable(addBuffKey);

        if (buffData.IsRemove())
        {
            entityBuffInfo.RemoveBuff(addInfo.buff);
            if (entityBuffInfo.Count == 0)
            {
                _Entity2BuffInfo.Remove(addInfo.entityID);
                ClassPoolMgr.Instance.Push(entityBuffInfo);
            }
            RemoveBuffTypeInfo(buffDataID);
            RemoveEntityBuffData(buffDataID);
        }

        ClassPoolMgr.Instance.Push(addInfo);
    }
    public bool ContainsBuff(int addBuffKey)
    {
        if (!_AddKeyData.ContainsKey(addBuffKey))
            return false;
        return true;
    }
    public bool ContainsBuff(int entityID, int addBuffKey)
    {
        if (!_AddKeyData.TryGetValue(addBuffKey, out var addInfo))
            return false;
        if (addInfo.entityID != entityID)
            return false;
        return true;
    }
    public bool ContainsBuff(int entityID, EnBuff buff)
    {
        if (!_Entity2BuffInfo.TryGetValue(entityID, out var buffInfo))
            return false;
        if (!buffInfo.Contains(buff))
            return false;
        return true;
    }
    public EnBuffType GetBuffType(int addKey)
    {
        var buff = GetBuff(addKey);
        var buffType = GetBuffType(buff);
        return buffType;
    }
    public EnBuff GetBuff(int addKey)
    {
        if (!_AddKeyData.TryGetValue(addKey, out var buffInfo))
            return EnBuff.None;
        return buffInfo.buff;
    }
    private ISkillEntityBuff GetBuffData(int buffDataID)
    {
        return _ID2BuffDataDic[buffDataID];
    }
    private ISkillEntityBuff GetBuffData(int entityID, EnBuff buff)
    {
        var buffDataID = _Entity2BuffInfo[entityID].GetBuffDataID(buff);
        var data = _ID2BuffDataDic[buffDataID];
        return data;
    }
    public ISkillEntityBuff GetBuffDataByAddKey(int addKey)
    {
        if (!_AddKeyData.TryGetValue(addKey, out var addInfo))
            return null;
        var data = GetBuffData(addInfo.entityID, addInfo.buff);
        return data;
    }

    private int CreateEntityBuffData(int sourceEntityID, int targetEntityID, EnBuff buff)
    {
        var key = ABBUtil.GetTempKey();

        var data = ClassPoolMgr.Instance.Pull<EntityBuffDataUserData>();
        data.targetEntityID = targetEntityID;
        data.sourceEntityID = sourceEntityID;
        data.buff = buff;
        var buffData = BuffUtil.CreateBuffData(buff, data);
        ClassPoolMgr.Instance.Push(data);
        _ID2BuffDataDic.Add(key, buffData);


        return key;
    }

    private void RemoveEntityBuffData(int buffDataID)
    {
        if (!_ID2BuffDataDic.TryGetValue(buffDataID, out var buffData))
            return;
        _ID2BuffDataDic.Remove(buffDataID);
        ClassPoolMgr.Instance.Push(buffData);
    }
    private void UpdateBuffTypeInfo<T>(int buffDataID, T buffInfo)
        where T : IClassPool
    {
        var buff = GetBuffData(buffDataID);
        var buffType = GetBuffType(buff.GetBuff());
        if (!_BuffTypeList.TryGetValue(buffType, out var buffList))
        {
            buffList = new(10);
            _BuffTypeList.Add(buffType, buffList);
        }
        if (!buffList.TryGetValue(buffDataID, out var curInfo))
        {
            curInfo = buffType switch
            {
                EnBuffType.Time => ClassPoolMgr.Instance.Pull<BuffTimeInfo>(),
                _ => ClassPoolMgr.Instance.Pull<BuffDefaultInfo>(),
            };
            buffList.Add(buffDataID, curInfo);
        }
        curInfo.Execute(buffInfo);
    }
    private void RemoveBuffTypeInfo(int buffDataID)
    {
        var buff = GetBuffData(buffDataID);
        var buffType = GetBuffType(buff.GetBuff());
        if (!_BuffTypeList.TryGetValue(buffType, out var buffList))
            return;
        if (!buffList.Remove(buffDataID, out var curInfo))
            return;
        ClassPoolMgr.Instance.Push(curInfo);
        if (buffList.Count == 0)
            _BuffTypeList.Remove(buffType);
    }

    public override void Update()
    {
        base.Update();

        if (_BuffTypeList.TryGetValue(EnBuffType.Time, out var buffList))
        {
            var curTime = ABBUtil.GetGameTimeSeconds();
            var removeList = new HashSet<int>();
            foreach (var item in buffList)
            {
                var value = item.Value as BuffTimeInfo;
                if (value.endTime > curTime)
                    continue;
                var buffDataID = item.Key;
                if (!_BuffDataID2AddKey.TryGetValue(buffDataID, out var addKeyList))
                    continue;
                foreach (var addKey in addKeyList)
                    removeList.Add(addKey);
                _BuffDataID2AddKey.Remove(buffDataID);
            }
            foreach (var addKey in removeList)
                RemoveEntityBuff2(addKey);
        }
    }
}

