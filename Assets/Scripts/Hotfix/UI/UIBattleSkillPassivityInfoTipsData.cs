
using System.Collections.Generic;

public class UIBattleSkillPassivityInfoTipsUserData : UITipsUserData
{
    public int skillPassivityId;
    public int star;

    public override void OnPoolDestroy()
    {
        base.OnPoolDestroy();
        skillPassivityId
            = star
            = -1;
    }
}
public class UIBattleSkillPassivityInfoTipsData : UITipsData<UIBattleSkillBallInfoTips, UIBattleSkillPassivityInfoTipsUserData>
{
    public override EnLoadTarget LoadTarget => EnLoadTarget.Pre_UIBattleSkillBallInfoTips;


    private int _SkillPassivityId;
    private int _Star;
    public override void OnPoolDestroy()
    {
        TimeMgr.Instance.SetPause(false, false);
        base.OnPoolDestroy();
        _SkillPassivityId
            = _Star
            = -1;
    }

    protected override void OnPoolInit(UIBattleSkillPassivityInfoTipsUserData userData)
    {
        _SkillPassivityId = userData.skillPassivityId;
        _Star = userData.star;
        TimeMgr.Instance.SetPause(true, true);
    }

    public override void OnGOCreate(GameEntity entity)
    {
        base.OnGOCreate(entity);

        var partCfg = GameSchedule.Instance.GetSkillPassivityCfg0(_SkillPassivityId);
        var starData = GameSkillPassivityMgr.Instance.GetStarData(_SkillPassivityId, _Star);
        var list = new List<DescriptionInfo>(10);
        starData.GetDescriptionList(ref list, EnDescriptionLevel.Level1);

        var data = GameSkillPassivityMgr.Instance.GetData(_SkillPassivityId);
        data.GetDescriptionList(ref list, EnDescriptionLevel.Level2);

        var nameStr = LanguageUtility.GetLanguage(partCfg.nNameLangId);
        _UITips.SettingBallInfo(nameStr,(EnLoadTarget)partCfg.nIconAssetId, list, _Star);

        for (int i = 0; i < list.Count; i++)
        {
            ClassPoolMgr.Instance.Push(list[i]);
        }
    }
}