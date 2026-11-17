
using System.Collections.Generic;

public class GameBattleSkillPassivityStarUserData : IClassPoolDestroy, ISerializeToIntArrayUserData
{
    public int skillPassivityId;
    public int skillPassivityStar;

    public int startIndex { get; set; }
    public int count { get; set; }
    public int[] arrParams { get; set; }

    public void OnPoolDestroy()
    {
        skillPassivityId
            = skillPassivityStar
            = -1;
    }
}

public abstract class GameBattleSkillPassivityStarData : ISerializeToIntArray<GameBattleSkillPassivityStarUserData>
{
    public EnTypeId GetTypeDefineId() => throw new System.NotImplementedException();

    protected int _SkillPassivityId;
    protected int _SkillPassivityStar;
    public virtual void OnPoolDestroy()
    {
        _SkillPassivityId
            = _SkillPassivityStar
            = -1;
    }
    public abstract void DeserializeFromIntArray(int[] datas, int start, int count, ISerializeToIntArrayUserData userData);
    public abstract List<int> SerializeToIntArray();

    public void OnPoolInit(GameBattleSkillPassivityStarUserData userData)
    {
        _SkillPassivityId = userData.skillPassivityId;
        _SkillPassivityStar = userData.skillPassivityStar;
    }

    public virtual void GetDescriptionList(ref List<DescriptionInfo> descInfos, EnDescriptionLevel level)
    {

    }

}
public abstract class GameBattleSkillPassivityData<T> : GameBattleSkillPassivityData
    where T : GameBattleSkillPassivityStarData, new()
{
    private readonly Dictionary<int, T> _StarDatas = new();
    public override void OnPoolDestroy()
    {
        foreach (var item in _StarDatas)
        {
            ClassPoolMgr.Instance.Push(item.Value);
        }
        base.OnPoolDestroy();
        _StarDatas.Clear();
    }
    public T GetStarUserData()
    {
        var star = GameSkillPassivityMgr.Instance.GetStar(_SkillPassivityId);
        return GetStarData(star) as T;
    }
    public T GetStarUserData(int star)
    {
        return GetStarData(star) as T;
    }
    public override GameBattleSkillPassivityStarData GetStarData(int star)
    {
        if (!_StarDatas.TryGetValue(star, out var starData))
        {
            var userData = ClassPoolMgr.Instance.Pull<GameBattleSkillPassivityStarUserData>();
            userData.skillPassivityId = _SkillPassivityId;
            userData.skillPassivityStar = star;
            starData = GameSkillPassivityMgr.Instance.CreateStarData<T>(_SkillPassivityId, star, userData);
            _StarDatas.Add(star, starData);
        }
        return starData;
    }

}
public class GameBattleSkillPassivityUserData : IClassPoolDestroy
{
    public int skillPassivityId;
    public void OnPoolDestroy()
    {
        skillPassivityId
            = -1;
    }
}

public abstract class GameBattleSkillPassivityData : IClassPoolInit<GameBattleSkillPassivityUserData>
{
    protected int _SkillPassivityId;
    public virtual void OnPoolDestroy()
    {
        _SkillPassivityId
            = -1;
    }

    public void OnPoolInit(GameBattleSkillPassivityUserData userData)
    {
        _SkillPassivityId = userData.skillPassivityId;
    }

    public abstract GameBattleSkillPassivityStarData GetStarData(int star);

    public abstract void OnEnable(int star);
    public abstract void OnDisable();
    public abstract void OnUpstar(int lastStar, int curStar);
    public virtual void GetDescriptionList(ref List<DescriptionInfo> descInfos, EnDescriptionLevel level)
    {

    }
}