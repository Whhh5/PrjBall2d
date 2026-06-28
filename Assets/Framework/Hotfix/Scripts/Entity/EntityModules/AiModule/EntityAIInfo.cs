using System.Collections.Generic;

public class EntityAIInfo : IClassPoolDestroy
{
    private Dictionary<int, List<int>> _AIModuleIDDic = new();
    private List<int> _SortModuleCfgIDList = new();
    public int Count => _AIModuleIDDic.Count;
    public void AddAIModuleData(int moduleCfgID, int aiModuleID)
    {
        if (!_AIModuleIDDic.TryGetValue(moduleCfgID, out var moduleList))
        {
            moduleList = new(1);
            _AIModuleIDDic.Add(moduleCfgID, moduleList);

            var level = GameSchedule.Instance.GetAIModuleCfg0(moduleCfgID);
            var index = _SortModuleCfgIDList.FindIndex(0, _SortModuleCfgIDList.Count, value =>
            {
                var aiModuleCfg = GameSchedule.Instance.GetAIModuleCfg0(value);
                return level.nLevel < aiModuleCfg.nLevel;
            });
            if (index < 0)
                _SortModuleCfgIDList.Add(moduleCfgID);
            else
                _SortModuleCfgIDList.Insert(index, moduleCfgID);
        }
        moduleList.Add(aiModuleID);
    }
    public void RemoveAIModuleData(int moduleCfgID, int aiModuleID)
    {
        if (!_AIModuleIDDic.TryGetValue(moduleCfgID, out var moduleList))
            return;
        if (!moduleList.Remove(aiModuleID))
            return;
        if (moduleList.Count == 0)
        {
            _AIModuleIDDic.Remove(moduleCfgID);
            _SortModuleCfgIDList.Remove(moduleCfgID);
        }
    }
    public bool TryGetModuleList(int moduleCfgID, out List<int> moduleDataIDList)
    {
        if (!_AIModuleIDDic.TryGetValue(moduleCfgID, out moduleDataIDList))
            return false;
        return true;
    }

    public void OnPoolDestroy()
    {
        _AIModuleIDDic.Clear();
        _SortModuleCfgIDList.Clear();
    }
    public ref List<int> GetKeyList()
    {
        return ref _SortModuleCfgIDList;
    }
    public int GetAllAIModuleDataIDs(ref List<int> moduleDataIDs)
    {
        foreach (var item in _AIModuleIDDic)
        {
            moduleDataIDs.AddRange(item.Value);
        }
        return moduleDataIDs.Count;
    }
}