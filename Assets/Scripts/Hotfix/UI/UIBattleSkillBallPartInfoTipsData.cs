
using System.Collections.Generic;

public class UIBattleSkillBallPartInfoTipsUserData : UITipsUserData
{
    public int partId;
    public int star;

    public override void OnPoolDestroy()
    {
        base.OnPoolDestroy();
        partId
            = star
            = -1;
    }
}
public class UIBattleSkillBallPartInfoTipsData : UITipsData<UIBattleSkillBallInfoTips, UIBattleSkillBallPartInfoTipsUserData>
{
    public override EnLoadTarget LoadTarget => EnLoadTarget.Pre_UIBattleSkillBallInfoTips;


    private int _PartId;
    private int _Star;
    public override void OnPoolDestroy()
    {
        base.OnPoolDestroy();
        _PartId
            = _Star
            = -1;
    }

    protected override void OnPoolInit(UIBattleSkillBallPartInfoTipsUserData userData)
    {
        _PartId = userData.partId;
        _Star = userData.star;
    }

    public override void OnGOCreate(GameEntity entity)
    {
        base.OnGOCreate(entity);

        var partCfg = GameSchedule.Instance.GetSkillBallPartCfg0(_PartId);
        var starData = GameBattleBallPartsMgr.Instance.GetBallPartStarData(_PartId, _Star);
        var list = new List<DescriptionInfo>(10);
        starData.GetDescriptionList(ref list, EnDescriptionLevel.Level1);

        var nameStr = LanguageUtility.GetLanguage(partCfg.nNameLangId);
        _UITips.SettingBallInfo(nameStr, list, _Star);

        for (int i = 0; i < list.Count; i++)
        {
            ClassPoolMgr.Instance.Push(list[i]);
        }
    }
}