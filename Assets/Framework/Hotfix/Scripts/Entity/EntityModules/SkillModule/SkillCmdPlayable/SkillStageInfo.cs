using System.Collections.Generic;

public delegate void SkillItemEventInfoEvent(int entityID, IClassPoolUserData userData);
public class SkillItemEventInfo : IClassPoolUserData
{
    public float schedule;
    public IClassPoolUserData userData;
    public SkillItemEventInfoEvent onEvent;

    public void OnPoolDestroy()
    {
        schedule = -1;
        userData = null;
        onEvent = null;
    }
}
public class SkillStageInfo : IClassPoolInit<ISerializeToIntArrayUserData>, ISerializeToIntArray
{
    public List<int> SerializeToIntArray()
    {
        var result = new List<int>();
        ISerializeToIntArray.SerializeToInt(ref result, in _ClipId);
        ISerializeToIntArray.SerializeToInt(ref result, in _CanNextTime);
        ISerializeToIntArray.SerializeToInt(ref result, in _AtkEndTime);
        ISerializeToIntArray.SerializeToInt(ref result, in _IsAutoRemove);
        ISerializeToIntArray.SerializeToInt(ref result, in _BuffInfoList);
        ISerializeToIntArray.SerializeToInt(ref result, in _ArrAtkLinkSchedule);

        return result;
    }

    public void DeserializeFromIntArray(int[] datas, int start, int count, ISerializeToIntArrayUserData userData)
    {
        var localIndex = 0;
        ISerializeToIntArray.ParseToValue(in datas, in start, in count, ref localIndex, out _ClipId);
        ISerializeToIntArray.ParseToValue(in datas, in start, in count, ref localIndex, out _CanNextTime);
        ISerializeToIntArray.ParseToValue(in datas, in start, in count, ref localIndex, out _AtkEndTime);
        ISerializeToIntArray.ParseToValue(in datas, in start, in count, ref localIndex, out _IsAutoRemove);
        ISerializeToIntArray.ParseToValue(in datas, in start, in count, ref localIndex, out _BuffInfoList, userData);
        ISerializeToIntArray.ParseToValue(in datas, in start, in count, ref localIndex, out _ArrAtkLinkSchedule, userData);
    }
    private int _ClipId;
    private float _CanNextTime;
    private float _AtkEndTime;
    private bool _IsAutoRemove = true;
    private IEntityBuffParams[] _BuffInfoList = null;
    private ISkillScheduleAction[] _ArrAtkLinkSchedule = null;
    public EnTypeId GetTypeDefineId() => EnTypeId.SkillItemInfo;

    private readonly List<int> _BuffAddKeyList = new();


    private List<SkillItemEventInfo> _EventList = new(10);
    private int _EventListIndex = 0;

    public int GetClipID() => _ClipId;
    public float GetCanNextTime() => _CanNextTime;
    public float GetAtkEndTime() => _AtkEndTime;
    public bool GetIsAutoRemove() => _IsAutoRemove;
    
    public void OnPoolDestroy()
    {
        foreach (var t in _EventList)
            ClassPoolMgr.Instance.Push(t);

        ResetAllItemData();
        foreach (var item in _ArrAtkLinkSchedule)
            ClassPoolMgr.Instance.Push(item);
        foreach (var item in _BuffInfoList)
            ClassPoolMgr.Instance.Push(item);

        _BuffAddKeyList.Clear();
        _EventList.Clear();
        _ArrAtkLinkSchedule = null;
        _EventListIndex = 0;
    }

    public void OnPoolInit(ISerializeToIntArrayUserData userData)
    {
        DeserializeFromIntArray(userData.arrParams, userData.startIndex, userData.count, userData);

        foreach (var skillScheduleAction in _ArrAtkLinkSchedule)
        {
            skillScheduleAction.GetEventList(ref _EventList);
        }
        _EventList.Sort((item1, item2) => item1.schedule <  item2.schedule ? -1 : 1);
    }

    public bool ScheduleEventIsValid()
    {
        return _EventList.Count > 0 && _EventListIndex < _EventList.Count;
    }
    public SkillItemEventInfo GetCurScheduleItem()
    {
        return _EventList[_EventListIndex];
    }
    public bool NextEventAction()
    {
        _EventListIndex++;
        if (_EventListIndex > _EventList.Count - 1)
            return false;
        return true;
    }
    public void ResetAllItemData()
    {
        _EventListIndex = 0;
        foreach (var item in _ArrAtkLinkSchedule)
        {
            item.Reset();
        }
    }

    public void OnEnable(int entityID)
    {
        foreach (var buffDataParams in _BuffInfoList)
        {
            var addKey = BuffMgr.Instance.AddEntityBuff(entityID, entityID, buffDataParams);

            if (BuffMgr.Instance.GetBuffType(addKey) != EnBuffType.Time)
                _BuffAddKeyList.Add(addKey);
        }
    }
    public void OnDisable(int entityID)
    {
        foreach (var addKey in _BuffAddKeyList)
        {
            BuffMgr.Instance.RemoveEntityBuff(addKey);
        }
        ResetAllItemData();
        _EventListIndex = 0;
        _BuffAddKeyList.Clear();
    }

    public bool IsCanNextAction(float schedule)
    {
        var result = schedule >= _CanNextTime;
        return result;
    }
}
